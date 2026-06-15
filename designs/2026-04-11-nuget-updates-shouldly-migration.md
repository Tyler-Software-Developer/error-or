---
date: 2026-04-11 20:50:00 CDT
author: claude
research_doc: "research/docs/2026-04-11-nuget-updates-shouldly-migration.md"
questions_doc: "research/questions/2026-04-11-nuget-updates-shouldly-migration.md"
ticket: "Update NuGet packages, replace FluentAssertions (commercial) with Shouldly, align with April 2026 xUnit/Shouldly recommendations"
status: ready-for-structure
phase: design-discussion
decisions_resolved: 4
---

# Design Discussion: NuGet Updates & FluentAssertions → Shouldly Migration

## Summary

FluentAssertions has been removed from `tests/Tests.csproj` but 18 of 19 test files still use its `.Should()` API (431 total calls). The test project no longer compiles. This design finalizes the assertion-library swap to Shouldly 4.3.0, resolves a handful of idiomatic mapping questions (`BeEquivalentTo`, exception+`ParamName`, global using), and addresses a missing xUnit v3 project property. Package versions are already current; no bumps are in scope.

## Current State

- `tests/Tests.csproj:19–29` — already on `xunit.v3` 3.2.2, `xunit.runner.visualstudio` 3.1.5, `Microsoft.NET.Test.Sdk` 18.4.0, `Shouldly` 4.3.0, `coverlet.collector` 8.0.1. **No `FluentAssertions` package reference.**
- `tests/Tests.csproj` — does **not** declare `<OutputType>Exe</OutputType>` even though xUnit v3 documents it as required for the v3 runner model.
- `tests/Usings.cs:1` — contains only `global using Xunit;`. No `global using Shouldly;`.
- 18 files still have `using FluentAssertions;` + 431 `.Should()` calls; full per-file breakdown in research doc §2.
- `tests/ErrorOr/ErrorOr.AggregationTests.cs` is partially migrated: `using Shouldly;` at line 2, two `ShouldBe` calls at lines 20–21, and 53 remaining `.Should()` calls from line 22 onward.
- `tests/ErrorOr/TestUtils.cs` contains only domain helpers (`Convert`). **No assertion helpers or base classes** — migration is pure call-site rewriting.
- `src/ErrorOr.csproj` is untouched in spirit: targets `netstandard2.0; net8.0; net9.0; net10.0`, all deps already current.
- `ErrorOr.sln` was deleted; `ErrorOr.slnx` added. CI uses CWD discovery, which SDK 9+ handles transparently.
- `.github/workflows/build.yml` runs `dotnet test` across `net8.0/9.0/10.0` matrix. No workflow pins SDK via `global.json` and none reference FluentAssertions.

## Desired End State

- `tests/Tests.csproj` compiles and all tests pass on `net8.0/9.0/10.0` across the CI matrix.
- Every `.Should()` FluentAssertions call is gone. No file has `using FluentAssertions;`. No package manifest references FluentAssertions anywhere in the repo.
- Assertions use idiomatic Shouldly 4.x: subject-first extensions, `Should.Throw<T>` / `Should.ThrowAsync<T>` for exceptions, `ShouldSatisfyAllConditions` only where multi-assertion grouping is genuinely useful.
- xUnit v3 project conventions are satisfied (see Decision 4).
- `src/ErrorOr.csproj` and public API are **unchanged**. AOT workflow (`aot-compatibility.yml`) and publish workflow (`publish.yml`) continue to work unmodified.
- `codecov.yml` coverage thresholds continue to pass — no assertion should be silently dropped during the rewrite.

## Design Decisions

### Decision 1: `using Shouldly;` — global vs per-file

**Options:**
- (A) Add `global using Shouldly;` to `tests/Usings.cs`.
- (B) Add `using Shouldly;` at the top of each migrated file individually.

**Chosen:** **(A)**.

**Rationale:** `tests/Usings.cs` already globalizes `Xunit` — Shouldly is the parallel cross-cutting testing concern and belongs next to it. (A) removes 19 redundant import lines and keeps style consistent with how the repo already treats test infrastructure.

**Why not (B):** It would add 19 near-identical `using` directives and diverge from the existing `Xunit` precedent for no benefit.

---

### Decision 2: xUnit v3 `<OutputType>Exe</OutputType>`

**Options:**
- (A) Add `<OutputType>Exe</OutputType>` to `tests/Tests.csproj`.
- (B) Leave it omitted (CI currently tolerates this).

**Chosen:** **(A)**.

**Rationale:** The xUnit v3 documented project template (research doc §13) sets it. Omitting it is supported today via VSTest compatibility but diverges from the v3 runner model and risks subtle failures when the Microsoft Testing Platform path is exercised. It is a one-line additive change with no cost.

**Why not (B):** Drifting from the documented template is tech debt that will surface the next time the runner model changes.

---

### Decision 3: Package-version bumps

**Options:**
- (A) Bump any package that has a newer version.
- (B) No bumps — migration only.

**Chosen:** **(B)**.

**Rationale:** Research doc §7 confirms every package in `src/` and `tests/` is already at the current stable as of April 2026 (`xunit.v3` 3.2.2, `Shouldly` 4.3.0, `Microsoft.NET.Test.Sdk` 18.4.0, `coverlet.collector` 8.0.1, `Microsoft.SourceLink.GitHub` 10.0.201, `Microsoft.Bcl.HashCode` 6.0.0, `Nullable` 1.3.1). Nothing to bump.

**Why not (A):** There is no newer stable to move to; (A) is a no-op dressed up as a decision.

---

### Decision 4: Scope of `src/` and CI changes

**Chosen:** **None.** `src/ErrorOr.csproj` metadata is unchanged, AOT and publish workflows are unchanged, `.slnx` CI handling is unchanged (CWD discovery is fine on SDK 9+). This is a test-project-only change. `netstandard2.0` is retained in `src/` TargetFrameworks; the conditional `Microsoft.Bcl.HashCode` reference stays.

---

### Decision 5: `BeEquivalentTo` replacement strategy

**Options:** (A) per-site judgment — `ShouldBe(expected, ignoreOrder: true)` for error-list comparisons, order-sensitive `ShouldBe` for `Value` comparisons where input order is deliberate; (B) always order-insensitive; (C) always order-sensitive; (D) expand to `ShouldSatisfyAllConditions`.

**Chosen:** **(A)** — decided file-by-file during the rewrite.

**Rationale:** Error lists in `ErrorOr` tests are compared without caring about ordering; `Value` comparisons for concrete collections in `FactoryTests.cs`/`ElseTests.cs`/`MatchAsyncTests.cs` were typically constructed with deliberate ordering and should stay strict. A one-size rule would either loosen or tighten semantics unnecessarily.

**Why not (B)/(C):** Both trade correctness for mechanical simplicity in a set of ~10 sites — not worth it.
**Why not (D):** Verbose; diagnostic gain is marginal because the existing tests already isolate single assertions per test method.

---

### Decision 6: Exception + `ParamName` idiom

**Options:** (A) inline `var ex = Should.Throw<T>(...); ex.ParamName.ShouldBe("p");` at every site; (B) shared helper in `TestUtils.cs`; (C) per-file local static helper.

**Chosen:** **(A)** — inline at every site.

**Rationale:** `TestUtils.cs` currently has no assertion helpers — only domain `Convert` helpers. Adding assertion infrastructure crosses a line the repo deliberately avoids. The inline form is also exactly what Shouldly docs show, so it is the least-surprising code for a new reader.

**Why not (B):** Introduces a bespoke wrapper that the rest of the repo neither uses nor justifies.
**Why not (C):** Duplicates a helper across files — worst of both options.

---

### Decision 7: `ThrowExactly<T>` fidelity

**Options:** (A) map to `Should.Throw<T>` (assignable-type) and accept the loosening; (B) add `ex.GetType().ShouldBe(typeof(T));` after capture to preserve exact-type semantics.

**Chosen:** **(A)**.

**Rationale:** All existing `ThrowExactly` sites target `ArgumentNullException`/`ArgumentException`. Neither has test-relevant BCL subclasses that a test in this repo would ever be asked to distinguish. In practice `Should.Throw<T>` is semantically equivalent here.

**Why not (B):** Adds a line of noise at every exception site for a distinction that never fires.

---

## Patterns to Reuse

- **Global test usings pattern:** `tests/Usings.cs:1` (`global using Xunit;`) — Decision 1 extends this file.
- **Test file naming:** `<TypeUnderTest>.<Feature>Tests.cs` under `namespace Tests;` — unchanged; migration edits files in place.
- **Fact-only style:** `AggregationTests.cs` uses `[Fact]` exclusively. No `[Theory]`/`[InlineData]`/fixtures exist anywhere in the project. Migration does not introduce any new test-infrastructure primitives.
- **xUnit v3 csproj template:** research doc §13 — Decision 2 aligns `tests/Tests.csproj` with it.
- **Shouldly mapping table:** research doc `2026-04-11-dotnet-test-stack-versions.md` §5 is the authoritative mechanical cheat sheet for the rewrite. No ad-hoc mapping invented here.

## Open Questions

*(All open questions resolved 2026-04-11 — see Decisions 4–7.)*

## Non-Goals

- No changes to `src/ErrorOr.csproj`, `src/*.cs`, or the public API.
- No new tests, no coverage expansion, no test refactoring beyond the mechanical assertion swap.
- No `Directory.Packages.props` / central package management adoption.
- No `global.json` pinning or CI workflow restructuring.
- No `.sln` ↔ `.slnx` churn; the migration to `.slnx` is already done in the working tree.
- No `StyleCop.Analyzers` upgrade (still on `1.2.0-beta.556`, which research confirms is current practice).
- No AOT workflow or publish workflow changes.
- No drop of `netstandard2.0` from the library target matrix (see Q4 (B) — explicitly out of scope).

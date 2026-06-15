---
date: 2026-04-11 22:47:40 CDT
author: Tyler Miller
structure_doc: "structures/2026-04-11-nuget-updates-shouldly-migration.md"
design_doc: "designs/2026-04-11-nuget-updates-shouldly-migration.md"
research_doc: "research/docs/2026-04-11-nuget-updates-shouldly-migration.md"
ticket: "Finish FluentAssertions → Shouldly migration; align tests/Tests.csproj with xUnit v3 template"
status: ready-for-implementation
phase: plan
phase_count: 5
---

# NuGet Updates & FluentAssertions → Shouldly Migration — Implementation Spec

## Summary

Pure test-project rewrite: globalize `Shouldly`, add `<OutputType>Exe</OutputType>` to the test csproj, and mechanically translate 431 `.Should()` call sites across 18 files using the Shouldly 4.x mapping table. No production code, package versions, or CI workflows change. Phases are carved by file-cluster so each checkpoint is a `dotnet build` + `dotnet test` on a strictly smaller set of compile errors than the previous phase.

## Inputs

- **Structure Outline:** [structures/2026-04-11-nuget-updates-shouldly-migration.md](../structures/2026-04-11-nuget-updates-shouldly-migration.md)
- **Design Discussion:** [designs/2026-04-11-nuget-updates-shouldly-migration.md](../designs/2026-04-11-nuget-updates-shouldly-migration.md)
- **Research:** [research/docs/2026-04-11-nuget-updates-shouldly-migration.md](../research/docs/2026-04-11-nuget-updates-shouldly-migration.md)

## Functional Goals

- [ ] `tests/Tests.csproj` compiles and all tests pass on `net8.0/9.0/10.0` across the CI matrix.
- [ ] Every `.Should()` FluentAssertions call is gone. No file has `using FluentAssertions;`. No package manifest references FluentAssertions anywhere in the repo.
- [ ] Assertions use idiomatic Shouldly 4.x: subject-first extensions, `Should.Throw<T>` / `Should.ThrowAsync<T>` for exceptions.
- [ ] xUnit v3 project convention `<OutputType>Exe</OutputType>` is satisfied on `tests/Tests.csproj`.
- [ ] `src/ErrorOr.csproj` and public API are unchanged. AOT workflow and publish workflow continue to work unmodified.
- [ ] `codecov.yml` coverage thresholds continue to pass — no assertion silently dropped.

## Non-Goals

- No changes to `src/ErrorOr.csproj`, `src/*.cs`, or the public API.
- No new tests, no coverage expansion, no test refactoring beyond the mechanical assertion swap.
- No `Directory.Packages.props` / central package management adoption.
- No `global.json` pinning or CI workflow restructuring.
- No `.sln` ↔ `.slnx` churn.
- No `StyleCop.Analyzers` upgrade.
- No AOT workflow or publish workflow changes.
- No drop of `netstandard2.0` from the library target matrix.

## Shouldly Mapping Reference

Applied at every call site — this is the mapping budget, not new code.

```csharp
// Equality
expr.Should().Be(x);                   // → expr.ShouldBe(x);
expr.Should().NotBe(x);                // → expr.ShouldNotBe(x);
expr.Should().BeNull();                // → expr.ShouldBeNull();
expr.Should().NotBeNull();             // → expr.ShouldNotBeNull();
expr.Should().BeTrue() / BeFalse();    // → expr.ShouldBeTrue() / ShouldBeFalse();
expr.Should().BeOfType<T>();           // → expr.ShouldBeOfType<T>();

// Collections — Decision 5: order-sensitivity chosen file-by-file
errors.Should().BeEquivalentTo(expected);  // error lists  → errors.ShouldBe(expected, ignoreOrder: true);
value.Should().BeEquivalentTo(expected);   // deliberate-order Value comparisons → value.ShouldBe(expected);
coll.Should().ContainSingle();             // → coll.Count.ShouldBe(1);
coll.Should().HaveCount(n);                // → coll.Count.ShouldBe(n);
coll.Should().Contain(x);                  // → coll.ShouldContain(x);

// Exceptions — Decision 6 (inline) + Decision 7 (Throw, not ThrowExactly)
Action a = () => ...;
a.Should().Throw<ArgumentNullException>()
 .And.ParamName.Should().Be("p");
// →
var ex = Should.Throw<ArgumentNullException>(() => ...);
ex.ParamName.ShouldBe("p");

// Async exceptions
Func<Task> a = async () => ...;
await a.Should().ThrowAsync<InvalidOperationException>();
// →
await Should.ThrowAsync<InvalidOperationException>(async () => ...);
```

## Implementation Phases

### Phase 1: Project scaffolding (globals + csproj)

**Goal:** Lay down the two infra changes that every subsequent phase depends on, in isolation, so later phases produce clean single-purpose diffs.

**Checkpoint:**
- `dotnet build tests/Tests.csproj` still fails (expected — 18 files still use `.Should()`), but the error set is **unchanged** from baseline in count and kind (only CS-level FluentAssertions-missing errors, no new csproj/usings errors introduced).
- `dotnet build src/ErrorOr.csproj` succeeds unchanged.

**Files touched:**
- MODIFY: `tests/Tests.csproj`
- MODIFY: `tests/Usings.cs`

**Implementation steps:**
1. Capture a baseline: run `dotnet build tests/Tests.csproj 2>&1 | tee /tmp/baseline-build.log` and `dotnet test tests/Tests.csproj --logger "console;verbosity=normal" 2>&1 | tee /tmp/baseline-test.log` (expected to fail — record the error count and, if a previous commit compiles, record the test totals for Phase 5 comparison).
2. In `tests/Tests.csproj`, add `<OutputType>Exe</OutputType>` inside the existing `<PropertyGroup>` (around lines 3–10, per Decision 2). Do not touch any `<ItemGroup>` / `PackageReference` entries (Decision 3 — Shouldly 4.3.0 at `tests/Tests.csproj:20` already present; no FluentAssertions reference exists to remove).
3. In `tests/Usings.cs`, append `global using Shouldly;` on the line immediately after the existing `global using Xunit;` at `tests/Usings.cs:1` (Decision 1).
4. Re-run `dotnet build tests/Tests.csproj` and confirm the error set is the same kind and count as baseline (only FluentAssertions-missing CS errors from the 18 unmigrated files; zero new errors attributable to csproj or usings changes).
5. Run `dotnet build src/ErrorOr.csproj` and confirm it still succeeds unchanged.

**Tests:**
- Build-only checkpoint; no new unit tests. Verification is the build-error diff vs. baseline.

---

### Phase 2: Finish the partially-migrated file

**Goal:** Close out `AggregationTests.cs` first, because it is the only file already straddling both libraries and contains the highest density of the trickiest idioms (`BeEquivalentTo` on error lists, `ParamName`, async exceptions). Finishing it first de-risks the mapping table against the toughest cases before fanning out.

**Checkpoint:**
- `dotnet build tests/Tests.csproj` error count drops (AggregationTests stops contributing errors).
- `dotnet test tests/Tests.csproj --framework net10.0 --filter FullyQualifiedName~AggregationTests` passes.
- `grep -l FluentAssertions tests/ErrorOr/ErrorOr.AggregationTests.cs` returns nothing.

**Files touched:**
- MODIFY: `tests/ErrorOr/ErrorOr.AggregationTests.cs` (~53 call-site rewrites)

**Implementation steps:**
1. Remove the file-local `using Shouldly;` at `tests/ErrorOr/ErrorOr.AggregationTests.cs:2` (now global via Phase 1).
2. Remove any residual `using FluentAssertions;` directive from the file header.
3. Preserve the already-migrated `ShouldBe` calls at `tests/ErrorOr/ErrorOr.AggregationTests.cs:20–21` as the reference style for the remainder of the rewrite.
4. Rewrite the remaining 53 `.Should()` call sites per the mapping reference, applying:
   - Decision 5: error-list `BeEquivalentTo` → `ShouldBe(expected, ignoreOrder: true)`.
   - Decision 6: inline `var ex = Should.Throw<T>(() => ...); ex.ParamName.ShouldBe("p");` — do **not** add helpers to `tests/ErrorOr/TestUtils.cs`.
   - Decision 7: `Should.Throw<T>` / `Should.ThrowAsync<T>`, never `ThrowExactly<T>`.
5. Run `dotnet build tests/Tests.csproj` — confirm overall error count drops and no new errors originate from `ErrorOr.AggregationTests.cs`.
6. Run `dotnet test tests/Tests.csproj --framework net10.0 --filter FullyQualifiedName~AggregationTests`. All tests must pass.

**Tests:**
- Filtered test run on `net10.0` for `AggregationTests` — single-TFM is sufficient here; full matrix is verified in Phase 5.

---

### Phase 3: High-exception-density files

**Goal:** Migrate the files dominated by `Should.Throw` / `ParamName` idioms, where the Decision 6 / Decision 7 patterns matter most. Doing these together keeps the exception-rewrite mental model hot.

**Checkpoint:**
- `dotnet build tests/Tests.csproj` error count drops to only the Phase-4 file set.
- `dotnet test tests/Tests.csproj --framework net10.0 --filter "FullyQualifiedName~InstantiationTests|FullyQualifiedName~BuilderTests|FullyQualifiedName~FailIfTests|FullyQualifiedName~FailIfAsyncTests|FullyQualifiedName~ErrorTests|FullyQualifiedName~Error.EqualityTests"` passes.
- `grep -l "using FluentAssertions" tests/ErrorOr/ErrorOr.InstantiationTests.cs tests/ErrorOr/ErrorOr.BuilderTests.cs tests/ErrorOr/ErrorOr.FailIfTests.cs tests/ErrorOr/ErrorOr.FailIfAsyncTests.cs tests/Errors/ErrorTests.cs tests/Errors/Error.EqualityTests.cs` returns nothing.

**Files touched (in order — highest exception density first):**
- MODIFY: `tests/ErrorOr/ErrorOr.InstantiationTests.cs` *(21 ParamName/exception sites — highest)*
- MODIFY: `tests/ErrorOr/ErrorOr.BuilderTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.FailIfTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.FailIfAsyncTests.cs`
- MODIFY: `tests/Errors/ErrorTests.cs`
- MODIFY: `tests/Errors/Error.EqualityTests.cs`

**Implementation steps:**
For each file above, in the listed order:
1. Delete the `using FluentAssertions;` directive from the file header.
2. Rewrite every `.Should()...` call per the mapping reference. Apply Decision 6 (inline `Should.Throw<T>` + `ex.ParamName.ShouldBe(...)`) and Decision 7 (`Throw<T>`, never `ThrowExactly<T>`) at every exception site.
3. Leave all other code — test method names, `[Fact]` attributes, `Arrange/Act/Assert` structure, domain calls to `TestUtils.Convert`, variable names — untouched. No structural edits, no logic changes.
4. After each file, run `dotnet build tests/Tests.csproj` to confirm that file no longer contributes errors and no new errors were introduced elsewhere (early-warning on accidental cross-file regressions).
5. After all six files are migrated, run the filtered test command from the checkpoint above on `net10.0`.

**Tests:**
- Filtered test run covering the six migrated files on `net10.0`.

---

### Phase 4: Remaining Match/Switch/Then/Else/Factory/Equality/ToErrorOr/Aot files

**Goal:** Mop up the remaining 12 files. These are dominated by `Be`/`BeOfType`/`BeEquivalentTo` sites — mostly mechanical.

**Checkpoint:**
- `dotnet build tests/Tests.csproj` succeeds on `net10.0` with **zero errors and zero FluentAssertions-related warnings**.
- `grep -rn "using FluentAssertions" tests/` returns nothing.
- `grep -rn "\.Should()" tests/` returns nothing (Shouldly uses `ShouldXxx()`, never `.Should()`).
- `dotnet test tests/Tests.csproj --framework net10.0` passes all tests.

**Files touched:**
- MODIFY: `tests/ErrorOr/ErrorOr.MatchTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.MatchAsyncTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.SwitchTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.SwitchAsyncTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.ThenTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.ThenAsyncTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.ElseTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.ElseAsyncTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.FactoryTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.EqualityTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.ToErrorOrTests.cs`
- MODIFY: `tests/ErrorOr/ErrorOr.AotCompatibilityTests.cs`

**Implementation steps:**
For each file above:
1. Delete the `using FluentAssertions;` directive.
2. Rewrite every `.Should()` call per the mapping reference.
3. Apply Decision 5 file-by-file for `BeEquivalentTo`:
   - **Strict order (`.ShouldBe(expected)`):** `tests/ErrorOr/ErrorOr.FactoryTests.cs`, `tests/ErrorOr/ErrorOr.ElseTests.cs`, `tests/ErrorOr/ErrorOr.MatchAsyncTests.cs` (Value comparisons with deliberate input ordering).
   - **Order-insensitive (`.ShouldBe(expected, ignoreOrder: true)`):** everywhere else, for error-list comparisons.
4. Confirm `tests/ErrorOr/ErrorOr.AotCompatibilityTests.cs` still compiles cleanly under the test project. Do **not** touch `.github/workflows/aot-compatibility.yml` (Decision 4 — AOT workflow frozen).
5. After all 12 files are migrated, run the full checkpoint commands:
   - `dotnet build tests/Tests.csproj --framework net10.0` → zero errors, zero warnings related to FluentAssertions.
   - `grep -rn "using FluentAssertions" tests/` → empty.
   - `grep -rn "\.Should()" tests/` → empty.
   - `dotnet test tests/Tests.csproj --framework net10.0` → all tests pass.

**Tests:**
- Full `net10.0` test run; every test in the suite should execute and pass.

---

### Phase 5: Hardening — full matrix, coverage, cleanup sweep

**Goal:** Prove the rewrite holds across the full CI matrix and catch anything a single-TFM build missed (e.g., TFM-conditional nullability or API differences that Shouldly might surface).

**Checkpoint:**
- `dotnet build tests/Tests.csproj` succeeds on `net8.0`, `net9.0`, `net10.0`.
- `dotnet test tests/Tests.csproj` passes on all three TFMs with the pre-migration test count preserved — no tests silently skipped or dropped.
- `dotnet build src/ErrorOr.csproj` unchanged (sanity: confirm `src/` was not touched).
- Repo-wide negative checks (see steps below) are all clean.
- `codecov.yml` thresholds still pass locally.
- `.github/workflows/build.yml` runs green on push.

**Files touched:**
- None by default. If the matrix build surfaces an issue, fix at the relevant call site only (will almost always be an overload-resolution fix, never a semantic change).

**Implementation steps:**
1. Full-matrix build: `dotnet build tests/Tests.csproj` (no `--framework` flag — builds all TFMs). Must succeed on `net8.0`, `net9.0`, `net10.0` with zero errors or warnings.
2. Full-matrix test: `dotnet test tests/Tests.csproj --logger "console;verbosity=normal"`. Compare the per-TFM passing-test totals against the baseline recorded in Phase 1 step 1 (or against the pre-migration git commit if the baseline build was broken). **Totals must match exactly** — a lower count means a test was silently dropped during rewrite.
3. `src/` sanity: `dotnet build src/ErrorOr.csproj` — confirm it succeeds unchanged. Run `git status src/` and `git diff src/` — both must be empty (Decision 4).
4. Repo-wide negative grep checks:
   - `grep -rn "FluentAssertions" . --include="*.cs" --include="*.csproj"` returns nothing (docs under `research/`, `designs/`, `structures/`, `specs/` are allowed to mention it).
   - `grep -rn "\.Should()" tests/` returns nothing.
   - `grep -rn "ThrowExactly" tests/` returns nothing (Decision 7).
5. Coverage sanity: `dotnet test tests/Tests.csproj --collect:"XPlat Code Coverage"` — inspect the report and confirm thresholds in `codecov.yml` still pass locally.
6. If the matrix surfaces a Shouldly overload-resolution issue on a specific TFM, fix the single affected call site and re-run steps 1–2. Do not introduce helpers or broaden scope (Decisions 4, 6 still apply).
7. Push the branch and confirm `.github/workflows/build.yml` is green across the matrix.

**Tests:**
- Full matrix test run across `net8.0/9.0/10.0`. Test totals must equal baseline.

## Test Strategy

The test strategy for this migration is **"the existing tests are the test strategy."** No new tests are added (Non-Goals). The migration's correctness is measured by:

1. **Build-error monotonic decrease:** Each phase strictly reduces the set of unresolved `.Should()`-related compile errors. A phase that doesn't reduce the error set is a regression.
2. **Filtered test passes at each phase:** Phases 2–4 each run the filtered `dotnet test` for the files they just migrated on `net10.0`. Phase 4 runs the full `net10.0` suite. Phase 5 runs the full matrix.
3. **Test-count preservation:** Phase 5 step 2 compares passing-test totals across TFMs against a pre-migration baseline. A silently dropped test (e.g., an assertion accidentally commented out during rewrite) would lower the count — this is the primary guard against the most likely failure mode of a mechanical rewrite.
4. **Negative greps:** `FluentAssertions`, `.Should()`, and `ThrowExactly` must not appear anywhere in `tests/` by the end of Phase 5.

Anti-patterns to avoid during rewrite (per `testing-anti-patterns` skill):
- **Do not** swap assertions for weaker ones (`ShouldBeOfType<T>` → `ShouldNotBeNull`). The mapping table is 1:1 — use it exactly.
- **Do not** comment out a failing assertion to "come back to it." Fix it in the same pass or note the specific call-site and surface it to the user before moving on.
- **Do not** group unrelated assertions into `ShouldSatisfyAllConditions` to silence noisy output. Keep each assertion as its own call, matching the pre-migration structure 1:1.
- **Do not** add helpers in `tests/ErrorOr/TestUtils.cs` (Decision 6 — explicitly prohibited).

## Rollback Strategy

Each phase is an independent, additive set of commits on a feature branch. Rollback granularity is per-phase via `git revert`:

- **Phase 1 revert** undoes two one-line infra edits; subsequent phases would then fail to compile (their `ShouldXxx` calls depend on the global using), so Phase 1 revert implies reverting all later phases too.
- **Phase 2–4 reverts** each undo a bounded set of file-level rewrites; the previous phase's state (fewer files migrated, build still broken) is restored.
- **Phase 5** is verification-only in the expected case, so "revert" just means abandoning the branch.

Because no migration, no schema change, no CI change, and no `src/` change is in scope, there is no production rollback surface and no data to reconcile. The branch is either merged green or not merged. If post-merge a regression appears, `git revert` of the merge commit is clean: it restores the previous test-project state and leaves `src/`, `.slnx`, and workflows untouched. No feature flags, no follow-up drops, no migrations to reverse.

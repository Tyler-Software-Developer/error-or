---
date: 2026-04-11 22:45:00 CDT
design_doc: "designs/2026-04-11-nuget-updates-shouldly-migration.md"
research_doc: "research/docs/2026-04-11-nuget-updates-shouldly-migration.md"
ticket: "Finish FluentAssertions → Shouldly migration; align tests/Tests.csproj with xUnit v3 template"
status: ready-for-plan
phase: structure-outline
slice_count: 5
---

# Structure Outline: NuGet Updates & FluentAssertions → Shouldly Migration

## Summary

Pure test-project rewrite: globalize `Shouldly`, add `<OutputType>Exe</OutputType>` to the test csproj, and mechanically translate 431 `.Should()` call sites across 18 files using the Shouldly 4.x mapping table from the research doc. No production code, package versions, or CI workflows change. Slices are carved by file-cluster so each checkpoint is a `dotnet build` + `dotnet test` on a strictly smaller set of compile errors than the previous slice.

## New Files

*None.* This migration edits files in place; no new source, helpers, migrations, or configuration files are introduced.

## Modified Files

### Project / infra (2 files)
- `tests/Tests.csproj` — add `<OutputType>Exe</OutputType>` under the existing `<PropertyGroup>` (Decision 2). No package changes (Decision 3).
- `tests/Usings.cs` — add `global using Shouldly;` below the existing `global using Xunit;` (Decision 1).

### Partially migrated (finish in place, 1 file)
- `tests/ErrorOr/ErrorOr.AggregationTests.cs` — already has `using Shouldly;` and two `ShouldBe` calls at lines 20–21; remove the file-local `using Shouldly;` (now global), rewrite the remaining 53 `.Should()` calls (incl. 18 `BeEquivalentTo`/exception/ParamName sites — the highest density in the repo), strip any residual `using FluentAssertions;`.

### Call-site rewrites (17 files, all under `tests/`)
Each of these: delete `using FluentAssertions;`, rewrite every `.Should()...` call per the Shouldly mapping table, leave everything else untouched.

- `tests/ErrorOr/ErrorOr.BuilderTests.cs`
- `tests/ErrorOr/ErrorOr.ElseAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.ElseTests.cs`
- `tests/ErrorOr/ErrorOr.EqualityTests.cs`
- `tests/ErrorOr/ErrorOr.FactoryTests.cs`
- `tests/ErrorOr/ErrorOr.FailIfAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.FailIfTests.cs`
- `tests/ErrorOr/ErrorOr.InstantiationTests.cs` *(21 ParamName/exception sites — highest exception density)*
- `tests/ErrorOr/ErrorOr.MatchAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.MatchTests.cs`
- `tests/ErrorOr/ErrorOr.SwitchAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.SwitchTests.cs`
- `tests/ErrorOr/ErrorOr.ThenAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.ThenTests.cs`
- `tests/ErrorOr/ErrorOr.ToErrorOrTests.cs`
- `tests/ErrorOr/ErrorOr.AotCompatibilityTests.cs` *(verify it compiles under test project; AOT workflow itself is unchanged per Decision 4)*
- `tests/Errors/Error.EqualityTests.cs`
- `tests/Errors/ErrorTests.cs`

### Not touched (non-goals, restated for reviewer clarity)
- `src/ErrorOr.csproj`, `src/**/*.cs`, `ErrorOr.slnx`, `.github/workflows/**`, `codecov.yml`, `tests/ErrorOr/TestUtils.cs` (domain helpers only — no assertion helpers added per Decision 6).

## New Types / Signatures

*None.* Per Decision 6, no helper methods, base classes, or wrappers are added. Per Decision 4, no public API or `src/` types change. The migration is a 1:1 textual rewrite using only Shouldly's built-in extension surface.

### Shouldly idiom reference (applied at every call site — not new code, just the mapping budget)

```csharp
// Equality
expr.Should().Be(x);                   // → expr.ShouldBe(x);
expr.Should().NotBe(x);                // → expr.ShouldNotBe(x);
expr.Should().BeNull();                // → expr.ShouldBeNull();
expr.Should().NotBeNull();             // → expr.ShouldNotBeNull();
expr.Should().BeTrue() / BeFalse();    // → expr.ShouldBeTrue() / ShouldBeFalse();
expr.Should().BeOfType<T>();           // → expr.ShouldBeOfType<T>();

// Collections — per Decision 5, order-sensitivity is chosen file-by-file
errors.Should().BeEquivalentTo(expected);  // error lists  → errors.ShouldBe(expected, ignoreOrder: true);
value.Should().BeEquivalentTo(expected);   // deliberate-order Value comparisons → value.ShouldBe(expected);
coll.Should().ContainSingle();             // → coll.Count.ShouldBe(1); (or ShouldHaveSingleItem())
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

## Vertical Slices

Vertical slicing for a pure-rewrite task is unusual: there is no "end-to-end path" to demo until the project compiles. The slicing principle here is **monotonically shrinking the compile-error set** — each slice ends with a concrete, verifiable checkpoint (`dotnet build` output, `dotnet test` count) that is strictly better than the previous slice's. A reviewer can pause after any slice, inspect the diff, and run the same check locally.

---

### Slice 1: Project scaffolding (globals + csproj)

**Goal:** Lay down the two infra changes that every subsequent slice depends on, in isolation, so later slices produce clean single-purpose diffs.

**Changes:**
- MODIFY: `tests/Tests.csproj` — add `<OutputType>Exe</OutputType>`.
- MODIFY: `tests/Usings.cs` — add `global using Shouldly;`.

**Checkpoint:**
- `dotnet build tests/Tests.csproj` still fails (expected — 18 files still use `.Should()`), but the error set is **unchanged** from baseline in count and kind (only CS-level FluentAssertions-missing errors, no new csproj/usings errors introduced).
- `dotnet build src/ErrorOr.csproj` succeeds unchanged.

**Estimated size:** 2 lines changed across 2 files.

---

### Slice 2: Finish the partially-migrated file

**Goal:** Close out `AggregationTests.cs` first, because it is the only file already straddling both libraries and it contains the highest density of the trickiest idioms (`BeEquivalentTo` on error lists, `ParamName`, async exceptions). Finishing it first de-risks the mapping table against the toughest cases before fanning out.

**Changes:**
- MODIFY: `tests/ErrorOr/ErrorOr.AggregationTests.cs`
  - Remove file-local `using Shouldly;` (now global via Slice 1).
  - Remove any `using FluentAssertions;` if present.
  - Rewrite the remaining 53 `.Should()` sites per the mapping table. Apply Decision 5 (error-list equivalence → `ignoreOrder: true`), Decision 6 (inline `Should.Throw<T>` + `ex.ParamName.ShouldBe(...)`), Decision 7 (`Throw<T>`, not `ThrowExactly<T>`).

**Checkpoint:**
- `dotnet build tests/Tests.csproj` error count drops (AggregationTests stops contributing errors).
- `dotnet test tests/Tests.csproj --filter FullyQualifiedName~AggregationTests` passes on `net10.0` (single-TFM run is sufficient for the slice checkpoint; full matrix is verified in Slice 5).
- `grep -l FluentAssertions tests/ErrorOr/ErrorOr.AggregationTests.cs` returns nothing.

**Estimated size:** ~53 call-site rewrites in one file.

---

### Slice 3: High-exception-density files

**Goal:** Migrate the files dominated by `Should.Throw` / `ParamName` idioms, where the Decision 6 / Decision 7 patterns matter most. Doing these together keeps the exception-rewrite mental model hot.

**Files (in order):**
1. `tests/ErrorOr/ErrorOr.InstantiationTests.cs` *(21 ParamName/exception sites)*
2. `tests/ErrorOr/ErrorOr.BuilderTests.cs`
3. `tests/ErrorOr/ErrorOr.FailIfTests.cs`
4. `tests/ErrorOr/ErrorOr.FailIfAsyncTests.cs`
5. `tests/Errors/ErrorTests.cs`
6. `tests/Errors/Error.EqualityTests.cs`

**Changes:** For each file: delete `using FluentAssertions;`, rewrite every `.Should()` call, leave all other code untouched. No structural edits, no test logic changes.

**Checkpoint:**
- `dotnet build tests/Tests.csproj` error count drops to only the Slice-4 file set.
- `dotnet test --filter FullyQualifiedName~InstantiationTests|BuilderTests|FailIfTests|FailIfAsyncTests|Errors` passes on `net10.0`.
- `grep -l "using FluentAssertions" tests/ErrorOr/ErrorOr.{Instantiation,Builder,FailIf,FailIfAsync}Tests.cs tests/Errors/*.cs` returns nothing.

**Estimated size:** ~150 call-site rewrites across 6 files.

---

### Slice 4: Remaining Match/Switch/Then/Else/Factory/Equality/ToErrorOr/Aot files

**Goal:** Mop up the remaining 11 files. These are dominated by `Be`/`BeOfType`/`BeEquivalentTo` sites — mostly mechanical. Decision 5 (`BeEquivalentTo` order sensitivity) is applied file-by-file per the design rationale: `FactoryTests.cs`, `ElseTests.cs`, `MatchAsyncTests.cs` keep strict order; elsewhere use `ignoreOrder: true` for error-list comparisons.

**Files:**
- `tests/ErrorOr/ErrorOr.MatchTests.cs`
- `tests/ErrorOr/ErrorOr.MatchAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.SwitchTests.cs`
- `tests/ErrorOr/ErrorOr.SwitchAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.ThenTests.cs`
- `tests/ErrorOr/ErrorOr.ThenAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.ElseTests.cs`
- `tests/ErrorOr/ErrorOr.ElseAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.FactoryTests.cs`
- `tests/ErrorOr/ErrorOr.EqualityTests.cs`
- `tests/ErrorOr/ErrorOr.ToErrorOrTests.cs`
- `tests/ErrorOr/ErrorOr.AotCompatibilityTests.cs`

**Changes:** Same pattern as Slice 3 — delete `using FluentAssertions;`, rewrite `.Should()` sites using the mapping table.

**Checkpoint:**
- `dotnet build tests/Tests.csproj` succeeds on `net10.0` with **zero errors and zero FluentAssertions-related warnings**.
- `grep -rn "using FluentAssertions" tests/` returns nothing.
- `grep -rn "\.Should()" tests/` returns nothing (Shouldly uses `ShouldXxx()`, never `.Should()`).
- `dotnet test tests/Tests.csproj --framework net10.0` passes all tests.

**Estimated size:** ~225 call-site rewrites across 12 files.

---

### Slice 5: Hardening — full matrix, coverage, cleanup sweep

**Goal:** Prove the rewrite holds across the full CI matrix and catch anything a single-TFM build missed (e.g., TFM-conditional nullability or API differences that Shouldly might surface).

**Changes:** No new file edits by default. If the matrix build surfaces any issue, fix at the relevant call site (will almost always be an overload-resolution fix, never a semantic change).

**Checkpoint:**
- `dotnet build tests/Tests.csproj` succeeds on `net8.0`, `net9.0`, `net10.0`.
- `dotnet test tests/Tests.csproj` passes on all three TFMs with the pre-migration test count preserved — no tests silently skipped or dropped (compare `--logger "console;verbosity=normal"` totals against the baseline recorded before Slice 1).
- `dotnet build src/ErrorOr.csproj` is byte-for-byte unchanged (sanity: confirm `src/` was not touched).
- Repo-wide negative checks:
  - `grep -rn "FluentAssertions" .` returns nothing except research/design/structure docs.
  - `grep -rn "\.Should()" tests/` returns nothing.
  - `grep -rn "ThrowExactly" tests/` returns nothing (Decision 7).
- `codecov.yml` thresholds still pass locally (`dotnet test --collect:"XPlat Code Coverage"` + report inspection).
- GitHub Actions `build.yml` is green when pushed.

**Estimated size:** 0 lines expected; < ~10 line adjustments if matrix surfaces any overload issue.

---

## Integration Points

All integration is test-project-local. No production code seams are touched.

- `tests/Tests.csproj:3` *(`<PropertyGroup>` block, lines 3–10)* — Slice 1 adds `<OutputType>Exe</OutputType>` here (Decision 2).
- `tests/Tests.csproj:18–30` *(`<ItemGroup>` with `PackageReference`s)* — Confirm no FluentAssertions package reference exists or needs removing; Shouldly 4.3.0 is already at line 20 (Decision 3 — no changes).
- `tests/Usings.cs:1` — Slice 1 appends `global using Shouldly;` after the existing `global using Xunit;` (Decision 1, extending the existing global-usings pattern documented in design §Patterns to Reuse).
- `tests/ErrorOr/ErrorOr.AggregationTests.cs:2` — Slice 2 removes the now-redundant file-local `using Shouldly;` (globalized in Slice 1).
- `tests/ErrorOr/ErrorOr.AggregationTests.cs:20–21` — Slice 2 preserves these already-migrated `ShouldBe` calls as reference form; the rest of the file is rewritten in the same style.
- `tests/ErrorOr/TestUtils.cs` — **no integration.** Decision 6 explicitly keeps this file to domain helpers only; no assertion helpers are added here.
- `.github/workflows/build.yml` — **no integration.** The workflow runs `dotnet test` with no FluentAssertions-specific steps; it picks up the rewrite automatically on push.
- `src/ErrorOr.csproj` and all `src/**/*.cs` — **no integration.** Decision 4 freezes `src/` completely.

## Rollback Strategy

Each slice is an independent, additive set of commits on a feature branch. Rollback granularity is per-slice via `git revert`:

- **Slice 1 revert** undoes two one-line infra edits; subsequent slices would then fail to compile (their `ShouldXxx` calls depend on the global using), so Slice 1 revert implies reverting all later slices too.
- **Slice 2–4 reverts** each undo a bounded set of file-level rewrites; the previous slice's state (fewer-files-migrated, build still broken) is restored.
- **Slice 5** is verification-only in the expected case, so "revert" just means abandoning the branch.

Because no migration, no schema change, no CI change, and no `src/` change is in scope, there is no production rollback surface and no data to reconcile. The branch is either merged green or not merged. If post-merge a regression appears, `git revert` of the merge commit is clean: it restores the previous test-project state and leaves `src/`, `.slnx`, and workflows untouched. No feature flags, no follow-up drops, no migrations to reverse.

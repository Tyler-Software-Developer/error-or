---
date: 2026-04-11 20:19:32 CDT
researcher: Tyler Miller
git_commit: 14e4f324235c18948e6e377970c0ce8429d20dca
branch: main
repository: TylerSoftware.ErrorOr
topic: "NuGet updates and FluentAssertions → Shouldly migration — current state"
tags: [research, codebase, testing, shouldly, xunit, fluentassertions, nuget]
status: complete
last_updated: 2026-04-11
last_updated_by: Tyler Miller
---

# Research: NuGet Updates & FluentAssertions → Shouldly Migration

## Research Question

What is the actual current state of the `ErrorOr` test project's FluentAssertions → Shouldly migration and its NuGet dependency surface, and what do the latest (April 2026) recommendations look like for the testing stack it uses?

## Summary

The migration is **in progress and incomplete**. FluentAssertions has already been removed from `tests/Tests.csproj` as a package reference, but **18 of 19 test files still contain `using FluentAssertions;` directives and 431 total `.Should()`-style FluentAssertions calls across 19 files**. The one file whose migration has been started — `tests/ErrorOr/ErrorOr.AggregationTests.cs` — is itself in a **mixed, inconsistent state**: it has `using Shouldly;`, two Shouldly-style assertions (`ShouldBe`), and 53 remaining FluentAssertions-style `.Should()` calls. In its current form, the test project will not compile because the API those calls bind to is no longer referenced.

The test project targets `net8.0; net9.0; net10.0` and already pulls in the modern xUnit v3 stack (`xunit.v3` 3.2.2, `xunit.runner.visualstudio` 3.1.5, `Microsoft.NET.Test.Sdk` 18.4.0, `coverlet.collector` 8.0.1, `Shouldly` 4.3.0). External research confirms these are the current stable versions as of April 2026. The `src/ErrorOr.csproj` library targets `netstandard2.0; net8.0; net9.0; net10.0` and depends on `Microsoft.Bcl.HashCode` 6.0.0 (conditional), `Nullable` 1.3.1, and `Microsoft.SourceLink.GitHub` 10.0.201 — all current. There is no `Directory.Packages.props`/CPM, no `global.json`, and no `NuGet.config`. The solution has been converted from `ErrorOr.sln` to `ErrorOr.slnx` (XML format), which SDK 9.0.200+ and VS 17.14+ handle transparently.

One notable xUnit v3 migration requirement surfaced by external research is **missing** from `tests/Tests.csproj`: xUnit v3 expects test projects to declare `<OutputType>Exe</OutputType>` (the v3 runner model builds tests as executables). The current csproj does not set this.

A second observation: `Usings.cs` contains only `global using Xunit;`. There is no `global using Shouldly;`, so each file that migrates needs its own `using Shouldly;` (or a global using must be added).

## Detailed Findings

### 1. Current package inventory

**`src/ErrorOr.csproj`** (`C:\GitHub\TylerSoftware.ErrorOr\src\ErrorOr.csproj`)

- `TargetFrameworks`: `netstandard2.0; net8.0; net9.0; net10.0` (line 5)
- `LangVersion`: `latest` (line 6)
- `Nullable`: `enable` (line 8)
- `GenerateDocumentationFile`: `true` (line 9)
- `IsAotCompatible`: `true` for `net8.0`+ (lines 13–15)
- `PackageId`: `TylerSoftware.ErrorOr`, `Version`: `3.1.0` (lines 29–30)
- PackageReferences:
  - `Nullable` 1.3.1, PrivateAssets=all (line 79)
  - `Microsoft.SourceLink.GitHub` 10.0.201, PrivateAssets=All (line 85)
  - `Microsoft.Bcl.HashCode` 6.0.0 — conditional on `$(TargetFramework) == 'netstandard2.0'` (line 88)
- No `Directory.Packages.props` and no CPM in use.

**`tests/Tests.csproj`** (`C:\GitHub\TylerSoftware.ErrorOr\tests\Tests.csproj`)

- `TargetFrameworks`: `net8.0; net9.0; net10.0` (line 4)
- `IsPackable`: `false`, `IsTestProject`: `true` (lines 8–9)
- Excludes the AOT console project subfolder: `<Compile Remove="AotCompatibility\**" />`, `<None Remove="AotCompatibility\**" />` (lines 14–15)
- PackageReferences:
  - `Microsoft.NET.Test.Sdk` 18.4.0 (line 19)
  - `Shouldly` 4.3.0 (line 20)
  - `xunit.runner.visualstudio` 3.1.5 (line 21) — full asset/private flags
  - `coverlet.collector` 8.0.1 (line 25)
  - `xunit.v3` 3.2.2 (line 29)
- **No `FluentAssertions` package reference.**
- **No `<OutputType>Exe</OutputType>`** — external research flags this as a xUnit v3 requirement (see section 7).
- ProjectReference to `../src/ErrorOr.csproj` (line 33).

**`tests/AotCompatibility/AotCompatibility.csproj`**

- `OutputType`: `Exe`, `TargetFramework`: `net10.0`, `PublishAot`: `true`, `InvariantGlobalization`: `true`, `IlcGenerateStackTraceData`: `false`, `TreatWarningsAsErrors`: `true`
- ProjectReference to `../../src/ErrorOr.csproj`
- This is a separate standalone AOT check console app; it is NOT built by `tests/Tests.csproj`.

**`Directory.Build.props`** (`C:\GitHub\TylerSoftware.ErrorOr\Directory.Build.props`)

- `LangVersion`: `14.0`
- `TreatWarningsAsErrors`: `true` (appears twice)
- `ImplicitUsings`: `enable`, `Nullable`: `enable`, `GenerateDocumentationFile`: `true`
- PackageReference: `StyleCop.Analyzers` 1.2.0-beta.556 (PrivateAssets=all)

**Not present in the repo**: `Directory.Packages.props`, `Directory.Build.targets`, `global.json`, `NuGet.config`/`nuget.config`.

### 2. Migration completeness — NOT complete

**Files that still have `using FluentAssertions;` (18 files):**

- `tests/Errors/ErrorTests.cs`
- `tests/Errors/Error.EqualityTests.cs`
- `tests/ErrorOr/ErrorOr.BuilderTests.cs`
- `tests/ErrorOr/ErrorOr.FactoryTests.cs`
- `tests/ErrorOr/ErrorOr.InstantiationTests.cs`
- `tests/ErrorOr/ErrorOr.EqualityTests.cs`
- `tests/ErrorOr/ErrorOr.ElseTests.cs`
- `tests/ErrorOr/ErrorOr.ElseAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.MatchTests.cs`
- `tests/ErrorOr/ErrorOr.MatchAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.SwitchTests.cs`
- `tests/ErrorOr/ErrorOr.SwitchAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.ThenTests.cs`
- `tests/ErrorOr/ErrorOr.ThenAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.FailIfTests.cs`
- `tests/ErrorOr/ErrorOr.FailIfAsyncTests.cs`
- `tests/ErrorOr/ErrorOr.ToErrorOrTests.cs`
- `tests/ErrorOr/ErrorOr.AotCompatibilityTests.cs`

**`.Should()` call counts by file (431 total across 19 files):**

| File | Count |
|---|---|
| `tests/ErrorOr/ErrorOr.AotCompatibilityTests.cs` | 64 |
| `tests/ErrorOr/ErrorOr.ElseTests.cs` | 62 |
| `tests/ErrorOr/ErrorOr.InstantiationTests.cs` | 67 |
| `tests/ErrorOr/ErrorOr.AggregationTests.cs` | 53 |
| `tests/ErrorOr/ErrorOr.FactoryTests.cs` | 23 |
| `tests/ErrorOr/ErrorOr.ElseAsyncTests.cs` | 20 |
| `tests/ErrorOr/ErrorOr.FailIfTests.cs` | 18 |
| `tests/ErrorOr/ErrorOr.FailIfAsyncTests.cs` | 18 |
| `tests/ErrorOr/ErrorOr.MatchAsyncTests.cs` | 12 |
| `tests/ErrorOr/ErrorOr.SwitchTests.cs` | 12 |
| `tests/ErrorOr/ErrorOr.SwitchAsyncTests.cs` | 12 |
| `tests/ErrorOr/ErrorOr.MatchTests.cs` | 12 |
| `tests/ErrorOr/ErrorOr.ToErrorOrTests.cs` | 12 |
| `tests/ErrorOr/ErrorOr.EqualityTests.cs` | 11 |
| `tests/ErrorOr/ErrorOr.BuilderTests.cs` | 11 |
| `tests/ErrorOr/ErrorOr.ThenTests.cs` | 10 |
| `tests/Errors/ErrorTests.cs` | 5 |
| `tests/Errors/Error.EqualityTests.cs` | 5 |
| `tests/ErrorOr/ErrorOr.ThenAsyncTests.cs` | 4 |

**Compilation implication:** With `FluentAssertions` removed from `Tests.csproj`, the 431 `.Should()` calls have no extension method definitions to bind to. The test project is not currently compilable.

**FluentAssertions package reference:** None. A repo-wide grep for `FluentAssertions` only matches `using FluentAssertions;` in the 18 files above plus the research/questions file. No csproj anywhere in the repo references the package (neither `tests/Tests.csproj` nor `Directory.Build.props`).

### 3. Existing Shouldly usage patterns — only one file, inconsistent

**Only one file currently `using Shouldly;`**: `tests/ErrorOr/ErrorOr.AggregationTests.cs:2`.

Inside that file, the migration is **partial and mixed**:

- Shouldly-style (2 occurrences, both in the first test method):
  - `result.IsError.ShouldBe(true);` — `tests/ErrorOr/ErrorOr.AggregationTests.cs:20`
  - `result.Errors.Count.ShouldBe(2);` — `tests/ErrorOr/ErrorOr.AggregationTests.cs:21`
- FluentAssertions-style (53 occurrences, rest of the file):
  - `result.Errors.Should().Contain(error1);` — `...AggregationTests.cs:22`
  - `result.IsError.Should().BeTrue();` — `...AggregationTests.cs:39`
  - `result.Errors.Should().HaveCount(3);` — `...AggregationTests.cs:40`
  - `result.Errors.Should().BeEquivalentTo(errors);` — `...AggregationTests.cs:92`
  - `act.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("errors");` — `...AggregationTests.cs:57–58, 71–72, 129–130, 143–144, 212–213, 223–224, 292–293, 303–304`
  - Indexer access: `result.Errors[0].Should().Be(existingError);` — `...AggregationTests.cs:113–115`

No other test file uses any `ShouldXxx` Shouldly API. There is no `global using Shouldly;` in `tests/Usings.cs` (which contains only `global using Xunit;`).

### 4. xUnit version & conventions

- Packages referenced: `xunit.v3` 3.2.2 + `xunit.runner.visualstudio` 3.1.5 + `Microsoft.NET.Test.Sdk` 18.4.0 — this is the **xUnit v3** stack. There is no `xunit` (v2 metapackage) reference.
- `tests/Usings.cs` (2 lines total): `global using Xunit;`
- Tests use `[Fact]` consistently; a grep for `[Theory]` during exploration did not surface any `[Theory]`/`[InlineData]` usage in `AggregationTests.cs` (the file read in full), and the other files were not fully read. Every test in `AggregationTests.cs` is a `[Fact]` (no `[Theory]`).
- No `[Collection]`, `[CollectionDefinition]`, `IClassFixture`, `IAsyncLifetime`, `ITestOutputHelper`, or `[assembly: …]` usages were encountered.
- No `xunit.runner.json` configuration file is present.
- External research notes that xUnit v3 expects test projects to set `<OutputType>Exe</OutputType>`; `tests/Tests.csproj` does not currently set it. See section 7.

### 5. Test project structure

File layout (`C:\GitHub\TylerSoftware.ErrorOr\tests\`):

```
tests/
├── .editorconfig
├── Tests.csproj
├── Usings.cs                         # only: global using Xunit;
├── ErrorOr/
│   ├── ErrorOr.AggregationTests.cs   # partially migrated to Shouldly
│   ├── ErrorOr.AotCompatibilityTests.cs
│   ├── ErrorOr.BuilderTests.cs
│   ├── ErrorOr.ElseAsyncTests.cs
│   ├── ErrorOr.ElseTests.cs
│   ├── ErrorOr.EqualityTests.cs
│   ├── ErrorOr.FactoryTests.cs
│   ├── ErrorOr.FailIfAsyncTests.cs
│   ├── ErrorOr.FailIfTests.cs
│   ├── ErrorOr.InstantiationTests.cs
│   ├── ErrorOr.MatchAsyncTests.cs
│   ├── ErrorOr.MatchTests.cs
│   ├── ErrorOr.SwitchAsyncTests.cs
│   ├── ErrorOr.SwitchTests.cs
│   ├── ErrorOr.ThenAsyncTests.cs
│   ├── ErrorOr.ThenTests.cs
│   ├── ErrorOr.ToErrorOrTests.cs
│   └── TestUtils.cs
├── Errors/
│   ├── Error.EqualityTests.cs
│   └── ErrorTests.cs
└── AotCompatibility/
    ├── AotCompatibility.csproj
    └── Program.cs
```

**Naming convention**: dotted-namespace-style filenames grouping tests by type under test plus the method/feature: `ErrorOr.<Feature>Tests.cs`, `Error.<Feature>Tests.cs`. All test classes appear in `namespace Tests;`.

**Shared helpers**: `tests/ErrorOr/TestUtils.cs` declares `public static class Convert` with four helper methods that wrap `int.Parse`/`ToString` in `ErrorOr<>` results (`ToString(int)`, `ToInt(string)`, `ToIntAsync(string)`, `ToStringAsync(int)`). **There are no assertion helpers, base classes, or custom `.Should()` wrappers** — the migration does not need to update any bespoke assertion infrastructure; the work is confined to direct `.Should()...` call sites.

### 6. Build & CI surface

`.github/workflows/` contains three workflows; none reference FluentAssertions by name, and none pin SDK versions via `global.json`.

**`.github/workflows/build.yml`** — triggers on push/PR to `main` + `workflow_dispatch`. Matrix across `dotnet-version: [8.0.x, 9.0.x, 10.0.x]` on ubuntu-latest. Steps: `actions/checkout@v4` (fetch-depth 0), `actions/setup-dotnet@v4`, `dotnet restore`, `dotnet build -c Release --no-restore`, `dotnet test -c Release --no-restore --verbosity normal --collect:"XPlat Code Coverage"`, then `codecov/codecov-action@v5` conditional on `dotnet-version == 10.0.x` using `CODECOV_TOKEN`.

**`.github/workflows/aot-compatibility.yml`** — triggers on push/PR to `main` + `workflow_dispatch`. Matrix across `{ubuntu-latest/linux-x64, windows-latest/win-x64, macos-latest/osx-x64}`. Uses `setup-dotnet@v4` with `dotnet-version: 10.0.x`, restores/builds/publishes `tests/AotCompatibility/AotCompatibility.csproj -r <rid>`, runs the published native binary, then reports binary size.

**`.github/workflows/publish.yml`** — triggers on tags `v*` + `workflow_dispatch`. `permissions: id-token: write, contents: read` (OIDC). Uses `dotnet-version: 10.0.x`, runs `dotnet test -c Release --no-build`, `dotnet pack -c Release --no-build src/ErrorOr.csproj`, then `dotnet nuget push ./artifacts/*.nupkg` and `...*.snupkg` with `NUGET_API_KEY` and `--skip-duplicate`.

**`codecov.yml`** — project & patch coverage targets `auto` with 1% threshold; comment layout `"reach,diff,flags,files"`, `require_changes: true`.

**Solution file change** — `ErrorOr.sln` has been **deleted** (shown in git status), replaced by `ErrorOr.slnx`:

```xml
<Solution>
  <Project Path="src/ErrorOr.csproj" />
  <Project Path="tests/Tests.csproj" />
</Solution>
```

No workflow step passes an explicit `.sln`/`.slnx` path to `dotnet build`/`dotnet test`; they rely on CWD discovery, which SDK 9.0.200+ handles for `.slnx`.

### 7. Latest recommended versions (external research, April 2026)

Full external research document: `research/docs/2026-04-11-dotnet-test-stack-versions.md`.

| Package | Repo version | Current stable (April 2026) | Notes |
|---|---|---|---|
| `xunit.v3` | 3.2.2 | **3.2.2** | Current. `xunit` (v2) 2.9.3 is the final v2 release — deprecated/migrate path. |
| `xunit.runner.visualstudio` | 3.1.5 | **3.1.5** | Must be 3.x when using `xunit.v3`. |
| `Shouldly` | 4.3.0 | **4.3.0** | Current. |
| `Microsoft.NET.Test.Sdk` | 18.4.0 | **18.4.0** | Current (released ~2026-04-06). |
| `coverlet.collector` | 8.0.1 | **8.0.1** | Current. |
| `Microsoft.SourceLink.GitHub` | 10.0.201 | **10.0.201** | Current. |
| `Microsoft.Bcl.HashCode` | 6.0.0 | **6.0.0** | Current. |
| `Nullable` | 1.3.1 | **1.3.1** (final, 2022) | No further updates; still current. |
| `StyleCop.Analyzers` | 1.2.0-beta.556 | beta.556 is latest; 1.1.118 is last "stable" | No stable release since 2019; pinning to beta.556 is the common practice. |

**xUnit v3 migration caveats** surfaced by external research:

- Test projects should declare `<OutputType>Exe</OutputType>` (v3 runner model builds tests as executables). `tests/Tests.csproj` does not currently set this.
- Minimum targets are .NET Framework 4.7.2 or **.NET 8**; .NET 6/7 unsupported. Current repo targets `net8.0; net9.0; net10.0`, so it is compliant.
- `xunit.abstractions` has been removed.
- `IAsyncLifetime` method signatures switched from `Task` to `ValueTask`.
- `async void` tests now hard-fail; use `async Task`.
- New APIs: `Assert.Skip`, `TestContext.Current`, `[assembly: AssemblyFixture]`, `[Fact(Explicit = true)]`, `MatrixTheoryData`.
- `global using Xunit;` works unchanged in v3.

**.slnx format status** (April 2026):

- GA in Visual Studio 17.14+.
- `dotnet sln migrate` requires SDK 9.0.200+.
- Standard CLI commands (`dotnet build`/`test`/`restore`) handle `.slnx` transparently on SDK 9+.
- Becomes the default for `dotnet new sln` in .NET 10.

### 8. Shouldly idiom guidance (external research)

From `research/docs/2026-04-11-dotnet-test-stack-versions.md`:

- **Subject-first syntax**: `result.ShouldBe(expected)` — not a builder chain.
- **Equality**: `actual.ShouldBe(expected)`, `actual.ShouldNotBe(expected)`, `value.ShouldBeTrue()`/`ShouldBeFalse()`, `value.ShouldBeNull()`/`ShouldNotBeNull()`.
- **Type checks**: `value.ShouldBeOfType<T>()` (strict) / `value.ShouldBeAssignableTo<T>()`.
- **Exceptions**: `var ex = Should.Throw<T>(() => action()); ex.ParamName.ShouldBe("name");` and async `Should.ThrowAsync<T>(() => asyncAction());`. No chained `.And.ParamName.Should().Be(...)` — capture the exception and assert on its members.
- **Collections**:
  - `collection.ShouldContain(item)` / `ShouldNotContain`
  - `collection.ShouldHaveSingleItem()`
  - `collection.ShouldBeEmpty()` / `ShouldNotBeEmpty()`
  - `collection.Count.ShouldBe(n)` for count assertions
  - Ordering: `collection.ShouldBe(new[] { a, b, c })` compares element-by-element; `ignoreOrder: true` parameter exists for unordered equality.
- **Equivalence**: **No direct replacement for FluentAssertions `BeEquivalentTo`**. Options: implement `Equals` on the type, use `ShouldBe(expected, ignoreOrder: true)` for collections, or use `ShouldSatisfyAllConditions` to spell out per-member equality.
- **Multi-assertion grouping**:
  ```csharp
  result.ShouldSatisfyAllConditions(
      r => r.IsError.ShouldBeTrue(),
      r => r.Errors.Count.ShouldBe(2),
      r => r.Errors.ShouldContain(error1));
  ```
  Collects all failures before reporting.
- **Anti-patterns**: mixing `async void` tests with Shouldly async helpers (use `async Task`); chaining `.And.` style (that is FluentAssertions, not Shouldly); relying on a non-existent `BeEquivalentTo` analogue.
- **No official FluentAssertions → Shouldly cheat sheet** exists. Mapping is mechanical:
  - `x.Should().Be(y)` → `x.ShouldBe(y)`
  - `x.Should().BeTrue()` → `x.ShouldBeTrue()`
  - `x.Should().HaveCount(n)` → `x.Count.ShouldBe(n)`
  - `x.Should().Contain(y)` → `x.ShouldContain(y)`
  - `act.Should().ThrowExactly<T>().And.ParamName.Should().Be("p")` → `var ex = Should.Throw<T>(() => act()); ex.ParamName.ShouldBe("p");`
  - `x.Should().BeEquivalentTo(y)` → no direct equivalent.

## Code References

- `src/ErrorOr.csproj:5` — TargetFrameworks `netstandard2.0; net8.0; net9.0; net10.0`
- `src/ErrorOr.csproj:79,85,88` — Nullable 1.3.1, SourceLink.GitHub 10.0.201, Microsoft.Bcl.HashCode 6.0.0
- `tests/Tests.csproj:4` — TargetFrameworks `net8.0; net9.0; net10.0`
- `tests/Tests.csproj:19–29` — Shouldly 4.3.0, Microsoft.NET.Test.Sdk 18.4.0, xunit.runner.visualstudio 3.1.5, coverlet.collector 8.0.1, xunit.v3 3.2.2
- `tests/Tests.csproj` — no `<OutputType>Exe</OutputType>` declared, no FluentAssertions reference
- `Directory.Build.props` — LangVersion 14.0, TreatWarningsAsErrors, StyleCop.Analyzers 1.2.0-beta.556
- `ErrorOr.slnx:1–5` — the new solution file (replaces the deleted `ErrorOr.sln`)
- `tests/Usings.cs:1` — only `global using Xunit;` (no Shouldly global)
- `tests/ErrorOr/ErrorOr.AggregationTests.cs:2` — `using Shouldly;`
- `tests/ErrorOr/ErrorOr.AggregationTests.cs:20–21` — the only two Shouldly-style assertions in the codebase (`ShouldBe`)
- `tests/ErrorOr/ErrorOr.AggregationTests.cs:22–304` — 53 remaining FluentAssertions-style `.Should()` calls in the same partially-migrated file
- `tests/ErrorOr/TestUtils.cs:1–15` — `public static class Convert`, no assertion helpers
- `.github/workflows/build.yml` — dotnet-version matrix `8.0.x, 9.0.x, 10.0.x`, `dotnet test … --collect:"XPlat Code Coverage"`, Codecov v5 conditional on 10.0.x
- `.github/workflows/publish.yml` — OIDC publish of `./artifacts/*.nupkg`/`*.snupkg` on `v*` tag
- `.github/workflows/aot-compatibility.yml` — publishes `tests/AotCompatibility/AotCompatibility.csproj` with `-r <rid>` and runs the binary

## Architecture Documentation

- **No central package management**: versions are declared per-project in each `.csproj`.
- **Target framework split**: the library spans `netstandard2.0; net8.0; net9.0; net10.0`, while tests skip `netstandard2.0` and only run on `net8.0; net9.0; net10.0`.
- **AOT compatibility** is verified via a **separate console project** (`tests/AotCompatibility/`) explicitly excluded from the xUnit test project's compile items (`tests/Tests.csproj:14–15`). Its compatibility is verified by the `aot-compatibility.yml` workflow actually publishing and running a native binary per RID — not by xUnit tests.
- **Solution format**: the repo has migrated from `.sln` to `.slnx` (XML) as part of this change set.
- **Test naming convention**: `<TypeUnderTest>.<FeatureOrMethod>Tests.cs`, all under `namespace Tests;`.
- **Assertion posture (intended end-state)**: a direct swap of FluentAssertions for Shouldly at each call site; there are no wrapper helpers to update because none exist (`TestUtils.cs` contains only `Convert` domain helpers, not assertion helpers).

## Historical Context (from research/)

- `research/questions/2026-04-11-nuget-updates-shouldly-migration.md` — the research-questions artifact driving this document; lists the eight questions answered above.
- `research/docs/2026-04-11-dotnet-test-stack-versions.md` — the external-research output document with per-package version detail, xUnit v3 migration caveats, Shouldly idiom guidance, and the .slnx GA status, with citations to NuGet.org/GitHub release notes/Microsoft Learn.

## Related Research

- `research/docs/2026-04-11-dotnet-test-stack-versions.md` (see above)

## Open Questions

1. Is there a deliberate reason `tests/Tests.csproj` does not declare `<OutputType>Exe</OutputType>` despite referencing `xunit.v3` 3.2.2? (Current CI builds/tests appear to work in the matrix prior to this change set — verify whether the change set has been run through CI yet and whether the runner tolerates the omission.)
2. `ErrorOr.AggregationTests.cs` contains a `BeEquivalentTo` assertion (`result.Errors.Should().BeEquivalentTo(errors);` — `tests/ErrorOr/ErrorOr.AggregationTests.cs:92`) and `result.Value.Should().BeEquivalentTo(new[] { 1, 2, 3 });` (line 240). Shouldly has no direct `BeEquivalentTo` — how should these be expressed? (Candidates: `ShouldBe(expected, ignoreOrder: true)` for collections, or expansion into `ShouldSatisfyAllConditions`.) This also applies to `ErrorOr.FactoryTests.cs`, `ErrorOr.ElseTests.cs`, and `ErrorOr.MatchAsyncTests.cs` which all use `BeEquivalentTo`.
3. The `.And.ParamName.Should().Be("...")` idiom is used in every exception-assertion site (Aggregation, Builder, and likely others). The Shouldly replacement requires capturing the exception (`var ex = Should.Throw<T>(...); ex.ParamName.ShouldBe(...)`). Confirm there is no desire for a shared helper for this recurring pattern before making the change across all sites.
4. Should a `global using Shouldly;` be added to `tests/Usings.cs` so that each migrated file does not need an individual `using Shouldly;`?
5. Is there interest in migrating `ErrorOr.slnx` registration under CI by passing it explicitly to `dotnet` commands, or is CWD discovery acceptable on SDK 9+ runners used by `setup-dotnet@v4`?

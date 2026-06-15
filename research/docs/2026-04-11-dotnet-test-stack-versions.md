---
date: 2026-04-11
topic: .NET testing stack — current stable versions and migration guidance
tags: [research, external, testing-stack, shouldly, xunit]
---

# .NET Testing Stack — Current Stable Versions and Migration Guidance

Research date: 2026-04-11

---

## Version Summary Table

| Package | Current Stable | Notes |
|---|---|---|
| `xunit` | 2.9.3 | Deprecated; migrate to `xunit.v3` |
| `xunit.v3` | 3.2.2 | GA stable |
| `xunit.runner.visualstudio` | 3.1.5 | Use 3.x series with xunit.v3 |
| `Shouldly` | 4.3.0 | Current stable; no update needed |
| `Microsoft.NET.Test.Sdk` | 18.4.0 | Released ~2026-04-06; current stable |
| `coverlet.collector` | 8.0.1 | Current stable; requires .NET 8+ or .NET Framework 4.7.2+ |
| `Microsoft.SourceLink.GitHub` | 10.0.201 | Current stable |
| `Microsoft.Bcl.HashCode` | 6.0.0 | Current stable |
| `Nullable` | 1.3.1 | Final release (July 2022); source-only polyfill |
| `StyleCop.Analyzers` | 1.1.118 (stable) / 1.2.0-beta.556 (prerelease) | No stable release since 2019 |

---

## 1. xunit (v2) — Deprecated

**Current version**: 2.9.3 (deprecated)

xUnit v2 is officially deprecated. The team recommends migrating all projects to the `xunit.v3` package. No new features will be added to v2; only critical security patches may be considered.

**Action**: Migrate to `xunit.v3`.

---

## 2. xunit.v3 — GA Stable

**Current version**: 3.2.2

xUnit v3 is the current, actively maintained major version. It is a ground-up rewrite with significant architectural changes.

### Key architectural changes from v2

- **Stand-alone executable model**: Test projects must set `<OutputType>Exe</OutputType>` in the `.csproj`. Each test assembly compiles to a self-contained executable.
- **No Application Domain boundary**: The `xunit.abstractions` package is removed entirely. Communication between the test framework and runners is now done via in-process interfaces.
- **Minimum runtimes**: .NET Framework 4.7.2+ or .NET 8+. .NET 6 and .NET 7 are not supported.
- **async void tests fast-fail**: Tests declared `async void` immediately fail with a clear error. Use `async Task` instead.

### IAsyncLifetime changes

In v3, `IAsyncLifetime` extends `IAsyncDisposable`. Both lifecycle methods return `ValueTask`:

```csharp
public class MyFixture : IAsyncLifetime
{
    public async ValueTask InitializeAsync() { /* ... */ }
    public async ValueTask DisposeAsync() { /* ... */ }
}
```

v2 used `Task`-returning methods; update return types to `ValueTask` when migrating.

### New features in v3

- **`Assert.Skip(reason)`** — skip a test at runtime (previously required third-party workarounds).
- **`TestContext.Current`** — ambient access to the running test context (name, cancellation token, output writer, etc.) without constructor injection.
- **`[assembly: AssemblyFixture(typeof(T))]`** — true assembly-scoped fixtures, replacing the `ICollectionFixture` workaround pattern.
- **`[Fact(Explicit = true)]`** — marks a test as explicit; it only runs when directly targeted, not as part of a normal test run.
- **`MatrixTheoryData<T1, T2, ...>`** — generates the Cartesian product of value sets as theory data rows.
- **Built-in CTRF and TRX report generation** — no external reporter packages needed.
- **Microsoft Testing Platform (MTP) support** — opt-in via `<UsesMicrosoftTestingPlatform>true</UsesMicrosoftTestingPlatform>`.

### [Fact] and [Theory] — what changed

`[Fact]` and `[Theory]` attributes are backward-compatible at the surface. The notable changes are:

- `[Fact(Explicit = true)]` is new and has no v2 equivalent.
- `[Theory]` now supports `MatrixTheoryData` as inline data.
- `Skip` property on both attributes continues to work unchanged.
- `async void` test methods now cause an immediate compile-time or runtime failure rather than silently passing.

### Global using

```csharp
// In a GlobalUsings.cs file or _GlobalUsings.cs
global using Xunit;
```

This works identically in v3. No special configuration is required.

---

## 3. xunit.runner.visualstudio

**Current version**: 3.1.5

This package provides Visual Studio / VSTest adapter support. Version 3.x supports xUnit v1, v2, and v3 projects in the same solution. When using `xunit.v3`, always reference the 3.x series of this runner.

**Action**: Update from any 2.x version to 3.1.5.

---

## 4. Shouldly

**Current version**: 4.3.0

Shouldly is a mature assertion library with a fluent, subject-first syntax. No update is needed if already on 4.3.0.

### Core idioms

```csharp
// Equality
result.ShouldBe(expected);
result.ShouldBe(expected, tolerance);          // numeric overload
result.ShouldBe(expected, StringCompareShould.IgnoreCase);

// Type assertions
result.ShouldBeOfType<MyClass>();
result.ShouldBeAssignableTo<IMyInterface>();

// Null / boolean
result.ShouldBeNull();
result.ShouldNotBeNull();
result.ShouldBeTrue();
result.ShouldBeFalse();

// Collections
list.ShouldContain(item);
list.ShouldNotContain(item);
list.ShouldContain(x => x.Id == 42);
list.ShouldBeEmpty();
list.ShouldNotBeEmpty();
list.ShouldHaveSingleItem();
list.ShouldAllBe(x => x.IsActive);
```

### Exception assertions

```csharp
// Synchronous
var ex = Should.Throw<ArgumentNullException>(() => sut.Method(null));
ex.Message.ShouldContain("value");

// Asynchronous
var ex = await Should.ThrowAsync<InvalidOperationException>(async () =>
    await sut.MethodAsync());
ex.Message.ShouldBe("Expected message.");

// Should not throw
Should.NotThrow(() => sut.Method(validArg));
await Should.NotThrowAsync(async () => await sut.MethodAsync());
```

### ParamName assertion pattern

Shouldly does not have a dedicated `ShouldHaveParamName` method. Capture the exception and assert the property directly:

```csharp
var ex = Should.Throw<ArgumentNullException>(() => sut.Method(null, "b"));
ex.ParamName.ShouldBe("a");
```

### Equivalence (no direct BeEquivalentTo)

Shouldly has no built-in deep-equivalence method comparable to FluentAssertions' `BeEquivalentTo`. Alternatives:

1. **`ShouldSatisfyAllConditions`** — assert multiple properties in one block:

```csharp
result.ShouldSatisfyAllConditions(
    () => result.Id.ShouldBe(expected.Id),
    () => result.Name.ShouldBe(expected.Name),
    () => result.Age.ShouldBe(expected.Age)
);
```

2. **Override `Equals`** on the type under test — then `ShouldBe` does deep comparison.
3. **JSON serialization comparison** — serialize both objects and `ShouldBe` the strings (fragile; use sparingly).

### ShouldSatisfyAllConditions

Collects all failures before reporting, similar to NUnit's `Assert.Multiple`. All assertions inside the lambda are evaluated; if any fail, all failures are reported together.

```csharp
actual.ShouldSatisfyAllConditions(
    () => actual.FirstName.ShouldBe("Jane"),
    () => actual.LastName.ShouldBe("Doe"),
    () => actual.Email.ShouldNotBeNullOrEmpty()
);
```

### Anti-patterns

- **Do not chain `.Should()` calls** — Shouldly uses subject-first extension methods, not a builder chain. `x.ShouldBe(y).ShouldNotBe(z)` does not work.
- **Do not use `Should.Throw` for non-throwing code** — use `Should.NotThrow` explicitly; wrapping non-throwing code in `Should.Throw` will fail the test.
- **Avoid `Assert.Equal` mixed with Shouldly** — mixing assertion libraries in one test class reduces diagnostic clarity.

---

## 5. FluentAssertions to Shouldly Migration Cheat Sheet

No official migration guide exists. The table below is derived from both libraries' documentation.

| FluentAssertions | Shouldly |
|---|---|
| `result.Should().Be(expected)` | `result.ShouldBe(expected)` |
| `result.Should().NotBe(expected)` | `result.ShouldNotBe(expected)` |
| `result.Should().BeNull()` | `result.ShouldBeNull()` |
| `result.Should().NotBeNull()` | `result.ShouldNotBeNull()` |
| `result.Should().BeTrue()` | `result.ShouldBeTrue()` |
| `result.Should().BeFalse()` | `result.ShouldBeFalse()` |
| `result.Should().BeOfType<T>()` | `result.ShouldBeOfType<T>()` |
| `result.Should().BeAssignableTo<T>()` | `result.ShouldBeAssignableTo<T>()` |
| `result.Should().BeGreaterThan(n)` | `result.ShouldBeGreaterThan(n)` |
| `result.Should().BeLessThan(n)` | `result.ShouldBeLessThan(n)` |
| `result.Should().BeInRange(lo, hi)` | `result.ShouldBeInRange(lo, hi)` |
| `result.Should().Contain(item)` | `result.ShouldContain(item)` |
| `result.Should().NotContain(item)` | `result.ShouldNotContain(item)` |
| `result.Should().BeEmpty()` | `result.ShouldBeEmpty()` |
| `result.Should().NotBeEmpty()` | `result.ShouldNotBeEmpty()` |
| `result.Should().HaveCount(n)` | `result.ShouldHaveCount(n)` |
| `result.Should().ContainSingle()` | `result.ShouldHaveSingleItem()` |
| `result.Should().AllSatisfy(x => ...)` | `result.ShouldAllBe(x => ...)` |
| `result.Should().BeEquivalentTo(expected)` | `result.ShouldSatisfyAllConditions(...)` (see section 4) |
| `act.Should().Throw<T>()` | `Should.Throw<T>(() => act())` |
| `act.Should().ThrowAsync<T>()` | `await Should.ThrowAsync<T>(async () => await act())` |
| `act.Should().NotThrow()` | `Should.NotThrow(() => act())` |
| `str.Should().Be(expected, because: "reason")` | `str.ShouldBe(expected, customMessage: "reason")` |
| `str.Should().StartWith(prefix)` | `str.ShouldStartWith(prefix)` |
| `str.Should().EndWith(suffix)` | `str.ShouldEndWith(suffix)` |
| `str.Should().Contain(sub)` | `str.ShouldContain(sub)` |
| `str.Should().Match(pattern)` | `str.ShouldMatch(pattern)` (regex) |

---

## 6. Microsoft.NET.Test.Sdk

**Current version**: 18.4.0 (released ~2026-04-06)

This is the MSBuild targets and properties package required by all .NET test projects. It brings in the VSTest platform infrastructure. No configuration changes are typically needed when updating.

---

## 7. coverlet.collector

**Current version**: 8.0.1

Provides in-process code coverage collection via the VSTest data collector interface. Version 8.x requires .NET 8+ or .NET Framework 4.7.2+. The package is referenced in test projects as:

```xml
<PackageReference Include="coverlet.collector" Version="8.0.1">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

---

## 8. Microsoft.SourceLink.GitHub

**Current version**: 10.0.201 (part of the dotnet/sourcelink repo, shipped with the .NET SDK toolchain)

Embeds source link information into PDB files, enabling debuggers to fetch source directly from GitHub. No configuration beyond the package reference is typically needed for public repositories.

---

## 9. Microsoft.Bcl.HashCode

**Current version**: 6.0.0 (aligned with .NET 6 BCL)

Provides the `HashCode` struct backport for targets below .NET Core 2.1. If targeting .NET 8+, the BCL type is available natively and this package may be unnecessary.

---

## 10. Nullable

**Current version**: 1.3.1 (final release, July 2022)

A source-code-only NuGet package that adds the nullable reference type annotation attributes (`[NotNull]`, `[MaybeNull]`, `[AllowNull]`, etc.) to projects targeting frameworks before .NET 5 where those attributes are not part of the BCL. The package injects source files that compile into the consuming project; no runtime dependency is added.

This package will not receive further updates. It covers all annotation types needed for full nullable reference type support on older targets.

---

## 11. StyleCop.Analyzers

**Current version**: 1.1.118 (stable, 2019) / 1.2.0-beta.556 (prerelease, December 2023)

The 1.1.x stable line has not had a release since 2019. The 1.2.0-beta series is actively maintained (latest: December 2023) and is the de facto standard for modern .NET projects. Using the beta is common practice; many teams pin to `1.2.0-beta.556`.

Configured via a `stylecop.json` file and optional `.editorconfig` entries. Does not require a separate `StyleCop.Analyzers.Unstable` package reference for 1.2.0-beta.

---

## 12. .slnx Format — SDK Support Status

The `.slnx` XML-based solution format is the successor to the legacy `.sln` text format.

- **Visual Studio 17.14+**: `.slnx` is GA stable and is the default format for new solutions.
- **.NET 10 SDK**: `.slnx` will be the default format for `dotnet new sln`.
- **`dotnet sln migrate`**: Requires .NET SDK 9.0.200 or later to convert an existing `.sln` to `.slnx`.
- **Earlier SDKs**: `.slnx` can be read by `dotnet build` / `dotnet test` with SDK 9.0.100+, but migration tooling requires 9.0.200+.
- **CI compatibility**: Standard `dotnet` CLI commands (build, test, restore) handle `.slnx` files transparently on SDK 9+.

---

## 13. xUnit v3 Project File Template

A minimal `.csproj` for an xUnit v3 test project:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit.v3" Version="3.2.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.4.0" />
    <PackageReference Include="Shouldly" Version="4.3.0" />
    <PackageReference Include="coverlet.collector" Version="8.0.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

---

## Citations

1. xUnit v3 migration guide — https://xunit.net/docs/getting-started/v3/migration
2. xUnit v3 what is new — https://xunit.net/docs/getting-started/v3/whats-new
3. xunit.v3 on NuGet — https://www.nuget.org/packages/xunit.v3
4. xunit (v2) on NuGet — https://www.nuget.org/packages/xunit
5. xunit.runner.visualstudio on NuGet — https://www.nuget.org/packages/xunit.runner.visualstudio
6. Shouldly documentation root — https://docs.shouldly.org/
7. Shouldly — ShouldBe — https://docs.shouldly.org/documentation/equality/shouldbe
8. Shouldly — Throw / ThrowAsync — https://docs.shouldly.org/documentation/exceptions/throw
9. Shouldly — ShouldSatisfyAllConditions — https://docs.shouldly.org/documentation/should-satisfy-all-conditions
10. Shouldly on NuGet — https://www.nuget.org/packages/Shouldly
11. FluentAssertions documentation — https://fluentassertions.com/introduction
12. Microsoft.NET.Test.Sdk on NuGet — https://www.nuget.org/packages/Microsoft.NET.Test.Sdk
13. coverlet.collector on NuGet — https://www.nuget.org/packages/coverlet.collector
14. coverlet on GitHub — https://github.com/coverlet-coverage/coverlet
15. Microsoft.SourceLink.GitHub on NuGet — https://www.nuget.org/packages/Microsoft.SourceLink.GitHub
16. dotnet/sourcelink on GitHub — https://github.com/dotnet/sourcelink
17. Microsoft.Bcl.HashCode on NuGet — https://www.nuget.org/packages/Microsoft.Bcl.HashCode
18. Nullable on NuGet — https://www.nuget.org/packages/Nullable
19. Nullable on GitHub (manuelroemer/Nullable) — https://github.com/manuelroemer/Nullable
20. StyleCop.Analyzers on NuGet — https://www.nuget.org/packages/StyleCop.Analyzers
21. StyleCop.Analyzers on GitHub — https://github.com/DotNetAnalyzers/StyleCopAnalyzers
22. .slnx GA announcement — https://devblogs.microsoft.com/visualstudio/solution-file-known-as-slnx-is-now-generally-available/
23. dotnet sln docs (Microsoft Learn) — https://learn.microsoft.com/dotnet/core/tools/dotnet-sln

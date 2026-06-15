---
date: 2026-04-11 20:09:53 CDT
ticket: "Update NuGet packages, replace FluentAssertions (commercial) with Shouldly, align with latest xUnit/Shouldly recommendations as of April 2026"
ticket_source: "inline"
status: ready-for-research
---

# Research Questions: NuGet Updates & FluentAssertions → Shouldly Migration

## Context (for the human, not sub-agents)

The `ErrorOr` library is being updated to the latest NuGet package versions. FluentAssertions has gone commercial (v8+) and is being removed from the Tests project in favor of Shouldly. Git status shows `src/ErrorOr.csproj`, `tests/Tests.csproj`, and `tests/ErrorOr/ErrorOr.AggregationTests.cs` already modified. We need to confirm current state, ensure the migration is complete and idiomatic, and verify alignment with latest xUnit + Shouldly guidance for April 2026.

## Questions

1. **Current package inventory** — What package references and exact versions are declared in `src/ErrorOr.csproj` and `tests/Tests.csproj` right now (including any Directory.Packages.props / CPM setup), and what are the TargetFrameworks?

2. **Migration completeness** — Are there any remaining `using FluentAssertions` directives, `.Should()` calls, or `FluentAssertions` package references anywhere in the tests project? Trace every test file to confirm the migration is fully done.

3. **Existing Shouldly usage patterns** — In the files already migrated (notably `tests/ErrorOr/ErrorOr.AggregationTests.cs`), what Shouldly assertion idioms are being used (`ShouldBe`, `ShouldSatisfyAllConditions`, `ShouldBeOfType`, collection assertions, etc.)? Are they consistent across files?

4. **xUnit version & conventions** — What xUnit version is currently referenced, what test discovery/runner packages are pulled in (xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, coverlet), and are tests using `[Fact]`/`[Theory]` patterns consistently? Is this xUnit v2 or v3?

5. **Test project structure** — How is the test project organized (file layout under `tests/ErrorOr/`, naming conventions, base classes or test helpers/fixtures), and are there any shared assertion helpers that wrapped FluentAssertions and now need Shouldly equivalents?

6. **Build & CI surface** — What does the CI pipeline (GitHub Actions workflows, if any) run for tests, and are there any references to FluentAssertions or pinned SDK versions in workflow files, global.json, or Directory.Build.props that could affect the upgrade?

7. **Latest recommended versions (external research)** — As of April 2026, what are the current stable/recommended versions of: xUnit (v2 vs v3), Shouldly, Microsoft.NET.Test.Sdk, coverlet.collector, and what migration caveats exist (e.g., xUnit v3 breaking changes, Shouldly API changes)?

8. **Shouldly idiom guidance (external research)** — What are the currently recommended Shouldly patterns for the assertion shapes most common in this repo: equality, type checks, exception assertions (`Should.Throw`), collection membership/ordering, and multi-assertion grouping? Any anti-patterns to avoid?

## Notes

- Git status shows `ErrorOr.sln` deleted and `ErrorOr.slnx` added — worth confirming the new slnx format is compatible with the updated SDK/test tooling.
- User explicitly called out "latest recommendations as of April 2026" — Q7 and Q8 should lean on external/up-to-date sources (context7, Microsoft Learn, package release notes), not just local conventions.
- Not in scope: redesigning tests, adding coverage, or touching `src/` behavior. This is a dependency + assertion-library swap.

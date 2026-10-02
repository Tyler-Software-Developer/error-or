# Benchmarks

BenchmarkDotNet suite for `TylerSoftware.ErrorOr`. Every benchmark runs on .NET 8 and .NET 10, so both runtimes need to be installed.

```shell
# All benchmarks (takes a while)
dotnet run -c Release -f net10.0 --project benchmarks -- --filter '*'

# One class or one method
dotnet run -c Release -f net10.0 --project benchmarks -- --filter '*AggregationBenchmarks*'
dotnet run -c Release -f net10.0 --project benchmarks -- --filter '*CollectionExpression*'

# Quicker, noisier pass while iterating (still both runtimes; `--job short` would add a third job instead)
dotnet run -c Release -f net10.0 --project benchmarks -- --filter '*' --launchCount 1 --warmupCount 3 --iterationCount 3
```

Results are written to `BenchmarkDotNet.Artifacts/` (git-ignored). When a change is meant to improve performance, run the affected benchmarks on `main` and on your branch, and put the before/after table in the PR.

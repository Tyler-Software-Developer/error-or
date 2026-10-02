using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

// Every benchmark runs on both shipped runtimes so .NET 8 and .NET 10 results sit side by side.
var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80).WithId(".NET 8"))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0).WithId(".NET 10"))
    .AddDiagnoser(MemoryDiagnoser.Default)
    .HideColumns(Column.Error, Column.StdDev, Column.Median, Column.RatioSD);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);

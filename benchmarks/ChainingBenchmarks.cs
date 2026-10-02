using BenchmarkDotNet.Attributes;
using TylerSoftware.ErrorOr;
using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Benchmarks;

/// <summary>
/// Fluent chains. Static lambdas keep closure allocations out of the measurement, so these isolate the
/// library's own cost (inlining of Then/FailIf and the async state machines).
/// </summary>
public class ChainingBenchmarks
{
    private static readonly Error ErrorA = Error.Validation("A.Code", "a");

    private readonly ErrorOr<int> _value = 5;
    private readonly ErrorOr<int> _error = ErrorA;
    private readonly Task<ErrorOr<int>> _completedValueTask = Task.FromResult<ErrorOr<int>>(5);

    [Benchmark]
    public ErrorOr<int> ThenChain_Value() =>
        _value.Then(static x => x + 1).Then(static x => x * 2).FailIf(static x => x < 0, ErrorA);

    [Benchmark]
    public ErrorOr<int> ThenChain_Error() =>
        _error.Then(static x => x + 1).Then(static x => x * 2).FailIf(static x => x < 0, ErrorA);

    [Benchmark]
    public Task<ErrorOr<int>> ThenAsync_Error() =>
        _error.ThenAsync(static x => Task.FromResult<ErrorOr<int>>(x + 1));

    [Benchmark]
    public Task<ErrorOr<int>> TaskThen_CompletedTask() => _completedValueTask.Then(static x => x + 1);
}

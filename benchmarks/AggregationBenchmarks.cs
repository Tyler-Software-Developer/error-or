using BenchmarkDotNet.Attributes;
using TylerSoftware.ErrorOr;
using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Benchmarks;

/// <summary>
/// Building and merging error lists. The collection-expression and params-span cases pass a stack
/// buffer to the library; they guard against bulk-copying <see cref="Error"/> structs out of it, which
/// measured 5-10x slower than element-wise copies (see CHANGELOG 5.1.0).
/// </summary>
public class AggregationBenchmarks
{
    private static readonly Error ErrorA = Error.Validation("A.Code", "a");
    private static readonly Error ErrorB = Error.NotFound("B.Code", "b");

    private readonly ErrorOr<int> _value = 5;
    private readonly ErrorOr<int> _error = ErrorA;

    [Benchmark]
    public ErrorOr<int> Combine_AllValues() => ErrorOrExtensions.Combine(_value, _value, _value);

    [Benchmark]
    public ErrorOr<int> Combine_WithError() => ErrorOrExtensions.Combine(_value, _error, _value);

    [Benchmark]
    public ErrorOr<List<int>> CombineAll_AllValues() => ErrorOrExtensions.CombineAll(_value, _value, _value);

    [Benchmark]
    public ErrorOr<List<int>> CombineAll_AllErrors() => ErrorOrExtensions.CombineAll(_error, _error, _error);

    [Benchmark]
    public ErrorOr<int> AppendErrors_ToError() => _error.AppendErrors(ErrorA, ErrorB);

    [Benchmark]
    public ErrorOr<int> CollectionExpression()
    {
        ErrorOr<int> result = [ErrorA, ErrorB];
        return result;
    }
}

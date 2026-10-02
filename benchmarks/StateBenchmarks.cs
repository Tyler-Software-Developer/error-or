using BenchmarkDotNet.Attributes;
using TylerSoftware.ErrorOr;
using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Benchmarks;

/// <summary>
/// Reading state off an existing result: value access, enumeration, equality and hashing.
/// </summary>
public class StateBenchmarks
{
    private static readonly Error ErrorA = Error.Validation("A.Code", "a");

    private readonly ErrorOr<int> _value = 5;
    private readonly ErrorOr<int> _error = ErrorA;
    private readonly ErrorOr<int>[] _values = Enumerable.Range(0, 1000).Select(static i => (ErrorOr<int>)i).ToArray();

    [Benchmark]
    public long SumValues_1000()
    {
        long sum = 0;
        var values = _values;
        for (var i = 0; i < values.Length; i++)
        {
            sum += values[i].Value;
        }

        return sum;
    }

    [Benchmark]
    public int Enumerate_Value()
    {
        var count = 0;
        foreach (var unused in _value)
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int ErrorsOrEmptyList_Value() => _value.ErrorsOrEmptyList.Count;

    [Benchmark]
    public bool Equals_Error() => _error.Equals(_error);

    [Benchmark]
    public int GetHashCode_Error() => _error.GetHashCode();

    [Benchmark]
    public string ToString_Value() => _value.ToString();
}

using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr;

/// <summary>
/// Builder class for creating <see cref="ErrorOr{TValue}"/> instances from collection expressions.
/// </summary>
public static class ErrorOrBuilder
{
    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> from a collection of errors.
    /// This method is used by the collection expression syntax: <c>TylerSoftware.ErrorOr&lt;int&gt; result = [error1, error2];</c>.
    /// </summary>
    /// <typeparam name="TValue">The type of the underlying value.</typeparam>
    /// <param name="errors">The span of errors.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> in error state with the provided errors.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
#if NET8_0_OR_GREATER
    public static ErrorOr<TValue> Create<TValue>(ReadOnlySpan<Error> errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentException("Cannot create an TylerSoftware.ErrorOr<TValue> from an empty collection of errors. Provide at least one error.", nameof(errors));
        }

        return new List<Error>(errors.ToArray());
    }
#else
    public static ErrorOr<TValue> Create<TValue>(Error[] errors)
    {
        if (errors is null || errors.Length == 0)
        {
            throw new ArgumentException("Cannot create an TylerSoftware.ErrorOr<TValue> from an empty collection of errors. Provide at least one error.", nameof(errors));
        }

        return new List<Error>(errors);
    }
#endif
}

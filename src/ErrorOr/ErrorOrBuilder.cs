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

    /// <summary>
    /// Creates an <see cref="IErrorOr{TValue}"/> from a collection of errors.
    /// This method enables collection expression syntax targeting <see cref="IErrorOr{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the underlying value.</typeparam>
    /// <param name="errors">The span of errors.</param>
    /// <returns>An <see cref="IErrorOr{TValue}"/> in error state with the provided errors.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
#if NET8_0_OR_GREATER
    public static IErrorOr<TValue> CreateIErrorOrValue<TValue>(ReadOnlySpan<Error> errors) => Create<TValue>(errors);
#else
    public static IErrorOr<TValue> CreateIErrorOrValue<TValue>(Error[] errors) => Create<TValue>(errors);
#endif

    /// <summary>
    /// Creates an <see cref="IErrorOr"/> from a collection of errors.
    /// This method enables collection expression syntax targeting <see cref="IErrorOr"/>.
    /// </summary>
    /// <param name="errors">The span of errors.</param>
    /// <returns>An <see cref="IErrorOr"/> in error state with the provided errors.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
#if NET8_0_OR_GREATER
    public static IErrorOr CreateIErrorOr(ReadOnlySpan<Error> errors) => Create<object>(errors);
#else
    public static IErrorOr CreateIErrorOr(Error[] errors) => Create<object>(errors);
#endif
}

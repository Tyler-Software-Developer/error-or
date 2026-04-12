using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr;

public static partial class ErrorOrExtensions
{
    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance with the given <paramref name="value"/>.
    /// </summary>
    public static ErrorOr<TValue> ToErrorOr<TValue>(this TValue value)
    {
        return value;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance with the given <paramref name="error"/>.
    /// </summary>
    public static ErrorOr<TValue> ToErrorOr<TValue>(this Error error)
    {
        return error;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance with the given <paramref name="errors"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors" /> is an empty list.</exception>
    public static ErrorOr<TValue> ToErrorOr<TValue>(this List<Error> errors)
    {
        return errors;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance with the given <paramref name="errors"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors" /> is an empty array.</exception>
    public static ErrorOr<TValue> ToErrorOr<TValue>(this Error[] errors)
    {
        return errors;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance from an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">The task containing the value.</param>
    /// <returns>A task containing an <see cref="ErrorOr{TValue}"/> with the awaited value.</returns>
    public static async Task<ErrorOr<TValue>> ToErrorOrAsync<TValue>(this Task<TValue> task)
    {
        var value = await task.ConfigureAwait(false);
        return value;
    }
}

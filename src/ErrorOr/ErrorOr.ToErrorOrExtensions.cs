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
    /// Creates an <see cref="ErrorOr{TValue}"/> instance with the given <paramref name="errors"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors" /> is an empty sequence.</exception>
    public static ErrorOr<TValue> ToErrorOr<TValue>(this IEnumerable<Error> errors)
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        return errors.ToList();
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

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance from an asynchronous operation that yields an error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">The task containing the error.</param>
    /// <returns>A task containing an <see cref="ErrorOr{TValue}"/> with the awaited error.</returns>
    public static async Task<ErrorOr<TValue>> ToErrorOrAsync<TValue>(this Task<Error> task)
    {
        var error = await task.ConfigureAwait(false);
        return error;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance from an asynchronous operation that yields a list of errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">The task containing the list of errors.</param>
    /// <returns>A task containing an <see cref="ErrorOr{TValue}"/> with the awaited errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the result is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the result is empty.</exception>
    public static async Task<ErrorOr<TValue>> ToErrorOrAsync<TValue>(this Task<List<Error>> task)
    {
        var errors = await task.ConfigureAwait(false);
        return errors;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance from an asynchronous operation that yields an array of errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">The task containing the array of errors.</param>
    /// <returns>A task containing an <see cref="ErrorOr{TValue}"/> with the awaited errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the result is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the result is empty.</exception>
    public static async Task<ErrorOr<TValue>> ToErrorOrAsync<TValue>(this Task<Error[]> task)
    {
        var errors = await task.ConfigureAwait(false);
        return errors;
    }

    /// <summary>
    /// Creates an <see cref="ErrorOr{TValue}"/> instance from an asynchronous operation that yields a sequence of errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="task">The task containing the sequence of errors.</param>
    /// <returns>A task containing an <see cref="ErrorOr{TValue}"/> with the awaited errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the result is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the result is empty.</exception>
    public static async Task<ErrorOr<TValue>> ToErrorOrAsync<TValue>(this Task<IEnumerable<Error>> task)
    {
        var errors = await task.ConfigureAwait(false);
        return errors.ToErrorOr<TValue>();
    }
}

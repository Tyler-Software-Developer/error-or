using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr;

/// <summary>
/// Provides factory methods for creating instances of <see cref="ErrorOr{TValue}"/>.
/// </summary>
public static class ErrorOrFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>An instance of <see cref="ErrorOr{TValue}"/> containing the provided value.</returns>
    public static ErrorOr<TValue> From<TValue>(TValue value)
    {
        return value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a single error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The error to wrap.</param>
    /// <returns>An instance of <see cref="ErrorOr{TValue}"/> containing the provided error.</returns>
    public static ErrorOr<TValue> From<TValue>(Error error)
    {
        return error;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a list of errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errors">The list of errors to wrap.</param>
    /// <returns>An instance of <see cref="ErrorOr{TValue}"/> containing the provided errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static ErrorOr<TValue> From<TValue>(List<Error> errors)
    {
        return errors;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with an array of errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errors">The array of errors to wrap.</param>
    /// <returns>An instance of <see cref="ErrorOr{TValue}"/> containing the provided errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static ErrorOr<TValue> From<TValue>(Error[] errors)
    {
        return errors;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a sequence of errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errors">The sequence of errors to wrap.</param>
    /// <returns>An instance of <see cref="ErrorOr{TValue}"/> containing the provided errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static ErrorOr<TValue> From<TValue>(IEnumerable<Error> errors)
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        return errors.ToList();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a value from an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="valueTask">The task containing the value to wrap.</param>
    /// <returns>A task containing an instance of <see cref="ErrorOr{TValue}"/> with the provided value.</returns>
    public static async Task<ErrorOr<TValue>> FromAsync<TValue>(Task<TValue> valueTask)
    {
        var value = await valueTask.ConfigureAwait(false);
        return value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a single error from an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errorTask">The task containing the error to wrap.</param>
    /// <returns>A task containing an instance of <see cref="ErrorOr{TValue}"/> with the provided error.</returns>
    public static async Task<ErrorOr<TValue>> FromAsync<TValue>(Task<Error> errorTask)
    {
        var error = await errorTask.ConfigureAwait(false);
        return error;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a list of errors from an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errorsTask">The task containing the list of errors to wrap.</param>
    /// <returns>A task containing an instance of <see cref="ErrorOr{TValue}"/> with the provided errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the result is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the result is empty.</exception>
    public static async Task<ErrorOr<TValue>> FromAsync<TValue>(Task<List<Error>> errorsTask)
    {
        var errors = await errorsTask.ConfigureAwait(false);
        return errors;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with an array of errors from an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errorsTask">The task containing the array of errors to wrap.</param>
    /// <returns>A task containing an instance of <see cref="ErrorOr{TValue}"/> with the provided errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the result is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the result is empty.</exception>
    public static async Task<ErrorOr<TValue>> FromAsync<TValue>(Task<Error[]> errorsTask)
    {
        var errors = await errorsTask.ConfigureAwait(false);
        return errors;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorOr{TValue}"/> with a sequence of errors from an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="errorsTask">The task containing the sequence of errors to wrap.</param>
    /// <returns>A task containing an instance of <see cref="ErrorOr{TValue}"/> with the provided errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the result is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the result is empty.</exception>
    public static async Task<ErrorOr<TValue>> FromAsync<TValue>(Task<IEnumerable<Error>> errorsTask)
    {
        var errors = await errorsTask.ConfigureAwait(false);
        return From<TValue>(errors);
    }
}

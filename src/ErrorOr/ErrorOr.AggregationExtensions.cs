using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr;

public static partial class ErrorOrExtensions
{
    /// <summary>
    /// Appends additional errors to the current <see cref="ErrorOr{TValue}"/> instance.
    /// If the state is value, converts it to an error state with the provided errors.
    /// If the state is already error, appends the new errors to the existing ones.
    /// </summary>
    /// <typeparam name="TValue">The type of the underlying value.</typeparam>
    /// <param name="errorOr">The <see cref="ErrorOr{TValue}"/> instance.</param>
    /// <param name="errors">The errors to append.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> in error state with all errors combined.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static ErrorOr<TValue> AppendErrors<TValue>(this ErrorOr<TValue> errorOr, params Error[] errors)
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        if (errors.Length == 0)
        {
            throw new ArgumentException("Cannot append an empty array of errors.", nameof(errors));
        }

        if (!errorOr.IsError)
        {
            return new List<Error>(errors);
        }

        var allErrors = new List<Error>(errorOr.Errors.Count + errors.Length);
        allErrors.AddRange(errorOr.Errors);
        allErrors.AddRange(errors);

        return allErrors;
    }

    /// <summary>
    /// Appends additional errors to the current <see cref="ErrorOr{TValue}"/> instance.
    /// If the state is value, converts it to an error state with the provided errors.
    /// If the state is already error, appends the new errors to the existing ones.
    /// </summary>
    /// <typeparam name="TValue">The type of the underlying value.</typeparam>
    /// <param name="errorOr">The <see cref="ErrorOr{TValue}"/> instance.</param>
    /// <param name="errors">The errors to append.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> in error state with all errors combined.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errors"/> is empty.</exception>
    public static ErrorOr<TValue> AppendErrors<TValue>(this ErrorOr<TValue> errorOr, List<Error> errors)
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        if (errors.Count == 0)
        {
            throw new ArgumentException("Cannot append an empty list of errors.", nameof(errors));
        }

        if (!errorOr.IsError)
        {
            return errors;
        }

        var allErrors = new List<Error>(errorOr.Errors.Count + errors.Count);
        allErrors.AddRange(errorOr.Errors);
        allErrors.AddRange(errors);

        return allErrors;
    }

    /// <summary>
    /// Combines multiple <see cref="ErrorOr{TValue}"/> instances into a single result.
    /// If any instance has errors, returns all errors combined.
    /// If all instances have values, returns the first value.
    /// </summary>
    /// <typeparam name="TValue">The type of the underlying value.</typeparam>
    /// <param name="errorOrs">The <see cref="ErrorOr{TValue}"/> instances to combine.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> with all errors combined or the first value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorOrs"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorOrs"/> is empty.</exception>
    public static ErrorOr<TValue> Combine<TValue>(params ErrorOr<TValue>[] errorOrs)
    {
        if (errorOrs is null)
        {
            throw new ArgumentNullException(nameof(errorOrs));
        }

        if (errorOrs.Length == 0)
        {
            throw new ArgumentException("Cannot combine an empty array of ErrorOr instances.", nameof(errorOrs));
        }

        var allErrors = new List<Error>();

        foreach (var errorOr in errorOrs)
        {
            if (errorOr.IsError)
            {
                allErrors.AddRange(errorOr.Errors);
            }
        }

        if (allErrors.Count > 0)
        {
            return allErrors;
        }

        return errorOrs[0].Value;
    }

    /// <summary>
    /// Combines multiple <see cref="ErrorOr{TValue}"/> instances into a single result.
    /// If any instance has errors, returns all errors combined.
    /// If all instances have values, returns all values as a list.
    /// </summary>
    /// <typeparam name="TValue">The type of the underlying value.</typeparam>
    /// <param name="errorOrs">The <see cref="ErrorOr{TValue}"/> instances to combine.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> with all errors combined or all values as a list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorOrs"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorOrs"/> is empty.</exception>
    public static ErrorOr<List<TValue>> CombineAll<TValue>(params ErrorOr<TValue>[] errorOrs)
    {
        if (errorOrs is null)
        {
            throw new ArgumentNullException(nameof(errorOrs));
        }

        if (errorOrs.Length == 0)
        {
            throw new ArgumentException("Cannot combine an empty array of ErrorOr instances.", nameof(errorOrs));
        }

        var allErrors = new List<Error>();
        var allValues = new List<TValue>();

        foreach (var errorOr in errorOrs)
        {
            if (errorOr.IsError)
            {
                allErrors.AddRange(errorOr.Errors);
            }
            else
            {
                allValues.Add(errorOr.Value);
            }
        }

        if (allErrors.Count > 0)
        {
            return allErrors;
        }

        return allValues;
    }
}

using System.Runtime.CompilerServices;
using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr;

/// <summary>
/// Interface for the <see cref="ErrorOr{TValue}"/> object.
/// </summary>
/// <typeparam name="TValue">The type of the underlying value.</typeparam>
[CollectionBuilder(typeof(ErrorOrBuilder), nameof(ErrorOrBuilder.CreateIErrorOrValue))]
public interface IErrorOr<out TValue> : IErrorOr
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    TValue Value { get; }
}

/// <summary>
/// Type-less interface for the <see cref="ErrorOr"/> object.
/// </summary>
/// <remarks>
/// This interface is intended for use when the underlying type of the <see cref="ErrorOr"/> object is unknown.
/// </remarks>
[CollectionBuilder(typeof(ErrorOrBuilder), nameof(ErrorOrBuilder.CreateIErrorOr))]
public interface IErrorOr : IRecordable
{
    /// <summary>
    /// Gets the list of errors.
    /// </summary>
    List<Error> Errors { get; }

    /// <summary>
    /// Gets a value indicating whether the state is error.
    /// </summary>
    bool IsError { get; }

    /// <summary>
    /// Gets a value indicating whether the state is a value (i.e. not an error).
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the value as an object. Useful for logging and serialization when the type is unknown.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no value is present.</exception>
    object ValueObject { get; }

    /// <summary>
    /// Returns an enumerator that iterates through the errors.
    /// </summary>
    /// <returns>An enumerator of <see cref="Error"/>.</returns>
    /// <remarks>This method primarily exists to support collection expressions.</remarks>
    IEnumerator<Error> GetEnumerator();
}

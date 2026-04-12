using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr;

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
public interface IErrorOr
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
    /// Gets the value as an object. Useful for logging and serialization when the type is unknown.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no value is present.</exception>
    object ValueObject { get; }
}

// AOT Compatibility Test Application
// This application exercises the ErrorOr library APIs to verify AOT compilation compatibility.
// It's designed to be published with Native AOT and will fail to compile if any AOT-incompatible
// code patterns are detected.

using TylerSoftware.ErrorOr;
using TylerSoftware.ErrorOr.Errors;
using TylerSoftware.ErrorOr.Results;

Console.WriteLine("ErrorOr AOT Compatibility Test");
Console.WriteLine("==============================");

// Test 1: Create ErrorOr with value
ErrorOr<int> valueResult = 42;
Console.WriteLine($"Test 1 - Value creation: IsError={valueResult.IsError}, Value={valueResult.Value}");

// Test 2: Create ErrorOr with error
ErrorOr<int> errorResult = Error.Validation("Test.Error", "A test error");
Console.WriteLine($"Test 2 - Error creation: IsError={errorResult.IsError}, Code={errorResult.FirstError.Code}");

// Test 3: Create different error types
var errors = new[]
{
    Error.Failure(),
    Error.Unexpected(),
    Error.Validation(),
    Error.Conflict(),
    Error.NotFound(),
    Error.Unauthorized(),
    Error.Forbidden(),
    Error.Custom(100, "Custom.Error", "Custom error type"),
};

Console.WriteLine($"Test 3 - Error types: Created {errors.Length} different error types");

// Test 4: Match pattern
var matchResult = valueResult.Match(
    onValue: value => $"Got value: {value}",
    onError: errors => $"Got {errors.Count} errors");
Console.WriteLine($"Test 4 - Match: {matchResult}");

// Test 5: Then chaining
ErrorOr<string> chainResult = valueResult.Then(value => value * 2).Then(value => $"Doubled: {value}");
Console.WriteLine($"Test 5 - Then chain: {chainResult.Value}");

// Test 6: Else for error handling
ErrorOr<int> elseResult = errorResult.Else(42);
Console.WriteLine($"Test 6 - Else: {elseResult.Value}");

// Test 7: Multiple errors
ErrorOr<string> multiErrorResult = new List<Error>
{
    Error.Validation("Error1", "First error"),
    Error.Validation("Error2", "Second error"),
};
Console.WriteLine($"Test 7 - Multiple errors: {multiErrorResult.Errors.Count} errors");

// Test 8: ErrorsOrEmptyList
var safeErrors = valueResult.ErrorsOrEmptyList;
Console.WriteLine($"Test 8 - ErrorsOrEmptyList: {safeErrors.Count} errors (safe access)");

// Test 9: Result types (Success, Created, Updated, Deleted)
ErrorOr<Success> successResult = Result.Success;
ErrorOr<Created> createdResult = Result.Created;
ErrorOr<Updated> updatedResult = Result.Updated;
ErrorOr<Deleted> deletedResult = Result.Deleted;
Console.WriteLine($"Test 9 - Result types: Success={!successResult.IsError}, Created={!createdResult.IsError}");

// Test 10: Error with metadata
var metadataError = Error.Validation(
    "Meta.Error",
    "Error with metadata",
    new Dictionary<string, object> { ["key"] = "value" });
Console.WriteLine($"Test 10 - Metadata: {metadataError.Metadata?.Count ?? 0} entries");

// Test 11: IsSuccess property
Console.WriteLine($"Test 11 - IsSuccess: value={valueResult.IsSuccess}, error={errorResult.IsSuccess}");

// Test 12: ThenEnsure
ErrorOr<int> ensuredResult = valueResult.ThenEnsure(value => value > 0 ? value : Error.Validation("Negative", "Value is negative"));
Console.WriteLine($"Test 12 - ThenEnsure: IsError={ensuredResult.IsError}");

// Test 13: Else returning ErrorOr
ErrorOr<int> elseErrorOrResult = errorResult.Else(_ => ErrorOrFactory.From(7));
Console.WriteLine($"Test 13 - Else(ErrorOr): {elseErrorOrResult.Value}");

// Test 14: GetEnumerator (foreach over errors)
var enumeratedErrors = new List<Error>();
foreach (Error error in errorResult)
{
    enumeratedErrors.Add(error);
}

Console.WriteLine($"Test 14 - GetEnumerator: enumerated {enumeratedErrors.Count} errors");

// Test 15: From IEnumerable<Error>
IEnumerable<Error> errorSequence = new[] { Error.Validation("E1", "first") };
ErrorOr<int> fromEnumerable = ErrorOrFactory.From<int>(errorSequence);
Console.WriteLine($"Test 15 - From(IEnumerable<Error>): IsError={fromEnumerable.IsError}");

// Test 16: GetRecording (recording subsystem)
var recording = valueResult.GetRecording(new AotRecordingSerializer());
Console.WriteLine($"Test 16 - GetRecording: {recording}");

Console.WriteLine("==============================");
Console.WriteLine("All AOT compatibility tests passed!");

return 0;

/// <summary>
/// A minimal <see cref="IRecordingSerializer{TOutput}"/> used to verify the recording subsystem under Native AOT.
/// </summary>
internal sealed class AotRecordingSerializer : IRecordingSerializer<string>
{
    /// <inheritdoc/>
    public string SerializeValue<TValue>(TValue value) => $"value:{value}";

    /// <inheritdoc/>
    public string SerializeErrors(List<Error> errors) => $"errors:{errors.Count}";
}

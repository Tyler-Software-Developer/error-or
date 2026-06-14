using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class RecordableTests
{
    private sealed class StringRecordingSerializer : IRecordingSerializer<string>
    {
        public string SerializeValue<TValue>(TValue value) => $"value:{value}";

        public string SerializeErrors(List<Error> errors) => $"errors:{string.Join(",", errors.Select(e => e.Code))}";
    }

    [Fact]
    public void GetRecording_WhenStateIsValue_ShouldSerializeValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 42;

        // Act
        var recording = errorOrInt.GetRecording(new StringRecordingSerializer());

        // Assert
        recording.ShouldBe("value:42");
    }

    [Fact]
    public void GetRecording_WhenStateIsError_ShouldSerializeErrors()
    {
        // Arrange
        ErrorOr<int> errorOrInt = new List<Error>
        {
            Error.Validation("Error.1", "First"),
            Error.Validation("Error.2", "Second"),
        };

        // Act
        var recording = errorOrInt.GetRecording(new StringRecordingSerializer());

        // Assert
        recording.ShouldBe("errors:Error.1,Error.2");
    }

    [Fact]
    public void GetRecording_ThroughIRecordableInterface_ShouldSerialize()
    {
        // Arrange
        IRecordable recordable = ErrorOrFactory.From(42);

        // Act
        var recording = recordable.GetRecording(new StringRecordingSerializer());

        // Assert
        recording.ShouldBe("value:42");
    }

    [Fact]
    public void GetRecording_WhenSerializerIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 42;

        // Assert
        var ex = Should.Throw<ArgumentNullException>(() => errorOrInt.GetRecording<string>(null!));
        ex.ParamName.ShouldBe("serializer");
    }
}

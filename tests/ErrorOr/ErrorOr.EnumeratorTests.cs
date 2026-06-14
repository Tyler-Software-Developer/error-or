using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class EnumeratorTests
{
    [Fact]
    public void GetEnumerator_WhenStateIsError_ShouldEnumerateErrors()
    {
        // Arrange
        List<Error> errors = [Error.Validation("Error.1", "First"), Error.Validation("Error.2", "Second")];
        ErrorOr<int> result = errors;

        // Act
        List<Error> enumerated = [];
        foreach (var error in result)
        {
            enumerated.Add(error);
        }

        // Assert
        enumerated.Count.ShouldBe(2);
        enumerated.ShouldBe(errors);
    }

    [Fact]
    public void GetEnumerator_WhenStateIsValue_ShouldEnumerateEmpty()
    {
        // Arrange
        ErrorOr<int> result = 5;

        // Act
        List<Error> enumerated = [];
        foreach (var error in result)
        {
            enumerated.Add(error);
        }

        // Assert
        enumerated.ShouldBeEmpty();
    }
}

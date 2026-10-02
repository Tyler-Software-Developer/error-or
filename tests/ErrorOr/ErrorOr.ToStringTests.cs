using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class ToStringTests
{
    [Fact]
    public void ToString_WhenStateIsValue_ShouldIncludeValue()
    {
        // Arrange
        ErrorOr<int> result = 5;

        // Act
        var text = result.ToString();

        // Assert
        text.ShouldBe("ErrorOr { IsError = False, Value = 5 }");
    }

    [Fact]
    public void ToString_WhenStateIsError_ShouldIncludeEveryError()
    {
        // Arrange
        ErrorOr<int> result = new List<Error> { Error.Validation("A.Code", "a"), Error.NotFound("B.Code", "b") };

        // Act
        var text = result.ToString();

        // Assert
        text.ShouldStartWith("ErrorOr { IsError = True, Errors = [");
        text.ShouldContain("A.Code");
        text.ShouldContain("B.Code");
    }
}

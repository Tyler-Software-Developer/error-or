using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class IsSuccessTests
{
    [Fact]
    public void IsSuccess_WhenStateIsValue_ShouldBeTrue()
    {
        // Arrange
        ErrorOr<int> result = 5;

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsError.ShouldBeFalse();
    }

    [Fact]
    public void IsSuccess_WhenStateIsError_ShouldBeFalse()
    {
        // Arrange
        ErrorOr<int> result = Error.Validation();

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public void IsSuccess_OnInterface_WhenStateIsValue_ShouldBeTrue()
    {
        // Arrange
        IErrorOr result = ErrorOrFactory.From(5);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsError.ShouldBeFalse();
    }

    [Fact]
    public void IsSuccess_OnInterface_WhenStateIsError_ShouldBeFalse()
    {
        // Arrange
        IErrorOr result = ErrorOrFactory.From<int>(Error.NotFound());

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
    }
}

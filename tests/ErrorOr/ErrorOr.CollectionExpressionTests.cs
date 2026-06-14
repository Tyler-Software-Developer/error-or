using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class CollectionExpressionTests
{
    [Fact]
    public void CollectionExpression_TargetingErrorOr_ShouldCreateErrorState()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First");
        var error2 = Error.Validation("Error.2", "Second");

        // Act
        ErrorOr<int> result = [error1, error2];

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void CollectionExpression_TargetingIErrorOrOfValue_ShouldCreateErrorState()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First");
        var error2 = Error.Validation("Error.2", "Second");

        // Act
        IErrorOr<int> result = [error1, error2];

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void CollectionExpression_TargetingIErrorOr_ShouldCreateErrorState()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First");
        var error2 = Error.Validation("Error.2", "Second");

        // Act
        IErrorOr result = [error1, error2];

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public void Builder_CreateIErrorOrValue_ShouldReturnErrorState()
    {
        // Arrange
        var error = Error.NotFound();

        // Act
        IErrorOr<string> result = ErrorOrBuilder.CreateIErrorOrValue<string>(new[] { error });

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(1);
    }

    [Fact]
    public void Builder_CreateIErrorOr_ShouldReturnErrorState()
    {
        // Arrange
        var error = Error.NotFound();

        // Act
        IErrorOr result = ErrorOrBuilder.CreateIErrorOr(new[] { error });

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(1);
    }
}

using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class ElseErrorOrTests
{
    [Fact]
    public void Else_WhenErrorAndOnErrorReturnsValue_ShouldReturnRecoveredValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = Error.NotFound();

        // Act
        ErrorOr<int> result = errorOrInt.Else(_ => ErrorOrFactory.From(99));

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(99);
    }

    [Fact]
    public void Else_WhenErrorAndOnErrorReturnsErrors_ShouldReturnNewErrors()
    {
        // Arrange
        ErrorOr<int> errorOrInt = Error.NotFound();
        var replacement = Error.Unexpected("Replaced", "Replaced error");

        // Act
        ErrorOr<int> result = errorOrInt.Else(_ => ErrorOrFactory.From<int>(replacement));

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(replacement);
    }

    [Fact]
    public void Else_WhenValue_ShouldReturnOriginalValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = errorOrInt.Else(_ => ErrorOrFactory.From(99));

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public async Task ElseAsync_WhenErrorAndOnErrorReturnsValue_ShouldReturnRecoveredValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = Error.NotFound();

        // Act
        ErrorOr<int> result = await errorOrInt.ElseAsync(_ => Task.FromResult(ErrorOrFactory.From(99)));

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(99);
    }

    [Fact]
    public async Task Else_OnTask_WhenError_ShouldReturnRecoveredValue()
    {
        // Arrange
        Task<ErrorOr<int>> errorOrInt = Task.FromResult(ErrorOrFactory.From<int>(Error.NotFound()));

        // Act
        ErrorOr<int> result = await errorOrInt.Else(_ => ErrorOrFactory.From(99));

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(99);
    }

    [Fact]
    public async Task ElseAsync_OnTask_WhenError_ShouldReturnRecoveredValue()
    {
        // Arrange
        Task<ErrorOr<int>> errorOrInt = Task.FromResult(ErrorOrFactory.From<int>(Error.NotFound()));

        // Act
        ErrorOr<int> result = await errorOrInt.ElseAsync(_ => Task.FromResult(ErrorOrFactory.From(99)));

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(99);
    }
}

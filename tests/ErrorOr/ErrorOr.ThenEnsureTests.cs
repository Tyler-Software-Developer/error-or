using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class ThenEnsureTests
{
    [Fact]
    public void ThenEnsure_WhenValueAndEnsurePasses_ShouldReturnOriginalValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = errorOrInt.ThenEnsure(value => value);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void ThenEnsure_WhenValueAndEnsureFails_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;
        var error = Error.Validation("Too.Big", "Value is too big");

        // Act
        ErrorOr<int> result = errorOrInt.ThenEnsure(_ => error);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public void ThenEnsure_WhenError_ShouldShortCircuitAndReturnOriginalErrors()
    {
        // Arrange
        var error = Error.NotFound();
        ErrorOr<int> errorOrInt = error;
        var invoked = false;

        // Act
        ErrorOr<int> result = errorOrInt.ThenEnsure(value =>
        {
            invoked = true;
            return value;
        });

        // Assert
        invoked.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public async Task ThenEnsureAsync_WhenValueAndEnsurePasses_ShouldReturnOriginalValue()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;

        // Act
        ErrorOr<int> result = await errorOrInt.ThenEnsureAsync(value => Task.FromResult(ErrorOrFactory.From(value)));

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public async Task ThenEnsureAsync_WhenValueAndEnsureFails_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<int> errorOrInt = 5;
        var error = Error.Validation("Too.Big", "Value is too big");

        // Act
        ErrorOr<int> result = await errorOrInt.ThenEnsureAsync(_ => Task.FromResult(ErrorOrFactory.From<int>(error)));

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public async Task ThenEnsure_OnTask_WhenValueAndEnsurePasses_ShouldReturnOriginalValue()
    {
        // Arrange
        Task<ErrorOr<int>> errorOrInt = Task.FromResult(ErrorOrFactory.From(5));

        // Act
        ErrorOr<int> result = await errorOrInt.ThenEnsure(value => value);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public async Task ThenEnsureAsync_OnTask_WhenValueAndEnsureFails_ShouldReturnErrors()
    {
        // Arrange
        Task<ErrorOr<int>> errorOrInt = Task.FromResult(ErrorOrFactory.From(5));
        var error = Error.Validation("Too.Big", "Value is too big");

        // Act
        ErrorOr<int> result = await errorOrInt.ThenEnsureAsync(_ => Task.FromResult(ErrorOrFactory.From<int>(error)));

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }
}

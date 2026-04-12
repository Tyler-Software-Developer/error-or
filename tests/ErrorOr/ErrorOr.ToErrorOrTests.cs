using TylerSoftware.ErrorOr;
using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class ToErrorOrTests
{
    [Fact]
    public void ValueToErrorOr_WhenAccessingValue_ShouldReturnValue()
    {
        // Arrange
        const int value = 5;

        // Act
        ErrorOr<int> result = value.ToErrorOr();

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(value);
    }

    [Fact]
    public void ErrorToErrorOr_WhenAccessingFirstError_ShouldReturnSameError()
    {
        // Arrange
        Error error = Error.Unauthorized();

        // Act
        ErrorOr<int> result = error.ToErrorOr<int>();

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public void ListOfErrorsToErrorOr_WhenAccessingErrors_ShouldReturnSameErrors()
    {
        // Arrange
        List<Error> errors = [Error.Unauthorized(), Error.Validation()];

        // Act
        ErrorOr<int> result = errors.ToErrorOr<int>();

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldBe(errors, ignoreOrder: true);
    }

    [Fact]
    public void ArrayOfErrorsToErrorOr_WhenAccessingErrors_ShouldReturnSimilarErrors()
    {
        Error[] errors = [Error.Unauthorized(), Error.Validation()];

        ErrorOr<int> result = errors.ToErrorOr<int>();

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
    }

    [Fact]
    public async Task ToErrorOrAsync_WhenValueTaskProvided_ShouldReturnErrorOrWithValue()
    {
        // Arrange
        var task = Task.FromResult(42);

        // Act
        var result = await task.ToErrorOrAsync();

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task ToErrorOrAsync_WhenAsyncOperation_ShouldAwaitAndReturnValue()
    {
        // Arrange
        static async Task<string> GetValueAsync()
        {
            await Task.Delay(1);
            return "async result";
        }

        // Act
        var result = await GetValueAsync().ToErrorOrAsync();

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("async result");
    }
}

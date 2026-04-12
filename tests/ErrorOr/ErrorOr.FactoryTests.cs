using ErrorOr;

namespace Tests;

public class FactoryTests
{
    [Fact]
    public void FromError_WhenErrorProvided_ShouldReturnErrorOrWithError()
    {
        // Arrange
        var error = Error.NotFound("Error.NotFound", "Resource not found");

        // Act
        var result = ErrorOrFactory.From<int>(error);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public void FromErrors_WhenErrorListProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("Error.1", "First error"),
            Error.Validation("Error.2", "Second error"),
        };

        // Act
        var result = ErrorOrFactory.From<int>(errors);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldBe(errors);
    }

    [Fact]
    public void FromErrors_WhenErrorArrayProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        Error[] errors =
        [
            Error.Validation("Error.1", "First error"),
            Error.Validation("Error.2", "Second error"),
        ];

        // Act
        var result = ErrorOrFactory.From<int>(errors);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldBe(errors);
    }

    [Fact]
    public async Task FromAsync_WhenValueTaskProvided_ShouldReturnErrorOrWithValue()
    {
        // Arrange
        var valueTask = Task.FromResult(42);

        // Act
        var result = await ErrorOrFactory.FromAsync<int>(valueTask);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public async Task FromAsync_WhenAsyncValueOperation_ShouldAwaitAndReturnValue()
    {
        // Arrange
        static async Task<string> GetValueAsync()
        {
            await Task.Delay(1);
            return "async result";
        }

        // Act
        var result = await ErrorOrFactory.FromAsync<string>(GetValueAsync());

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("async result");
    }

    [Fact]
    public async Task FromAsync_WhenErrorTaskProvided_ShouldReturnErrorOrWithError()
    {
        // Arrange
        var error = Error.NotFound("Error.NotFound", "Resource not found");
        var errorTask = Task.FromResult(error);

        // Act
        var result = await ErrorOrFactory.FromAsync<int>(errorTask);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public async Task FromAsync_WhenAsyncErrorOperation_ShouldAwaitAndReturnError()
    {
        // Arrange
        static async Task<Error> GetErrorAsync()
        {
            await Task.Delay(1);
            return Error.Validation("Error.Validation", "Async validation error");
        }

        // Act
        var result = await ErrorOrFactory.FromAsync<string>(GetErrorAsync());

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe("Error.Validation");
    }

    [Fact]
    public async Task FromAsync_WhenErrorListTaskProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("Error.1", "First error"),
            Error.Validation("Error.2", "Second error"),
        };
        var errorsTask = Task.FromResult(errors);

        // Act
        var result = await ErrorOrFactory.FromAsync<int>(errorsTask);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldBe(errors);
    }

    [Fact]
    public async Task FromAsync_WhenAsyncErrorListOperation_ShouldAwaitAndReturnErrors()
    {
        // Arrange
        static async Task<List<Error>> GetErrorsAsync()
        {
            await Task.Delay(1);
            return new List<Error>
            {
                Error.Conflict("Error.Conflict", "Async conflict error"),
                Error.Failure("Error.Failure", "Async failure error"),
            };
        }

        // Act
        var result = await ErrorOrFactory.FromAsync<string>(GetErrorsAsync());

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors[0].Code.ShouldBe("Error.Conflict");
        result.Errors[1].Code.ShouldBe("Error.Failure");
    }
}

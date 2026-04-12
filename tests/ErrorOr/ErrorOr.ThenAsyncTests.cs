using ErrorOr;

namespace Tests;

public class ThenAsyncTests
{
    [Fact]
    public async Task CallingThenAsync_WhenIsSuccess_ShouldInvokeNextThen()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(Convert.ToIntAsync)
            .ThenAsync(num => Task.FromResult(num * 2))
            .ThenDoAsync(num => Task.Run(() => { _ = 5; }))
            .ThenAsync(Convert.ToStringAsync);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("10");
    }

    [Fact]
    public async Task CallingThenAsync_WhenIsError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(Convert.ToIntAsync)
            .ThenAsync(num => Task.FromResult(num * 2))
            .ThenDoAsync(num => Task.Run(() => { _ = 5; }))
            .ThenAsync(Convert.ToStringAsync);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(errorOrString.FirstError);
    }
}

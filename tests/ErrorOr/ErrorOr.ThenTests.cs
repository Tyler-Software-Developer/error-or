using ErrorOr;

namespace Tests;

public class ThenTests
{
    [Fact]
    public void CallingThen_WhenIsSuccess_ShouldInvokeGivenFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(num => num * 2)
            .Then(Convert.ToString);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("10");
    }

    [Fact]
    public void CallingThen_WhenIsSuccess_ShouldInvokeGivenAction()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<int> result = errorOrString
            .ThenDo(str => { _ = 5; })
            .Then(Convert.ToInt)
            .ThenDo(str => { _ = 5; });

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void CallingThen_WhenIsError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(num => num * 2)
            .ThenDo(str => { _ = 5; })
            .Then(Convert.ToString);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(errorOrString.FirstError);
    }

    [Fact]
    public async Task CallingThenAfterThenAsync_WhenIsSuccess_ShouldInvokeGivenFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(Convert.ToIntAsync)
            .Then(num => num * 2)
            .ThenAsync(Convert.ToStringAsync)
            .Then(Convert.ToInt)
            .ThenAsync(Convert.ToStringAsync)
            .ThenDo(num => { _ = 5; });

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("10");
    }

    [Fact]
    public async Task CallingThenAfterThenAsync_WhenIsError_ShouldReturnErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .ThenAsync(Convert.ToIntAsync)
            .Then(Convert.ToString);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(errorOrString.FirstError);
    }
}

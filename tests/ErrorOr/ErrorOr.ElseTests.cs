using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class ElseTests
{
    [Fact]
    public void CallingElseWithValueFunc_WhenIsSuccess_ShouldNotInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(errors => $"Error count: {errors.Count}");

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(errorOrString.Value);
    }

    [Fact]
    public void CallingElseWithValueFunc_WhenIsError_ShouldInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(errors => $"Error count: {errors.Count}");

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("Error count: 1");
    }

    [Fact]
    public void CallingElseWithValue_WhenIsSuccess_ShouldNotReturnElseValue()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else("oh no");

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(errorOrString.Value);
    }

    [Fact]
    public void CallingElseWithValue_WhenIsError_ShouldInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else("oh no");

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("oh no");
    }

    [Fact]
    public void CallingElseWithError_WhenIsError_ShouldReturnElseError()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(Error.Unexpected());

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Unexpected);
    }

    [Fact]
    public void CallingElseWithError_WhenIsSuccess_ShouldNotReturnElseError()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(Error.Unexpected());

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(errorOrString.Value);
    }

    [Fact]
    public void CallingElseWithErrorsFunc_WhenIsError_ShouldReturnElseError()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(errors => Error.Unexpected());

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Unexpected);
    }

    [Fact]
    public void CallingElseWithErrorsFunc_WhenIsSuccess_ShouldNotReturnElseError()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(errors => Error.Unexpected());

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(errorOrString.Value);
    }

    [Fact]
    public void CallingElseWithErrorsFunc_WhenIsError_ShouldReturnElseErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(errors => [Error.Unexpected()]);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Unexpected);
    }

    [Fact]
    public void CallingElseWithErrorsFunc_WhenIsSuccess_ShouldNotReturnElseErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = "5";

        // Act
        ErrorOr<string> result = errorOrString
            .Then(Convert.ToInt)
            .Then(Convert.ToString)
            .Else(errors => [Error.Unexpected()]);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(errorOrString.Value);
    }

    [Fact]
    public async Task CallingElseWithValueAfterThenAsync_WhenIsError_ShouldInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .Then(Convert.ToInt)
            .ThenAsync(Convert.ToStringAsync)
            .Else("oh no");

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("oh no");
    }

    [Fact]
    public async Task CallingElseWithValueFuncAfterThenAsync_WhenIsError_ShouldInvokeElseFunc()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .Then(Convert.ToInt)
            .ThenAsync(Convert.ToStringAsync)
            .Else(errors => $"Error count: {errors.Count}");

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("Error count: 1");
    }

    [Fact]
    public async Task CallingElseWithErrorAfterThenAsync_WhenIsError_ShouldReturnElseError()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .Then(Convert.ToInt)
            .ThenAsync(Convert.ToStringAsync)
            .Else(Error.Unexpected());

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Unexpected);
    }

    [Fact]
    public async Task CallingElseWithErrorFuncAfterThenAsync_WhenIsError_ShouldReturnElseError()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .Then(Convert.ToInt)
            .ThenAsync(Convert.ToStringAsync)
            .Else(errors => Error.Unexpected());

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Unexpected);
    }

    [Fact]
    public async Task CallingElseWithErrorFuncAfterThenAsync_WhenIsError_ShouldReturnElseErrors()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();

        // Act
        ErrorOr<string> result = await errorOrString
            .Then(Convert.ToInt)
            .ThenAsync(Convert.ToStringAsync)
            .Else(errors => [Error.Unexpected()]);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Unexpected);
    }

    [Fact]
    public void CallingElseDo_WhenIsSuccess_ShouldNotInvokeAction()
    {
        // Arrange
        ErrorOr<string> errorOrString = "success";
        var wasInvoked = false;

        // Act
        ErrorOr<string> result = errorOrString.ElseDo(errors => wasInvoked = true);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("success");
        wasInvoked.ShouldBeFalse();
    }

    [Fact]
    public void CallingElseDo_WhenIsError_ShouldInvokeAction()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();
        List<Error>? capturedErrors = null;

        // Act
        ErrorOr<string> result = errorOrString.ElseDo(errors => capturedErrors = errors);

        // Assert
        result.IsError.ShouldBeTrue();
        capturedErrors.ShouldNotBeNull();
        capturedErrors.Count.ShouldBe(1);
        capturedErrors![0].Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public void CallingElseDo_WhenIsError_ShouldReturnOriginalErrorOr()
    {
        // Arrange
        var error = Error.Validation("Test.Error", "Test error");
        ErrorOr<int> errorOrInt = error;
        var actionCallCount = 0;

        // Act
        ErrorOr<int> result = errorOrInt.ElseDo(errors => actionCallCount++);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
        actionCallCount.ShouldBe(1);
    }

    [Fact]
    public async Task CallingElseDoOnTask_WhenIsSuccess_ShouldNotInvokeAction()
    {
        // Arrange
        var wasInvoked = false;

        // Act
        ErrorOr<string> result = await Task.FromResult<ErrorOr<string>>("success")
            .ElseDo(errors => wasInvoked = true);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("success");
        wasInvoked.ShouldBeFalse();
    }

    [Fact]
    public async Task CallingElseDoOnTask_WhenIsError_ShouldInvokeAction()
    {
        // Arrange
        List<Error>? capturedErrors = null;

        // Act
        ErrorOr<string> result = await Task.FromResult<ErrorOr<string>>(Error.NotFound())
            .ElseDo(errors => capturedErrors = errors);

        // Assert
        result.IsError.ShouldBeTrue();
        capturedErrors.ShouldNotBeNull();
        capturedErrors.Count.ShouldBe(1);
    }

    [Fact]
    public async Task CallingElseDoAsync_WhenIsSuccess_ShouldNotInvokeAction()
    {
        // Arrange
        ErrorOr<string> errorOrString = "success";
        var wasInvoked = false;

        // Act
        ErrorOr<string> result = await errorOrString.ElseDoAsync(async errors =>
        {
            await Task.Delay(1);
            wasInvoked = true;
        });

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("success");
        wasInvoked.ShouldBeFalse();
    }

    [Fact]
    public async Task CallingElseDoAsync_WhenIsError_ShouldInvokeAction()
    {
        // Arrange
        ErrorOr<string> errorOrString = Error.NotFound();
        List<Error>? capturedErrors = null;

        // Act
        ErrorOr<string> result = await errorOrString.ElseDoAsync(async errors =>
        {
            await Task.Delay(1);
            capturedErrors = errors;
        });

        // Assert
        result.IsError.ShouldBeTrue();
        capturedErrors.ShouldNotBeNull();
        capturedErrors.Count.ShouldBe(1);
        capturedErrors![0].Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task CallingElseDoAsync_WhenIsError_ShouldReturnOriginalErrorOr()
    {
        // Arrange
        var error = Error.Validation("Test.Error", "Test error");
        ErrorOr<int> errorOrInt = error;
        var actionCallCount = 0;

        // Act
        ErrorOr<int> result = await errorOrInt.ElseDoAsync(async errors =>
        {
            await Task.Delay(1);
            actionCallCount++;
        });

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
        actionCallCount.ShouldBe(1);
    }

    [Fact]
    public async Task CallingElseDoAsyncOnTask_WhenIsSuccess_ShouldNotInvokeAction()
    {
        // Arrange
        var wasInvoked = false;

        // Act
        ErrorOr<string> result = await Task.FromResult<ErrorOr<string>>("success")
            .ElseDoAsync(async errors =>
            {
                await Task.Delay(1);
                wasInvoked = true;
            });

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("success");
        wasInvoked.ShouldBeFalse();
    }

    [Fact]
    public async Task CallingElseDoAsyncOnTask_WhenIsError_ShouldInvokeAction()
    {
        // Arrange
        List<Error>? capturedErrors = null;

        // Act
        ErrorOr<string> result = await Task.FromResult<ErrorOr<string>>(Error.NotFound())
            .ElseDoAsync(async errors =>
            {
                await Task.Delay(1);
                capturedErrors = errors;
            });

        // Assert
        result.IsError.ShouldBeTrue();
        capturedErrors.ShouldNotBeNull();
        capturedErrors.Count.ShouldBe(1);
    }
}

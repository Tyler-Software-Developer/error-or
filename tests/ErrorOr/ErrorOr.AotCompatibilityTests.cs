using TylerSoftware.ErrorOr.Errors;
using TylerSoftware.ErrorOr.Results;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

/// <summary>
/// Tests that verify AOT (Ahead-of-Time) compilation compatibility.
/// These tests exercise all public APIs without relying on reflection-based features.
/// </summary>
public class ErrorOrAotCompatibilityTests
{
    [Fact]
    public void ErrorOr_ShouldWorkWithValueTypes_WithoutReflection()
    {
        // Arrange & Act
        ErrorOr<int> errorOrInt = 42;
        ErrorOr<double> errorOrDouble = 3.14;
        ErrorOr<bool> errorOrBool = true;
        ErrorOr<Guid> errorOrGuid = Guid.NewGuid();
        ErrorOr<DateTime> errorOrDateTime = DateTime.UtcNow;

        // Assert
        errorOrInt.IsError.ShouldBeFalse();
        errorOrInt.Value.ShouldBe(42);

        errorOrDouble.IsError.ShouldBeFalse();
        errorOrDouble.Value.ShouldBe(3.14);

        errorOrBool.IsError.ShouldBeFalse();
        errorOrBool.Value.ShouldBeTrue();

        errorOrGuid.IsError.ShouldBeFalse();
        errorOrGuid.Value.ShouldNotBe(Guid.Empty);

        errorOrDateTime.IsError.ShouldBeFalse();
    }

    [Fact]
    public void ErrorOr_ShouldWorkWithReferenceTypes_WithoutReflection()
    {
        // Arrange & Act
        ErrorOr<string> errorOrString = "test";
        ErrorOr<List<int>> errorOrList = new List<int> { 1, 2, 3 };
        ErrorOr<object> errorOrObject = new object();

        // Assert
        errorOrString.IsError.ShouldBeFalse();
        errorOrString.Value.ShouldBe("test");

        errorOrList.IsError.ShouldBeFalse();
        errorOrList.Value.Count.ShouldBe(3);

        errorOrObject.IsError.ShouldBeFalse();
        errorOrObject.Value.ShouldNotBeNull();
    }

    [Fact]
    public void AllErrorFactoryMethods_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        var failure = Error.Failure("code", "desc");
        var unexpected = Error.Unexpected("code", "desc");
        var validation = Error.Validation("code", "desc");
        var conflict = Error.Conflict("code", "desc");
        var notFound = Error.NotFound("code", "desc");
        var unauthorized = Error.Unauthorized("code", "desc");
        var forbidden = Error.Forbidden("code", "desc");
        var custom = Error.Custom(100, "code", "desc");

        // Assert
        failure.Type.ShouldBe(ErrorType.Failure);
        unexpected.Type.ShouldBe(ErrorType.Unexpected);
        validation.Type.ShouldBe(ErrorType.Validation);
        conflict.Type.ShouldBe(ErrorType.Conflict);
        notFound.Type.ShouldBe(ErrorType.NotFound);
        unauthorized.Type.ShouldBe(ErrorType.Unauthorized);
        forbidden.Type.ShouldBe(ErrorType.Forbidden);
        custom.NumericType.ShouldBe(100);
    }

    [Fact]
    public void ErrorWithMetadata_ShouldWork_WithoutReflection()
    {
        // Arrange
        var metadata = new Dictionary<string, object>
        {
            ["key1"] = "value1",
            ["key2"] = 42,
            ["key3"] = true,
        };

        // Act
        var error = Error.Validation("code", "desc", metadata);

        // Assert
        error.Metadata.ShouldNotBeNull();
        error.Metadata.Count.ShouldBe(3);
        error.Metadata!["key1"].ShouldBe("value1");
        error.Metadata!["key2"].ShouldBe(42);
        error.Metadata!["key3"].ShouldBe(true);
    }

    [Fact]
    public void Match_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> valueResult = 42;
        ErrorOr<int> errorResult = Error.Validation();

        // Act
        var valueMatchResult = valueResult.Match(
            onValue: v => $"Value: {v}",
            onError: e => $"Errors: {e.Count}");

        var errorMatchResult = errorResult.Match(
            onValue: v => $"Value: {v}",
            onError: e => $"Errors: {e.Count}");

        // Assert
        valueMatchResult.ShouldBe("Value: 42");
        errorMatchResult.ShouldBe("Errors: 1");
    }

    [Fact]
    public void Then_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> result = 10;

        // Act
        var chainedResult = result
            .Then(v => v * 2)
            .Then(v => v + 5)
            .Then(v => $"Result: {v}");

        // Assert
        chainedResult.IsError.ShouldBeFalse();
        chainedResult.Value.ShouldBe("Result: 25");
    }

    [Fact]
    public void Else_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> errorResult = Error.Validation();

        // Act
        var elseValue = errorResult.Else(42);
        var elseFunc = errorResult.Else(errors => errors.Count * 10);

        // Assert
        elseValue.Value.ShouldBe(42);
        elseFunc.Value.ShouldBe(10);
    }

    [Fact]
    public void FailIf_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> result = 10;

        // Act
        var failedResult = result.FailIf(v => v > 5, Error.Validation("TooLarge", "Value is too large"));
        var passedResult = result.FailIf(v => v > 100, Error.Validation("TooLarge", "Value is too large"));

        // Assert
        failedResult.IsError.ShouldBeTrue();
        failedResult.FirstError.Code.ShouldBe("TooLarge");

        passedResult.IsError.ShouldBeFalse();
        passedResult.Value.ShouldBe(10);
    }

    [Fact]
    public void Switch_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> valueResult = 42;
        ErrorOr<int> errorResult = Error.Validation();
        var switchedValue = 0;
        var switchedErrorCount = 0;

        // Act
        valueResult.Switch(
            onValue: v => switchedValue = v,
            onError: e => switchedErrorCount = e.Count);

        errorResult.Switch(
            onValue: v => switchedValue = v,
            onError: e => switchedErrorCount = e.Count);

        // Assert
        switchedValue.ShouldBe(42);
        switchedErrorCount.ShouldBe(1);
    }

    [Fact]
    public void ResultTypes_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        ErrorOr<Success> success = Result.Success;
        ErrorOr<Created> created = Result.Created;
        ErrorOr<Updated> updated = Result.Updated;
        ErrorOr<Deleted> deleted = Result.Deleted;

        // Assert
        success.IsError.ShouldBeFalse();
        success.Value.ShouldBe(Result.Success);

        created.IsError.ShouldBeFalse();
        created.Value.ShouldBe(Result.Created);

        updated.IsError.ShouldBeFalse();
        updated.Value.ShouldBe(Result.Updated);

        deleted.IsError.ShouldBeFalse();
        deleted.Value.ShouldBe(Result.Deleted);
    }

    [Fact]
    public void ErrorOrFactory_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        var result = ErrorOrFactory.From("test value");

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("test value");
    }

    [Fact]
    public void MultipleErrors_ShouldWork_WithoutReflection()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("Error1", "First error"),
            Error.Validation("Error2", "Second error"),
            Error.Validation("Error3", "Third error"),
        };

        // Act
        ErrorOr<int> result = errors;

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(3);
        result.FirstError.Code.ShouldBe("Error1");
        result.ErrorsOrEmptyList.Count.ShouldBe(3);
    }

    [Fact]
    public void ErrorEquality_ShouldWork_WithoutReflection()
    {
        // Arrange
        var error1 = Error.Validation("code", "desc");
        var error2 = Error.Validation("code", "desc");
        var error3 = Error.Validation("other", "desc");

        // Assert
        error1.ShouldBe(error2);
        error1.ShouldNotBe(error3);
        error1.GetHashCode().ShouldBe(error2.GetHashCode());
    }

    [Fact]
    public void ToErrorOr_ExtensionMethod_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        var result = "test".ToErrorOr();

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe("test");
    }

    [Fact]
    public void GenericStructs_ShouldWork_WithoutReflection()
    {
        // This test verifies that the library works with user-defined structs
        // which is important for AOT scenarios where the JIT cannot generate code at runtime

        // Arrange
        var point = new Point(10, 20);

        // Act
        ErrorOr<Point> result = point;

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.X.ShouldBe(10);
        result.Value.Y.ShouldBe(20);
    }

    [Fact]
    public void NestedGenericTypes_ShouldWork_WithoutReflection()
    {
        // Arrange
        var nested = new Dictionary<string, List<int>>
        {
            ["key"] = [1, 2, 3],
        };

        // Act
        ErrorOr<Dictionary<string, List<int>>> result = nested;

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value["key"].Count.ShouldBe(3);
    }

    private readonly record struct Point(int X, int Y);
}

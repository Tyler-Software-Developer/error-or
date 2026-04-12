using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class AggregationTests
{
    [Fact]
    public void AppendErrors_WhenErrorOrIsValue_ShouldReturnErrorsOnly()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");

        // Act
        var result = errorOrValue.AppendErrors(error1, error2);

        // Assert
        result.IsError.ShouldBe(true);
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldContain(error1);
        result.Errors.ShouldContain(error2);
    }

    [Fact]
    public void AppendErrors_WhenErrorOrIsError_ShouldCombineErrors()
    {
        // Arrange
        var existingError = Error.NotFound("Existing.Error", "Existing error");
        ErrorOr<int> errorOrErrors = existingError;
        var newError1 = Error.Validation("Error.1", "First error");
        var newError2 = Error.Validation("Error.2", "Second error");

        // Act
        var result = errorOrErrors.AppendErrors(newError1, newError2);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(3);
        result.Errors.ShouldContain(existingError);
        result.Errors.ShouldContain(newError1);
        result.Errors.ShouldContain(newError2);
    }

    [Fact]
    public void AppendErrors_WhenErrorsArrayIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        Error[]? nullErrors = null;

        // Assert
        var ex = Should.Throw<ArgumentNullException>(() => errorOrValue.AppendErrors(nullErrors!));
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void AppendErrors_WhenErrorsArrayIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;

        // Assert
        var ex = Should.Throw<ArgumentException>(() => errorOrValue.AppendErrors(Array.Empty<Error>()));
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void AppendErrorsList_WhenErrorOrIsValue_ShouldReturnErrorsOnly()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        var errors = new List<Error>
        {
            Error.Validation("Error.1", "First error"),
            Error.Validation("Error.2", "Second error"),
        };

        // Act
        var result = errorOrValue.AppendErrors(errors);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldBe(errors, ignoreOrder: true);
    }

    [Fact]
    public void AppendErrorsList_WhenErrorOrIsError_ShouldCombineErrors()
    {
        // Arrange
        var existingError = Error.NotFound("Existing.Error", "Existing error");
        ErrorOr<int> errorOrErrors = existingError;
        var newErrors = new List<Error>
        {
            Error.Validation("Error.1", "First error"),
            Error.Validation("Error.2", "Second error"),
        };

        // Act
        var result = errorOrErrors.AppendErrors(newErrors);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(3);
        result.Errors[0].ShouldBe(existingError);
        result.Errors[1].ShouldBe(newErrors[0]);
        result.Errors[2].ShouldBe(newErrors[1]);
    }

    [Fact]
    public void AppendErrorsList_WhenErrorsListIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        List<Error>? nullErrors = null;

        // Assert
        var ex = Should.Throw<ArgumentNullException>(() => errorOrValue.AppendErrors(nullErrors!));
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void AppendErrorsList_WhenErrorsListIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;

        // Assert
        var ex = Should.Throw<ArgumentException>(() => errorOrValue.AppendErrors(new List<Error>()));
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void Combine_WhenAllAreValues_ShouldReturnFirstValue()
    {
        // Arrange
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = 2;
        ErrorOr<int> errorOr3 = 3;

        // Act
        var result = ErrorOrExtensions.Combine(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(1);
    }

    [Fact]
    public void Combine_WhenSomeAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = error1;
        ErrorOr<int> errorOr3 = error2;

        // Act
        var result = ErrorOrExtensions.Combine(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldContain(error1);
        result.Errors.ShouldContain(error2);
    }

    [Fact]
    public void Combine_WhenAllAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        var error3 = Error.Validation("Error.3", "Third error");
        ErrorOr<int> errorOr1 = error1;
        ErrorOr<int> errorOr2 = error2;
        ErrorOr<int> errorOr3 = error3;

        // Act
        var result = ErrorOrExtensions.Combine(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(3);
        result.Errors.ShouldContain(error1);
        result.Errors.ShouldContain(error2);
        result.Errors.ShouldContain(error3);
    }

    [Fact]
    public void Combine_WhenErrorsArrayIsNull_ShouldThrowArgumentNullException()
    {
        // Assert
        var ex = Should.Throw<ArgumentNullException>(() => ErrorOrExtensions.Combine<int>(null!));
        ex.ParamName.ShouldBe("errorOrs");
    }

    [Fact]
    public void Combine_WhenErrorsArrayIsEmpty_ShouldThrowArgumentException()
    {
        // Assert
        var ex = Should.Throw<ArgumentException>(() => ErrorOrExtensions.Combine(Array.Empty<ErrorOr<int>>()));
        ex.ParamName.ShouldBe("errorOrs");
    }

    [Fact]
    public void CombineAll_WhenAllAreValues_ShouldReturnAllValues()
    {
        // Arrange
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = 2;
        ErrorOr<int> errorOr3 = 3;

        // Act
        var result = ErrorOrExtensions.CombineAll(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CombineAll_WhenSomeAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = error1;
        ErrorOr<int> errorOr3 = error2;

        // Act
        var result = ErrorOrExtensions.CombineAll(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldContain(error1);
        result.Errors.ShouldContain(error2);
    }

    [Fact]
    public void CombineAll_WhenAllAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        var error3 = Error.Validation("Error.3", "Third error");
        ErrorOr<int> errorOr1 = error1;
        ErrorOr<int> errorOr2 = error2;
        ErrorOr<int> errorOr3 = error3;

        // Act
        var result = ErrorOrExtensions.CombineAll(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(3);
        result.Errors.ShouldContain(error1);
        result.Errors.ShouldContain(error2);
        result.Errors.ShouldContain(error3);
    }

    [Fact]
    public void CombineAll_WhenErrorsArrayIsNull_ShouldThrowArgumentNullException()
    {
        // Assert
        var ex = Should.Throw<ArgumentNullException>(() => ErrorOrExtensions.CombineAll<int>(null!));
        ex.ParamName.ShouldBe("errorOrs");
    }

    [Fact]
    public void CombineAll_WhenErrorsArrayIsEmpty_ShouldThrowArgumentException()
    {
        // Assert
        var ex = Should.Throw<ArgumentException>(() => ErrorOrExtensions.CombineAll(Array.Empty<ErrorOr<int>>()));
        ex.ParamName.ShouldBe("errorOrs");
    }
}

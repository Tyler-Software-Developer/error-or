using ErrorOr;

namespace Tests;

public class BuilderTests
{
    [Fact]
    public void Create_WhenErrorsProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");

        // Act
        var result = ErrorOrBuilder.Create<int>(new[] { error1, error2 });

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldContain(error1);
        result.Errors.ShouldContain(error2);
    }

    [Fact]
    public void Create_WhenSingleErrorProvided_ShouldReturnErrorOrWithError()
    {
        // Arrange
        var error = Error.NotFound("Error.NotFound", "Resource not found");

        // Act
        var result = ErrorOrBuilder.Create<string>(new[] { error });

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(1);
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public void Create_WhenEmptyErrorsProvided_ShouldThrowArgumentException()
    {
        // Assert
        var ex = Should.Throw<ArgumentException>(() => ErrorOrBuilder.Create<int>(Array.Empty<Error>()));
        ex.ParamName.ShouldBe("errors");
    }

#if !NET8_0_OR_GREATER
    [Fact]
    public void Create_WhenNullErrorsProvided_ShouldThrowArgumentException()
    {
        // Assert
        var ex = Should.Throw<ArgumentException>(() => ErrorOrBuilder.Create<int>(null!));
        ex.ParamName.ShouldBe("errors");
    }
#endif
}

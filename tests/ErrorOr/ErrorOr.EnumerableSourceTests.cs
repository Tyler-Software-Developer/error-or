using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class EnumerableSourceTests
{
    [Fact]
    public void From_WhenEnumerableOfErrorsProvided_ShouldReturnErrorOrWithErrors()
    {
        // Act
        ErrorOr<int> result = ErrorOrFactory.From<int>(EnumerateErrors());

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldBe(EnumerateErrors());
    }

    [Fact]
    public void From_WhenEnumerableIsNull_ShouldThrowArgumentNullException()
    {
        // Assert
        var ex = Should.Throw<ArgumentNullException>(() => ErrorOrFactory.From<int>((IEnumerable<Error>)null!));
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void From_WhenEnumerableIsEmpty_ShouldThrowArgumentException()
    {
        // Assert
        var ex = Should.Throw<ArgumentException>(() => ErrorOrFactory.From<int>(Enumerable.Empty<Error>()));
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void ToErrorOr_WhenEnumerableOfErrorsProvided_ShouldReturnErrorOrWithErrors()
    {
        // Act
        ErrorOr<int> result = EnumerateErrors().ToErrorOr<int>();

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ToErrorOrAsync_WhenErrorTaskProvided_ShouldReturnErrorOrWithError()
    {
        // Arrange
        var error = Error.NotFound();
        var task = Task.FromResult(error);

        // Act
        ErrorOr<int> result = await task.ToErrorOrAsync<int>();

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(error);
    }

    [Fact]
    public async Task ToErrorOrAsync_WhenErrorListTaskProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        var errors = new List<Error> { Error.Validation("Error.1", "First"), Error.Validation("Error.2", "Second") };
        var task = Task.FromResult(errors);

        // Act
        ErrorOr<int> result = await task.ToErrorOrAsync<int>();

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldBe(errors);
    }

    [Fact]
    public async Task ToErrorOrAsync_WhenErrorArrayTaskProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        Error[] errors = [Error.Validation("Error.1", "First"), Error.Validation("Error.2", "Second")];
        var task = Task.FromResult(errors);

        // Act
        ErrorOr<int> result = await task.ToErrorOrAsync<int>();

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ToErrorOrAsync_WhenEnumerableTaskProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        var task = Task.FromResult(EnumerateErrors());

        // Act
        ErrorOr<int> result = await task.ToErrorOrAsync<int>();

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task FromAsync_WhenErrorArrayTaskProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        Error[] errors = [Error.Validation("Error.1", "First"), Error.Validation("Error.2", "Second")];
        var task = Task.FromResult(errors);

        // Act
        ErrorOr<int> result = await ErrorOrFactory.FromAsync<int>(task);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task FromAsync_WhenEnumerableTaskProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        var task = Task.FromResult(EnumerateErrors());

        // Act
        ErrorOr<int> result = await ErrorOrFactory.FromAsync<int>(task);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(2);
    }

    private static IEnumerable<Error> EnumerateErrors()
    {
        yield return Error.Validation("Error.1", "First");
        yield return Error.Validation("Error.2", "Second");
    }
}

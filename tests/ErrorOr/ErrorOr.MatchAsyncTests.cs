using ErrorOr;

namespace Tests;

public class MatchAsyncTests
{
    private record Person(string Name);

    [Fact]
    public async Task CallingMatchAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task<string> ThenAction(Person person)
        {
            person.ShouldBe(errorOrPerson.Value);
            return Task.FromResult("Nice");
        }

        Task<string> ElsesAction(IReadOnlyList<Error> _) => throw new Exception("Should not be called");

        // Act
        Func<Task<string>> action = async () => await errorOrPerson.MatchAsync(
            ThenAction,
            ElsesAction);

        // Assert
        var result = await action();
        result.ShouldBe("Nice");
    }

    [Fact]
    public async Task CallingMatchAsync_WhenIsError_ShouldExecuteElseAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task<string> ThenAction(Person _) => throw new Exception("Should not be called");

        Task<string> ElsesAction(IReadOnlyList<Error> errors)
        {
            errors.ShouldBe(errorOrPerson.Errors);
            return Task.FromResult("Nice");
        }

        // Act
        Func<Task<string>> action = async () => await errorOrPerson.MatchAsync(
            ThenAction,
            ElsesAction);

        // Assert
        var result = await action();
        result.ShouldBe("Nice");
    }

    [Fact]
    public async Task CallingMatchFirstAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Amichai");
        Task<string> ThenAction(Person person)
        {
            person.ShouldBe(errorOrPerson.Value);
            return Task.FromResult("Nice");
        }

        Task<string> OnFirstErrorAction(Error _) => throw new Exception("Should not be called");

        // Act
        Func<Task<string>> action = async () => await errorOrPerson.MatchFirstAsync(
            ThenAction,
            OnFirstErrorAction);

        // Assert
        var result = await action();
        result.ShouldBe("Nice");
    }

    [Fact]
    public async Task CallingMatchFirstAsync_WhenIsError_ShouldExecuteOnFirstErrorAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task<string> ThenAction(Person _) => throw new Exception("Should not be called");
        Task<string> OnFirstErrorAction(Error errors)
        {
            errors.ShouldBe(errorOrPerson.Errors[0]);
            errors.ShouldBe(errorOrPerson.FirstError);

            return Task.FromResult("Nice");
        }

        // Act
        Func<Task<string>> action = async () => await errorOrPerson.MatchFirstAsync(
            ThenAction,
            OnFirstErrorAction);

        // Assert
        var result = await action();
        result.ShouldBe("Nice");
    }

    [Fact]
    public async Task CallingMatchFirstAsyncAfterThenAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task<string> ThenAction(Person _) => throw new Exception("Should not be called");
        Task<string> OnFirstErrorAction(Error errors)
        {
            errors.ShouldBe(errorOrPerson.Errors[0]);
            errors.ShouldBe(errorOrPerson.FirstError);

            return Task.FromResult("Nice");
        }

        // Act
        Func<Task<string>> action = () => errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .MatchFirstAsync(ThenAction, OnFirstErrorAction);

        // Assert
        var result = await action();
        result.ShouldBe("Nice");
    }

    [Fact]
    public async Task CallingMatchAsyncAfterThenAsync_WhenIsSuccess_ShouldExecuteThenAction()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new List<Error> { Error.Validation(), Error.Conflict() };
        Task<string> ThenAction(Person _) => throw new Exception("Should not be called");

        Task<string> ElsesAction(IReadOnlyList<Error> errors)
        {
            errors.ShouldBe(errorOrPerson.Errors);
            return Task.FromResult("Nice");
        }

        // Act
        Func<Task<string>> action = () => errorOrPerson
            .ThenAsync(person => Task.FromResult(person))
            .MatchAsync(ThenAction, ElsesAction);

        // Assert
        var result = await action();
        result.ShouldBe("Nice");
    }
}

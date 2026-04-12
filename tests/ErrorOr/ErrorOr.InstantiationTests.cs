using TylerSoftware.ErrorOr.Errors;
using TylerSoftware.ErrorOr.Results;

namespace TylerSoftware.ErrorOr.Tests.ErrorOr;

public class ErrorOrInstantiationTests
{
    private record Person(string Name);

    [Fact]
    public void CreateFromFactory_WhenAccessingValue_ShouldReturnValue()
    {
        // Arrange
        IEnumerable<string> value = ["value"];

        // Act
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Assert
        errorOrPerson.IsError.ShouldBeFalse();
        errorOrPerson.Value.ShouldBeSameAs(value);
    }

    [Fact]
    public void CreateFromFactory_WhenAccessingErrors_ShouldThrow()
    {
        // Arrange
        IEnumerable<string> value = ["value"];
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Assert
        Should.Throw<InvalidOperationException>(() => errorOrPerson.Errors);
    }

    [Fact]
    public void CreateFromFactory_WhenAccessingErrorsOrEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        IEnumerable<string> value = ["value"];
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Act
        List<Error> errors = errorOrPerson.ErrorsOrEmptyList;

        // Assert
        errors.ShouldBeEmpty();
    }

    [Fact]
    public void CreateFromFactory_WhenAccessingFirstError_ShouldThrow()
    {
        // Arrange
        IEnumerable<string> value = ["value"];
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Assert
        Should.Throw<InvalidOperationException>(() => errorOrPerson.FirstError);
    }

    [Fact]
    public void CreateFromValue_WhenAccessingValue_ShouldReturnValue()
    {
        // Arrange
        IEnumerable<string> value = ["value"];

        // Act
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Assert
        errorOrPerson.IsError.ShouldBeFalse();
        errorOrPerson.Value.ShouldBeSameAs(value);
    }

    [Fact]
    public void CreateFromValue_WhenAccessingErrors_ShouldThrow()
    {
        // Arrange
        IEnumerable<string> value = ["value"];
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Assert
        Should.Throw<InvalidOperationException>(() => errorOrPerson.Errors);
    }

    [Fact]
    public void CreateFromValue_WhenAccessingErrorsOrEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        IEnumerable<string> value = ["value"];
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Act
        List<Error> errors = errorOrPerson.ErrorsOrEmptyList;

        // Assert
        errors.ShouldBeEmpty();
    }

    [Fact]
    public void CreateFromValue_WhenAccessingFirstError_ShouldThrow()
    {
        // Arrange
        IEnumerable<string> value = ["value"];
        ErrorOr<IEnumerable<string>> errorOrPerson = ErrorOrFactory.From(value);

        // Assert
        Should.Throw<InvalidOperationException>(() => errorOrPerson.FirstError);
    }

    [Fact]
    public void CreateFromErrorList_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        List<Error> errors = [Error.Validation("User.Name", "Name is too short")];
        ErrorOr<Person> errorOrPerson = ErrorOr<Person>.From(errors);

        // Act & Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.Errors.Count.ShouldBe(1);
        errorOrPerson.Errors[0].ShouldBe(errors.Single());
    }

    [Fact]
    public void CreateFromErrorList_WhenAccessingErrorsOrEmptyList_ShouldReturnErrorList()
    {
        // Arrange
        List<Error> errors = [Error.Validation("User.Name", "Name is too short")];
        ErrorOr<Person> errorOrPerson = ErrorOr<Person>.From(errors);

        // Act & Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.ErrorsOrEmptyList.Count.ShouldBe(1);
        errorOrPerson.ErrorsOrEmptyList[0].ShouldBe(errors.Single());
    }

    [Fact]
    public void CreateFromErrorList_WhenAccessingValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        List<Error> errors = [Error.Validation("User.Name", "Name is too short")];
        ErrorOr<Person> errorOrPerson = ErrorOr<Person>.From(errors);

        // Assert
        var ex = Should.Throw<InvalidOperationException>(() => errorOrPerson.Value);
        ex.Message.ShouldBe("The Value property cannot be accessed when errors have been recorded. Check IsError before accessing Value.");
    }

    [Fact]
    public void ImplicitCastResult_WhenAccessingResult_ShouldReturnValue()
    {
        // Arrange
        Person result = new Person("Amici");

        // Act
        ErrorOr<Person> errorOr = result;

        // Assert
        errorOr.IsError.ShouldBeFalse();
        errorOr.Value.ShouldBe(result);
    }

    [Fact]
    public void ImplicitCastResult_WhenAccessingErrors_ShouldThrow()
    {
        ErrorOr<Person> errorOrPerson = new Person("Amichai");

        // Assert
        Should.Throw<InvalidOperationException>(() => errorOrPerson.Errors);
    }

    [Fact]
    public void ImplicitCastResult_WhenAccessingFirstError_ShouldThrow()
    {
        ErrorOr<Person> errorOrPerson = new Person("Amichai");

        // Assert
        Should.Throw<InvalidOperationException>(() => errorOrPerson.FirstError);
    }

    [Fact]
    public void ImplicitCastPrimitiveResult_WhenAccessingResult_ShouldReturnValue()
    {
        // Arrange
        const int result = 4;

        // Act
        ErrorOr<int> errorOrInt = result;

        // Assert
        errorOrInt.IsError.ShouldBeFalse();
        errorOrInt.Value.ShouldBe(result);
    }

    [Fact]
    public void ImplicitCastErrorOrType_WhenAccessingResult_ShouldReturnValue()
    {
        // Act
        ErrorOr<Success> errorOrSuccess = Result.Success;
        ErrorOr<Created> errorOrCreated = Result.Created;
        ErrorOr<Deleted> errorOrDeleted = Result.Deleted;
        ErrorOr<Updated> errorOrUpdated = Result.Updated;

        // Assert
        errorOrSuccess.IsError.ShouldBeFalse();
        errorOrSuccess.Value.ShouldBe(Result.Success);

        errorOrCreated.IsError.ShouldBeFalse();
        errorOrCreated.Value.ShouldBe(Result.Created);

        errorOrDeleted.IsError.ShouldBeFalse();
        errorOrDeleted.Value.ShouldBe(Result.Deleted);

        errorOrUpdated.IsError.ShouldBeFalse();
        errorOrUpdated.Value.ShouldBe(Result.Updated);
    }

    [Fact]
    public void ImplicitCastSingleError_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        Error error = Error.Validation("User.Name", "Name is too short");

        // Act
        ErrorOr<Person> errorOrPerson = error;

        // Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.Errors.Count.ShouldBe(1);
        errorOrPerson.Errors[0].ShouldBe(error);
    }

    [Fact]
    public void ImplicitCastError_WhenAccessingValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = Error.Validation("User.Name", "Name is too short");

        // Assert
        var ex = Should.Throw<InvalidOperationException>(() => errorOrPerson.Value);
        ex.Message.ShouldBe("The Value property cannot be accessed when errors have been recorded. Check IsError before accessing Value.");
    }

    [Fact]
    public void ImplicitCastSingleError_WhenAccessingFirstError_ShouldReturnError()
    {
        // Arrange
        Error error = Error.Validation("User.Name", "Name is too short");

        // Act
        ErrorOr<Person> errorOrPerson = error;

        // Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.FirstError.ShouldBe(error);
    }

    [Fact]
    public void ImplicitCastErrorList_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        List<Error> errors =
        [
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young")
        ];

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.Errors.Count.ShouldBe(errors.Count);
        errorOrPerson.Errors.ShouldBe(errors, ignoreOrder: true);
    }

    [Fact]
    public void ImplicitCastErrorArray_WhenAccessingErrors_ShouldReturnErrorArray()
    {
        // Arrange
        Error[] errors =
        [
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        ];

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.Errors.Count.ShouldBe(errors.Length);
        errorOrPerson.Errors.ShouldBe(errors, ignoreOrder: true);
    }

    [Fact]
    public void ImplicitCastErrorList_WhenAccessingFirstError_ShouldReturnFirstError()
    {
        // Arrange
        List<Error> errors =
        [
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young")
        ];

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.FirstError.ShouldBe(errors[0]);
    }

    [Fact]
    public void ImplicitCastErrorArray_WhenAccessingFirstError_ShouldReturnFirstError()
    {
        // Arrange
        Error[] errors =
        [
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        ];

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.ShouldBeTrue();
        errorOrPerson.FirstError.ShouldBe(errors[0]);
    }

    [Fact]
    public void CreateErrorOr_WhenUsingEmptyConstructor_ShouldThrow()
    {
        // Assert
        Should.Throw<InvalidOperationException>(() => new ErrorOr<int>());
    }

    [Fact]
    public void CreateErrorOr_WhenEmptyErrorsList_ShouldThrow()
    {
        // Assert
        var ex = Should.Throw<ArgumentException>(() => (ErrorOr<int>)new List<Error>());
        ex.Message.ShouldBe("Cannot create an ErrorOr<TValue> from an empty collection of errors. Provide at least one error. (Parameter 'errors')");
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void CreateErrorOr_WhenEmptyErrorsArray_ShouldThrow()
    {
        // Assert
        var ex = Should.Throw<ArgumentException>(() => (ErrorOr<int>)Array.Empty<Error>());
        ex.Message.ShouldBe("Cannot create an ErrorOr<TValue> from an empty collection of errors. Provide at least one error. (Parameter 'errors')");
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void CreateErrorOr_WhenNullIsPassedAsErrorsList_ShouldThrowArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => (ErrorOr<int>)default(List<Error>)!);
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void CreateErrorOr_WhenNullIsPassedAsErrorsArray_ShouldThrowArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => (ErrorOr<int>)default(Error[])!);
        ex.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void CreateErrorOr_WhenValueIsNull_ShouldThrowArgumentNullException()
    {
        var ex = Should.Throw<ArgumentNullException>(() => (ErrorOr<int?>)default(int?));
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void IErrorOrValueObject_WhenHasValue_ShouldReturnValue()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Test");
        IErrorOr errorOr = errorOrPerson;

        // Act
        var valueObject = errorOr.ValueObject;

        // Assert
        valueObject.ShouldBeOfType<Person>();
        ((Person)valueObject).Name.ShouldBe("Test");
    }

    [Fact]
    public void IErrorOrValueObject_WhenIsError_ShouldThrowInvalidOperationException()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = Error.NotFound();
        IErrorOr errorOr = errorOrPerson;

        // Assert
        var ex = Should.Throw<InvalidOperationException>(() => errorOr.ValueObject);
        ex.Message.ShouldBe("The ValueObject property cannot be accessed when errors have been recorded. Check IsError before accessing ValueObject.");
    }

    [Fact]
    public void IErrorOrErrors_WhenHasValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        ErrorOr<Person> errorOrPerson = new Person("Test");
        IErrorOr errorOr = errorOrPerson;

        // Assert
        errorOr.IsError.ShouldBeFalse();
        var ex = Should.Throw<InvalidOperationException>(() => errorOr.Errors);
        ex.Message.ShouldBe("The Errors property cannot be accessed when no errors have been recorded. Check IsError before accessing Errors.");
    }

    [Fact]
    public void IErrorOrErrors_WhenIsError_ShouldReturnErrors()
    {
        // Arrange
        var error = Error.NotFound("Test.NotFound", "Test not found");
        ErrorOr<Person> errorOrPerson = error;
        IErrorOr errorOr = errorOrPerson;

        // Act & Assert
        errorOr.IsError.ShouldBeTrue();
        errorOr.Errors.ShouldNotBeNull();
        errorOr.Errors.Count.ShouldBe(1);
        errorOr.Errors![0].ShouldBe(error);
    }
}

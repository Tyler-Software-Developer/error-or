using TylerSoftware.ErrorOr.Errors;

namespace TylerSoftware.ErrorOr;

internal static class EmptyErrors
{
    public static List<Error> Instance { get; } = [];
}

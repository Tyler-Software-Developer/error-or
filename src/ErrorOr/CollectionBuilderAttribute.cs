#if !NET8_0_OR_GREATER
namespace TylerSoftware.ErrorOr;

/// <summary>
/// Specifies the type and method used to create collection instances from collection expressions.
/// Polyfill for .NET Standard 2.0 and earlier.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
internal sealed class CollectionBuilderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionBuilderAttribute"/> class.
    /// </summary>
    /// <param name="builderType">The type containing the static builder method.</param>
    /// <param name="methodName">The name of the static builder method.</param>
    public CollectionBuilderAttribute(Type builderType, string methodName)
    {
        BuilderType = builderType;
        MethodName = methodName;
    }

    /// <summary>
    /// Gets the type that contains the static builder method.
    /// </summary>
    public Type BuilderType { get; }

    /// <summary>
    /// Gets the name of the static builder method.
    /// </summary>
    public string MethodName { get; }
}
#endif

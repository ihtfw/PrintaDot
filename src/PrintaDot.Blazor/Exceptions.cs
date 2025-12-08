namespace PrintaDot.Blazor;

public sealed class ExtensionConnectionFailedException : Exception
{
    public ExtensionConnectionFailedException()
        : base("Extension connection failed!") { }
}

public sealed class NativeAppConnectionFailedException : Exception
{
    public NativeAppConnectionFailedException()
        : base("Native app connection failed!") { }
}

namespace MinimalApiBuilder.UnitTests.Infrastructure;

internal static class StaticUri
{
    public static readonly Uri DataTxtUri = new("/data.txt", UriKind.Relative);

    public static readonly Uri RangeTxtUri = new("/range.txt", UriKind.Relative);

    public static readonly Uri FooTxtUri = new("/foo.txt", UriKind.Relative);

    public static readonly Uri SubDataJsUri = new("/sub/data.js", UriKind.Relative);

    public static readonly Uri DoesNotExistUri = new("/does-not-exist.txt", UriKind.Relative);
}

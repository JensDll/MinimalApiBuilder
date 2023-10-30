namespace MinimalApiBuilder.Entities;

internal static class TempDirectory
{
    static TempDirectory()
    {
        string tempDirectory = Environment.GetEnvironmentVariable("ASPNETCORE_TEMP") ?? System.IO.Path.GetTempPath();

        if (!Directory.Exists(tempDirectory))
        {
#pragma warning disable CA1065
            throw new DirectoryNotFoundException(tempDirectory);
#pragma warning restore CA1065
        }

        Path = tempDirectory;
    }

    public static string Path { get; }
}

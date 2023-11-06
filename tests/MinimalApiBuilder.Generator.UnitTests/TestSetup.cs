using System.Runtime.CompilerServices;

namespace MinimalApiBuilder.Generator.UnitTests;

public static class TestSetup
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();

        DerivePathInfo(static (sourceFile, projectDirectory, type, method) => new PathInfo(
            directory: Path.Combine(projectDirectory, "__snapshots__"), typeName: type.Name, methodName: method.Name));
    }
}

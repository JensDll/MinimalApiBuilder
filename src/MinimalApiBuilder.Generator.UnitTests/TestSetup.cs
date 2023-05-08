using System.Runtime.CompilerServices;

namespace MinimalApiBuilder.Generator.UnitTests;

public static class TestSetup
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}

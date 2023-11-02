// namespace MinimalApiBuilder.Generator.UnitTests;
//
// [UsesVerify]
// public class BasicTests
// {
//     [Theory]
//     [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
//     public Task One_Endpoint_Without_Parameters(TestAnalyzerConfigOptionsProvider provider)
//     {
//         // lang=cs
//         const string source = """
//             using MinimalApiBuilder;
//             using Microsoft.AspNetCore.Builder;
//
//             namespace Features;
//
//             public partial class Endpoint : MinimalApiBuilderEndpoint
//             {
//                 private static int Handle(Endpoint e) => 1;
//
//                 public static void Configure(RouteHandlerBuilder builder) { }
//             }
//             """;
//
//         return TestHelper.Verify(source, provider);
//     }
//
//     [Theory]
//     [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
//     public Task Multiple_Endpoints_Without_Parameters(TestAnalyzerConfigOptionsProvider provider)
//     {
//         // lang=cs
//         const string source = """
//             using MinimalApiBuilder;
//             using Microsoft.AspNetCore.Builder;
//
//             namespace Features;
//
//             public partial class FirstEndpoint : MinimalApiBuilderEndpoint
//             {
//                 private static int Handle(FirstEndpoint e) => 1;
//
//                 public static void Configure(RouteHandlerBuilder builder) { }
//             }
//
//             public partial class SecondEndpoint : MinimalApiBuilderEndpoint
//             {
//                 private static string HandleAsync(SecondEndpoint e) => "Hello"
//
//                 public static void Configure(RouteHandlerBuilder builder) { }
//             }
//             """;
//
//         return TestHelper.Verify(source, provider);
//     }
//
//     [Theory]
//     [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
//     public Task Endpoint_Without_Configure(TestAnalyzerConfigOptionsProvider provider)
//     {
//         // lang=cs
//         const string source = """
//             using MinimalApiBuilder;
//
//             namespace Features;
//
//             public partial class Endpoint : MinimalApiBuilderEndpoint
//             {
//                 private static int Handle(Endpoint e) => 1;
//             }
//             """;
//
//         return TestHelper.Verify(source, provider);
//     }
//
//     [Theory]
//     [ClassData(typeof(TestAnalyzerConfigOptionsProviderClassData))]
//     public Task Endpoint_In_Global_Namespace(TestAnalyzerConfigOptionsProvider provider)
//     {
//         // lang=cs
//         const string source = """
//             using MinimalApiBuilder;
//
//             public partial class Endpoint : MinimalApiBuilderEndpoint
//             {
//                 private static int Handle(Endpoint e) => 1;
//             }
//             """;
//
//         return TestHelper.Verify(source, provider);
//     }
// }

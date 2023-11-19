using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal class ConfigureToGenerate
{
    public ConfigureToGenerate(
        int arity,
        string filePath,
        int lineNumber,
        List<(int, string)> endpoints)
    {
        Arity = arity;
        FilePath = filePath;
        LineNumber = lineNumber;
        Endpoints = endpoints;
    }

    public int Arity { get; }

    public string FilePath { get; }

    public int LineNumber { get; }

    public List<(int, string)> Endpoints { get; }

    public static ConfigureToGenerate? Create(IInvocationOperation configure, IArrayInitializerOperation builders)
    {
        int arity = builders.ElementValues.Length;

        if (arity == 0)
        {
            return null;
        }

        (string filePath, int lineNumber) = GetLocation(configure);
        List<(int, string)> endpoints = GetEndpoints(builders);

        if (endpoints.Count == 0)
        {
            return null;
        }

        return new ConfigureToGenerate(
            arity: arity,
            filePath: filePath,
            lineNumber: lineNumber,
            endpoints: endpoints);
    }

    private static List<(int, string)> GetEndpoints(IArrayInitializerOperation builders)
    {
        List<(int, string)> result = new();

        for (int i = 0; i < builders.ElementValues.Length; ++i)
        {
            INamedTypeSymbol? endpoint = ResolveEndpointFromOperation(builders.ElementValues[i].ChildOperations.Last());

            if (endpoint is null)
            {
                continue;
            }

            if (endpoint.BaseType is not
                {
                    Name: "MinimalApiBuilderEndpoint", ContainingNamespace:
                    {
                        Name: "MinimalApiBuilder", ContainingNamespace.IsGlobalNamespace: true
                    }
                })
            {
                continue;
            }

            result.Add((i, endpoint.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
        }

        return result;
    }

    private static INamedTypeSymbol? ResolveEndpointFromOperation(IOperation operation)
    {
        return operation switch
        {
            IArgumentOperation argument => ResolveEndpointFromOperation(argument.Value),
            IConversionOperation conversion => ResolveEndpointFromOperation(conversion.Operand),
            IDelegateCreationOperation delegateCreation => ResolveEndpointFromOperation(delegateCreation.Target),
            IMethodReferenceOperation methodRef => methodRef.Member.ContainingType,
            _ => null
        };
    }

    private static (string, int) GetLocation(IInvocationOperation configure)
    {
        InvocationExpressionSyntax configureSyntax = Unsafe.As<InvocationExpressionSyntax>(configure.Syntax);

        Location location = configureSyntax.Expression switch
        {
            IdentifierNameSyntax identifierName => identifierName.Identifier.GetLocation(),
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.GetLocation(),
            _ => throw new InvalidOperationException()
        };

        string filePath = configureSyntax.SyntaxTree.GetInterceptorFilePath(configure.SemanticModel?.Compilation.Options.SourceReferenceResolver);
        int lineNumber = location.GetLineSpan().StartLinePosition.Line + 1;

        return (filePath, lineNumber);
    }
}

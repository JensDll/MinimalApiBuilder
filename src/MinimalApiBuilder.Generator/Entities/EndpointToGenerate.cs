using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerate
{
    private EndpointToGenerate(string id, string className, string namespaceName, string handlerMethodName)
    {
        Id = id;
        ClassName = className;
        NamespaceName = namespaceName;
        HandlerMethodName = handlerMethodName;
    }

    public string Id { get; }

    public string ClassName { get; }

    public string NamespaceName { get; }

    public string HandlerMethodName { get; }

    public static IEnumerable<EndpointToGenerate> Collect(Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> endpointDeclarations,
        CancellationToken cancellationToken)
    {
        foreach (ClassDeclarationSyntax endpointDeclaration in endpointDeclarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SemanticModel semanticModel = compilation.GetSemanticModel(endpointDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(endpointDeclaration) is not INamedTypeSymbol endpointSymbol)
            {
                continue;
            }

            string? handlerMethodName = null;

            foreach (ISymbol member in endpointSymbol.GetMembers())
            {
                if (member is not IMethodSymbol methodSymbol)
                {
                    continue;
                }

                string methodName = methodSymbol.Name;

                switch (methodName)
                {
                    case "Handle":
                    case "HandleAsync":
                        handlerMethodName = methodName;
                        break;
                    case "Configure":
                        break;
                }
            }

            if (handlerMethodName is null)
            {
                continue;
            }

            yield return new EndpointToGenerate(
                id: endpointSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                namespaceName: endpointSymbol.ContainingNamespace.ToDisplayString(),
                className: endpointDeclaration.Identifier.Text,
                handlerMethodName: handlerMethodName);
        }
    }
}

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerate
{
    private readonly string _identifier;

    private EndpointToGenerate(string identifier, string className, string namespaceName,
        EndpointToGenerateHandler handler)
    {
        _identifier = identifier;
        ClassName = className;
        NamespaceName = namespaceName;
        Handler = handler;
    }

    public string ClassName { get; }

    public string NamespaceName { get; }

    public EndpointToGenerateHandler Handler { get; }

    public override string ToString() => _identifier;

    public static IEnumerable<EndpointToGenerate> Collect(Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> endpointDeclarations,
        CancellationToken cancellationToken)
    {
        foreach (ClassDeclarationSyntax endpointDeclaration in endpointDeclarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SemanticModel semanticModel = compilation.GetSemanticModel(endpointDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(endpointDeclaration) is not INamespaceOrTypeSymbol endpointSymbol)
            {
                continue;
            }

            if (!TryGetHandler(endpointSymbol, out EndpointToGenerateHandler? handler))
            {
                continue;
            }

            EndpointToGenerate endpoint = new(
                identifier: endpointSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                namespaceName: endpointSymbol.ContainingNamespace.ToDisplayString(),
                className: endpointSymbol.Name,
                handler: handler!);

            yield return endpoint;
        }
    }

    private static bool TryGetHandler(INamespaceOrTypeSymbol endpointSymbol, out EndpointToGenerateHandler? handler)
    {
        handler = null;

        foreach (ISymbol member in endpointSymbol.GetMembers())
        {
            if (member is not IMethodSymbol methodSymbol)
            {
                continue;
            }

            switch (methodSymbol.Name)
            {
                case "Handle":
                case "HandleAsync":
                    var parameters = new EndpointToGenerateHandlerParameter[methodSymbol.Parameters.Length];
                    EndpointToGenerateHandlerParameter? endpointParameter = null;

                    for (int i = 0; i < methodSymbol.Parameters.Length; ++i)
                    {
                        IParameterSymbol parameterSymbol = methodSymbol.Parameters[i];

                        parameters[i] =
                            new EndpointToGenerateHandlerParameter(
                                identifier: parameterSymbol.Type.ToDisplayString(
                                    SymbolDisplayFormat.FullyQualifiedFormat),
                                position: i);

                        if (SymbolEqualityComparer.Default.Equals(parameterSymbol.Type, endpointSymbol))
                        {
                            endpointParameter = parameters[i];
                        }
                    }

                    if (endpointParameter is null)
                    {
                        return false;
                    }

                    handler = new EndpointToGenerateHandler(
                        name: methodSymbol.Name,
                        endpointParameter: endpointParameter,
                        parameters: parameters);

                    return true;
            }
        }

        return false;
    }
}

internal class EndpointToGenerateHandler
{
    public EndpointToGenerateHandler(string name, EndpointToGenerateHandlerParameter endpointParameter,
        EndpointToGenerateHandlerParameter[] parameters)
    {
        Name = name;
        EndpointParameter = endpointParameter;
        Parameters = parameters;
    }

    public string Name { get; }

    public EndpointToGenerateHandlerParameter EndpointParameter { get; }

    public EndpointToGenerateHandlerParameter[] Parameters { get; }
}

internal class EndpointToGenerateHandlerParameter
{
    private readonly string _identifier;

    public EndpointToGenerateHandlerParameter(string identifier, int position)
    {
        _identifier = identifier;
        Position = position;
    }

    public int Position { get; }

    public override string ToString() => _identifier;
}

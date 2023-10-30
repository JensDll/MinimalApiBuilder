using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerate : IWithSyntaxTree
{
    private const string MinimalApiBuilderEndpointName = "MinimalApiBuilder.MinimalApiBuilderEndpoint";
    private const string RouteHandlerBuilderName = "Microsoft.AspNetCore.Builder.RouteHandlerBuilder";
    private readonly string _identifier;

    private EndpointToGenerate(
        string identifier,
        SyntaxTree syntaxTree,
        string className,
        string? namespaceName,
        EndpointToGenerateHandler handler,
        bool needsConfigure)
    {
        _identifier = identifier;
        ClassName = className;
        NamespaceName = namespaceName;
        Handler = handler;
        NeedsConfigure = needsConfigure;
        SyntaxTree = syntaxTree;
    }

    public string ClassName { get; }

    public string? NamespaceName { get; }

    public EndpointToGenerateHandler Handler { get; }

    public bool NeedsConfigure { get; }

    public SyntaxTree SyntaxTree { get; }

    public override string ToString() => _identifier;

    public static EndpointToGenerate? Create(ClassDeclarationSyntax endpointDeclaration,
        SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (semanticModel.GetDeclaredSymbol(endpointDeclaration, cancellationToken)
            is not INamedTypeSymbol endpointSymbol)
        {
            return null;
        }

        if (semanticModel.Compilation.GetTypeByMetadataName(MinimalApiBuilderEndpointName)
            is not { } minimalApiBuilderEndpointSymbol)
        {
            return null;
        }

        if (!minimalApiBuilderEndpointSymbol.Equals(endpointSymbol.BaseType, SymbolEqualityComparer.Default))
        {
            return null;
        }

        if (semanticModel.Compilation.GetTypeByMetadataName(RouteHandlerBuilderName)
            is not { } routeHandlerBuilderSymbol)
        {
            return null;
        }

        bool needsConfigure = true;
        EndpointToGenerateHandler? handler = null;

        foreach (ISymbol member in endpointSymbol.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (member)
            {
                case IMethodSymbol methodSymbol:
                    if (TryGetHandler(endpointSymbol, methodSymbol, out var endpointHandler))
                    {
                        handler = endpointHandler;
                    }
                    else if (IsConfigure(methodSymbol, routeHandlerBuilderSymbol))
                    {
                        needsConfigure = false;
                    }

                    break;
            }
        }

        if (handler is null)
        {
            return null;
        }

        EndpointToGenerate endpoint = new(
            identifier: endpointSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            syntaxTree: endpointDeclaration.SyntaxTree,
            namespaceName: endpointSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : endpointSymbol.ContainingNamespace.ToDisplayString(),
            className: endpointSymbol.Name,
            handler: handler,
            needsConfigure: needsConfigure);

        return endpoint;
    }

    private static bool TryGetHandler(ISymbol endpointSymbol, IMethodSymbol methodSymbol,
        out EndpointToGenerateHandler? handler)
    {
        handler = null;

        if (methodSymbol.Name is not ("Handle" or "HandleAsync"))
        {
            return false;
        }

        var parameters = new EndpointToGenerateHandlerParameter[methodSymbol.Parameters.Length];
        EndpointToGenerateHandlerParameter? endpointParameter = null;

        for (int i = 0; i < methodSymbol.Parameters.Length; ++i)
        {
            IParameterSymbol parameterSymbol = methodSymbol.Parameters[i];

            parameters[i] = new EndpointToGenerateHandlerParameter(
                identifier: parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                position: i);

            if (parameterSymbol.Type.Equals(endpointSymbol, SymbolEqualityComparer.Default))
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

    private static bool IsConfigure(IMethodSymbol methodSymbol, ISymbol routeHandlerBuilderSymbol)
    {
        return methodSymbol is { Name: "Configure", Parameters.Length: 1, ReturnsVoid: true, IsStatic: true } &&
               methodSymbol.Parameters[0].Type.Equals(routeHandlerBuilderSymbol, SymbolEqualityComparer.Default);
    }
}

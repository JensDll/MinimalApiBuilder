using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerate : IWithSyntaxTree
{
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

    public static EndpointToGenerate? Create(
        INamedTypeSymbol endpoint,
        ClassDeclarationSyntax endpointSyntax,
        WellKnownTypes wellKnownTypes,
        CancellationToken cancellationToken)
    {
        INamedTypeSymbol minimalApiBuilderEndpoint =
            wellKnownTypes[WellKnownTypes.Type.MinimalApiBuilder_MinimalApiBuilderEndpoint];

        if (!SymbolEqualityComparer.Default.Equals(endpoint.BaseType, minimalApiBuilderEndpoint))
        {
            return null;
        }

        INamedTypeSymbol routeHandlerBuilder =
            wellKnownTypes[WellKnownTypes.Type.Microsoft_AspNetCore_Builder_RouteHandlerBuilder];

        bool needsConfigure = true;
        EndpointToGenerateHandler? handler = null;

        foreach (ISymbol member in endpoint.GetMembers())
        {
            if (member is not IMethodSymbol method)
            {
                continue;
            }

            if (TryGetHandler(endpoint, method, wellKnownTypes, out var endpointHandler))
            {
                handler = endpointHandler;
                continue;
            }

            if (IsConfigure(method, routeHandlerBuilder))
            {
                needsConfigure = false;
            }
        }

        if (handler is null)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        EndpointToGenerate result = new(
            identifier: endpoint.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            syntaxTree: endpointSyntax.SyntaxTree,
            namespaceName: endpoint.ContainingNamespace.IsGlobalNamespace
                ? null
                : endpoint.ContainingNamespace.ToDisplayString(),
            className: endpoint.Name,
            handler: handler,
            needsConfigure: needsConfigure);

        return result;
    }

    private static bool TryGetHandler(ISymbol endpoint, IMethodSymbol method, WellKnownTypes wellKnownTypes,
        out EndpointToGenerateHandler? handler)
    {
        handler = null;

        if (method.Name is not ("Handle" or "HandleAsync"))
        {
            return false;
        }

        var parameters = new EndpointToGenerateHandlerParameter[method.Parameters.Length];
        EndpointToGenerateHandlerParameter? endpointParameter = null;

        for (int i = 0; i < method.Parameters.Length; ++i)
        {
            IParameterSymbol parameter = method.Parameters[i];
            parameters[i] = new EndpointToGenerateHandlerParameter(parameter, i, wellKnownTypes);
            if (SymbolEqualityComparer.Default.Equals(parameter.Type, endpoint))
            {
                endpointParameter = parameters[i];
            }
        }

        if (endpointParameter is null)
        {
            return false;
        }

        handler = new EndpointToGenerateHandler(
            name: method.Name,
            endpointParameter: endpointParameter,
            parameters: parameters);

        return true;
    }

    private static bool IsConfigure(IMethodSymbol method, ISymbol routeHandlerBuilder)
    {
        return method is { Name: "Configure", Parameters.Length: 1, ReturnsVoid: true, IsStatic: true } &&
               SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, routeHandlerBuilder);
    }
}

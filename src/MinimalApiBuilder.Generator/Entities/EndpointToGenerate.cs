using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerate
{
    private readonly string _identifier;

    private EndpointToGenerate(string identifier,
        string className,
        string namespaceName,
        EndpointToGenerateHandler handler,
        bool needsConfigure)
    {
        _identifier = identifier;
        ClassName = className;
        NamespaceName = namespaceName;
        Handler = handler;
        NeedsConfigure = needsConfigure;
    }

    public string ClassName { get; }

    public string NamespaceName { get; }

    public EndpointToGenerateHandler Handler { get; }

    public bool NeedsConfigure { get; }

    public override string ToString() => _identifier;

    public static EndpointToGenerate? Create(ClassDeclarationSyntax endpointDeclaration,
        SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (semanticModel.GetDeclaredSymbol(endpointDeclaration) is not INamespaceOrTypeSymbol endpointSymbol)
        {
            return null;
        }

        bool needsConfigure = true;
        EndpointToGenerateHandler? handler = null;

        foreach (ISymbol member in endpointSymbol.GetMembers())
        {
            switch (member)
            {
                case IMethodSymbol methodSymbol:
                    if (TryGetHandler(endpointSymbol, methodSymbol, out var endpointHandler))
                    {
                        handler = endpointHandler;
                    }
                    else if (IsConfigure(methodSymbol))
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
            namespaceName: endpointSymbol.ContainingNamespace.ToDisplayString(),
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

    private static bool IsConfigure(IMethodSymbol methodSymbol)
    {
        return methodSymbol is { Name: "Configure", Parameters.Length: 1, ReturnsVoid: true, IsStatic: true } &&
               methodSymbol.Parameters[0].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
               "global::Microsoft.AspNetCore.Builder.RouteHandlerBuilder";
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

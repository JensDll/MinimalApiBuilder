using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerate
{
    private readonly string _identifier;

    private EndpointToGenerate(string identifier,
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
    }

    public string ClassName { get; }

    public string? NamespaceName { get; }

    public EndpointToGenerateHandler Handler { get; }

    public bool NeedsConfigure { get; }

    public override string ToString() => _identifier;

    public static EndpointToGenerate? Create(ClassDeclarationSyntax endpointDeclaration,
        SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (semanticModel.GetDeclaredSymbol(endpointDeclaration, cancellationToken) is not INamespaceOrTypeSymbol
            endpointSymbol)
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

internal class EndpointToGenerateEqualityComparer : IEqualityComparer<EndpointToGenerate>
{
    public static readonly EndpointToGenerateEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerate? x, EndpointToGenerate? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        var handlerComparer = EndpointToGenerateHandlerEqualityComparer.Instance;

        return x.ClassName == y.ClassName &&
               x.NamespaceName == y.NamespaceName &&
               handlerComparer.Equals(x.Handler, y.Handler) &&
               x.NeedsConfigure == y.NeedsConfigure;
    }

    public int GetHashCode(EndpointToGenerate obj)
    {
        throw new NotImplementedException();
    }
}

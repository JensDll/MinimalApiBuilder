using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace MinimalApiBuilder.Generator.Common;

internal static class PredicateExtensions
{
    public static bool IsMinimalApiBuilderEndpoint(this INamedTypeSymbol symbol)
    {
        return symbol.BaseType is
        {
            Name: "MinimalApiBuilderEndpoint",
            ContainingNamespace:
            {
                Name: "MinimalApiBuilder",
                ContainingNamespace.IsGlobalNamespace: true
            }
        };
    }

    public static bool IsRouteHandlerBuilder(this ITypeSymbol symbol)
    {
        return symbol is
        {
            Name: "RouteHandlerBuilder",
            ContainingNamespace:
            {
                Name: "Builder",
                ContainingNamespace:
                {
                    Name: "AspNetCore",
                    ContainingNamespace:
                    {
                        Name: "Microsoft",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };
    }

    public static bool IsValueTask(this INamedTypeSymbol symbol)
    {
        return symbol is
        {
            Name: "ValueTask",
            ContainingNamespace:
            {
                Name: "Tasks",
                ContainingNamespace:
                {
                    Name: "Threading",
                    ContainingNamespace:
                    {
                        Name: "System",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };
    }

    public static bool IsHttpContext(this ITypeSymbol symbol)
    {
        return symbol is
        {
            Name: "HttpContext",
            ContainingNamespace:
            {
                Name: "Http",
                ContainingNamespace:
                {
                    Name: "AspNetCore",
                    ContainingNamespace:
                    {
                        Name: "Microsoft",
                        ContainingNamespace.IsGlobalNamespace: true
                    }
                }
            }
        };
    }

    public static bool IsParameterInfo(this ITypeSymbol symbol)
    {
        return symbol is
        {
            Name: "ParameterInfo",
            ContainingNamespace:
            {
                Name: "Reflection",
                ContainingNamespace:
                {
                    Name: "System",
                    ContainingNamespace.IsGlobalNamespace: true
                }
            }
        };
    }

    public static bool IsIFormatProvider(this ITypeSymbol symbol)
    {
        return symbol is
        {
            Name: "IFormatProvider",
            ContainingNamespace:
            {
                Name: "System",
                ContainingNamespace.IsGlobalNamespace: true
            }
        };
    }

    public static bool IsRegisterValidatorAttribute(this INamedTypeSymbol symbol)
    {
        return symbol is
        {
            Name: "RegisterValidatorAttribute",
            ContainingNamespace:
            {
                Name: "MinimalApiBuilder",
                ContainingNamespace.IsGlobalNamespace: true
            }
        };
    }

    public static bool IsAbstractValidator(this INamedTypeSymbol symbol)
    {
        return symbol is
        {
            Name: "AbstractValidator",
            ContainingNamespace:
            {
                Name: "FluentValidation",
                ContainingNamespace.IsGlobalNamespace: true
            }
        };
    }

    public static bool IsConfigure(this IInvocationOperation operation, out IArrayInitializerOperation builders)
    {
        if (operation.TargetMethod is
            {
                Name: "Configure",
                ContainingNamespace:
                {
                    Name: "MinimalApiBuilder",
                    ContainingNamespace.IsGlobalNamespace: true
                }
            }
            && operation.Arguments.Length == 1
            && operation.Arguments[0].Value is IArrayCreationOperation { Initializer: { } initializer })
        {
            builders = initializer;
            return true;
        }

        builders = null!;
        return false;
    }
}

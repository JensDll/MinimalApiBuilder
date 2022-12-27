﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinimalApiBuilder.Generator.Providers;

internal static class ValidatorProvider
{
    public static IncrementalValuesProvider<ClassDeclarationSyntax> ForValidatorWithDependenciesDeclarations(
        this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsValidatorWithDependencies(s),
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node);
    }

    private static bool IsValidatorWithDependencies(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } classDeclaration &&
               classDeclaration.BaseList.Types[0].Type is GenericNameSyntax
               {
                   Identifier.Text: "AbstractValidator",
                   TypeArgumentList.Arguments.Count: 1
               };
    }
}

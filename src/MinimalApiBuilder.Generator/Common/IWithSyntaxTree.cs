using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Common;

internal interface IWithSyntaxTree
{
    SyntaxTree SyntaxTree { get; }
}

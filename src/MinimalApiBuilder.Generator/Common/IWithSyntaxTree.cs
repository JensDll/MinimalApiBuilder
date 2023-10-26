using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Common;

public interface IWithSyntaxTree
{
    SyntaxTree SyntaxTree { get; }
}

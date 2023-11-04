using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Common;

internal interface IToGenerate
{
    ISymbol Symbol { get; }
}

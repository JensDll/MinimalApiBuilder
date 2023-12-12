using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MinimalApiBuilder.Generator.UnitTests.Infrastructure;

internal sealed class GeneratedCodeRewriter : CSharpSyntaxRewriter
{
    private static readonly SourceText s_generatedCodeAttributeSourceText
        = SourceText.From("global::System.CodeDom.Compiler.GeneratedCodeAttribute");

    public override SyntaxNode? VisitAttributeList(AttributeListSyntax node)
    {
        var withoutGeneratedCodeAttribute = node.Attributes
            .Where(static attribute => !attribute.Name.GetText().ContentEquals(s_generatedCodeAttributeSourceText));
        AttributeListSyntax newNode = node.WithAttributes(SyntaxFactory.SeparatedList(withoutGeneratedCodeAttribute));
        return newNode.Attributes.Count == 0 ? null : newNode;
    }
}

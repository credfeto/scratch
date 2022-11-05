using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator;

public sealed class EnumSyntaxReceiver : ISyntaxContextReceiver
{
    public EnumSyntaxReceiver()
    {
        this.Enums = new();
    }

    public List<EnumGeneration> Enums { get; }

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is not EnumDeclarationSyntax enumDeclarationSyntax)
        {
            return;
        }

        INamedTypeSymbol enumSymbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(enumDeclarationSyntax)!;

        // ImmutableArray<AttributeData> attributes = enumSymbol.GetAttributes();

        SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclarationSyntax.Members;

        this.Enums.Add(new(name: enumSymbol.Name, enumSymbol.ContainingNamespace.ToDisplayString(), members: members));
    }
}
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        INamedTypeSymbol enumSymbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(declaration: enumDeclarationSyntax)!;

        AccessType accessType = GetAccessType(enumDeclarationSyntax);

        if (accessType == AccessType.Private)
        {
            // skip privates
            return;
        }

        SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclarationSyntax.Members;

        this.Enums.Add(new(accessType: accessType, name: enumSymbol.Name, enumSymbol.ContainingNamespace.ToDisplayString(), members: members));
    }

    private static AccessType GetAccessType(EnumDeclarationSyntax generatorSyntaxContext)
    {
        bool isPublic = generatorSyntaxContext.Modifiers.Any(SyntaxKind.PublicKeyword);

        if (isPublic)
        {
            return AccessType.Public;
        }

        bool isPrivate = generatorSyntaxContext.Modifiers.Any(SyntaxKind.PrivateKeyword);

        if (isPrivate)
        {
            return AccessType.Private;
        }

        bool isInternal = generatorSyntaxContext.Modifiers.Any(SyntaxKind.InternalKeyword);

        bool isProtected = generatorSyntaxContext.Modifiers.Any(SyntaxKind.ProtectedKeyword);

        if (isProtected)
        {
            return isInternal
                ? AccessType.ProtectedInternal
                : AccessType.Protected;
        }

        return AccessType.Internal;
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Helpers;

namespace SourceGenerator;

public sealed class EnumGeneration
{
    public EnumGeneration(AccessType accessType, string name, string @namespace, in SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
    {
        this.AccessType = accessType;
        this.Name = name;
        this.Namespace = @namespace;
        this.Members = members;
    }

    public SeparatedSyntaxList<EnumMemberDeclarationSyntax> Members { get; }

    public AccessType AccessType { get; }

    public string Name { get; }

    public string Namespace { get; }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator;

public sealed class EnumGeneration
{
    public EnumGeneration(string name, string @namespace, in SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
    {
        this.Name = name;
        this.Namespace = @namespace;
        this.Members = members;
    }

    public SeparatedSyntaxList<EnumMemberDeclarationSyntax> Members { get; }

    public string Name { get; }

    public string Namespace { get; }
}
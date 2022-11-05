using Microsoft.CodeAnalysis.CSharp;

namespace SourceGenerator;

public enum AccessType
{
    Public = SyntaxKind.PublicKeyword,
    Private = SyntaxKind.PrivateKeyword,
    Protected = SyntaxKind.ProtectedKeyword,
    ProtectedInternal = SyntaxKind.ProtectedKeyword | SyntaxKind.InternalKeyword,
    Internal = SyntaxKind.InternalKeyword
}
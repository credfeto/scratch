using Microsoft.CodeAnalysis.CSharp;

namespace SourceGenerator.Helpers;

public enum AccessType
{
    PUBLIC = SyntaxKind.PublicKeyword,
    PRIVATE = SyntaxKind.PrivateKeyword,
    PROTECTED = SyntaxKind.ProtectedKeyword,
    PROTECTED_INTERNAL = SyntaxKind.ProtectedKeyword | SyntaxKind.InternalKeyword,
    INTERNAL = SyntaxKind.InternalKeyword
}
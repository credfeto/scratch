using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceGenerator.Helpers;

namespace SourceGenerator;

[Generator]
public sealed class EnumGenerator : ISourceGenerator
{
    private const string VERSION = "development";

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not EnumSyntaxReceiver receiver)
        {
            return;
        }

        foreach (EnumGeneration enumDeclaration in receiver.Enums)
        {
            string className = enumDeclaration.Name + "GeneratedExtensions";

            CodeBuilder source = new();

            source.AppendLine("using System;")
                  .AppendLine("using System.CodeDom.Compiler;")
                  .AppendLine("using System.Diagnostics.CodeAnalysis;")
                  .AppendBlankLine()
                  .AppendLine("namespace " + enumDeclaration.Namespace + ";")
                  .AppendBlankLine()
                  .AppendLine($"[GeneratedCode(tool: \"{nameof(EnumGenerator)}\", version: \"{VERSION}\")]");

            using (source.StartBlock(ConvertAccessType(enumDeclaration.AccessType) + " static class " + className))
            {
                GenerateGetName(source: source, enumDeclaration: enumDeclaration);
            }

            context.AddSource(enumDeclaration.Namespace + "." + className, SourceText.From(text: source.Text, encoding: Encoding.UTF8));
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new EnumSyntaxReceiver());
    }

    private static void GenerateGetName(CodeBuilder source, EnumGeneration enumDeclaration)
    {
        using (source.StartBlock("public static string GetName(this " + enumDeclaration.Name + " value)"))
        {
            using (source.StartBlock(text: "return value switch", start: "{", end: "};"))
            {
                ImmutableHashSet<string> names = UniqueEnumMemberNames(enumDeclaration);

                foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
                {
                    if (member.EqualsValue?.Value.Kind() == SyntaxKind.IdentifierName)
                    {
                        string memberName = member.EqualsValue.Value.ToString();

                        source.AppendLine("// " + member.EqualsValue.Value.Kind());
                        source.AppendLine("// " + memberName);

                        if (names.Contains(memberName))
                        {
                            continue;
                        }
                    }

                    if (IsObsolete(member))
                    {
                        continue;
                    }

                    source.AppendLine(enumDeclaration.Name + "." + member.Identifier.Text + " => \"" + member.Identifier.Text + "\",");
                }

                source.AppendLine("_ => throw new ArgumentOutOfRangeException(nameof(value), actualValue: value, message: \"Unknown enum member\")");
            }
        }
    }

    private static bool IsObsolete(EnumMemberDeclarationSyntax member)
    {
        List<string> a = member.AttributeLists.SelectMany(x => x.Attributes)
                               .Select(x => x.Name.ToString())
                               .ToList();
        bool isObsolete = a.Contains(value: "Obsolete", comparer: StringComparer.Ordinal) || a.Contains(value: "ObsoleteAttribute", comparer: StringComparer.Ordinal);

        return isObsolete;
    }

    private static ImmutableHashSet<string> UniqueEnumMemberNames(EnumGeneration enumDeclaration)
    {
        return enumDeclaration.Members.Select(m => m.Identifier.Text)
                              .Distinct(StringComparer.Ordinal)
                              .ToImmutableHashSet(StringComparer.Ordinal);
    }

    private static string ConvertAccessType(AccessType accessType)
    {
        return accessType switch
        {
            AccessType.PUBLIC => "public",
            AccessType.PRIVATE => "private",
            AccessType.PROTECTED => "protected",
            AccessType.PROTECTED_INTERNAL => "protected internal",
            AccessType.INTERNAL => "internal",
            _ => throw new ArgumentOutOfRangeException(nameof(accessType), actualValue: accessType, message: "Unknown access type")
        };
    }
}
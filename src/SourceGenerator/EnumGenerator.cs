using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
                  .AppendLine(";")
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
            HashSet<string> names = new(StringComparer.Ordinal);

            using (source.StartBlock(text: "return value switch", start: "{", end: "};"))
            {
                foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
                {
                    string gubbins = member.EqualsValue!.Value.ToString();

                    if (!names.Add(gubbins))
                    {
                        continue;
                    }

                    source.AppendLine(enumDeclaration.Name + "." + member.Identifier.Text + " => \"" + member.Identifier.Text + "\",");
                }

                source.AppendLine("_ => throw new ArgumentOutOfRangeException(nameof(value), actualValue: value, message: \"Unknown enum member\")");
            }
        }
    }

    private static string ConvertAccessType(AccessType accessType)
    {
        return accessType switch
        {
            AccessType.Public => "public",
            AccessType.Private => "private",
            AccessType.Protected => "protected",
            AccessType.ProtectedInternal => "protected internal",
            AccessType.Internal => "internal",
            _ => throw new ArgumentOutOfRangeException(nameof(accessType), actualValue: accessType, message: "Unknown access type")
        };
    }
}
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

            StringBuilder source = new();

            source.AppendLine("using System;")
                  .AppendLine("using System.CodeDom.Compiler;")
                  .AppendLine("using System.Diagnostics.CodeAnalysis;")
                  .AppendLine()
                  .Append("namespace ")
                  .Append(enumDeclaration.Namespace)
                  .AppendLine(";")
                  .AppendLine()
                  .AppendLine($"[GeneratedCode(tool: \"{nameof(EnumGenerator)}\", version: \"{VERSION}\")]")
                  .Append("public static class ")
                  .AppendLine(className)
                  .AppendLine("{");

            foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
            {
                source.Append(" // ")
                      .AppendLine(member.Identifier.ValueText);
            }

            source.AppendLine("}");

            context.AddSource(enumDeclaration.Namespace + "." + className, SourceText.From(source.ToString(), encoding: Encoding.UTF8));
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new EnumSyntaxReceiver());
    }
}
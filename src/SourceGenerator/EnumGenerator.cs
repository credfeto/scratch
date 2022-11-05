using Microsoft.CodeAnalysis;

namespace SourceGenerator;

[Generator]
public sealed class EnumGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        // TODO - actual source generator goes here!
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}
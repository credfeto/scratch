using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Experiments.InlineStructOptimisations;

[SuppressMessage(category: "Microsoft.Design", checkId: "CA1815", Justification = "No equals needed here")]
[SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0011", Justification = "Can't be read only for InlineArray")]
[InlineArray(32)]
public struct KeccakSizedArray
{
    [SuppressMessage(category: "SonarAnalyzer.CSharp", checkId: "S1144: Unused private property", Justification = "Required for InlineArray to work")]
    [SuppressMessage(category: "SonarAnalyzer.CSharp", checkId: "S4487: Unused private property", Justification = "Required for InlineArray to work")]
    private byte _element0;
}
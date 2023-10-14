using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Implementations.InlineStructOptimisations;

[SuppressMessage(category: "Microsoft.Design", checkId: "CA1815", Justification = "No equals needed here")]
[SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0011", Justification = "Can't be read only for InlineArray")]
[InlineArray(Length)]
public struct KeccakSizedArray
{
    public const int Length = 32;

    public KeccakSizedArray(in ReadOnlySpan<byte> source)
    {
        source.CopyTo(MemoryMarshal.CreateSpan(reference: ref this._element0, length: Length));
    }

    [SuppressMessage(category: "SonarAnalyzer.CSharp", checkId: "S1144: Unused private property", Justification = "Required for InlineArray to work")]
    [SuppressMessage(category: "SonarAnalyzer.CSharp", checkId: "S4487: Unused private property", Justification = "Required for InlineArray to work")]
    [SuppressMessage(category: "ReSharper", checkId: "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Required for InlineArray to work")]
    [SuppressMessage(category: "ReSharper", checkId: "FieldCanBeMadeReadOnly.Local", Justification = "Required for InlineArray to work")]
    private byte _element0;
}
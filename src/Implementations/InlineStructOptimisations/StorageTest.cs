using System.Diagnostics;

namespace Implementations.InlineStructOptimisations;

[DebuggerDisplay("{Bytes}")]
public readonly record struct StorageTest<T>(T Bytes)
    where T : struct;
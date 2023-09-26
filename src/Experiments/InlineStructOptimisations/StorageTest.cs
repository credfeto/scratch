using System.Diagnostics;

namespace Experiments.InlineStructOptimisations;

[DebuggerDisplay("{Bytes}")]
public readonly record struct StorageTest<T>(T Bytes)
    where T : struct;
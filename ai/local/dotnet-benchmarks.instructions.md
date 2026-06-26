# .NET Benchmark Instructions

[Back to Local Instructions Index](index.md)

## FFS0012 Suppression on Benchmark Classes (APPROVED EXCEPTION)

Benchmark classes decorated with `[SimpleJob]` (from BenchmarkDotNet) must be non-abstract and cannot be sealed. BenchmarkDotNet generates derived types from benchmark classes at runtime to inject measurement infrastructure; sealing the class would prevent this code generation from compiling.

The approved pattern for benchmark classes that cannot be sealed is:

```csharp
[SimpleJob]
[MemoryDiagnoser(false)]
[SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0012:Make Sealed", Justification = "Benchmarks")]
public class MyBench : BenchBase
{
    // ...
}
```

This `[SuppressMessage]` is **explicitly permitted** for all benchmark classes under `src/Bench/` that:

- Inherit from `BenchBase`
- Are decorated with `[SimpleJob]`
- Cannot be sealed due to BenchmarkDotNet's runtime code-generation requirement

This pattern matches the documented example in the external NuGet package `FunFair.Test.Common` (owned by this repo's owner), specifically in its `ExampleBenchmarks.cs` test file. Do not search for this file locally — it is not present in this repository.

AI agents do **not** need additional per-PR approval to apply this suppression on benchmark classes meeting the above criteria.

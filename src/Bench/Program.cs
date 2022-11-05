#define ENUM
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace Bench;

public static class Program
{
    private static void Main()
    {
        RunRegexBenchmarks();
        RunEnumBenchmarks();
    }

    [Conditional("REGEX")]
    private static void RunRegexBenchmarks()
    {
        BenchmarkRunner.Run<RegexBench>();
    }

    [Conditional("ENUM")]
    private static void RunEnumBenchmarks()
    {
        BenchmarkRunner.Run<EnumBench>();
    }
}
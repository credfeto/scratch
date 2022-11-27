#define REGED
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace Bench;

public static class Program
{
    private static void Main()
    {
        RunRegexBenchmarks();
    }

    [Conditional("REGEX")]
    private static void RunRegexBenchmarks()
    {
        BenchmarkRunner.Run<RegexBench>();
    }
}
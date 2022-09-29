using BenchmarkDotNet.Running;

namespace Bench;

public static class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<RegexBench>();
    }
}
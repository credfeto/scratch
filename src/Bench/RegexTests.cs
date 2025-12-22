using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using FunFair.Test.Common;
using Xunit;

namespace Bench;

public sealed class RegexTests : LoggingTestBase
{
    public RegexTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void Run_Benchmarks()
    {
        (Summary _, AccumulationLogger logger) = Benchmark<Benchmarks.RegexBench>();

        this.Output.WriteLine(logger.GetLog());
    }
}

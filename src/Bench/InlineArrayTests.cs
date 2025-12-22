using Bench.Benchmarks;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using FunFair.Test.Common;
using Xunit;

namespace Bench;

public sealed class InlineArrayTests : LoggingTestBase
{
    public InlineArrayTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void Run_Benchmarks()
    {
        (Summary _, AccumulationLogger logger) = Benchmark<InlineArrayBench>();

        this.Output.WriteLine(logger.GetLog());
    }
}

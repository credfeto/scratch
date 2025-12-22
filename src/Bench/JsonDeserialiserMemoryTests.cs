using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using FunFair.Test.Common;
using Xunit;

namespace Bench;

public sealed class JsonDeserialiserMemoryTests : LoggingTestBase
{
    public JsonDeserialiserMemoryTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void Run_Benchmarks()
    {
        (Summary _, AccumulationLogger logger) = Benchmark<JsonDeserialiserMemoryBench>();

        this.Output.WriteLine(logger.GetLog());
    }
}
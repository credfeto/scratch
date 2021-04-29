using System;
using System.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Experiments
{
    public sealed class BigIntegerTests
    {
        private readonly ITestOutputHelper _output;

        public BigIntegerTests(ITestOutputHelper output)
        {
            this._output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Theory]
        [InlineData("649661572269124152138")]
        [InlineData("16069726249898850")]
        [InlineData("20853920117998328678")]
        public void Parse(string value)
        {
            bool ok = BigInteger.TryParse(value: value, out BigInteger parsed);
            Assert.True(condition: ok, userMessage: "Should have parsed");
            this._output.WriteLine($"{parsed}");
        }
    }
}
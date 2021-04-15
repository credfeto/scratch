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

    public sealed class TimeSpanTests
    {
        private readonly ITestOutputHelper _output;

        public TimeSpanTests(ITestOutputHelper output)
        {
            this._output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Theory]
        [InlineData("07:43:07.2762671")]
        [InlineData("12:43:07.2762671")]
        [InlineData("22:12:43:07.2762671")]
        [InlineData("704:12:43:07.2762671")]
        public void Parse(string value)
        {
            TimeSpan ts = TimeSpan.Parse(value);

            this._output.WriteLine(ts.ToString(@"dd\ \d\a\y\s\,\ hh\ \h\o\u\r\s\ mm\ \m\i\n\s\ ss\ \s\e\c\o\n\d\s"));
        }
    }
}
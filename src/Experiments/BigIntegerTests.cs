using System;
using System.Globalization;
using System.Numerics;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class BigIntegerTests : TestBase
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
        bool ok = BigInteger.TryParse(
            value: value,
            style: NumberStyles.Integer,
            provider: CultureInfo.InvariantCulture,
            out BigInteger parsed
        );
        Assert.True(condition: ok, userMessage: "Should have parsed");
        this._output.WriteLine($"{parsed}");
    }

    [Fact]
    public void Percentage50()
    {
        BigInteger total = BigInteger.Parse(
            value: "250000000000000000",
            provider: CultureInfo.InvariantCulture
        );

        const int percentage = 50;
        BigInteger expected = total / 2;

        this._output.WriteLine($"Expected: {expected}");

        BigInteger actual = this.Check(percentage: percentage, total: total);

        Assert.Equal(expected: expected, actual: actual);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(35)]
    [InlineData(50)]
    public void Percentage(int percentage)
    {
        Assert.True(percentage >= 0, userMessage: "Should be 0% or more");
        Assert.True(percentage <= 100, userMessage: "Should be 100% or less");

        BigInteger total = BigInteger.Parse(
            value: "250000000000000000",
            provider: CultureInfo.InvariantCulture
        );

        this.Check(percentage: percentage, total: total);
    }

    private BigInteger Check(int percentage, in BigInteger total)
    {
        const int accuracy = 10000;
        BigInteger numerator = total * accuracy * percentage;
        const long divisor = accuracy * 100;

        BigInteger actual = numerator / divisor;

        this._output.WriteLine($"Total:  {total}");
        this._output.WriteLine($"Actual: {actual}");

        Assert.True(actual < total, userMessage: "Should be less");

        return actual;
    }
}

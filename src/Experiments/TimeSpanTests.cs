using System;
using System.Globalization;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class TimeSpanTests : TestBase
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
    [InlineData("00:04:19.3520847")]
    public void Parse(string value)
    {
        TimeSpan ts = TimeSpan.Parse(input: value, formatProvider: CultureInfo.InvariantCulture);

        this._output.WriteLine(
            ts.ToString(
                format: @"dd\ \d\a\y\s\,\ hh\ \h\o\u\r\s\ mm\ \m\i\n\s\ ss\ \s\e\c\o\n\d\s",
                formatProvider: CultureInfo.InvariantCulture
            )
        );

        Assert.NotEqual(expected: TimeSpan.Zero, actual: ts);
    }
}

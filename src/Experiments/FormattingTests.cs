using System;
using System.Globalization;
using FunFair.Test.Common;
using Xunit;

namespace Experiments;

public sealed class FormattingTests : TestBase
{
    private readonly ITestOutputHelper _output;

    public FormattingTests(ITestOutputHelper output)
    {
        this._output = output ?? throw new ArgumentNullException(nameof(output));
    }

    [Theory]
    [InlineData(1.0, "1")]
    [InlineData(1.1, "1.1")]
    [InlineData(1.01, "1.01")]
    public void Format(double value, string expected)
    {
        string actual = value.ToString(format: "0.##", provider: CultureInfo.CurrentCulture);

        this._output.WriteLine($"Actual: {actual}");

        Assert.Equal(expected: expected, actual: actual);
    }
}

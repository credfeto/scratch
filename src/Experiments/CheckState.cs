using System;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class CheckState : TestBase
{
    private readonly ITestOutputHelper _output;

    public CheckState(ITestOutputHelper output)
    {
        this._output = output ?? throw new ArgumentNullException(nameof(output));
    }

    [Theory]
    [InlineData(-100, false)]
    [InlineData(-0, false)]
    [InlineData(100, true)]
    public void DoStuff(int lastCheckedOffset, bool pass)
    {
        DateTime now = new(year: 2019, month: 1, day: 1, hour: 12, minute: 34, second: 56, kind: DateTimeKind.Utc);
        DateTime lastChecked = now.AddSeconds(lastCheckedOffset);

        TimeSpan whenCanRetry = now - lastChecked;

        this._output.WriteLine($"Now: {now}");
        this._output.WriteLine($"LC : {lastChecked}");
        this._output.WriteLine($"RTR: {whenCanRetry}");

        Assert.Equal(expected: pass, lastChecked > now);
    }
}
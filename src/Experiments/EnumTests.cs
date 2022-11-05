using FunFair.Test.Common;
using Implementations;
using Models;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class EnumTests : LoggingTestBase
{
    public EnumTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    public void DoIt()
    {
        Assert.Equal(expected: "ONE", EnumHelpers.GetName(ExampleEnumValues.ONE));
        Assert.Equal(expected: "ONE", EnumHelpers.GetName(ExampleEnumValues.SAME_AS_ONE));
    }
}
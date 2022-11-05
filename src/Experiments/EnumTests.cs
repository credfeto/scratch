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
        Assert.Equal(expected: "ONE", ExampleEnumValues.ONE.GetNameReflection());
        Assert.Equal(expected: "ONE", ExampleEnumValues.SAME_AS_ONE.GetNameReflection());
    }
}
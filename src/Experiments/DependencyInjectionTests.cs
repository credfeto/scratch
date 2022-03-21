using System;
using Experiments.ReferenceObjects;
using FunFair.Test.Common;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Experiments;

public sealed class DependencyInjectionTests : TestBase
{
    private static IServiceProvider Build(Func<IServiceCollection, IServiceCollection> registration)
    {
        return registration(new ServiceCollection()).BuildServiceProvider();
    }

    [Fact]
    public void Simple()
    {
        IServiceProvider serviceProvider = Build(services => services.AddSingleton<ISimpleInterface, SimpleInterface>());

        ISimpleInterface simpleInterface = serviceProvider.GetRequiredService<ISimpleInterface>();
        Assert.NotNull(simpleInterface);
    }
}
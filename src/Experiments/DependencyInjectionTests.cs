using System;
using Experiments.ReferenceObjects;
using Experiments.ReferenceObjects.Services;
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

    [Fact]
    public void GenericWithOneTypeParameter()
    {
        IServiceProvider serviceProvider = Build(services => services.AddSingleton(typeof(IGenericInterface<>), typeof(GenericInterface<>)));

        IGenericInterface<T1> simpleInterface = serviceProvider.GetRequiredService<IGenericInterface<T1>>();
        Assert.NotNull(simpleInterface);

        Assert.Equal(typeof(T1), simpleInterface.ItemType);
    }
}
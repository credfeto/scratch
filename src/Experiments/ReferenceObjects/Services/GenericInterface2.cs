using System;

namespace Experiments.ReferenceObjects.Services;

public sealed class GenericInterface2<TType1, TType2> : IGenericInterface2<TType1, TType2>
    where TType1 : class where TType2 : class
{
    public Type ItemType => typeof(TType1);

    public Type OtherType => typeof(TType2);

    public TType2 Build(TType1 source)
    {
        throw new NotSupportedException();
    }
}
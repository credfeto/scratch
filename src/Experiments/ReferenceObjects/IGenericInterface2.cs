using System;

namespace Experiments.ReferenceObjects;

public interface IGenericInterface2<TType1, TType2>
    where TType1 : class
    where TType2 : class
{
    Type ItemType { get; }

    Type OtherType { get; }

    TType2 Build(TType1 source);
}
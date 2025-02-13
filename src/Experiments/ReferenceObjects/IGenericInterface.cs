using System;

namespace Experiments.ReferenceObjects;

public interface IGenericInterface<T>
    where T : class
{
    Type ItemType { get; }

    T Build();
}

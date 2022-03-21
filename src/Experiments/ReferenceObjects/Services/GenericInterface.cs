using System;

namespace Experiments.ReferenceObjects.Services;

public sealed class GenericInterface<T> : IGenericInterface<T>
    where T : class
{
    public Type ItemType => typeof(T);

    public T Build()
    {
        throw new NotSupportedException();
    }
}
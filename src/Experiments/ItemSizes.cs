using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class ItemSizes : TestBase
{
    private readonly ITestOutputHelper _output;

    public ItemSizes(ITestOutputHelper output)
    {
        this._output = output;
    }

    private static int GetSize<T>(T item)
    {
        if (typeof(T).IsValueType && typeof(T).IsPrimitive)
        {
            return Marshal.SizeOf(item);
        }

        return GetSizeInt(item);
    }

    private static int GetSizeInt<T>([NotNull] T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        RuntimeTypeHandle th = item.GetType()
                                   .TypeHandle;

        return Marshal.ReadInt32(ptr: th.Value, ofs: 4);
    }

    private void OutputSize<T>(T item)
    {
        this._output.WriteLine($"{typeof(T).FullName}: {GetSize(item)}");
    }

    [Fact]
    public void ValueTypes()
    {
        this.OutputSize(default(int));
        this.OutputSize(default(double));
        this.OutputSize(default(byte));
        this.OutputSize(new TestValueTypeWithOnlyValueTypes(i: 1, j: 5));
        this.OutputSize(new TestValueTypeWithOnlyMixedValueTypesAndReferenceTypes(i: 44, j: "Hello"));

        Assert.True(condition: true, userMessage: "Not really a test");
    }

    [Fact]
    public void Objects()
    {
        this.OutputSize(string.Empty);
        this.OutputSize("Hello World");
        this.OutputSize("Hello World sdfshfhdfkdsjfhdjfhdjksh fdhsjfhjkfhdsjhf kshfkdsjhf kjhfjdkshfjdhfs shfskhfsdjkhk");
        this.OutputSize(new TestRefTypeWithOnlyValueTypes(i: 1, j: 5));
        this.OutputSize(new TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes(i: 44, j: "Hello"));
        this.OutputSize(new TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes2(i: 44, j: "Hello", new(i: 44, j: "Hello")));

        Assert.True(condition: true, userMessage: "Not really a test");
    }

    [StructLayout(LayoutKind.Auto)]
    private readonly struct TestValueTypeWithOnlyValueTypes
    {
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public int I { get; }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public long J { get; }

        public TestValueTypeWithOnlyValueTypes(int i, long j)
        {
            this.I = i;
            this.J = j;
        }
    }

    private sealed class TestRefTypeWithOnlyValueTypes
    {
        public TestRefTypeWithOnlyValueTypes(int i, long j)
        {
            this.I = i;
            this.J = j;
        }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public int I { get; }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public long J { get; }
    }

    private readonly struct TestValueTypeWithOnlyMixedValueTypesAndReferenceTypes
    {
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public int I { get; }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public string J { get; }

        public TestValueTypeWithOnlyMixedValueTypesAndReferenceTypes(int i, string j)
        {
            this.I = i;
            this.J = j;
        }
    }

    private sealed class TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes
    {
        public TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes(int i, string j)
        {
            this.I = i;
            this.J = j;
        }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public int I { get; }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public string J { get; }
    }

    private sealed class TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes2
    {
        public TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes2(int i, string j, TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes k)
        {
            this.I = i;
            this.J = j;
            this.K = k;
        }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public int I { get; }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public string J { get; }

        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "Required for test")]
        public TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes K { get; }
    }
}
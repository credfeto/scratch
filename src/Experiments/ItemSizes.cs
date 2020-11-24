using System;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace Experiments
{
    public sealed class ItemSizes
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
        }

        [Fact]
        public void Objects()
        {
            //this.OutputSize(default(object));
            this.OutputSize(string.Empty);
            this.OutputSize("Hello World");
            this.OutputSize("Hello World sdfshfhdfkdsjfhdjfhdjksh fdhsjfhjkfhdsjhf kshfkdsjhf kjhfjdkshfjdhfs shfskhfsdjkhk");
            this.OutputSize(new TestRefTypeWithOnlyValueTypes(i: 1, j: 5));
            this.OutputSize(new TestRefTypeWithOnlyMixedValueTypesAndReferenceTypes(i: 44, j: "Hello"));
        }

        private readonly struct TestValueTypeWithOnlyValueTypes
        {
            public int I { get; }

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

            public int I { get; }

            public long J { get; }
        }

        private readonly struct TestValueTypeWithOnlyMixedValueTypesAndReferenceTypes
        {
            public int I { get; }

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

            public int I { get; }

            public string J { get; }
        }
    }
}
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Experiments
{
    public sealed class MakeMatrix
    {
        private readonly ITestOutputHelper _output;

        public MakeMatrix(ITestOutputHelper output)
        {
            this._output = output ?? throw new ArgumentNullException(nameof(output));
        }

        private static CompatibilityCheck<T>[,] Matrix<T>(IReadOnlyList<T> make)
        {
            CompatibilityCheck<T>[,] x = new CompatibilityCheck<T>[make.Count, make.Count - 1];

            for (int i = 0; i < make.Count; ++i)
            {
                int ypos = 0;

                for (int j = 0; j < make.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    x[i, ypos] = new CompatibilityCheck<T>(make[i], make[j]);
                    ++ypos;
                }
            }

            return x;
        }

        [Fact]
        public void Produce()
        {
            int[] source = {1, 2, 3, 4};

            CompatibilityCheck<int>[,] m = Matrix(source);

            foreach (CompatibilityCheck<int> x in m)
            {
                this._output.WriteLine($"{x.From},{x.To}");
            }
        }

        private sealed class CompatibilityCheck<T>
        {
            public CompatibilityCheck(T from, T to)
            {
                this.From = from;
                this.To = to;
            }

            public T From { get; }

            public T To { get; }

            public bool? Compatibile { get; set; }
        }
    }
}
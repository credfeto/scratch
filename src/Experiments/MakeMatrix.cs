using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FunFair.Test.Common;
using Xunit;

namespace Experiments;

public sealed class MakeMatrix : TestBase
{
    private readonly ITestOutputHelper _output;

    public MakeMatrix(ITestOutputHelper output)
    {
        this._output = output ?? throw new ArgumentNullException(nameof(output));
    }

    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1814: Use Jagged Array", Justification = "By Design")]
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

                x[i, ypos] = new(make[i], make[j]);
                ++ypos;
            }
        }

        return x;
    }

    [Fact]
    public void Produce()
    {
        int[] source = [1, 2, 3, 4];

        CompatibilityCheck<int>[,] m = Matrix(source);

        foreach (CompatibilityCheck<int> x in m)
        {
            this._output.WriteLine($"{x.From},{x.To}");
        }

        Assert.True(condition: true, userMessage: "Not really a test");
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

        [SuppressMessage(
            category: "SonarAnalyzer.CSharp",
            checkId: "S1144: Unused private property",
            Justification = "For testing purposes"
        )]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Local", Justification = "Required for test")]
        public bool? Compatibile { get; set; }
    }
}

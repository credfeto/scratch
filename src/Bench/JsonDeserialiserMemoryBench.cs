using System;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace Bench;

[SimpleJob]
[MemoryDiagnoser(false)]
public abstract class JsonDeserialiserMemoryBench : BenchBase
{
    private const string GOOD = "{ \"foo\": \"bar\" }";
    private static readonly byte[] GoodBytes = Encoding.UTF8.GetBytes(GOOD);

    [Benchmark]
    public void BytesToStringToDoc()
    {
        string s = Encoding.UTF8.GetString(GoodBytes);

        using (JsonDocument doc = JsonDocument.Parse(s))
        {
            this.Test(doc);
        }
    }

    [Benchmark]
    public void BytesToSpanToDoc()
    {
        ReadOnlySpan<byte> span = GoodBytes;
        this.ToJsonFromSpan(span);
    }

    [Benchmark]
    public void BytesToReadOnlyMemorySpanToDoc()
    {
        ReadOnlyMemory<byte> readOnlyMemory = new(GoodBytes);
        this.ToJsonFromSpan(readOnlyMemory.Span);
    }

    private void ToJsonFromSpan(in ReadOnlySpan<byte> span)
    {
        Utf8JsonReader reader = new(span);

        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            this.Test(doc);
        }
    }
}
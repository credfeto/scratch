using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Bench;

[SimpleJob]
[MemoryDiagnoser(false)]
public abstract partial class RegexBench : BenchBase
{
    private const string GOOD = "0123456789abcdef";
    private const string BAD = "0123456789abcdefg";

    private static readonly Regex CompiledRegex = new(pattern: @"^[0-9a-fA-F]+$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

    [GeneratedRegex(pattern: @"^[0-9a-fA-F]+$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline)]
    private static partial Regex SourceGeneratedRegex();

    [Benchmark]
    public void GoodNormalRegexString()
    {
        this.Test(ShouldBeNormalHexRegexString(GOOD));
    }

    [Benchmark]
    public void GoodNormalRegexSpan()
    {
        this.Test(ShouldBeNormalHexRegexSpan(GOOD));
    }

    [Benchmark]
    public void GoodSourceGeneratedRegexString()
    {
        this.Test(ShouldBeHexSourceGeneratedRegexString(GOOD));
    }

    [Benchmark]
    public void GoodSourceGeneratedRegexSpan()
    {
        this.Test(ShouldBeHexSourceGeneratedRegexSpan(GOOD));
    }

    [Benchmark]
    public void GoodMethodSpan()
    {
        this.Test(ShouldBeHexMethodSpan(GOOD));
    }

    [Benchmark]
    public void GoodMethod7Span()
    {
        this.Test(ShouldBeHexMethodSpan7(GOOD));
    }

    [Benchmark]
    public void BadSourceGeneratedRegexString()
    {
        this.Test(ShouldBeHexSourceGeneratedRegexString(BAD));
    }

    [Benchmark]
    public void BadNormalRegexString()
    {
        this.Test(ShouldBeNormalHexRegexString(BAD));
    }

    [Benchmark]
    public void BadRegexSpan()
    {
        this.Test(ShouldBeNormalHexRegexSpan(BAD));
    }

    [Benchmark]
    public void BadSourceGeneratedRegexSpan()
    {
        this.Test(ShouldBeHexSourceGeneratedRegexSpan(BAD));
    }

    [Benchmark]
    public void BadMethodSpan()
    {
        this.Test(ShouldBeHexMethodSpan(BAD));
    }

    [Benchmark]
    public void BadMethodSpan7()
    {
        this.Test(ShouldBeHexMethodSpan7(BAD));
    }

    private static bool ShouldBeNormalHexRegexString(string input)
    {
        return CompiledRegex.IsMatch(input);
    }

    private static bool ShouldBeNormalHexRegexSpan(string input)
    {
        ReadOnlySpan<char> span = input;

        return CompiledRegex.IsMatch(span);
    }

    private static bool ShouldBeHexSourceGeneratedRegexString(string input)
    {
        return SourceGeneratedRegex()
            .IsMatch(input);
    }

    private static bool ShouldBeHexSourceGeneratedRegexSpan(string input)
    {
        ReadOnlySpan<char> span = input;

        return SourceGeneratedRegex()
            .IsMatch(span);
    }

    private static bool ShouldBeHexMethodSpan(string input)
    {
        return IsValidHexStringWithoutPrefix(input);
    }

    private static bool ShouldBeHexMethodSpan7(string input)
    {
        return IsValidHexStringWithoutPrefix7(input);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidHexStringWithoutPrefix(in ReadOnlySpan<char> value)
    {
        foreach (char c in value)
        {
            if (!IsHex(c))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidHexStringWithoutPrefix7(in ReadOnlySpan<char> value)
    {
        foreach (char c in value)
        {
            if (!char.IsAsciiHexDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    private static bool IsHex(in char c)
    {
        return IsNumber(c) || IsHexLetter(c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    private static bool IsNumber(in char c)
    {
        return c is >= '0' and <= '9';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    private static bool IsHexLetter(in char c)
    {
        return IsLowerCaseHexLetter(c) || IsUpperCaseHexLetter(c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    private static bool IsLowerCaseHexLetter(in char c)
    {
        return c is >= 'a' and <= 'f';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Pure]
    private static bool IsUpperCaseHexLetter(in char c)
    {
        return c is >= 'A' and <= 'F';
    }
}
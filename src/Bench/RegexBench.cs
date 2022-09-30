using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Bench;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1822:Mark methods static", Justification = "Needed for BenchmarkDotNet")]
[SimpleJob]
[MemoryDiagnoser(false)]
public partial class RegexBench
{
    private const string GOOD = "0123456789abcdef";
    private const string BAD = "0123456789abcdefg";

    private static readonly Regex CompiledRegex = new(pattern: @"^[0-9a-fA-F]+$",
                                                      RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline,
                                                      TimeSpan.FromSeconds(1));

    [GeneratedRegexAttribute(pattern: @"^[0-9a-fA-F]+$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline)]
    private static partial Regex SourceGeneratedRegex();

    [Benchmark]
    public bool GoodNormalRegexString()
    {
        return ShouldBeNormalHexRegexString(GOOD);
    }

    [Benchmark]
    public bool GoodNormalRegexSpan()
    {
        return ShouldBeNormalHexRegexSpan(GOOD);
    }

    [Benchmark]
    public bool GoodSourceGeneratedRegexString()
    {
        return ShouldBeHexSourceGeneratedRegexString(GOOD);
    }

    [Benchmark]
    public bool GoodSourceGeneratedRegexSpan()
    {
        return ShouldBeHexSourceGeneratedRegexSpan(GOOD);
    }

    [Benchmark]
    public bool GoodMethodSpan()
    {
        return ShouldBeHexMethodSpan(GOOD);
    }

    [Benchmark]
    public bool GoodMethod7Span()
    {
        return ShouldBeHexMethodSpan7(GOOD);
    }

    [Benchmark]
    public bool BadSourceGeneratedRegexString()
    {
        return ShouldBeHexSourceGeneratedRegexString(BAD);
    }

    [Benchmark]
    public bool BadNormalRegexString()
    {
        return ShouldBeNormalHexRegexString(BAD);
    }

    [Benchmark]
    public bool BadRegexSpan()
    {
        return ShouldBeNormalHexRegexSpan(BAD);
    }

    [Benchmark]
    public bool BadSourceGeneratedRegexSpan()
    {
        return ShouldBeHexSourceGeneratedRegexSpan(BAD);
    }

    [Benchmark]
    public bool BadMethodSpan()
    {
        return ShouldBeHexMethodSpan(BAD);
    }

    [Benchmark]
    public bool BadMethodSpan7()
    {
        return ShouldBeHexMethodSpan7(BAD);
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
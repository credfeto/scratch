using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Bench;

[MemoryDiagnoser]
public static class Program
{
    private static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                         .Run(args);
    }
}

public class RegexBench
{
    private const string GOOD = "0123456789abcdef";
    private const string BAD = "0123456789abcdefg";

    private static readonly Regex Regex = new(pattern: @"^[0-9a-fA-F]+$",
                                              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline,
                                              TimeSpan.FromSeconds(1));

    [Benchmark]
    public bool GoodRegexString()
    {
        return ShouldBeHexRegexString(GOOD);
    }

    [Benchmark]
    public bool GoodRegexSpan()
    {
        return ShouldBeHexRegexSpan(GOOD);
    }

    [Benchmark]
    public bool GoodMethodSpan()
    {
        return ShouldBeHexMethodSpan(GOOD);
    }

    [Benchmark]
    public bool BadRegexString()
    {
        return ShouldBeHexRegexString(BAD);
    }

    [Benchmark]
    public bool BadRegexSpan()
    {
        return ShouldBeHexRegexSpan(BAD);
    }

    [Benchmark]
    public bool BadMethodSpan()
    {
        return ShouldBeHexMethodSpan(BAD);
    }

    private static bool ShouldBeHexRegexString(string input)
    {
        bool actual = Regex.IsMatch(input);

        return actual;
    }

    private static bool ShouldBeHexRegexSpan(string input)
    {
        ReadOnlySpan<char> span = input;

        return Regex.IsMatch(span);
    }

    private static bool ShouldBeHexMethodSpan(string input)
    {
        return IsValidHexStringWithoutPrefix(input);
    }

    /// <summary>
    ///     Checks to see if the string is a valid hex string;
    /// </summary>
    /// <param name="value">the string to check.</param>
    /// <returns>True, if the string is all hex characters; otherwise, false.</returns>
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
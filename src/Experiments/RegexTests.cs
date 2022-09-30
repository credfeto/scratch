using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FunFair.Test.Common;
using Xunit;

namespace Experiments;

public sealed class RegexTests : TestBase
{
    private static readonly Regex Regex = new(pattern: @"^[0-9a-fA-F]+$",
                                              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline,
                                              TimeSpan.FromSeconds(1));

    [Theory]
    [InlineData("1234567890abcdefABCDEF", true)]
    public void ShouldBeHexRegexString(string input, bool expected)
    {
        bool actual = Regex.IsMatch(input);

        Assert.Equal(expected: expected, actual: actual);
    }

    [Theory]
    [InlineData("1234567890abcdefABCDEF", true)]
    public void ShouldBeHexRegexSpan(string input, bool expected)
    {
        ReadOnlySpan<char> span = input;
        bool actual = Regex.IsMatch(span);

        Assert.Equal(expected: expected, actual: actual);
    }

    [Theory]
    [InlineData("1234567890abcdefABCDEF", true)]
    public void ShouldBeHexMethodSpan(string input, bool expected)
    {
        bool actual = IsValidHexStringWithoutPrefix(input);

        Assert.Equal(expected: expected, actual: actual);
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
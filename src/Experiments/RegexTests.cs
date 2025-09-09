using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FunFair.Test.Common;
using Xunit;

namespace Experiments;

public sealed class RegexTests : TestBase
{
    private const string REGEX_PATTERN = "^[0-9a-fA-F]+$";

    private const RegexOptions OPTIONS = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline;

    [SuppressMessage(category: "Meziantou.Analyzers", checkId: "MA0110: Use regex source generator", Justification = "cannot be for a test case")]
    private static readonly Regex Regex = new(pattern: REGEX_PATTERN, options: OPTIONS, TimeSpan.FromSeconds(1));

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

    [SuppressMessage(category: "Meziantou.Analyzers", checkId: "MA0110: Use regex source generator", Justification = "cannot be for a test case")]
    [Theory]
    [InlineData("@rinukam143 Rotom registered in Pokédex: ✔ 👾 @rinukam143 Rotom (Mow) registered in Alt. Dex: ❌", true)]
    [InlineData("@rinukam143 Rotom registered in Pokédex: ❌ 👾 @rinukam143 Rotom (Mow) registered in Alt. Dex: ✔", true)]
    [InlineData("@rinukam143 Rotom registered in Pokédex: ❌ 👾 @rinukam143 Rotom (Mow) registered in Alt. Dex: ❌", true)]
    [InlineData("@rinukam143 Rotom registered in Pokédex: ✔ 👾 @rinukam143 Rotom (Mow) registered in Alt. Dex: ✔", false)]
    [InlineData("@rinukam143 Deino registered in Pokédex: ❌", true)]
    [InlineData("I'm a banana", false)]
    public void Match(string source, bool matched)
    {
        const string pattern = "^@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Pokédex:\\s+❌$|@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Pokédex:\\s+[✔❌]\\s+👾\\s+@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Alt.\\s+Dex:\\s+❌$|@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Pokédex:\\s+❌\\s+👾\\s+@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Alt.\\s+Dex:\\s+[✔❌]$"
            ;

/*
 * {
     "Streamer": "sleepypan",
     "Bot": "PokemonCommunityGame",
     "MatchType": "REGEX",
     "Match": "^@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Pokédex:\\s+❌$",
              "^@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Pokédex:\\s+❌$|@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Pokédex:\\s+[✔❌]\\s+👾\\s+@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Alt.\\s+Dex:\\s+❌$|@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Pokédex:\\s+❌\\s+👾\\s+@rinukam143\\s+(.+)\\s+registered\\s+in\\s+Alt.\\s+Dex:\\s+[✔❌]$"
     "Issue": "!pokecatch"
   },

 */
        Regex test = new(pattern: pattern, options: OPTIONS, TimeSpan.FromSeconds(1));

        bool isMatch = test.IsMatch(source);
        Assert.Equal(expected: matched, actual: isMatch);
    }
}
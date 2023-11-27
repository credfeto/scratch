using System;
using System.Text;
using System.Text.RegularExpressions;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class ReleaseNoteFormatting : TestBase
{
    private const string SIMPLE = @"### Added
- Some Stuff
- Some Other Stuff
### Changed
- FF-1324 - some text
- ff-1244 - some more text
";

    private static readonly TimeSpan TimeOut = TimeSpan.FromSeconds(5);

    private readonly ITestOutputHelper _output;

    public ReleaseNoteFormatting(ITestOutputHelper output)
    {
        this._output = output;
    }

    private static string Bold(string value)
    {
        return "**" + value + "**";
    }

    private static string Italic(string value)
    {
        return "*" + value + "*";
    }

    private static string Underline(string value)
    {
        return "__" + value + "__";
    }

    [Fact]
    public void Convert()
    {
        this._output.WriteLine(SIMPLE);

        this._output.WriteLine(message: "****************************************************");
        StringBuilder builder = new();
        string[] text = SIMPLE.Split(Environment.NewLine);

        foreach (string line in text)
        {
            if (line.StartsWith(value: "### ", comparisonType: StringComparison.Ordinal))
            {
                string replacement = Bold(Underline(line.Substring(startIndex: 4)
                                                        .Trim()));
                builder.AppendLine(replacement);

                continue;
            }

            builder.AppendLine(Regex.Replace(input: line,
                                             pattern: "(ff\\-\\d+)",
                                             evaluator: MakeUpperCase,
                                             RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
                                             matchTimeout: TimeOut)
                                    .Trim());
        }

        this._output.WriteLine(builder.ToString());

        Assert.True(condition: true, userMessage: "Not really a test");

        static string MakeUpperCase(Match match)
        {
            return Italic(match.ToString());
        }
    }
}
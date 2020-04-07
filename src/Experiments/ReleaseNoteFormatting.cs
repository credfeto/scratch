using System;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Experiments
{
    public class ReleaseNoteFormatting
    {
        public ReleaseNoteFormatting(ITestOutputHelper output)
        {
            this._output = output;
        }

        private readonly ITestOutputHelper _output;

        private const string SIMPLE = @"### Added
- Some Stuff
- Some Other Stuff
### Changed
- FF-1324 - some text
- ff-1244 - some more text
";

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

            static string MakeUpperCase(Match match)
            {
                return Italic(match.ToString());
            }

            this._output.WriteLine(message: "****************************************************");
            StringBuilder builder = new StringBuilder();
            string[] text = SIMPLE.Split(Environment.NewLine);

            foreach (string line in text)
            {
                if (line.StartsWith(value: "### "))
                {
                    string replacement = Bold(Underline(line.Substring(startIndex: 4)
                                                            .Trim()));
                    builder.AppendLine(replacement);

                    continue;
                }

                builder.AppendLine(Regex.Replace(line, pattern: "(ff\\-\\d+)", MakeUpperCase, RegexOptions.IgnoreCase)
                                        .Trim());
            }

            this._output.WriteLine(builder.ToString());
        }
    }
}
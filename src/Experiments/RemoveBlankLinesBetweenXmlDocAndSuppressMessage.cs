using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using FunFair.Test.Common;
using Xunit;

namespace Experiments;

public sealed class RemoveBlankLinesBetweenXmlDocAndSuppressMessage : TestBase
{
    private const string EXAMPLE =
        @"
        public CasinoMetadataDto? Metadata { get; set; }

        
        public WalletAppId WalletAppId { get; set; } = default!;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(category: ""ReSharper"", checkId: ""AutoPropertyCanBeMadeGetOnly.Global"", Justification = ""TODO: Review"")]
        public WalletAppId OtherWalletAppId { get; set; } = default!;
";

    private readonly ITestOutputHelper _output;

    public RemoveBlankLinesBetweenXmlDocAndSuppressMessage(ITestOutputHelper output)
    {
        this._output = output ?? throw new ArgumentNullException(nameof(output));
    }

    [Fact]
    [SuppressMessage(
        category: "Meziantou.Analyzers",
        checkId: "MA0110: Use regex source generator",
        Justification = "cannot be for a test case"
    )]
    public void Detect()
    {
        this._output.WriteLine("-------------------------");

        const string pattern =
            @"^((\s+)///\s</(.*?)\>((\r|\n|\r\n)?))(?<LinesToRemove>(\r|\n|\r\n){1,})(\s+\[(System\.Diagnostics\.CodeAnalysis\.)?SuppressMessage)";

        Regex regex = new(
            pattern: pattern,
            RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ExplicitCapture,
            TimeSpan.FromSeconds(2)
        );

        MatchCollection matches = regex.Matches(EXAMPLE);

        this._output.WriteLine("Matches:");

        int matchNumber = 0;

        foreach (Match match in matches)
        {
            this._output.WriteLine($"---------------- {matchNumber++} ----------------");
            this._output.WriteLine(match.ToString());
        }

        this._output.WriteLine("---------------- REPLACE ----------------");
        this._output.WriteLine(regex.Replace(input: EXAMPLE, replacement: "$1$7"));

        Assert.True(condition: true, userMessage: "Not really a test");
    }
}

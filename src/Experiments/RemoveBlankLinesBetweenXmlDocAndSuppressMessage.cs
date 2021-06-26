using System;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Experiments
{
    public sealed class RemoveBlankLinesBetweenXmlDocAndSuppressMessage
    {
        private const string EXAMPLE = @"
        /// <summary>
        ///     The casino metadata
        /// </summary>
        public CasinoMetadataDto? Metadata { get; set; }

        /// <summary>
        ///     The Wallet App ID.
        /// </summary>
        [SuppressMessage(category: ""ReSharper"", checkId: ""AutoPropertyCanBeMadeGetOnly.Global"", Justification = ""TODO: Review"")]
        public WalletAppId WalletAppId { get; set; } = default!;

        /// <summary>
        ///     The Other Wallet App ID.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(category: ""ReSharper"", checkId: ""AutoPropertyCanBeMadeGetOnly.Global"", Justification = ""TODO: Review"")]
        public WalletAppId OtherWalletAppId { get; set; } = default!;
";

        private readonly ITestOutputHelper _output;

        public RemoveBlankLinesBetweenXmlDocAndSuppressMessage(ITestOutputHelper output)
        {
            this._output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact]
        public void Detect()
        {
            this._output.WriteLine("-------------------------");

            const string pattern = @"^((\s+)///\s</(.*?)\>((\r|\n|\r\n)?))(?<LinesToRemove>(\r|\n|\r\n){1,})(\s+\[(System\.Diagnostics\.CodeAnalysis\.)?SuppressMessage)";

            Regex regex = new(pattern: pattern, RegexOptions.Multiline | RegexOptions.Compiled);

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
        }
    }
}
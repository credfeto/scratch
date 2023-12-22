using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class BuildGroupMetadata : TestBase
{
    private readonly ITestOutputHelper _output;

    public BuildGroupMetadata(ITestOutputHelper output)
    {
        this._output = output;
    }

    private void BuildGroups(int groupCount, IReadOnlyList<string> tests)
    {
        string[] groupNames = BuildGroupNames(groupCount);

        int group = 0;

        foreach (string item in tests)
        {
            this._output.WriteLine($"public const string {item} = @\"{groupNames[group % groupNames.Length]}\";");
            ++group;
        }
    }

    private static string[] BuildGroupNames(int groupCount)
    {
        return Enumerable.Range(start: 1, count: groupCount)
                         .Select(n => $"Group {n:x2}")
                         .ToArray();
    }

    [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0051: Method is too long", Justification = "Code generation")]
    [Fact]
    public void Generate()
    {
        string[] tests =
        [
            "Affiliation0PercentValidRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation0PercentValidRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "Affiliation0PercentValidRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation0PercentValidRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation0PercentValidRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation0PercentValidRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation0PercentValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "Affiliation0PercentValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "Affiliation0PercentValidRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "Affiliation0PercentValidRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "Affiliation0PercentValidRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "Affiliation0PercentValidRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "Affiliation0PercentValidRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "Affiliation0PercentValidRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "Affiliation0PercentValidRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "Affiliation0PercentValidRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "Affiliation0PercentValidRtpNoneOrderly0Tests",
            "Affiliation0PercentValidRtpNoneOrderly1Tests",
            "Affiliation0PercentValidRtpNoneOrderly2Tests",
            "Affiliation100PercentTooHighRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentTooHighRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "Affiliation100PercentTooHighRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentTooHighRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation100PercentTooHighRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentTooHighRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation100PercentTooHighRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "Affiliation100PercentTooHighRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "Affiliation100PercentTooHighRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "Affiliation100PercentTooHighRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "Affiliation100PercentTooHighRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "Affiliation100PercentTooHighRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "Affiliation100PercentTooHighRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "Affiliation100PercentTooHighRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "Affiliation100PercentTooHighRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "Affiliation100PercentTooHighRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "Affiliation100PercentTooHighRtpNoneOrderly0Tests",
            "Affiliation100PercentTooHighRtpNoneOrderly1Tests",
            "Affiliation100PercentTooHighRtpNoneOrderly2Tests",
            "Affiliation100PercentTooLowRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentTooLowRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "Affiliation100PercentTooLowRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentTooLowRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation100PercentTooLowRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentTooLowRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation100PercentTooLowRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "Affiliation100PercentTooLowRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "Affiliation100PercentTooLowRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "Affiliation100PercentTooLowRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "Affiliation100PercentTooLowRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "Affiliation100PercentTooLowRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "Affiliation100PercentTooLowRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "Affiliation100PercentTooLowRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "Affiliation100PercentTooLowRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "Affiliation100PercentTooLowRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "Affiliation100PercentTooLowRtpNoneOrderly0Tests",
            "Affiliation100PercentTooLowRtpNoneOrderly1Tests",
            "Affiliation100PercentTooLowRtpNoneOrderly2Tests",
            "Affiliation100PercentValidRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentValidRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "Affiliation100PercentValidRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentValidRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation100PercentValidRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation100PercentValidRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation100PercentValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "Affiliation100PercentValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "Affiliation100PercentValidRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "Affiliation100PercentValidRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "Affiliation100PercentValidRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "Affiliation100PercentValidRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "Affiliation100PercentValidRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "Affiliation100PercentValidRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "Affiliation100PercentValidRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "Affiliation100PercentValidRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "Affiliation100PercentValidRtpNoneOrderly0Tests",
            "Affiliation100PercentValidRtpNoneOrderly1Tests",
            "Affiliation100PercentValidRtpNoneOrderly2Tests",
            "Affiliation20PercentValidRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation20PercentValidRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "Affiliation20PercentValidRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation20PercentValidRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation20PercentValidRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation20PercentValidRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation20PercentValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "Affiliation20PercentValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "Affiliation20PercentValidRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "Affiliation20PercentValidRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "Affiliation20PercentValidRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "Affiliation20PercentValidRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "Affiliation20PercentValidRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "Affiliation20PercentValidRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "Affiliation20PercentValidRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "Affiliation20PercentValidRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "Affiliation20PercentValidRtpNoneOrderly0Tests",
            "Affiliation20PercentValidRtpNoneOrderly1Tests",
            "Affiliation20PercentValidRtpNoneOrderly2Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "Affiliation20PercentValidRtpPlayerWinsAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "Affiliation20PercentValidRtpPlayerWinsBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "Affiliation20PercentValidRtpPlayerWinsBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "Affiliation20PercentValidRtpPlayerWinsBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "Affiliation20PercentValidRtpPlayerWinsInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "Affiliation20PercentValidRtpPlayerWinsInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "Affiliation20PercentValidRtpPlayerWinsInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "Affiliation20PercentValidRtpPlayerWinsNoneOrderly0Tests",
            "Affiliation20PercentValidRtpPlayerWinsNoneOrderly1Tests",
            "Affiliation20PercentValidRtpPlayerWinsNoneOrderly2Tests",
            "AffiliationNoRecordExistsValidRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "AffiliationNoRecordExistsValidRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "AffiliationNoRecordExistsValidRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "AffiliationNoRecordExistsValidRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "AffiliationNoRecordExistsValidRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "AffiliationNoRecordExistsValidRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "AffiliationNoRecordExistsValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "AffiliationNoRecordExistsValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "AffiliationNoRecordExistsValidRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "AffiliationNoRecordExistsValidRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "AffiliationNoRecordExistsValidRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "AffiliationNoRecordExistsValidRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "AffiliationNoRecordExistsValidRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "AffiliationNoRecordExistsValidRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "AffiliationNoRecordExistsValidRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "AffiliationNoRecordExistsValidRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "AffiliationNoRecordExistsValidRtpNoneOrderly0Tests",
            "AffiliationNoRecordExistsValidRtpNoneOrderly1Tests",
            "AffiliationNoRecordExistsValidRtpNoneOrderly2Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpNoneOrderly0Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpNoneOrderly1Tests",
            "AffiliationRecordExistsButNotAffiliatedValidRtpNoneOrderly2Tests",
            "NoGameOwnerPayoutValidRtpAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "NoGameOwnerPayoutValidRtpAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "NoGameOwnerPayoutValidRtpAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "NoGameOwnerPayoutValidRtpAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "NoGameOwnerPayoutValidRtpAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "NoGameOwnerPayoutValidRtpAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "NoGameOwnerPayoutValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "NoGameOwnerPayoutValidRtpAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "NoGameOwnerPayoutValidRtpAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "NoGameOwnerPayoutValidRtpAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "NoGameOwnerPayoutValidRtpBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "NoGameOwnerPayoutValidRtpBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "NoGameOwnerPayoutValidRtpBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "NoGameOwnerPayoutValidRtpInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "NoGameOwnerPayoutValidRtpInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "NoGameOwnerPayoutValidRtpInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "NoGameOwnerPayoutValidRtpNoneOrderly0Tests",
            "NoGameOwnerPayoutValidRtpNoneOrderly1Tests",
            "NoGameOwnerPayoutValidRtpNoneOrderly2Tests",
            "ReferenceGame1CoinFlipAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "ReferenceGame1CoinFlipAfterConfirmingContinueRoundCallPlayerRaisesTimeoutEndOfRound1Tests",
            "ReferenceGame1CoinFlipAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "ReferenceGame1CoinFlipAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "ReferenceGame1CoinFlipAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "ReferenceGame1CoinFlipAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "ReferenceGame1CoinFlipAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "ReferenceGame1CoinFlipAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "ReferenceGame1CoinFlipAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "ReferenceGame1CoinFlipAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "ReferenceGame1CoinFlipBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "ReferenceGame1CoinFlipBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "ReferenceGame1CoinFlipBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "ReferenceGame1CoinFlipInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "ReferenceGame1CoinFlipInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "ReferenceGame1CoinFlipInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "ReferenceGame1CoinFlipNoneOrderly0Tests",
            "ReferenceGame1CoinFlipNoneOrderly1Tests",
            "ReferenceGame1CoinFlipNoneOrderly2Tests",
            "ReferenceGame2CoinFlipMultiAfterConfirmingContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "ReferenceGame2CoinFlipMultiAfterConfirmingContinueRoundCallPlayerRaisesTimeoutMidRoundActionForNewState1Tests",
            "ReferenceGame2CoinFlipMultiAfterConfirmingStartRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "ReferenceGame2CoinFlipMultiAfterConfirmingStartRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "ReferenceGame2CoinFlipMultiAfterContinueRoundCallHouseRaisesTimeoutMidRoundPlayerIgnores1Tests",
            "ReferenceGame2CoinFlipMultiAfterContinueRoundCallPlayerRaisesTimeoutMidRound1Tests",
            "ReferenceGame2CoinFlipMultiAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerContinues2Tests",
            "ReferenceGame2CoinFlipMultiAfterProposeBetHouseRaisesEndOfRoundTimeoutPlayerIgnores2Tests",
            "ReferenceGame2CoinFlipMultiAfterProposeBetPlayerRaisesMidRoundTimeoutAfterProposeBet1Tests",
            "ReferenceGame2CoinFlipMultiAfterStartRoundPlayerRaisesTimeoutMidRoundOnIncompleteStartRound1Tests",
            "ReferenceGame2CoinFlipMultiBetweenGamesHouseRaisesTimeoutEndOfRoundPlayerSettles2Tests",
            "ReferenceGame2CoinFlipMultiBetweenGamesPlayerRaisesTimeoutEndOfRound2Tests",
            "ReferenceGame2CoinFlipMultiBetweenGamesSendOldStatePlayerRaisesTimeoutEndOfRoundWithOldState2Tests",
            "ReferenceGame2CoinFlipMultiInsteadOfConfirmingInitialStateHouseRaisesTimeoutOnInitialStatePlayerSettles0Tests",
            "ReferenceGame2CoinFlipMultiInsteadOfConfirmingInitialStatePlayerRaisesTimeoutOnInitialState0Tests",
            "ReferenceGame2CoinFlipMultiInsteadOfConfirmingRoundOverHouseRaisesTimeoutMidRoundPlayerSettles1Tests",
            "ReferenceGame2CoinFlipMultiNoneOrderly0Tests",
            "ReferenceGame2CoinFlipMultiNoneOrderly1Tests",
            "ReferenceGame2CoinFlipMultiNoneOrderly2Tests"
        ];

        this.BuildGroups(groupCount: 22, tests: tests);

        Assert.True(condition: true, userMessage: "Not really a test");
    }
}
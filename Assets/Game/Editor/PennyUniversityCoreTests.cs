#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NUnit.Framework;
using PennyUniversity;

public class PennyUniversityCoreTests
{
    [Test]
    public void Archive_IsTrueCostsMoneyAndStopsAfterFourVisits()
    {
        var state = new PennyGameState(40, TruthScenario.JustResistance, null);
        for (int i = 0; i < 4; i++)
        {
            Assert.IsTrue(state.TryReadArchive(new ArchiveRecord {
                Id = "r" + i, Title = "Record " + i, Content = "Verified entry"
            }, 10, out EvidenceEntry fact));
            Assert.IsTrue(fact.VerifiedFact);
        }
        Assert.AreEqual(0, state.Money);
        Assert.AreEqual(4, state.ArchiveVisits);
        Assert.IsFalse(state.TryReadArchive(new ArchiveRecord {
            Id = "r4", Title = "Last", Content = "No"
        }, 10, out _));
    }

    [Test]
    public void Debate_CanWinWithoutObjectiveTruth()
    {
        var choice = new DebateChoice { Text = "Logical rebuttal", Kind = RebuttalKind.Logic, PersuasionPoints = 3 };
        Assert.AreEqual(3, PennyDebateRules.ScoreChoice(choice, null));
        Assert.IsTrue(PennyDebateRules.PlayerWon(3, 2, false));
        Assert.IsFalse(PennyDebateRules.PlayerWon(3, 2, true));
    }

    [Test]
    public void Campaign_EndsAfterSevenDays()
    {
        var state = new PennyGameState(100, TruthScenario.PeacefulSevenDays, null);
        for (int i = 0; i < 7; i++) Assert.IsTrue(state.EndDay());
        Assert.IsTrue(state.Finished);
        Assert.IsFalse(state.EndDay());
    }

    [Test]
    public void Donation_RejectsInvalidTransactionsWithoutCharging()
    {
        var state = new PennyGameState(30, TruthScenario.ForeignShadow, null);
        Assert.IsFalse(state.TryDonate(Faction.Bystander, 10, 5));
        Assert.IsFalse(state.TryDonate(Faction.Royalist, 10, -1));
        Assert.AreEqual(30, state.Money);
    }
}
#endif
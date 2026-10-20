#if UNITY_EDITOR
using System.Collections.Generic;
using NUnit.Framework;
using PennyUniversity;

public class PennyNewspaperTests
{
    private static NewspaperReader MakeReader(PennyGameState state)
    {
        return new NewspaperReader(state, new [] {
            new NewspaperArticle {
                Id = "a1", NewspaperName = "The Crown", Headline = "Budget Approved",
                Body = "The assembly approved a budget.", PublicationDay = 1,
                Bias = NewspaperBias.Royalist
            },
            new NewspaperArticle {
                Id = "a2", NewspaperName = "The People's Voice", Headline = "New Protests",
                Body = "Protesters gathered.", PublicationDay = 2,
                Bias = NewspaperBias.Revolutionary
            }
        });
    }

    [Test]
    public void HeadlinesDoNotRevealBodyOrFutureArticles()
    {
        var state = new PennyGameState(100, TruthScenario.JustResistance, null);
        var reader = MakeReader(state);
        var headlines = reader.GetAvailableHeadlines();
        Assert.AreEqual(1, headlines.Count);
        Assert.AreEqual("Budget Approved", headlines[0].Headline);
        Assert.IsFalse(reader.TryRead("a2", out _));
    }

    [Test]
    public void ReadingAddsOneUnverifiedEntryEvenWhenReadTwice()
    {
        var state = new PennyGameState(100, TruthScenario.JustResistance, null);
        var reader = MakeReader(state);
        Assert.IsTrue(reader.TryRead("a1", out var body));
        Assert.AreEqual("The assembly approved a budget.", body);
        Assert.IsTrue(reader.TryRead("a1", out _));
        Assert.AreEqual(1, state.Evidence.Count);
        Assert.IsFalse(state.Evidence[0].VerifiedFact);
        Assert.AreEqual(EvidenceSource.Newspaper, state.Evidence[0].Source);
    }

    [Test]
    public void ArticlesUnlockAsDaysAdvance()
    {
        var state = new PennyGameState(100, TruthScenario.PeacefulSevenDays, null);
        var reader = MakeReader(state);
        Assert.IsTrue(state.EndDay());
        Assert.AreEqual(2, reader.GetAvailableHeadlines().Count);
        Assert.IsTrue(reader.TryRead("a2", out _));
    }
}
#endif
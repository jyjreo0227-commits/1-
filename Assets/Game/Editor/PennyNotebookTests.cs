#if UNITY_EDITOR
using NUnit.Framework;
using PennyUniversity;

public sealed class PennyNotebookTests
{
    [Test]
    public void FiltersSeparateClaimsFromVerifiedArchiveFacts()
    {
        var state = new PennyGameState(100, TruthScenario.ForeignShadow, null);
        state.RecordUnverifiedEvidence("news1", "Rumor", "A political claim", EvidenceSource.Newspaper);
        Assert.IsTrue(state.TryReadArchive(
            new ArchiveRecord { Id = "archive1", Title = "Ledger", Content = "Recorded fact" },
            10, out _));
        var notebook = new PennyNotebook(state);
        Assert.AreEqual(2, notebook.GetEntries(NotebookFilter.All).Count);
        Assert.AreEqual(1, notebook.GetEntries(NotebookFilter.VerifiedFacts).Count);
        Assert.AreEqual(1, notebook.GetEntries(NotebookFilter.UnverifiedClaims).Count);
        Assert.AreEqual("Ledger", notebook.GetEntries(NotebookFilter.VerifiedFacts)[0].Title);
    }

    [Test]
    public void HypothesesCanChangeButCannotAlterEvidence()
    {
        var state = new PennyGameState(100, TruthScenario.JustResistance, null);
        state.RecordUnverifiedEvidence("n", "Headline", "Article", EvidenceSource.Newspaper);
        var notebook = new PennyNotebook(state);
        Assert.IsTrue(notebook.TryAddHypothesis("The king is hiding something", out int id));
        Assert.IsTrue(notebook.TryEditHypothesis(id, "Maybe the king is innocent"));
        Assert.AreEqual("Maybe the king is innocent", notebook.Hypotheses[0].Text);
        var copy = notebook.GetEntries(NotebookFilter.All)[0];
        copy.Text = "Altered";
        Assert.AreEqual("Article", state.Evidence[0].Text);
        Assert.IsTrue(notebook.RemoveHypothesis(id));
        Assert.AreEqual(0, notebook.Hypotheses.Count);
    }

    [Test]
    public void EmptyHypothesesAreRejected()
    {
        var notebook = new PennyNotebook(new PennyGameState(0, TruthScenario.PeacefulSevenDays, null));
        Assert.IsFalse(notebook.TryAddHypothesis("  ", out _));
        Assert.AreEqual(0, notebook.Hypotheses.Count);
    }
}
#endif
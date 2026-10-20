#if UNITY_EDITOR
using NUnit.Framework;
using PennyUniversity;

public sealed class PennyArchiveCatalogTests
{
    [Test]
    public void TitlesHideContentUntilPayment()
    {
        var catalog = new PennyArchiveCatalog(new[] {
            new ArchiveRecord { Id = "finance", Title = "Royal Ledger", Content = "True revenue: 30" }
        });
        var title = catalog.GetTitles()[0];
        Assert.AreEqual("Royal Ledger", title.Title);
        Assert.AreEqual(0, title.GetType().GetProperties().Length - 2);
        var state = new PennyGameState(10, TruthScenario.AmbitiousRevolution, null);
        Assert.IsFalse(catalog.TryPurchase(state, "finance", 11, out _));
        Assert.AreEqual(10, state.Money);
        Assert.IsTrue(catalog.TryPurchase(state, "finance", 10, out var fact));
        Assert.IsTrue(fact.VerifiedFact);
        Assert.AreEqual("True revenue: 30", fact.Text);
        Assert.AreEqual(1, state.ArchiveVisits);
    }

    [Test]
    public void RepeatSelectionDoesNotChargeTwice()
    {
        var catalog = new PennyArchiveCatalog(new[] {
            new ArchiveRecord { Id = "a", Title = "A", Content = "Truth A" }
        });
        var state = new PennyGameState(20, TruthScenario.JustResistance, null);
        Assert.IsTrue(catalog.TryPurchase(state, "a", 10, out _));
        Assert.IsFalse(catalog.TryPurchase(state, "a", 10, out _));
        Assert.AreEqual(10, state.Money);
        Assert.AreEqual(1, state.ArchiveVisits);
    }
}
#endif
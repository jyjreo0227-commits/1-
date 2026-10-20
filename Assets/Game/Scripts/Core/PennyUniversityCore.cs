using System;
using System.Collections.Generic;

namespace PennyUniversity
{
    public enum TruthScenario { AmbitiousRevolution, JustResistance, ForeignShadow, PeacefulSevenDays }
    public enum Faction { Royalist, Revolutionary, Nobility, Prince, Constitutional, Restoration, Collaborator, Capitalist, Socialist, Bystander }
    public enum EvidenceSource { Newspaper, Debate, RoyalArchive }

    [Serializable]
    public sealed class EvidenceEntry
    {
        public string Id;
        public string Title;
        public string Text;
        public EvidenceSource Source;
        public bool VerifiedFact;
    }

    [Serializable]
    public sealed class ArchiveRecord
    {
        public string Id;
        public string Title;
        public string Content;
    }

    [Serializable]
    public sealed class PennyGameState
    {
        public const int CampaignLength = 7;
        public const int MaximumArchiveVisits = 4;

        public int CurrentDay { get; private set; } = 1;
        public int Money { get; private set; }
        public int ArchiveVisits { get; private set; }
        public TruthScenario Scenario { get; private set; }
        public bool Finished { get { return CurrentDay > CampaignLength; } }

        private readonly Dictionary<Faction, int> influence = new Dictionary<Faction, int>();
        private readonly HashSet<string> readArchiveIds = new HashSet<string>();
        private readonly List<EvidenceEntry> evidence = new List<EvidenceEntry>();

        public PennyGameState(int startingMoney, TruthScenario scenario, IDictionary<Faction, int> initialInfluence)
        {
            if (startingMoney < 0) throw new ArgumentOutOfRangeException(nameof(startingMoney));
            Money = startingMoney;
            Scenario = scenario;
            foreach (Faction faction in Enum.GetValues(typeof(Faction)))
                if (faction != Faction.Bystander) influence[faction] = 0;
            if (initialInfluence != null)
                foreach (var pair in initialInfluence)
                    if (pair.Key != Faction.Bystander) influence[pair.Key] = Math.Max(0, pair.Value);
        }

        public int GetInternalInfluence(Faction faction)
        {
            return influence.TryGetValue(faction, out int value) ? value : 0;
        }

        public bool TrySpend(int amount)
        {
            if (Finished || amount <= 0 || amount > Money) return false;
            Money -= amount;
            return true;
        }

        public bool TryDonate(Faction faction, int amount, int influenceGained)
        {
            if (faction == Faction.Bystander || influenceGained < 0 || !TrySpend(amount)) return false;
            AddInfluence(faction, influenceGained);
            return true;
        }

        public void AddInfluence(Faction faction, int delta)
        {
            if (Finished || faction == Faction.Bystander) return;
            influence[faction] = Math.Max(0, GetInternalInfluence(faction) + delta);
        }

        public bool TryReadArchive(ArchiveRecord record, int entranceFee, out EvidenceEntry fact)
        {
            fact = null;
            if (Finished || record == null || string.IsNullOrWhiteSpace(record.Id) ||
                string.IsNullOrWhiteSpace(record.Title) || string.IsNullOrWhiteSpace(record.Content) ||
                ArchiveVisits >= MaximumArchiveVisits || entranceFee <= 0 || readArchiveIds.Contains(record.Id))
                return false;
            if (!TrySpend(entranceFee)) return false;
            ArchiveVisits++;
            readArchiveIds.Add(record.Id);
            fact = new EvidenceEntry
            {
                Id = record.Id, Title = record.Title, Text = record.Content,
                Source = EvidenceSource.RoyalArchive, VerifiedFact = true
            };
            evidence.Add(fact);
            return true;
        }

        public void RecordUnverifiedEvidence(string id, string title, string text, EvidenceSource source)
        {
            if (source == EvidenceSource.RoyalArchive)
                throw new InvalidOperationException("Royal archive facts must enter through TryReadArchive.");
            evidence.Add(new EvidenceEntry { Id = id, Title = title, Text = text, Source = source, VerifiedFact = false });
        }

        public IReadOnlyList<EvidenceEntry> Evidence { get { return evidence.AsReadOnly(); } }

        public bool EndDay()
        {
            if (Finished) return false;
            CurrentDay++;
            return true;
        }

        // Internal result only. Never display these scores or odds to players.
        public Faction DeterminePoliticalWinner()
        {
            Faction best = Faction.Royalist;
            foreach (Faction faction in Enum.GetValues(typeof(Faction)))
            {
                if (faction == Faction.Capitalist || faction == Faction.Socialist || faction == Faction.Bystander) continue;
                if (GetInternalInfluence(faction) > GetInternalInfluence(best)) best = faction;
            }
            return best;
        }
    }
}
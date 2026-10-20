using System;
using System.Collections.Generic;

namespace PennyUniversity
{
    public enum NotebookFilter { All, UnverifiedClaims, VerifiedFacts }

    [Serializable]
    public sealed class PlayerHypothesis
    {
        public int Id { get; private set; }
        public string Text { get; private set; }

        internal PlayerHypothesis(int id, string text)
        {
            Id = id;
            Text = text;
        }

        internal void ReplaceText(string text) { Text = text; }
    }

    /// <summary>
    /// Player-facing notes, intentionally separate from scenario truth and faction scores.
    /// Newspaper and debate claims remain unverified even if the player believes them.
    /// </summary>
    public sealed class PennyNotebook
    {
        private readonly PennyGameState state;
        private readonly List<PlayerHypothesis> hypotheses = new List<PlayerHypothesis>();
        private int nextHypothesisId = 1;

        public PennyNotebook(PennyGameState state)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public IReadOnlyList<EvidenceEntry> GetEntries(NotebookFilter filter)
        {
            var entries = new List<EvidenceEntry>();
            foreach (var entry in state.Evidence)
            {
                if (entry == null) continue;
                if (filter == NotebookFilter.VerifiedFacts && !entry.VerifiedFact) continue;
                if (filter == NotebookFilter.UnverifiedClaims && entry.VerifiedFact) continue;
                // Copy prevents the UI from accidentally editing authoritative evidence.
                entries.Add(new EvidenceEntry {
                    Id = entry.Id, Title = entry.Title, Text = entry.Text,
                    Source = entry.Source, VerifiedFact = entry.VerifiedFact
                });
            }
            return entries.AsReadOnly();
        }

        public IReadOnlyList<PlayerHypothesis> Hypotheses
        {
            get { return hypotheses.AsReadOnly(); }
        }

        public bool TryAddHypothesis(string text, out int id)
        {
            id = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            id = nextHypothesisId++;
            hypotheses.Add(new PlayerHypothesis(id, text.Trim()));
            return true;
        }

        public bool TryEditHypothesis(int id, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            foreach (var hypothesis in hypotheses)
            {
                if (hypothesis.Id != id) continue;
                hypothesis.ReplaceText(text.Trim());
                return true;
            }
            return false;
        }

        public bool RemoveHypothesis(int id)
        {
            for (int i = 0; i < hypotheses.Count; i++)
            {
                if (hypotheses[i].Id != id) continue;
                hypotheses.RemoveAt(i);
                return true;
            }
            return false;
        }
    }
}
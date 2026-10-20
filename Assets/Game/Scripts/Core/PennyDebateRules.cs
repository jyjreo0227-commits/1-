using System;
using System.Collections.Generic;

namespace PennyUniversity
{
    public enum RebuttalKind { FactualEvidence, Logic, Inconsistency, Values, Consequences, ConcedeAndRefute }

    [Serializable]
    public sealed class DebateChoice
    {
        public string Text;
        public RebuttalKind Kind;
        public int PersuasionPoints;
        public string RequiredEvidenceId;
    }

    public static class PennyDebateRules
    {
        public static bool IsAvailable(DebateChoice choice, IReadOnlyList<EvidenceEntry> evidence)
        {
            if (choice == null) return false;
            if (string.IsNullOrEmpty(choice.RequiredEvidenceId)) return true;
            if (evidence == null) return false;
            foreach (var item in evidence)
                if (item.Id == choice.RequiredEvidenceId) return true;
            return false;
        }

        // Persuasion is independent of the scenario's objective truth.
        public static int ScoreChoice(DebateChoice choice, IReadOnlyList<EvidenceEntry> evidence)
        {
            if (!IsAvailable(choice, evidence)) throw new InvalidOperationException("Locked debate choice.");
            return Math.Max(0, choice.PersuasionPoints);
        }

        public static bool PlayerWon(int playerPersuasion, int opponentPersuasion, bool surrendered)
        {
            return !surrendered && playerPersuasion > opponentPersuasion;
        }
    }
}
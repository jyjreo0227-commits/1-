using System;
using System.Collections.Generic;

namespace PennyUniversity
{
    /// <summary>
    /// Supplies public titles without exposing record contents.
    /// The full ArchiveRecord is retained in trusted game logic only.
    /// </summary>
    public sealed class PennyArchiveCatalog
    {
        private readonly Dictionary<string, ArchiveRecord> records =
            new Dictionary<string, ArchiveRecord>(StringComparer.Ordinal);

        public PennyArchiveCatalog(IEnumerable<ArchiveRecord> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var record in source)
            {
                if (record == null || string.IsNullOrWhiteSpace(record.Id) ||
                    string.IsNullOrWhiteSpace(record.Title) ||
                    string.IsNullOrWhiteSpace(record.Content) ||
                    records.ContainsKey(record.Id))
                    throw new ArgumentException("Archive records require unique IDs and complete content.");
                records.Add(record.Id, record);
            }
        }

        public IReadOnlyList<ArchiveTitle> GetTitles()
        {
            var titles = new List<ArchiveTitle>();
            foreach (var pair in records)
                titles.Add(new ArchiveTitle(pair.Key, pair.Value.Title));
            titles.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));
            return titles.AsReadOnly();
        }

        public bool TryPurchase(PennyGameState state, string recordId, int entranceFee, out EvidenceEntry fact)
        {
            fact = null;
            if (state == null || string.IsNullOrEmpty(recordId) ||
                !records.TryGetValue(recordId, out var record))
                return false;
            return state.TryReadArchive(record, entranceFee, out fact);
        }
    }

    public sealed class ArchiveTitle
    {
        public string Id { get; }
        public string Title { get; }
        public ArchiveTitle(string id, string title) { Id = id; Title = title; }
    }
}
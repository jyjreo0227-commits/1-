using System;
using System.Collections.Generic;

namespace PennyUniversity
{
    public enum NewspaperBias
    {
        Royalist, Revolutionary, Conservative, Reformist, Commercial, Independent
    }

    [Serializable]
    public sealed class NewspaperArticle
    {
        public string Id;
        public string NewspaperName;
        public string Headline;
        public string Body;
        public int PublicationDay;
        public NewspaperBias Bias;
        // A bias label is editorial metadata, not proof that an article is false.
    }

    /// <summary>
    /// Select articles by headline, then read full text. Reading adds an
    /// unverified evidence entry. No preview body is returned by headlines.
    /// </summary>
    public sealed class NewspaperReader
    {
        private readonly PennyGameState state;
        private readonly Dictionary<string, NewspaperArticle> articles =
            new Dictionary<string, NewspaperArticle>(StringComparer.Ordinal);
        private readonly HashSet<string> readIds = new HashSet<string>(StringComparer.Ordinal);

        public NewspaperReader(PennyGameState state, IEnumerable<NewspaperArticle> catalog)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));

            foreach (var article in catalog)
            {
                if (article == null || string.IsNullOrWhiteSpace(article.Id) ||
                    string.IsNullOrWhiteSpace(article.Headline) ||
                    string.IsNullOrWhiteSpace(article.Body) ||
                    article.PublicationDay < 1 ||
                    article.PublicationDay > PennyGameState.CampaignLength)
                    throw new ArgumentException("Invalid newspaper article.", nameof(catalog));

                if (articles.ContainsKey(article.Id))
                    throw new ArgumentException("Duplicate article ID: " + article.Id, nameof(catalog));

                articles.Add(article.Id, article);
            }
        }

        public IReadOnlyList<ArticleHeadline> GetAvailableHeadlines()
        {
            var result = new List<ArticleHeadline>();
            if (state.Finished) return result.AsReadOnly();

            foreach (var article in articles.Values)
            {
                if (article.PublicationDay > state.CurrentDay) continue;
                result.Add(new ArticleHeadline(
                    article.Id, article.Headline, article.NewspaperName, article.PublicationDay));
            }
            result.Sort((a, b) =>
            {
                int byDay = b.PublicationDay.CompareTo(a.PublicationDay);
                return byDay != 0 ? byDay : string.CompareOrdinal(a.Id, b.Id);
            });
            return result.AsReadOnly();
        }

        public bool TryRead(string articleId, out string body)
        {
            body = null;
            if (state.Finished || string.IsNullOrEmpty(articleId) ||
                !articles.TryGetValue(articleId, out var article) ||
                article.PublicationDay > state.CurrentDay)
                return false;

            body = article.Body;
            if (readIds.Add(articleId))
                state.RecordUnverifiedEvidence(article.Id, article.Headline,
                    article.Body, EvidenceSource.Newspaper);
            return true;
        }
    }

    public sealed class ArticleHeadline
    {
        public string Id { get; }
        public string Headline { get; }
        public string NewspaperName { get; }
        public int PublicationDay { get; }

        public ArticleHeadline(string id, string headline, string newspaperName, int publicationDay)
        {
            Id = id;
            Headline = headline;
            NewspaperName = newspaperName;
            PublicationDay = publicationDay;
        }
    }
}
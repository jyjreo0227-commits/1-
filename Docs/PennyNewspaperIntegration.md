# Newspaper module — incremental implementation

This is a **logic-only** component, deliberately separate from the existing Virus Secret runtime.

## API
- Construct `NewspaperReader(state, authoredArticles)`.
- `GetAvailableHeadlines()` returns **titles, newspaper names, IDs, and publication days only**. Never expose `NewspaperArticle.Body` through the headline UI.
- `TryRead(id, out body)` reveals full article text only after the user selects a title, and records an **unverified** entry in the notebook.
- Future-day articles cannot be read early; old articles remain available.
- Reading the same article twice does not duplicate its evidence entry.

## Editorial and scenario constraints
- `NewspaperBias` is authored metadata and does not mean every article is a lie.
- Author different article catalogs for each of the four truth scenarios. This module **does not yet** implement scenario-aware content selection.
- The `PeacefulSevenDays` scenario must not have armed conflict in its authored article pool.

## Remaining decisions
Article-reading time cost, number of newspapers per day, randomized newspaper names, article selection UI, and how news influences debates remain **unapproved**.

## Verification
Run Unity Edit Mode tests: `PennyNewspaperTests`. The connector cannot execute Unity tests, so no pass is claimed.

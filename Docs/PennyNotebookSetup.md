# Deduction notebook — Unity UI integration

Added in the same isolated `feature/penny-university-core` branch; the existing Virus Secret scene is unchanged.

## Scene wiring
1. Make a uGUI Canvas with a notebook panel and a Vertical Layout Group for evidence rows.
2. Make a Button prefab with a child `UnityEngine.UI.Text` component for evidence entries.
3. Assign `panelRoot`, `entryListRoot`, `entryButtonPrefab`, `detailText`, `hypothesesText`, `hypothesisInput`, `addHypothesisButton`, and `closeButton` on `PennyNotebookPanel`.
4. Construct `PennyNotebook notebook = new PennyNotebook(gameState)` using the SAME `PennyGameState` passed to newspaper and archive modules.
5. Call `panel.Initialize(notebook)`; connect filter buttons to `ShowAll`, `ShowClaims`, `ShowFacts`. Call `Open()` when the player opens their notebook.
6. Newspaper reading and paid archive selection automatically appear after `Refresh()` or reopening.

## Rules preserved
- Royal archive entries are labeled **verified facts**.
- Newspaper and debate entries remain **unverified claims**, not automatically false.
- The player can add, edit and remove personal hypotheses in the underlying notebook model. The prototype UI supports adding hypotheses; editing and deletion controls are not yet wired.
- Hidden scenario and influence values are not exposed.

## Pending work
- Save/load of notes and hypotheses.
- Full edit/delete UI for hypotheses, linking evidence to theories, sorting/searching, polished UI, and debate-to-notebook integration.
- This code has not been compiled or run in Unity here. Run `PennyNotebookTests` via Unity Edit Mode tests before merging.

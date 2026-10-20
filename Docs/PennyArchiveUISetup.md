# Royal Archive — Unity uGUI integration

This adds a reusable archive panel; it does **not** alter the existing Virus Secret scene.

## Scene setup
1. Create a Canvas and a Panel for the archive window.
2. Add a Vertical Layout Group object as the record-title list.
3. Create a uGUI Button prefab with a child `Text` component.
4. Add `PennyArchivePanel` to an active controller GameObject, assign the panel root, title list, title button prefab, status Text, document Text and close Button.
5. Call `Initialize(gameState, authoredArchiveRecords, entranceFee)` from your own bootstrap. This prototype intentionally does not choose the fee or starting cash.
6. Call `Open()` when the player interacts with the archive.

## Important behavior
- Before purchase, UI shows **only record titles**, not contents or summaries.
- Selecting a title charges one admission fee and immediately reveals **one** book. Closing before selecting costs nothing.
- The underlying game state prevents more than four successful purchases and prevents rereading the same record for another fee.
- A successful selection clears the list for the current panel session. Reopen the panel to see titles again; the next successful selection requires another fee.
- The UI is a prototype; a dedicated confirmation dialog and title-list filtering are future improvements.

## Tests and status
Run `PennyArchiveCatalogTests` as Unity Edit Mode tests. Unity Editor execution was not available in this session.

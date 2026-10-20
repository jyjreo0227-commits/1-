# Penny University — prototype core (unapproved balance)

This folder introduces **isolated** C# logic for the proposed new game. It does not replace or interact with the existing Virus Secret game, scene, or scripts.

## Files
- `Assets/Game/Scripts/Core/PennyUniversityCore.cs`: campaign day, funds, influence, authentic archive entry tracking, evidence.
- `Assets/Game/Scripts/Core/PennyScenarioFactory.cs`: seeded scenario selection and provisional initial influence.
- `Assets/Game/Scripts/Core/PennyDebateRules.cs`: deterministic authored-choice score evaluator.
- `Assets/Game/Editor/PennyUniversityCoreTests.cs`: Unity Edit Mode test examples.

## Integration
1. Open the Unity project using its existing editor version.
2. Check Console for compile errors. Run Edit Mode tests if Unity Test Framework is available.
3. Wire state to UI only after time, money, NPC, and influence balance is approved.
4. Show *only archive record titles* before charging entry. The code expects record content to be supplied by trusted authored game data; it does **not** implement an archive selection UI.
5. Never expose `GetInternalInfluence` or `DeterminePoliticalWinner` in player-facing HUD.
6. The peaceful scenario must not generate armed conflict; event generation is not implemented yet.

## Not implemented
Save/load, Unity scenes, UI, news article pools, multi-turn NPC dialogs, archive title-only browser, NPC simulation, economic victory, 16 ending variations, finalized influence equations, final balancing, or narrative content.

**Important:** This is a foundation, **not a playable game**, and it has not been compiled in a Unity Editor in this environment.

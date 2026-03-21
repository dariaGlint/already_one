## Task
Create the stage data model and JSON loader for the maze puzzle game "うらがえし迷宮".

## Context
Unity 6000.3.10f1 (URP, WebGL target) project.
Read the AGENTS.md file in the project root for coding standards and project conventions.

## Requirements

### StageData.cs
Create a plain C# class (NOT MonoBehaviour, NOT ScriptableObject) in namespace `AlreadyOne.Data` that represents a single stage:
- `int Width` - grid width
- `int Height` - grid height
- `int[] Cells` - row-major 1D array. 1 = white (floor on front, wall on back). 0 = black (wall on front, floor on back).
- `int StartX`, `int StartY` - player start position
- `int GoalX`, `int GoalY` - goal position
- `PositionData[] Keys` - optional array of key positions (can be null or empty)
- `PositionData[] Doors` - optional array of door positions (can be null or empty)
- A helper class `PositionData` with `int X` and `int Y`
- Methods:
  - `bool IsWalkable(int x, int y, bool isFront)` - returns true if the cell at (x,y) is walkable given the current face. On front: cell value 1 is walkable. On back: cell value 0 is walkable. Out of bounds returns false.
  - `int GetCell(int x, int y)` - returns the cell value at (x,y). Returns -1 if out of bounds.

Use `[System.Serializable]` attribute on both classes so they can be deserialized from JSON using Unity's JsonUtility.

### StageLoader.cs
Create a static class in namespace `AlreadyOne.Data` that loads stage data from JSON:
- `static StageData LoadStage(int stageNumber)` - loads from `Resources/Stages/stage_XX` where XX is zero-padded (01, 02, etc.). Uses `Resources.Load<TextAsset>` and `JsonUtility.FromJson<StageData>`.
- `static StageData LoadStageFromJson(string json)` - parses JSON string directly.

## Files to Create
- `Assets/Scripts/Data/StageData.cs`
- `Assets/Scripts/Data/StageLoader.cs`

## Constraints
- Do NOT create .meta files
- Do NOT modify any existing files
- Use namespace `AlreadyOne.Data`
- Use `[SerializeField]` on private fields per coding standards
- File names must match class names
- Include all necessary `using` statements

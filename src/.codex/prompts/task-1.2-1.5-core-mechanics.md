## Task
Create the core game mechanics for "うらがえし迷宮": maze rendering, player movement, flip mechanic, and game manager.

## Context
Unity 6000.3.10f1 (URP, WebGL target) project.
Read the AGENTS.md file in the project root for coding standards and project conventions.

## Existing Files

### Assets/Scripts/Data/StageData.cs
```csharp
namespace AlreadyOne.Data
{
    [Serializable]
    public sealed class StageData
    {
        public int Width { get; }
        public int Height { get; }
        public int[] Cells { get; }
        public int StartX { get; }
        public int StartY { get; }
        public int GoalX { get; }
        public int GoalY { get; }
        public PositionData[] Keys { get; }
        public PositionData[] Doors { get; }

        public bool IsWalkable(int x, int y, bool isFront);
        public int GetCell(int x, int y); // returns -1 if out of bounds
    }

    [Serializable]
    public sealed class PositionData
    {
        public int X { get; }
        public int Y { get; }
    }
}
```

### Assets/Scripts/Data/StageLoader.cs
```csharp
namespace AlreadyOne.Data
{
    public static class StageLoader
    {
        public static StageData LoadStage(int stageNumber); // loads from Resources/Stages/stage_XX
        public static StageData LoadStageFromJson(string json);
    }
}
```

## Requirements

### 1. MazeRenderer.cs (Assets/Scripts/Game/MazeRenderer.cs)
MonoBehaviour in namespace `AlreadyOne.Game` that renders the maze grid.
- `Initialize(StageData stageData)` - Creates a grid of GameObjects (simple quads or sprites) representing each cell. Each cell should be a child of this transform. Position cells so (0,0) is bottom-left. Cell size = 1 unit.
- `SetFace(bool isFront)` - Updates all cell visuals. On front: cell=1 shows white (#FFFFFF), cell=0 shows black (#1A1A1A). On back: cell=1 shows black, cell=0 shows white. Also render the goal cell in a distinct color (e.g., green #00CC44).
- `Clear()` - Destroys all child cell objects.
- Store cell GameObjects in a 2D array for later reference.
- Use SpriteRenderer on each cell. Create sprites programmatically using `Sprite.Create` with a 1x1 white texture.
- The start position should be visually distinct (e.g., slightly different shade).
- Key cells (if any) should show red (#CC3333) only when on the back face.
- Door cells (if any) should show red (#CC3333) only when on the front face, and disappear when the door is opened.

### 2. PlayerController.cs (Assets/Scripts/Game/PlayerController.cs)
MonoBehaviour in namespace `AlreadyOne.Game` that handles player movement.
- The player is a colored square (e.g., blue #3366CC) rendered as a SpriteRenderer.
- Grid-based movement: WASD or arrow keys move one cell at a time.
- Before moving, check `StageData.IsWalkable(targetX, targetY, isFront)`. If not walkable, don't move.
- Also check door cells: if the target is a closed door, block movement.
- Expose `int GridX` and `int GridY` as the current grid position.
- Provide a method `void SetPosition(int x, int y)` to teleport the player.
- Provide a method `void SetStageData(StageData data, bool isFront)` to update the reference data.
- Use `Input.GetKeyDown` for discrete grid movement (not continuous).
- Fire a C# event `System.Action<int, int> OnMoved` when the player moves to a new cell.

### 3. FlipManager.cs (Assets/Scripts/Game/FlipManager.cs)
MonoBehaviour in namespace `AlreadyOne.Game` that manages the front/back flip.
- `bool IsFront` - current face state (starts as true).
- `bool TryFlip(int playerX, int playerY, StageData stageData)` - Attempts to flip. Checks if the player's current position is walkable on the opposite face. Returns true if flip succeeded.
- Fire a C# event `System.Action<bool> OnFlipped` with the new isFront value when flip succeeds.
- Fire a C# event `System.Action OnFlipFailed` when flip fails.
- Flip is triggered by Space key.
- Provide `void Reset()` to reset to front face.

### 4. GameManager.cs (Assets/Scripts/Game/GameManager.cs)
MonoBehaviour in namespace `AlreadyOne.Game` that ties everything together.
- Has serialized references to `MazeRenderer`, `PlayerController`, `FlipManager`.
- `void LoadStage(int stageNumber)` - Uses `StageLoader` to load data, initializes the maze, places the player at start position, resets FlipManager.
- Listens to `PlayerController.OnMoved` to check for goal reached.
- Listens to `FlipManager.OnFlipped` to update `MazeRenderer.SetFace` and `PlayerController.SetStageData`.
- Provides `void RestartStage()` - reloads the current stage.
- Fire a C# event `System.Action OnStageCleared` when the player reaches the goal.
- On start, load stage 1 (or a configurable stage number via [SerializeField]).
- Restart is triggered by R key.

## Files to Create
- `Assets/Scripts/Game/MazeRenderer.cs`
- `Assets/Scripts/Game/PlayerController.cs`
- `Assets/Scripts/Game/FlipManager.cs`
- `Assets/Scripts/Game/GameManager.cs`

## Constraints
- Do NOT create .meta files
- Do NOT modify existing files (StageData.cs, StageLoader.cs, WebGLBuilder.cs, TutorialInfo/)
- Use namespace `AlreadyOne.Game`
- Use `[SerializeField]` on private fields per coding standards (private fields use _camelCase)
- File names must match class names
- Include all necessary `using` statements
- Camera setup is NOT your responsibility - just position cells with (0,0) at bottom-left

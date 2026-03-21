## Task
Create UI scripts for title screen, stage clear, game clear, and flip indicator for "うらがえし迷宮".

## Context
Unity 6000.3.10f1 (URP, WebGL target) project.
Read the AGENTS.md file in the project root for coding standards and project conventions.

## Existing Interfaces

### GameManager (AlreadyOne.Game)
```csharp
public event Action OnStageCleared;
public void LoadStage(int stageNumber);
public void RestartStage();
```

### StageProgressManager (AlreadyOne.Game)
```csharp
public int CurrentStageNumber { get; }
public bool IsAllCleared { get; }
public event Action<int> OnStageAdvanced;
public event Action OnAllStagesCleared;
public void AdvanceToNextStage();
public void ResetProgress();
```

### FlipManager (AlreadyOne.Game)
```csharp
public bool IsFront { get; }
public event Action<bool> OnFlipped;
public event Action OnFlipFailed;
```

## Requirements

### 1. TitleScreenUI.cs (Assets/Scripts/UI/TitleScreenUI.cs)
MonoBehaviour in namespace `AlreadyOne.UI`.
- Creates a full-screen Canvas with:
  - Game title "うらがえし迷宮" in large text (centered, upper area)
  - Subtitle "表では壁でも、うらがえすと道になる" (smaller, below title)
  - "スタート" button (centered, lower area)
  - Simple instruction text: "移動: WASD/矢印キー　反転: スペース" (bottom)
- Colors: white text on black (#1A1A1A) background
- On start button click: hide the title screen canvas and invoke a `System.Action OnGameStarted` event
- Use Unity UI (UnityEngine.UI namespace)
- The canvas should use Screen Space - Overlay, CanvasScaler with Scale With Screen Size (1920x1080 reference)
- Create all UI elements programmatically in Awake() - do NOT require prefab references

### 2. StageClearUI.cs (Assets/Scripts/UI/StageClearUI.cs)
MonoBehaviour in namespace `AlreadyOne.UI`.
- Shows "ステージクリア！" text when a stage is cleared
- Shows "次のステージへ" button
- Shows current stage number
- Hidden by default, shown via `Show(int clearedStageNumber)`
- Hidden via `Hide()`
- On button click: invoke `System.Action OnNextStageRequested` event and hide
- Same canvas setup as title screen (programmatic, overlay, scaled)

### 3. GameClearUI.cs (Assets/Scripts/UI/GameClearUI.cs)
MonoBehaviour in namespace `AlreadyOne.UI`.
- Shows "全ステージクリア！おめでとう！" text
- Shows "タイトルに戻る" button
- Hidden by default, shown via `Show()`
- On button click: invoke `System.Action OnReturnToTitleRequested` event and hide
- Same canvas setup

### 4. FlipIndicatorUI.cs (Assets/Scripts/UI/FlipIndicatorUI.cs)
MonoBehaviour in namespace `AlreadyOne.UI`.
- Shows a large "表" or "裏" text in the corner of the screen
- Updates via `SetFace(bool isFront)` method
- "表" shown in white, "裏" shown with inverted colors (white text on black background or similar distinction)
- Positioned top-right corner
- Uses a separate canvas or can be part of a shared game HUD canvas
- Create programmatically

### 5. UIManager.cs (Assets/Scripts/UI/UIManager.cs)
MonoBehaviour in namespace `AlreadyOne.UI` that orchestrates all UI.
- Has [SerializeField] references to GameManager, StageProgressManager, FlipManager
- Has [SerializeField] references to TitleScreenUI, StageClearUI, GameClearUI, FlipIndicatorUI
- Wires up events:
  - TitleScreenUI.OnGameStarted → start the game (load stage 1)
  - GameManager.OnStageCleared → show StageClearUI or GameClearUI
  - StageClearUI.OnNextStageRequested → advance stage and load next
  - GameClearUI.OnReturnToTitleRequested → reset progress and show title
  - FlipManager.OnFlipped → update FlipIndicatorUI
- Manages game flow states (title, playing, stage clear, game clear)

## Files to Create
- `Assets/Scripts/UI/TitleScreenUI.cs`
- `Assets/Scripts/UI/StageClearUI.cs`
- `Assets/Scripts/UI/GameClearUI.cs`
- `Assets/Scripts/UI/FlipIndicatorUI.cs`
- `Assets/Scripts/UI/UIManager.cs`

## Constraints
- Do NOT create .meta files
- Do NOT modify existing files
- Use namespace `AlreadyOne.UI`
- Use `[SerializeField]` on private fields with `_camelCase` naming
- All UI created programmatically in code (no prefab dependencies)
- Use `using UnityEngine.UI;` for Button, Text, Image, etc.
- Use TextMeshPro if available, otherwise fall back to legacy Text (use UnityEngine.UI.Text)
- File names must match class names

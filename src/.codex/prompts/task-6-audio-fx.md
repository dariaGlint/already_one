## Task
Create audio management and flip animation scripts for "うらがえし迷宮".

## Context
Unity 6000.3.10f1 (URP, WebGL target) project.
Read the AGENTS.md file in the project root for coding standards and project conventions.

## Existing Interfaces

### FlipManager (AlreadyOne.Game)
```csharp
public bool IsFront { get; }
public event Action<bool> OnFlipped;
public event Action OnFlipFailed;
```

### MazeRenderer (AlreadyOne.Game)
```csharp
public void SetFace(bool isFront);
// Has transform that is the parent of all cell objects
```

## Requirements

### 1. AudioManager.cs (Assets/Scripts/Audio/AudioManager.cs)
Singleton MonoBehaviour in namespace `AlreadyOne.Audio`.
- Persists across scenes using `DontDestroyOnLoad`
- Has two AudioSource components: one for BGM (looping), one for SE (one-shot)
- `void PlaySE(string clipName)` - loads AudioClip from `Resources/Audio/SE/{clipName}` and plays as one-shot
- `void PlayBGM(string clipName)` - loads AudioClip from `Resources/Audio/BGM/{clipName}`, sets as BGM clip and plays (looping)
- `void StopBGM()` - stops BGM
- Caches loaded AudioClips in a Dictionary to avoid repeated Resource.Load calls
- Singleton pattern: `public static AudioManager Instance { get; private set; }` with Awake check

### 2. GameAudioEvents.cs (Assets/Scripts/Audio/GameAudioEvents.cs)
Static class in namespace `AlreadyOne.Audio` defining audio event name constants:
```csharp
public const string FlipSuccess = "flip_success";
public const string FlipFail = "flip_fail";
public const string Move = "move";
public const string KeyPickup = "key_pickup";
public const string StageClear = "stage_clear";
public const string GameClear = "game_clear";
public const string BGMMain = "bgm_main";
```

### 3. FlipAnimator.cs (Assets/Scripts/Game/FlipAnimator.cs)
MonoBehaviour in namespace `AlreadyOne.Game` that animates the paper-flip visual effect.
- Has a [SerializeField] reference to `MazeRenderer _mazeRenderer` (the transform to animate)
- Has a [SerializeField] reference to `FlipManager _flipManager`
- `[SerializeField] private float _flipDuration = 0.3f;`
- When FlipManager.OnFlipped fires:
  1. Scale the MazeRenderer transform's X from 1 to 0 over half duration
  2. At midpoint, call MazeRenderer.SetFace(newIsFront)
  3. Scale from 0 back to 1 over remaining half
- Use a coroutine for the animation
- During animation, block player input by disabling PlayerController (expose a way to do this, or the FlipAnimator can have a [SerializeField] reference to PlayerController)
- When FlipManager.OnFlipFailed fires, do a quick shake effect (small X oscillation for 0.15s)

### 4. PaperTheme.cs (Assets/Scripts/Game/PaperTheme.cs)
Static class in namespace `AlreadyOne.Game` with color constants:
```csharp
public static readonly Color PaperWhite = new Color(0.96f, 0.94f, 0.91f); // #F5F0E8
public static readonly Color InkBlack = new Color(0.10f, 0.10f, 0.10f);   // #1A1A1A
public static readonly Color StampRed = new Color(0.80f, 0.20f, 0.20f);   // #CC3333
public static readonly Color FlipZoneGray = new Color(0.60f, 0.60f, 0.60f); // #999999
```

## Files to Create
- `Assets/Scripts/Audio/AudioManager.cs`
- `Assets/Scripts/Audio/GameAudioEvents.cs`
- `Assets/Scripts/Game/FlipAnimator.cs`
- `Assets/Scripts/Game/PaperTheme.cs`

## Constraints
- Do NOT create .meta files
- Do NOT modify existing files
- Use appropriate namespaces (AlreadyOne.Audio or AlreadyOne.Game)
- Use `[SerializeField]` on private fields with `_camelCase` naming
- File names must match class names

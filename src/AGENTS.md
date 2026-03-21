# AGENTS.md - うらがえし迷宮 Project Rules

## Project Overview
- **Game:** うらがえし迷宮 (Flip Maze) - 1画面パズルゲーム
- **Engine:** Unity 6000.3.10f1 (URP, WebGL target)
- **Language:** C#

## C# Coding Standards
- **Private Fields**: Use `_camelCase` for private fields.
- **Serialized Fields**: Always mark private fields with `[SerializeField]` if they need to be editable in the Inspector. Avoid `public` fields for Inspector configuration.
- **Methods**: Use `PascalCase`.
- **Classes**: Use `PascalCase` and ensure the file name matches the class name.
- **Namespaces**: Always organize code within a project-specific namespace:
  - `AlreadyOne.Game` - Core game logic (movement, flip, maze, etc.)
  - `AlreadyOne.UI` - UI screens (title, clear, indicators)
  - `AlreadyOne.Audio` - Audio management
  - `AlreadyOne.Data` - Data models and loaders

## Unity Best Practices
- **Null Checks**: Use `TryGetComponent` instead of `GetComponent` when checking for existence or in frequent calls.
- **Update Method**: Avoid heavy computations or allocations in `Update()`. Use coroutines or events where possible.
- **Tags vs Layers**: Prefer Layers and `CompareTag` over string equality checks.
- **Vector Math**: Use `sqrMagnitude` instead of `magnitude` for distance comparisons.
- **Attributes**: Use `[Header]`, `[Tooltip]`, and `[Space]` to make Inspector UIs user-friendly.
- **Range**: Use `[Range(min, max)]` for float/int fields that have logical limits.

## Project Structure
```
Assets/
  Scripts/
    Game/       # Core game logic
    UI/         # UI components
    Audio/      # Audio management
    Data/       # Data models and loaders
  Scenes/       # Unity scenes
  Prefabs/      # Prefab assets
  Materials/    # Materials
  Resources/
    Stages/     # Stage JSON files
  Audio/
    SE/         # Sound effects
    BGM/        # Background music
  Editor/       # Editor-only scripts
```

## Stage Data Format (JSON)
Stage files are located at `Assets/Resources/Stages/stage_XX.json`:
```json
{
  "width": 7,
  "height": 7,
  "cells": [1,1,1,0,0,1,1, ...],
  "startX": 1,
  "startY": 1,
  "goalX": 5,
  "goalY": 5,
  "keys": [{"x": 3, "y": 2}],
  "doors": [{"x": 4, "y": 3}]
}
```
- `cells`: Row-major 1D array. `0` = black (wall on front, floor on back). `1` = white (floor on front, wall on back). `2` = flip zone (walkable on both faces, where the player can safely flip).
- `keys` and `doors`: Optional arrays, used from stage 6 onwards.

## Important Constraints
- Do NOT create `.meta` files manually. Unity generates these automatically.
- Do NOT modify `Assets/Editor/WebGLBuilder.cs` or `Assets/TutorialInfo/` unless explicitly instructed.
- Every MonoBehaviour and ScriptableObject class name MUST match its filename.
- All new scripts must include the appropriate `using` statements.

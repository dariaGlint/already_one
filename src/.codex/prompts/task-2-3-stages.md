## Task
Create stage JSON files (stages 1-5) and a stage progression manager for "うらがえし迷宮".

## Context
Unity 6000.3.10f1 (URP, WebGL target) project.
Read the AGENTS.md file in the project root for coding standards and project conventions.

## Existing Files

### Stage Data Format (from AGENTS.md)
JSON files at `Assets/Resources/Stages/stage_XX.json`:
```json
{
  "width": 7,
  "height": 7,
  "cells": [1,1,1,0,0,1,1, ...],
  "startX": 1, "startY": 1,
  "goalX": 5, "goalY": 5
}
```
- `cells`: Row-major 1D array. `1` = white (floor on front/omote, wall on back/ura). `0` = black (wall on front, floor on back).
- Stages 1-5 do NOT have keys or doors.

### Game Mechanics Reference
- Front (表): white cells (1) are walkable, black cells (0) are walls
- Back (裏): black cells (0) are walkable, white cells (1) are walls
- Player flips between front/back; if current position would be a wall after flip, flip fails
- Player must reach the goal cell to clear the stage

### Existing Code Interfaces
```csharp
namespace AlreadyOne.Game {
    public sealed class GameManager : MonoBehaviour {
        public event Action OnStageCleared;
        public void LoadStage(int stageNumber);
        public void RestartStage();
    }
}
```

## Requirements

### Stage JSON Files
Create 5 stage JSON files following the design doc descriptions. Each maze should be on a small grid (7x7 to 9x9).

**CRITICAL RULES for stage design:**
- The start cell MUST be walkable on front (cell value = 1)
- The goal cell MUST be walkable on front (cell value = 1)
- Each stage must be SOLVABLE - there must exist a path from start to goal using flip mechanics
- Test your layout mentally: trace a path from start to goal, checking which cells are walkable on each face

**Stage 1: 表口 (Front Gate)** - 7x7 grid
- Simple maze where one flip is needed to reach the goal
- Nearly a straight path, but one section is blocked on front and open on back
- Very easy tutorial stage

**Stage 2: 白と黒 (White and Black)** - 7x7 grid
- Two distinct regions: a white-dominant area and a black-dominant area
- Player must flip twice: once to cross into the black region, once to return
- Teaches basic back-and-forth flipping

**Stage 3: すれ違い (Near Miss)** - 7x7 grid
- Goal is very close to start on the front view but unreachable directly
- Going through the back view creates a detour that reaches the goal
- Teaches that proximity doesn't mean accessibility

**Stage 4: 行き止まりの裏 (Dead End's Reverse)** - 7x7 grid
- Front view has obvious dead ends
- Flipping at dead ends reveals new passages in the back view
- Reinforces the core theme of "what's blocked can be opened"

**Stage 5: 裏道 (Back Road)** - 9x9 grid
- The main route to the goal exists primarily in the back view
- Player spends more time navigating in back than front
- Slightly larger grid for the first time

### StageProgressManager.cs (Assets/Scripts/Game/StageProgressManager.cs)
MonoBehaviour in namespace `AlreadyOne.Game`:
- `[SerializeField] private int _totalStages = 10;`
- `int CurrentStageNumber { get; }` - starts at 1
- `bool IsAllCleared { get; }` - true when all stages are completed
- `void AdvanceToNextStage()` - increments current stage
- `void ResetProgress()` - back to stage 1
- `event Action<int> OnStageAdvanced` - fires with new stage number
- `event Action OnAllStagesCleared` - fires when all stages done

## Files to Create
- `Assets/Resources/Stages/stage_01.json`
- `Assets/Resources/Stages/stage_02.json`
- `Assets/Resources/Stages/stage_03.json`
- `Assets/Resources/Stages/stage_04.json`
- `Assets/Resources/Stages/stage_05.json`
- `Assets/Scripts/Game/StageProgressManager.cs`

## Constraints
- Do NOT create .meta files
- Do NOT modify existing files
- Stage JSON must be valid JSON with no trailing commas
- Use namespace `AlreadyOne.Game` for the C# file
- Each stage MUST be solvable - verify your grid layouts carefully
- cells array length must equal width * height exactly

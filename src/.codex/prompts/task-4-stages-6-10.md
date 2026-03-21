## Task
Create stage JSON files 6-10 for "うらがえし迷宮" with key and door mechanics.

## Context
Unity 6000.3.10f1 (URP, WebGL target) project.
Read the AGENTS.md file in the project root for coding standards and project conventions.

## Cell Value System
- `0` = black cell: wall on front (表), floor on back (裏)
- `1` = white cell: floor on front (表), wall on back (裏)
- `2` = flip zone: walkable on BOTH faces. The player can only flip (switch between front/back) while standing on a cell with value 2.

## Key/Door Rules
- Keys can ONLY be collected while on the BACK face (裏)
- Collecting a key automatically opens the corresponding door (paired by array index)
- Doors block movement on the FRONT face (表) until opened
- Keys and doors are specified as coordinate arrays in the JSON

## Stage JSON Format
```json
{
  "width": 7, "height": 7,
  "cells": [...],
  "startX": 1, "startY": 1,
  "goalX": 5, "goalY": 5,
  "keys": [{"x": 3, "y": 4}],
  "doors": [{"x": 4, "y": 2}]
}
```

## Requirements

Create 5 stages following these designs. Each stage MUST be solvable.

### Stage 6: 裏の鍵 (Back Key) - 7x7
- First stage with key/door mechanic
- Key is in the back (裏) area, door blocks the front (表) path
- Player must: walk on front → flip to back → collect key → flip to front → pass through opened door → reach goal
- Simple layout to teach the key/door mechanic

### Stage 7: 見える鍵、届かない鍵 (Visible but Unreachable Key) - 7x7
- The key position is "visible" (in a white area) but collecting it requires navigating to it on the back face
- Requires figuring out the right flip sequence to reach the key
- Two flip zones positioned to create a puzzle

### Stage 8: 表裏一体 (Two Sides of One Coin) - 9x9
- Multiple flips required, frequent switching between front and back
- The "showcase" stage demonstrating the game's core appeal
- At least 3-4 flip transitions needed
- Interesting maze layout on both faces

### Stage 9: 裏口から先に (Back Entrance First) - 9x9
- The player must go through the back route FIRST to set up the front route
- Key is deep in the back area
- Door blocks the main front path near the goal
- Requires planning ahead

### Stage 10: 最深部 (The Depths) - 9x9
- Final stage, combined flip + key/door puzzle
- 2 keys and 2 doors
- Slightly larger and more complex
- Should feel like a satisfying finale but not frustratingly hard

## CRITICAL DESIGN RULES
1. Start cell MUST have cell value 1 (front walkable)
2. Goal cell MUST have cell value 1 (front walkable)
3. Key cells should have cell value 0 (so they're reachable on back face)
4. Door cells should have cell value 1 (blocking front path until opened)
5. Flip zones (cell=2) MUST connect front-walkable areas to back-walkable areas
6. Every stage MUST be solvable - trace the solution path mentally
7. cells array length MUST equal width * height

## Files to Create
- `Assets/Resources/Stages/stage_06.json`
- `Assets/Resources/Stages/stage_07.json`
- `Assets/Resources/Stages/stage_08.json`
- `Assets/Resources/Stages/stage_09.json`
- `Assets/Resources/Stages/stage_10.json`

## Constraints
- Do NOT create .meta files
- Do NOT modify any existing files
- Stage JSON must be valid JSON with no trailing commas
- Verify cells array length matches width * height

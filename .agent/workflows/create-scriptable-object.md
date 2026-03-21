---
description: Create a new ScriptableObject class and an optional instance
---

1.  **Request Details**:
    *   Ask the user for the **Class Name** (e.g., `GameConfig`).
    *   Ask for the **Target Directory** (default: `src/Assets/Scripts/ScriptableObjects`).
    *   Ask if they want to create an initial **Asset Instance** immediately.

2.  **Create Class File**:
    *   Generate the C# file at the specified path.
    *   Ensure the class inherits from `ScriptableObject`.
    *   Add the `[CreateAssetMenu]` attribute:
        ```csharp
        using UnityEngine;

        [CreateAssetMenu(fileName = "NewClassName", menuName = "AlreadyOne/ClassName")]
        public class ClassName : ScriptableObject
        {
            [Header("Settings")]
            [SerializeField] private string _description;
            
            // Add other fields here
        }
        ```
    *   **Rule**: Use the project name (e.g., "AlreadyOne") in the `menuName` path to keep the Create menu organized.

3.  **Compile & Check**:
    *   (Implicit) Unity will compile.
    *   If the user requested an instance, warn that we cannot strictly create the `.asset` file without Unity Editor scripting or the user doing it manually, OR suggest creating an Editor script to generate it if this is a frequent task. *For now, instruct the user to right-click and create it.*

4.  **Completion**:
    *   Notify the user that the ScriptableObject class is ready.

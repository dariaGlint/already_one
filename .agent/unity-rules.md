# Unity Development Rules for AI Agent

## C# Coding Standards
- **Private Fields**: Use `_camelCase` for private fields.
- **Serialized Fields**: Always mark private fields with `[SerializeField]` if they need to be editable in the Inspector. Avoid `public` fields for Inspector configuration.
- **Methods**: Use `PascalCase`.
- **Classes**: Use `PascalCase` and ensure the file name matches the class name.
- **Namespaces**: Always organize code within a project-specific namespace (e.g., `AlreadyOne.Game`, `AlreadyOne.UI`).

## Unity Best Practices
- **Null Checks**: Use `TryGetComponent` instead of `GetComponent` when checking for existence or in frequent calls.
- **Update Method**: Avoid heavy computations or allocations in `Update()`. Use coroutines or events where possible.
- **Tags vs Layers**: Prefer Layers and Comparison tags (`CompareTag`) over string equality checks.
- **Vector Math**: Use `sqrMagnitude` instead of `magnitude` for distance comparisons to avoid square root calculations.

## Project Structure
- **Assets Folder**: Keep the root `Assets` folder clean. Use subfolders like `Scripts`, `Scenes`, `Prefabs`, `Materials`.
- **Meta Files**: Ensure every new file and folder has a corresponding `.meta` file committed to version control.

## Editor Enhancements
- **Attributes**: Use `[Header]`, `[Tooltip]`, and `[Space]` to make Inspector UIs user-friendly.
- **Range**: Use `[Range(min, max)]` for float/int fields that have logical limits.

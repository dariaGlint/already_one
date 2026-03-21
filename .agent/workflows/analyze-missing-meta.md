---
description: Analyze project for missing .meta files
---

1.  **Run Analysis**:
    *   // turbo
    *   Execute a command to find files in `src/Assets` that do not have a corresponding `.meta` file.
    *   Command (PowerShell):
        ```powershell
        Get-ChildItem -Path src/Assets -Recurse -File | Where-Object { $_.Extension -ne ".meta" -and -not (Test-Path "$($_.FullName).meta") } | Select-Object FullName
        ```

2.  **Report**:
    *   List any missing `.meta` files found.
    *   **Action**: If files are found, warn the user that this can cause broken references in Unity. Suggest opening Unity to let it auto-generate them, or confirming if they are valid assets.

3.  **Check for Dangling Meta Files**:
    *   // turbo
    *   Execute a command to find `.meta` files that do not have a corresponding asset file.
    *   Command (PowerShell):
        ```powershell
        Get-ChildItem -Path src/Assets -Recurse -Filter "*.meta" | Where-Object { -not (Test-Path ($_.FullName -replace "\.meta$","")) -and ($_.Name -ne "context") } | Select-Object FullName
        ```
    *   Note: "context" check is just a placeholder, the logic is checking if the file *minus* .meta exists.

4.  **Clean Up**:
    *   Ask user if they want to delete dangling `.meta` files.

---
description: Setup Unity project with optimal .gitignore and .gitattributes
---

1.  **Check Existing Files**:
    *   Check if `.gitignore` and `.gitattributes` exist in the root. @check_files

2.  **Create/Update .gitignore**:
    *   If missing or incomplete, create/update `.gitignore` with standard Unity exclusions:
        *   `/src/[Ll]ibrary/`
        *   `/src/[Tt]emp/`
        *   `/src/[Oo]bj/`
        *   `/src/[Bb]uild/`
        *   `/src/[Bb]uilds/`
        *   `/src/[Ll]ogs/`
        *   `/src/[Uu]ser[Ss]ettings/`
        *   `*.csproj`
        *   `*.sln`
        *   (And others from standard https://github.com/github/gitignore/blob/main/Unity.gitignore)

3.  **Create/Update .gitattributes**:
    *   Create `.gitattributes` to handle LFS (Large File Storage) for binary assets (images, audio, models).
    *   Example content:
        ```
        *.psd filter=lfs diff=lfs merge=lfs -text
        *.jpg filter=lfs diff=lfs merge=lfs -text
        *.png filter=lfs diff=lfs merge=lfs -text
        *.gif filter=lfs diff=lfs merge=lfs -text
        *.mp3 filter=lfs diff=lfs merge=lfs -text
        *.wav filter=lfs diff=lfs merge=lfs -text
        *.ogg filter=lfs diff=lfs merge=lfs -text
        *.fbx filter=lfs diff=lfs merge=lfs -text
        *.blend filter=lfs diff=lfs merge=lfs -text
        ```

4.  **Verify**:
    *   Run `git status` to ensure correct files are ignored.

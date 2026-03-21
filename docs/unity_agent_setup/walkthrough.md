# Unity Agent Rules & Workflow 導入 (srcフォルダ対応)

AntigravityによるUnity開発を効率化するためのルールとワークフローを整備しました。
プロジェクト構成が `src` フォルダ配下にAssetsなどがある形式に対応しました。

## 実装内容
以下のファイルをプロジェクトに追加・更新しました。

### Agent Rules (`.agent/`)
-   [unity-rules.md](file:///c:/Users/daria/Documents/Antigravity/Unity1Week/already_one/.agent/unity-rules.md): コーディング規約、Unityベストプラクティス。

### Agent Workflows (`.agent/workflows/`)
-   [create-scriptable-object.md](file:///c:/Users/daria/Documents/Antigravity/Unity1Week/already_one/.agent/workflows/create-scriptable-object.md): `ScriptableObject` 作成フロー（`src` 対応済み）。
-   [setup-unity-git.md](file:///c:/Users/daria/Documents/Antigravity/Unity1Week/already_one/.agent/workflows/setup-unity-git.md): `.gitignore` 自動生成フロー（`src` 対応済み）。
-   [analyze-missing-meta.md](file:///c:/Users/daria/Documents/Antigravity/Unity1Week/already_one/.agent/workflows/analyze-missing-meta.md): `.meta` チェックフロー（`src` 対応済み）。

### Project Configuration
-   [.gitignore](file:///c:/Users/daria/Documents/Antigravity/Unity1Week/already_one/.gitignore): `src` フォルダ配下のUnity管理ファイルを無視するように修正しました。

## 次のステップ
-   新しいワークフローを実際に試してみる。

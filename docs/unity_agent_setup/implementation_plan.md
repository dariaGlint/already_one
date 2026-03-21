# Unity Agent Rules & Workflow 導入計画

Unity開発において、Antigravity（AIエージェント）がより効果的に動作するためのルールとワークフローを定義します。

## 提案内容

### 1. Agent Rules (Unity特化)
AIがUnityプロジェクトを扱う際のベストプラクティスを定義します。
- **コーディング規約**: C#の標準的な規約に加え、Unity特有の `SerializeField` を活用し、カプセル化を維持する。
- **名前空間**: 全てのスクリプトに適切な名前空間を付与する。
- **パフォーマンス**: `Update` 内での `GetComponent` 等、負荷の高い処理を避ける。
- **エディタ拡張**: 可能な限りエディタ上でのデバッグを容易にするための属性（`Header`, `Space`, `Tooltip`）を付与する。

### 2. Agent Workflow (Unity特化)
- **`create-scriptable-object`**: 各種データ管理用の ScriptableObject クラスとそのインスタンス作成メニューを自動生成。
- **`setup-unity-git`**: Unityプロジェクトに最適な `.gitignore` と `.gitattributes` を自動配置。
- **`analyze-missing-meta`**: プロジェクト内の `.meta` ファイルの欠落をチェック（Unityでのトラブル防止）。

## 具体的なファイル内容の草案

### `.agent/unity-rules.md`
```markdown
# Unity Development Rules for AI Agent

- **Private Fields**: Use `_camelCase` for private fields and mark with `[SerializeField]` if they should be visible in Inspector.
- **Methods**: Use `PascalCase`.
- **Boilerplate**: Avoid empty `Start()` and `Update()` methods.
- **Null Checks**: Use `TryGetComponent` instead of `GetComponent` when checking for existence.
- **Namespaces**: Always organize code within a project-specific namespace.
```

### `.agent/workflows/create-so.md`
```markdown
---
description: Create a new ScriptableObject template
---
1. Ask the user for the class name and target directory.
2. Create a C# file with the template including `[CreateAssetMenu]`.
```

## 検証計画
### 手動検証
- 各ワークフローが正しく認識されるか確認する。
- ルールに基づいたコード生成が行われるかテストする。

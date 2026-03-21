# .gitignore更新 実施結果

## 概要
`update-gitignore` ワークフローを実行し、`.gitignore` 内のUnity標準フォルダのパスを `src` ディレクトリ配下（`/src/` プレフィックス付き）に更新しました。

## 変更内容
- [x] `.gitignore` の各パスを `/src/` 配下に修正
    - `Assets`, `Library`, `Temp`, `Logs`, `UserSettings` 等
    - `Addressables` 関連のパス
    - `Visual Scripting` 生成ファイルのパス
    - その他 `ExportedObj`, `.consulo` 等

## 検証結果
- [x] `.gitignore` の内容が正しく書き換わっていることを確認

## 次のステップ
- 特になし。これでUnityのルートが `src` 下にある構成でも正しくGit除外が行われます。

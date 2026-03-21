---
description: Unityプロジェクトのルート構造が変更された際に .gitignore を更新するワークフロー。「.gitignoreの対象ディレクトリを[DIR]に更新して」や「Unityプロジェクトを[DIR]に移動した」といった依頼で使用する。
---

1. Unityプロジェクトのルートが移動された「ターゲットディレクトリ」を特定します。
   - ユーザーの依頼内容やファイルシステムを確認して、ターゲットディレクトリを特定してください。
   - 一般的なターゲットディレクトリには `src`, `Client`, `App` などがあります。
   - ここでは特定されたディレクトリを `[TARGET_DIR]` と呼びます。

2. ルートディレクトリにある `.gitignore` ファイルを読み込みます。

3. 以下のUnity標準フォルダのそれぞれについて、パスの先頭に `/[TARGET_DIR]/` が付いているか確認します。付いていない場合は、`/[TARGET_DIR]/` を含むようにパスを更新してください。
   - `/[Ll]ibrary/`
   - `/[Tt]emp/`
   - `/[Oo]bj/`
   - `/[Bb]uild/`
   - `/[Bb]uilds/`
   - `/[Ll]ogs/`
   - `/[Uu]ser[Ss]ettings/`
   - `/[Mm]emoryCaptures/`
   - `/[Rr]ecordings/`
   - `/[Aa]ssets/`
   - `/[Pp]ackages/`
   - `/[Pp]roject[Ss]ettings/`

   **例:**
   `[TARGET_DIR]` が `Client` の場合:
   - `/[Ll]ibrary/` を `/Client/[Ll]ibrary/` に変更
   - `/[Aa]ssets/` を `/Client/[Aa]ssets/` に変更

4. `ExportedObj/` や `.consulo/` などの他の一般的なパスについても、それらがターゲットディレクトリ内に存在する場合は、`/[TARGET_DIR]/` を先頭に付与してください。

5. `!/[Aa]ssets/` （除外設定）のようなパターンも `!/[TARGET_DIR]/[Aa]ssets/` に更新されるようにしてください。

6. 更新された `.gitignore` ファイルを保存します。

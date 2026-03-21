#!/bin/bash
# Unity WebGL ビルド → GitHub Pages デプロイ スクリプト
# Usage: bash build-and-serve.sh [--local]
#   --local: GitHub Pagesにデプロイせず、ローカルサーバーで確認する

set -e

UNITY_PATH="/c/Program Files/Unity/Hub/Editor/6000.3.1f1/Editor/Unity.exe"
PROJECT_PATH="$(cd "$(dirname "$0")" && pwd)"
BUILD_PATH="$PROJECT_PATH/Build/WebGL"
REPO_ROOT="$(cd "$PROJECT_PATH/.." && pwd)"

# --- WebGL ビルド ---
echo "=== WebGL ビルド開始 ==="
"$UNITY_PATH" -batchmode -nographics -quit \
  -projectPath "$PROJECT_PATH" \
  -executeMethod WebGLBuilder.Build \
  -logFile -

if [ ! -d "$BUILD_PATH" ]; then
  echo "ERROR: ビルド出力が見つかりません: $BUILD_PATH"
  exit 1
fi
echo "=== WebGL ビルド完了 ==="

# --- デプロイ ---
if [ "$1" = "--local" ]; then
  # ローカルサーバーで確認（同じWiFi内）
  echo "=== ローカルサーバー起動 (port 8080) ==="
  echo "スマホから http://$(hostname -I 2>/dev/null | awk '{print $1}' || echo '<PCのIP>'):8080 でアクセス"
  cd "$BUILD_PATH"
  python -m http.server 8080 --bind 0.0.0.0
else
  # GitHub Pages にデプロイ
  echo "=== GitHub Pages にデプロイ ==="
  cd "$REPO_ROOT"

  # gh-pagesブランチが存在しなければ作成
  if ! git show-ref --verify --quiet refs/heads/gh-pages; then
    git checkout --orphan gh-pages
    git rm -rf . 2>/dev/null || true
    git commit --allow-empty -m "Initialize gh-pages"
    git checkout -
  fi

  # ビルド成果物をgh-pagesブランチにコピー
  git checkout gh-pages

  # 既存ファイルを削除（.gitは除く）
  find . -maxdepth 1 ! -name '.git' ! -name '.' -exec rm -rf {} +

  # ビルド成果物をコピー
  cp -r "$BUILD_PATH"/* .

  # コミット＆プッシュ
  git add -A
  git commit -m "Deploy WebGL build $(date +%Y%m%d-%H%M%S)"
  git push origin gh-pages

  # 元のブランチに戻る
  git checkout -

  echo "=== デプロイ完了 ==="
  echo "URL: https://dariaglint.github.io/already_one/"
  echo "※ GitHub Pagesの設定でgh-pagesブランチを指定する必要があります（初回のみ）"
fi

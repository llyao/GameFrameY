#!/usr/bin/env bash
# 从 LubanSource/（vendored v5.1.0 源码）重新构建 Luban 生成器到 Luban/（gen-*.sh 使用该目录下的 Luban.dll）
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

dotnet build "$SCRIPT_DIR/LubanSource/src/Luban/Luban.csproj" -c Release -o "$SCRIPT_DIR/Luban" "$@"

echo "[build-luban] OK -> $SCRIPT_DIR/Luban/Luban.dll"

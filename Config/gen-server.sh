#!/usr/bin/env bash
# 生成服务端配置：代码与数据(bytes) -> Server/Config/Generate（仅 s 组的表）
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LUBAN_DLL="$SCRIPT_DIR/Tools/Luban/Luban.dll"

dotnet "$LUBAN_DLL" \
    -t server -c cs-bin -d bin \
    --conf "$SCRIPT_DIR/luban.conf" \
    -x "outputCodeDir=$SCRIPT_DIR/../Server/Config/Generate" \
    -x "outputDataDir=$SCRIPT_DIR/../Server/Config/Datas" \
    "$@"

echo "[gen-server] OK"

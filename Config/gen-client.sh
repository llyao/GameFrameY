#!/usr/bin/env bash
# 生成客户端配置：代码 -> Client/Assets/GameFrameY/Config/Generate，数据(bytes) -> Client/Assets/Bundles/Config
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LUBAN_DLL="$SCRIPT_DIR/Tools/Luban/Luban.dll"

dotnet "$LUBAN_DLL" \
    -t client -c cs-bin -d bin \
    --conf "$SCRIPT_DIR/luban.conf" \
    -x "outputCodeDir=$SCRIPT_DIR/../Client/Assets/GameFrameY/Config/Generate" \
    -x "outputDataDir=$SCRIPT_DIR/../Client/Assets/Bundles/Config" \
    "$@"

echo "[gen-client] OK"

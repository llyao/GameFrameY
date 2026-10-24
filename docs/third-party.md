# 第三方源码来源说明（vendored sources）

本仓库以**源码（embedded package / 目录快照）**形式内置全部第三方依赖，`Client/` 工程不依赖任何第三方 scoped registry
（仅 Unity 官方源提供 `com.unity.ugui`、SBP 等官方包）。所有包目录与包名一致，UPM 解析时 embedded 优先于 registry。

## Client/Packages/ 下的 vendored 包

| 包 | 版本（tag） | 上游 | 许可证 |
|---|---|---|---|
| com.gameframex.unity | 3.0.1 | github.com/GameFrameX/com.gameframex.unity | MIT / Apache-2.0 双许可（包内 LICENSE.md） |
| com.gameframex.unity.asset | 5.0.0 | github.com/GameFrameX/com.gameframex.unity.asset | 同上 |
| com.gameframex.unity.event | 2.0.0 | github.com/GameFrameX/com.gameframex.unity.event | 同上 |
| com.gameframex.unity.fsm | 2.0.0 | github.com/GameFrameX/com.gameframex.unity.fsm | 同上 |
| com.gameframex.unity.localization | 3.0.0 | github.com/GameFrameX/com.gameframex.unity.localization | 同上 |
| com.gameframex.unity.litjson | 1.2.4 | github.com/GameFrameX/com.gameframex.unity.litjson | 同上 |
| com.gameframex.unity.mono | 2.0.0 | github.com/GameFrameX/com.gameframex.unity.mono | 同上 |
| com.gameframex.unity.network | 3.0.0 | github.com/GameFrameX/com.gameframex.unity.network | 同上 |
| com.gameframex.unity.procedure | 2.0.0 | github.com/GameFrameX/com.gameframex.unity.procedure | 同上 |
| com.gameframex.unity.psygames.unitywebsocket | 2.9.3 | github.com/GameFrameX/com.gameframex.unity.psygames.unitywebsocket | 同上 |
| com.gameframex.unity.readassets | 1.2.2 | github.com/GameFrameX/com.gameframex.unity.readassets | 同上 |
| com.gameframex.unity.scene | 3.0.2 | github.com/GameFrameX/com.gameframex.unity.scene | 同上 |
| com.gameframex.unity.setting | 2.0.0 | github.com/GameFrameX/com.gameframex.unity.setting | 同上 |
| com.gameframex.unity.ui | 3.0.1 | github.com/GameFrameX/com.gameframex.unity.ui | 同上 |
| com.gameframex.unity.ui.ugui | 2.7.2 | github.com/GameFrameX/com.gameframex.unity.ui.ugui | 同上 |
| com.gameframex.unity.cysharp.unitask | 2.6.2 | github.com/GameFrameX/com.gameframex.unity.cysharp.unitask（fork of Cysharp/UniTask） | MIT |
| com.gameframex.unity.tuyoogame.yooasset | 3.2.0 | github.com/GameFrameX/com.gameframex.unity.tuyoogame.yooasset（fork of tuyoogame/YooAsset） | Apache-2.0 |
| jp.hadashikick.vcontainer | 1.19.0 | github.com/hadashiA/VContainer | MIT |
| com.gameframey.luban | 5.1.0 | focus-creative-games/luban `src/Luban.Tests/RuntimeSupport` | MIT |
| com.gameframey.core | 0.1.0 | 本仓库自研 | MIT |

vendoring 统一处理（升级同步时保留）：各包 `Tests/` → `Tests~/`（避免 Unity 导入测试程序集）并删除悬空 `Tests.meta`；
其余文件与上游 tag 完全一致。版本即 git tag（如 `3.0.1`），同步见文末。

未引入：`com.gameframex.unity.entry`（GameApp 静态门面包）——自研层直接使用 `GameEntry.GetComponent<T>()`，无需门面；
后续如需要可 `git clone --depth 1 --branch 3.0.0 https://github.com/GameFrameX/com.gameframex.unity.entry.git Client/Packages/com.gameframex.unity.entry`。

## 其他 vendored 源码

| 组件 | 位置 | 上游 | 版本 | 许可证 |
|---|---|---|---|---|
| Luban 生成器源码 | `Config/Tools/LubanSource/` | github.com/focus-creative-games/luban | tag v5.1.0 | MIT |
| Luban 构建产物 | `Config/Tools/Luban/` | 由 `build-luban.bat\|sh` 从 LubanSource 构建 | v5.1.0+df34215d | MIT |
| Luban 运行时（.NET） | `Server/Config/LubanRuntime/` | 同 LubanSource 的 RuntimeSupport | v5.1.0 | MIT |
| GameFrameX.Server | `Server/`（除 `Server/Config/`） | github.com/GameFrameX/GameFrameX.Server | commit `638bd527` | Apache-2.0（见 Server/PROVENANCE.md） |

vendored 脚本：`Config/Tools/vendor_packages.py` 可重跑（已存在的包会跳过），用于按新版本刷新或补充包。

## 与上游同步（通用做法）

```bash
# 示例：同步某个包到新版本 tag
cd Client/Packages/<包名>
git init . && git remote add upstream <上游地址> && git fetch upstream --tags --depth 1
git checkout <新版本tag>   # 解决冲突后清理 .git

# 或者删掉目录后重跑 vendor_packages.py（先更新 DIRECT 表中的版本号）
```

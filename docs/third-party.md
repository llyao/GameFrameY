# 第三方源码来源说明（vendored sources）

本仓库以源码形式内置以下第三方组件（便于阅读、断点与基建优化；均已保留原始 LICENSE）：

| 组件 | 位置 | 上游 | 版本/commit | 许可证 |
|---|---|---|---|---|
| Luban 生成器源码 | `Config/Tools/LubanSource/` | https://github.com/focus-creative-games/luban | tag **v5.1.0**（浅克隆快照，已去 .git） | MIT |
| Luban 构建产物 | `Config/Tools/Luban/` | 同上，由 `build-luban.bat\|sh` 从 LubanSource 构建 | v5.1.0+df34215d | MIT |
| Luban 运行时（Unity） | `Client/Packages/com.gameframey.luban/` | 同上 `src/Luban.Tests/RuntimeSupport/`（与 v5 生成代码命名空间 `Luban` 配套） | v5.1.0 | MIT |
| Luban 运行时（.NET） | `Server/Config/LubanRuntime/` | 同上 | v5.1.0 | MIT |
| YooAsset（GameFrameX fork） | `Client/Packages/com.gameframex.unity.tuyoogame.yooasset/` | https://github.com/GameFrameX/com.gameframex.unity.tuyoogame.yooasset | tag **3.2.0**（浅克隆快照，已去 .git） | Apache-2.0 |
| GameFrameX.Server | `Server/`（除 `Server/Config/`） | https://github.com/GameFrameX/GameFrameX.Server | commit `638bd52793109edff97e3196b8e639a26567021a` | Apache-2.0（见 Server/PROVENANCE.md） |

## YooAsset vendoring 说明

- 以 **embedded package**（同名目录于 `Client/Packages/`）形式引入：UPM 解析同名包时 embedded 包优先于
  scoped registry，因此 `com.gameframex.unity.asset` 的依赖 `com.gameframex.unity.tuyoogame.yooasset >= 3.1.0`
  自动落到本仓库源码，无需联网拉取。
- 本地改动仅两处（升级同步时注意保留）：`Tests/` 重命名为 `Tests~/`（避免 Unity 导入测试程序集）；删除悬空 `Tests.meta`。
- 修改源码后直接在 Unity 内生效；`.meta` 与上游一致，`AssetBundleCollectorSetting.asset` 引用的脚本 GUID 不受影响。

## Luban vendoring 说明

- `LubanSource/` 为完整生成器源码（`src/` 多目标：CSharp/Cpp/Dart/Bson/DataLoader 等）。
- 改模板/生成逻辑后执行 `Config/Tools/build-luban.bat|sh` 重新产出 `Luban/Luban.dll`，`gen-client/server` 即用新工具。
- 运行时与生成代码版本强配套（都取自 v5.1.0），升级时三处一起动。

## 与上游同步（通用做法）

```bash
# 示例：同步 YooAsset fork 新 tag
git remote add yoo-fork https://github.com/GameFrameX/com.gameframex.unity.tuyoogame.yooasset.git
git fetch yoo-fork --tags
git diff HEAD:yoo-fork/3.3.0 -- Client/Packages/com.gameframex.unity.tuyoogame.yooasset
```

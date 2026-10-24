# [2.0.0](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.4.1...2.0.0) (2026-10-15)


* feat(module)!: 流程模块薄入口化并新增 ProcedureOptions ([ee0246c](https://github.com/gameframex/com.gameframex.unity.procedure/commit/ee0246cb67cff78714489f61ed141c691831e05e))


### BREAKING CHANGES

* 组件序列化字段移除；ProcedureRuntimeOverrideProvider 删除，改经 ProcedureOptions 配置。

## [Unreleased]

### ⚠ BREAKING CHANGES

* **module:** 新增 `ProcedureOptions` 启动配置（`GameFrameXOptions.Configure`，首场景加载前注入；字段默认值同原 Inspector 序列化默认）：`EntranceProcedureTypeName`、`UseStartupRunner`（均为启动期型，仅首场景加载前注入生效）
* **module:** 组件值参数序列化字段移除，编辑期配置入口收敛到代码 Options
* **module:** 删除 `ProcedureRuntimeOverrideProvider`，运行时覆盖入口统一收敛到 `ProcedureOptions`

**迁移对照：**

| 旧访问方式 | 新访问方式 | 说明 |
|---|---|---|
| `m_EntranceProcedureTypeName` 序列化字段 | `ProcedureOptions.EntranceProcedureTypeName` | 启动期型 |
| `m_UseStartupRunner` 序列化字段 | `ProcedureOptions.UseStartupRunner` | 启动期型 |
| `ProcedureRuntimeOverrideProvider` | `ProcedureOptions`（`GameFrameXOptions.Configure`） | 删除类型，改经 Options 配置 |

# [2.0.0](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.4.1...2.0.0) (2026-10-08)


### ⚠ BREAKING CHANGES

* **module:** 模块契约全接口化：删除 `GameFrameworkModule` 抽象基类，改为 `IGameFrameworkModule` 标记接口 + 可选能力接口

**新契约形态与迁移对照：**

| 旧基类成员 | 新契约 | 迁移方式 |
|---|---|---|
| `: GameFrameworkModule` | `IGameFrameworkModule` | 声明处直接替换（标记接口，可注册进 `GameFrameworkEntry`） |
| `override void Update(float, float)` | `IUpdatableModule` | 真实实现改 `public`；空实现删除（未实现者不再被轮询） |
| `override void Shutdown()` | `IShutdownableModule` | 真实实现改 `public`；空实现删除（未实现者不再被关闭） |
| `override int Priority` | `IPrioritizedModule` | 改 `public int Priority { get; }`；未实现视为 0 |

本包变更：`ProcedureManager` 实现 `IShutdownableModule`、`IPrioritizedModule`（Priority=-2），删除空 `Update` 覆写。

`GameFrameworkEntry.Update` 仅分派 `IUpdatableModule`；`Shutdown` 按优先级反序仅分派 `IShutdownableModule`；tick 与关闭顺序与旧实现完全一致。能力方法仅由宿主入口调用，业务侧继续通过 `GetModule<IXxxManager>()` 访问模块。

## [1.4.1](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.4.0...1.4.1) (2026-07-30)


### Bug Fixes

* **dependencies:** 更新 com.gameframex.unity.fsm 依赖版本至 1.2.0 ([2ceadeb](https://github.com/gameframex/com.gameframex.unity.procedure/commit/2ceadebcd95dd3015136ba7ec29d4a3820348e79))

# [1.4.0](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.3.0...1.4.0) (2026-07-03)


### Features

* **runtime:** 区分编辑器手动添加与运行时自动添加的 ProcedureComponent 行为 ([d0de28e](https://github.com/gameframex/com.gameframex.unity.procedure/commit/d0de28e82b651cd8ae1f43fa228cc3003ded2a10))

# [1.3.0](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.2.3...1.3.0) (2026-06-16)


### Features

* **editor:** 添加 UseStartupRunner Inspector 配置 UI ([b1d1645](https://github.com/gameframex/com.gameframex.unity.procedure/commit/b1d164555a458e9679ebf220b5854c03d6c6308d))
* **runtime:** 添加 UseStartupRunner 运行时支持 ([59ddbc1](https://github.com/gameframex/com.gameframex.unity.procedure/commit/59ddbc147176361a4247342ec12dad02923f0849))

## [1.2.3](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.2.2...1.2.3) (2026-06-07)


### Bug Fixes

* 补全包规范文件（LICENSE/CHANGELOG/URL 字段/unity 字段） ([e7f52d4](https://github.com/gameframex/com.gameframex.unity.procedure/commit/e7f52d4e7ec54ba246f1b5643d53076b7667a486))

## [1.2.2](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.2.1...1.2.2) (2026-06-05)


### Bug Fixes

* **procedure:** 修复 Inspector 在流程未初始化时访问 CurrentProcedure 异常 ([516177f](https://github.com/gameframex/com.gameframex.unity.procedure/commit/516177f9b00405b1e8f2442db390c4f985258dfb))

## [1.2.1](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.2.0...1.2.1) (2026-05-30)


### Bug Fixes

* **test:** 修复单元测试编译错误 ([c691bfc](https://github.com/gameframex/com.gameframex.unity.procedure/commit/c691bfc431daa2cdcf34b0d544fdf5948327c222))

# [1.2.0](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.1.1...1.2.0) (2026-05-30)


### Bug Fixes

* **procedure:** Inspector 中 m_ProcedureTypeNames null 防护及 TypeHelper 修正 ([a1974b8](https://github.com/gameframex/com.gameframex.unity.procedure/commit/a1974b8bb8dd7fcc49c1680bd5ea576ec7871a37))
* **procedure:** 添加 ProcedureManager.Initialize 对 procedures 参数的校验 ([ac50077](https://github.com/gameframex/com.gameframex.unity.procedure/commit/ac500774869dab6660d049a74946878650dbd427))
* **procedure:** 添加 Start 协程中 AvailableProcedureTypeNames 的 null 防护 ([f3350e0](https://github.com/gameframex/com.gameframex.unity.procedure/commit/f3350e02741d22fbe03440b8a907d340d9440fd5))


### Features

* **procedure:** 为 IProcedureManager 接口所有公开成员添加 [Preserve] 防裁剪标签 ([26c57d6](https://github.com/gameframex/com.gameframex.unity.procedure/commit/26c57d6e448ba21c849d4c1d4a96d3d7c939bec8))
* **procedure:** 为 ProcedureComponent 所有公开成员添加 [Preserve] 防裁剪标签 ([4623151](https://github.com/gameframex/com.gameframex.unity.procedure/commit/46231516b1e7220e9189d7e85e979d245217a03d))
* **procedure:** 为 ProcedureManager 所有公开成员添加 [Preserve] 防裁剪标签 ([b9a3bb9](https://github.com/gameframex/com.gameframex.unity.procedure/commit/b9a3bb90d8ea63934df859c4ac04b5c85ca6ac8b))
* **procedure:** 在 IProcedureManager 接口新增 DestroyProcedures 和 ReinitializeProcedures ([a29e9f9](https://github.com/gameframex/com.gameframex.unity.procedure/commit/a29e9f952491ff2337c0646fae3499ffa57daaf0))
* **procedure:** 在 ProcedureComponent 暴露 DestroyProcedures 和 ReinitializeProcedures ([e15b0c6](https://github.com/gameframex/com.gameframex.unity.procedure/commit/e15b0c6cf7573983050353ce7e992ae494f4219b))
* **procedure:** 实现 ProcedureManager 的 DestroyProcedures 和 ReinitializeProcedures ([1162351](https://github.com/gameframex/com.gameframex.unity.procedure/commit/1162351ee917ba086e616e5d4ae2168bc1b60a82))

## [1.1.1](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.1.0...1.1.1) (2026-05-28)


### Bug Fixes

* **ci:** 统一 .github 工作流配置 ([d697d39](https://github.com/gameframex/com.gameframex.unity.procedure/commit/d697d39682432f1b1de7684d445f8ff7e1437674))
* **deps:** 补充 package.json 中缺失的包依赖 ([d180e92](https://github.com/gameframex/com.gameframex.unity.procedure/commit/d180e92f9e7ccb0086aa9f64a43d44bf53dde39b))

# [1.1.0](https://github.com/gameframex/com.gameframex.unity.procedure/compare/1.0.4...1.1.0) (2025-12-23)


### Features

* **ci:** change ci ([434b3ac](https://github.com/gameframex/com.gameframex.unity.procedure/commit/434b3ac9a89a326f2f395873e48fad7c3af4848b))

# Changelog

## [1.0.3](https://github.com/GameFrameX/com.gameframex.unity.procedure/tree/1.0.3) (2025-06-01)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.procedure/compare/1.0.2...1.0.3)

## [1.0.2](https://github.com/GameFrameX/com.gameframex.unity.procedure/tree/1.0.2) (2025-05-31)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.procedure/compare/1.0.1...1.0.2)

## [1.0.1](https://github.com/GameFrameX/com.gameframex.unity.procedure/tree/1.0.1) (2025-05-30)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.procedure/compare/1.0.0...1.0.1)

## [1.0.0](https://github.com/GameFrameX/com.gameframex.unity.procedure/tree/1.0.0) (2025-05-22)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.procedure/compare/26f05f45a6752c61fa6ebca5f110b7660de2331d...1.0.0)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*

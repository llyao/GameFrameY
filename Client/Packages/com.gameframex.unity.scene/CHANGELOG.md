## [3.0.2](https://github.com/gameframex/com.gameframex.unity.scene/compare/3.0.1...3.0.2) (2026-10-15)


### Bug Fixes

* **dependencies:** asset 依赖升至 5.0.0 ([952ea1c](https://github.com/gameframex/com.gameframex.unity.scene/commit/952ea1c3bf05544ebe3458e777b1797ca3414127))

## [3.0.1](https://github.com/gameframex/com.gameframex.unity.scene/compare/3.0.0...3.0.1) (2026-10-15)


### Bug Fixes

* **dependencies:** 对齐依赖至实际发布版本 ([2e757fd](https://github.com/gameframex/com.gameframex.unity.scene/commit/2e757fdc6ed6724e8985969d26217abdad88986a))

# [3.0.0](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.3.2...3.0.0) (2026-10-15)


* feat(module)!: 场景模块薄入口化并新增 SceneOptions ([75b798b](https://github.com/gameframex/com.gameframex.unity.scene/commit/75b798bee2537bb0021c9515a7d8248d76ba2582))


### Bug Fixes

* **deps:** 同步依赖版本至最新 ([75aab46](https://github.com/gameframex/com.gameframex.unity.scene/commit/75aab464c7c49856244b4e28908790e0a9f319fe))
* **package:** 修正品牌命名 ([e403b47](https://github.com/gameframex/com.gameframex.unity.scene/commit/e403b471b40c581428956cf42f01b856f6e028f6))


### BREAKING CHANGES

* SceneComponent 序列化字段移除，编辑期配置入口收敛到代码 SceneOptions。

## [Unreleased]

### ⚠ BREAKING CHANGES

* **module:** 新增 `SceneOptions` 启动配置（`GameFrameXOptions.Configure`，首场景加载前注入；字段默认值同原 Inspector 序列化默认）：`EnableLoadSceneUpdateEvent`、`EnableLoadSceneDependencyAssetEvent`（均为实时型，Play 期可经活视图 / 模块 API 调整）
* **module:** 组件值参数序列化字段移除，编辑期配置入口收敛到代码 Options；组件内事件访问由组件直达改为模块直达

**迁移对照：**

| 旧访问方式 | 新访问方式 | 说明 |
|---|---|---|
| `m_EnableLoadSceneUpdateEvent` 序列化字段 | `SceneOptions.EnableLoadSceneUpdateEvent` | 实时型 |
| `m_EnableLoadSceneDependencyAssetEvent` 序列化字段 | `SceneOptions.EnableLoadSceneDependencyAssetEvent` | 实时型 |
| `GameEntry.GetComponent<EventComponent>()` | `GameFrameworkEntry.GetModule<IEventManager>()` | 组件直达改模块直达 |

# [3.0.0](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.3.2...3.0.0) (2026-10-08)


### ⚠ BREAKING CHANGES

* **module:** 模块契约全接口化：删除 `GameFrameworkModule` 抽象基类，改为 `IGameFrameworkModule` 标记接口 + 可选能力接口

**新契约形态与迁移对照：**

| 旧基类成员 | 新契约 | 迁移方式 |
|---|---|---|
| `: GameFrameworkModule` | `IGameFrameworkModule` | 声明处直接替换（标记接口，可注册进 `GameFrameworkEntry`） |
| `override void Update(float, float)` | `IUpdatableModule` | 真实实现改 `public`；空实现删除（未实现者不再被轮询） |
| `override void Shutdown()` | `IShutdownableModule` | 真实实现改 `public`；空实现删除（未实现者不再被关闭） |
| `override int Priority` | `IPrioritizedModule` | 改 `public int Priority { get; }`；未实现视为 0 |

本包变更：`GameSceneManager` 实现 `IShutdownableModule`、`IPrioritizedModule`（Priority=2），删除空 `Update` 覆写。

`GameFrameworkEntry.Update` 仅分派 `IUpdatableModule`；`Shutdown` 按优先级反序仅分派 `IShutdownableModule`；tick 与关闭顺序与旧实现完全一致。能力方法仅由宿主入口调用，业务侧继续通过 `GetModule<IXxxManager>()` 访问模块。

## [2.3.2](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.3.1...2.3.2) (2026-08-28)


### Bug Fixes

* **scene:** 修复加载失败场景仍被标记已加载的字典缺陷 ([#2](https://github.com/gameframex/com.gameframex.unity.scene/issues/2)) ([b77ebc1](https://github.com/gameframex/com.gameframex.unity.scene/commit/b77ebc14658672686068086ca6abbc673963fcfc))

## [2.3.1](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.3.0...2.3.1) (2026-07-30)


### Bug Fixes

* **dependencies:** 更新依赖项版本以修复兼容性问题 ([93af967](https://github.com/gameframex/com.gameframex.unity.scene/commit/93af9674735de83b2501d6348b5b93d1ec2723ab))

# [2.3.0](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.2.5...2.3.0) (2026-07-03)


### Features

* **scene:** 添加 SceneComponent 自动注册特性 ([3edbf2b](https://github.com/gameframex/com.gameframex.unity.scene/commit/3edbf2b88c5cd8a7335fe7affd1d03aa1a8eea26))

## [2.2.5](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.2.4...2.2.5) (2026-06-14)


### Bug Fixes

* **scene:** 修复 Single 模式重新加载场景时的资源泄漏与重复加载异常 ([7a75d2b](https://github.com/gameframex/com.gameframex.unity.scene/commit/7a75d2baf0107dbf7afdf45c657e733ef2bcce4d))
* **scene:** 修复场景激活时序导致 SetActiveScene 抛 ArgumentException ([d97b902](https://github.com/gameframex/com.gameframex.unity.scene/commit/d97b902feaf87694f5805c41ca29e415d949599d))

## [2.2.4](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.2.3...2.2.4) (2026-06-08)


### Bug Fixes

* **scene:** 修复事件触发时直接传递内部事件参数的问题 ([08c2828](https://github.com/gameframex/com.gameframex.unity.scene/commit/08c2828dc5bdb1c28ff232e6b8c2a9b930ab05a9))

## [2.2.3](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.2.2...2.2.3) (2026-06-07)


### Bug Fixes

* 补全包规范文件（LICENSE/CHANGELOG/URL 字段/unity 字段） ([f18444d](https://github.com/gameframex/com.gameframex.unity.scene/commit/f18444d68922bcb6d69063819f36a78c26789363))

## [2.2.2](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.2.1...2.2.2) (2026-05-30)


### Bug Fixes

* **scene:** 修复 GameSceneManager 引用池泄漏和状态管理问题 ([c9a3f20](https://github.com/gameframex/com.gameframex.unity.scene/commit/c9a3f200126c654b6deb085e4349905c9c38bf8e))
* **scene:** 统一 SceneComponent 错误处理和默认加载模式 ([29c5244](https://github.com/gameframex/com.gameframex.unity.scene/commit/29c5244832ef735774d3ed62094a47eed04d9eae))

## [2.2.1](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.2.0...2.2.1) (2026-05-28)


### Bug Fixes

* **ci:** 统一 .github 工作流配置 ([5b8f25b](https://github.com/gameframex/com.gameframex.unity.scene/commit/5b8f25b9ab93a95c81c3240294ca17b4c277330a))
* **deps:** 补充 package.json 中缺失的包依赖 ([9e1cc9f](https://github.com/gameframex/com.gameframex.unity.scene/commit/9e1cc9fefe34a6777621f279e754e41ced4cb1a2))

# [2.2.0](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.1.2...2.2.0) (2026-05-07)


### Features

* 移除对YooAsset命名空间的直接依赖 ([0f634ea](https://github.com/gameframex/com.gameframex.unity.scene/commit/0f634eaf717c87144f6529619eb1c8bf4818d733))

## [2.1.2](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.1.1...2.1.2) (2026-03-16)


### Bug Fixes

* **场景管理:** 将加载成功回调参数从进度改为持续时间 ([ea75cb5](https://github.com/gameframex/com.gameframex.unity.scene/commit/ea75cb57102a7d3026303f9a1e211507a9bcf245))

## [2.1.1](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.1.0...2.1.1) (2026-03-13)


### Bug Fixes

* **scene:** 修复场景加载状态判断和回调参数 ([92b519a](https://github.com/gameframex/com.gameframex.unity.scene/commit/92b519a4e195d14af184c20a30e1f739a0f4c142))

# [2.1.0](https://github.com/gameframex/com.gameframex.unity.scene/compare/2.0.0...2.1.0) (2025-12-23)


### Features

* **Scene:** 更新场景管理器 ([0037773](https://github.com/gameframex/com.gameframex.unity.scene/commit/0037773017f7f773d01296f9923e6aeaac660113))

# Changelog

## [2.0.0](https://github.com/GameFrameX/com.gameframex.unity.scene/tree/2.0.0) (2025-10-25)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.scene/compare/1.0.2...2.0.0)

## [1.0.2](https://github.com/GameFrameX/com.gameframex.unity.scene/tree/1.0.2) (2025-06-01)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.scene/compare/1.0.1...1.0.2)

## [1.0.1](https://github.com/GameFrameX/com.gameframex.unity.scene/tree/1.0.1) (2025-05-31)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.scene/compare/1.0.0...1.0.1)

## [1.0.0](https://github.com/GameFrameX/com.gameframex.unity.scene/tree/1.0.0) (2024-04-11)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.scene/compare/c41c2f4ea0c5b6b005332ff5f732456012385538...1.0.0)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*

# [3.0.0](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.3.2...3.0.0) (2026-10-15)


* feat(module)!: 本地化模块薄入口化并新增 LocalizationOptions ([11b0c49](https://github.com/gameframex/com.gameframex.unity.localization/commit/11b0c494210444877217816f098c992e49332806))


### BREAKING CHANGES

* LocalizationComponent 序列化字段移除，编辑期配置入口收敛到代码 LocalizationOptions。

## [Unreleased]

### ⚠ BREAKING CHANGES

* **module:** 新增 `LocalizationOptions` 启动配置（`GameFrameXOptions.Configure`，首场景加载前注入；字段默认值同原 Inspector 序列化默认）
* **module:** 组件值参数序列化字段移除，编辑期配置入口收敛到代码 Options；Play 期经活视图 / 模块 API 调整
* **module:** Helper 实现选择从类型名字符串改为 `Func` 工厂闭包（Custom 引用优先）

# [3.0.0](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.3.2...3.0.0) (2026-10-08)


### ⚠ BREAKING CHANGES

* **module:** 模块契约全接口化：删除 `GameFrameworkModule` 抽象基类，改为 `IGameFrameworkModule` 标记接口 + 可选能力接口

**新契约形态与迁移对照：**

| 旧基类成员 | 新契约 | 迁移方式 |
|---|---|---|
| `: GameFrameworkModule` | `IGameFrameworkModule` | 声明处直接替换（标记接口，可注册进 `GameFrameworkEntry`） |
| `override void Update(float, float)` | `IUpdatableModule` | 真实实现改 `public`；空实现删除（未实现者不再被轮询） |
| `override void Shutdown()` | `IShutdownableModule` | 真实实现改 `public`；空实现删除（未实现者不再被关闭） |
| `override int Priority` | `IPrioritizedModule` | 改 `public int Priority { get; }`；未实现视为 0 |

本包变更：`LocalizationManager` 声明迁移至 `IGameFrameworkModule`，删除空 `Update` / `Shutdown` 覆写。

`GameFrameworkEntry.Update` 仅分派 `IUpdatableModule`；`Shutdown` 按优先级反序仅分派 `IShutdownableModule`；tick 与关闭顺序与旧实现完全一致。能力方法仅由宿主入口调用，业务侧继续通过 `GetModule<IXxxManager>()` 访问模块。

## [2.3.2](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.3.1...2.3.2) (2026-07-30)


### Bug Fixes

* 更新依赖版本以支持新功能和修复已知问题 ([2276581](https://github.com/gameframex/com.gameframex.unity.localization/commit/2276581ee875bd3eb73f8fbe1c0acebd7bd293a7))

## [2.3.1](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.3.0...2.3.1) (2026-06-07)


### Bug Fixes

* 补全包规范文件（LICENSE/CHANGELOG/URL 字段/unity 字段） ([348a2b8](https://github.com/gameframex/com.gameframex.unity.localization/commit/348a2b85bb56fd916316be79902a2bdbe0d5ade2))

# [2.3.0](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.2.4...2.3.0) (2026-06-05)


### Features

* **event:** 新增本地化语言改变前后事件参数 ([85887a4](https://github.com/gameframex/com.gameframex.unity.localization/commit/85887a43e9975b7107dcb43c792b21c023edc90a))

## [2.2.4](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.2.3...2.2.4) (2026-06-01)


### Bug Fixes

* **test:** 补充 UnitTests.cs 缺失的 using 声明 ([a129b70](https://github.com/gameframex/com.gameframex.unity.localization/commit/a129b70ecf38fa5b42bef9cda7d78269033cef2e))

## [2.2.3](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.2.2...2.2.3) (2026-06-01)


### Bug Fixes

* **component:** 修复接口一致性并统一常量定义 ([6b1a49f](https://github.com/gameframex/com.gameframex.unity.localization/commit/6b1a49fda692759edb8d32c8dd2680d4f31503e3))
* **event:** EventArgs 属性改为 private set 并动态获取系统语言 ([e3fedd7](https://github.com/gameframex/com.gameframex.unity.localization/commit/e3fedd72cdc7cf6715639372a08a1d9d8743b22e))
* **manager:** GetRawString 返回 null 并为 GetString(params) 添加异常保护 ([e9c804b](https://github.com/gameframex/com.gameframex.unity.localization/commit/e9c804b0ac787104f35c9bad4c07430aa0b016c1))

## [2.2.2](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.2.1...2.2.2) (2026-05-28)


### Bug Fixes

* **ci:** 统一 .github 工作流配置 ([b5e8498](https://github.com/gameframex/com.gameframex.unity.localization/commit/b5e84984b47e69bcf2ea575e63d394d54e947a4d))
* **deps:** 补充 package.json 中缺失的包依赖 ([a24612a](https://github.com/gameframex/com.gameframex.unity.localization/commit/a24612aa8049c1b8281363640a326ce0d7daae5f))

## [2.2.1](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.2.0...2.2.1) (2026-01-19)


### Bug Fixes

* **本地化:** 将空键和未找到键的返回值从null/default改为空字符串 ([d03fc4e](https://github.com/gameframex/com.gameframex.unity.localization/commit/d03fc4e043739b48cf672dbcd73f187d209393f5))

# [2.2.0](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.1.1...2.2.0) (2026-01-19)


### Features

* **本地化:** 为LocalizationComponent添加Preserve属性和参数化GetString方法 ([3f9b100](https://github.com/gameframex/com.gameframex.unity.localization/commit/3f9b1009913d6327499042c0d4d4a650daaca642))

## [2.1.1](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.1.0...2.1.1) (2026-01-14)


### Bug Fixes

* **本地化:** 当语言为未知时返回系统语言 ([e87776b](https://github.com/gameframex/com.gameframex.unity.localization/commit/e87776b1cffffbf1311f370183072e4fcf1c31fe))

# [2.1.0](https://github.com/gameframex/com.gameframex.unity.localization/compare/2.0.0...2.1.0) (2025-12-24)


### Bug Fixes

* **LocalizationManager:** 修改空键检查逻辑以直接返回默认值 ([5ec0446](https://github.com/gameframex/com.gameframex.unity.localization/commit/5ec04467d3c930bd4a88a465e34ae5512359b238))


### Features

* **本地化:** 为LocalizationComponent添加RequireComponent特性 ([f9b0d75](https://github.com/gameframex/com.gameframex.unity.localization/commit/f9b0d757d7e630d347d531bd06f4c370e9d3377a))

# Changelog

## [2.0.0](https://github.com/GameFrameX/com.gameframex.unity.localization/tree/2.0.0) (2025-06-23)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.localization/compare/1.0.4...2.0.0)

## [1.0.4](https://github.com/GameFrameX/com.gameframex.unity.localization/tree/1.0.4) (2025-05-31)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.localization/compare/1.0.3...1.0.4)

## [1.0.3](https://github.com/GameFrameX/com.gameframex.unity.localization/tree/1.0.3) (2025-05-30)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.localization/compare/1.0.2...1.0.3)

## [1.0.2](https://github.com/GameFrameX/com.gameframex.unity.localization/tree/1.0.2) (2025-01-13)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.localization/compare/1.0.1...1.0.2)

## [1.0.1](https://github.com/GameFrameX/com.gameframex.unity.localization/tree/1.0.1) (2024-12-17)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.localization/compare/1.0.0...1.0.1)

## [1.0.0](https://github.com/GameFrameX/com.gameframex.unity.localization/tree/1.0.0) (2024-04-10)

[Full Changelog](https://github.com/GameFrameX/com.gameframex.unity.localization/compare/28c0dc8ec4eb24172fd309d19bba410e9ad6cb2f...1.0.0)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*

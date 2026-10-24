<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Scene

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 项目简介

GameFrameX Scene 是基于 [YooAsset](https://github.com/tuyoogame/YooAsset) 的 Unity 场景管理包。提供异步场景加载/卸载、事件驱动的状态通知、进度追踪和活跃场景排序系统。

### 功能特性

- **异步场景加载** — 基于 `Task` 的异步 API，支持加载和卸载场景
- **LoadSceneMode 支持** — 支持 `Single`（替换全部）和 `Additive`（叠加）两种模式
- **进度追踪** — 实时加载进度事件，可用于加载界面
- **活跃场景排序** — 基于优先级的排序系统，控制哪个已加载场景成为活跃场景
- **自动相机刷新** — 活跃场景切换时自动刷新 `Camera.main` 引用
- **事件通知** — 订阅加载/卸载的成功、失败和更新事件
- **状态查询** — 随时查询场景是否已加载、正在加载或正在卸载
- **编辑器扩展** — `SceneComponent` 的自定义 Inspector 面板

---

## 快速开始

### 依赖

| 包 | 版本 |
|---|------|
| `com.gameframex.unity.asset` | >= 2.5.0 |
| `com.gameframex.unity.event` | >= 1.1.0 |

### 安装

选择以下方式之一：

**1. UPM Scoped Registry（推荐）**

编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：

```json
{
  "scopedRegistries": [
    {
      "name": "GameFrameX",
      "url": "https://gameframex.upm.alianblank.uk",
      "scopes": [
        "com.gameframex"
      ]
    }
  ],
  "dependencies": {
    "com.gameframex.unity.scene": "2.2.1"
  }
}
```

`scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

**2. Git URL**

在项目的 `Packages/manifest.json` 的 `dependencies` 中添加：

```json
{
  "com.gameframex.unity.scene": "https://github.com/GameFrameX/com.gameframex.unity.scene.git"
}
```

或通过 Unity Package Manager（`Window > Package Manager > + > Add package from git URL`）：

```
https://github.com/GameFrameX/com.gameframex.unity.scene.git
```

**3. 手动安装**

下载仓库并放置到 Unity 项目的 `Packages` 目录下，会自动加载。

---

## 使用示例

### 初始化

在 GameEntry GameObject 上添加 `SceneComponent`（通过 `Add Component > GameFrameX > Scene`）。

### 加载场景

```csharp
// Single 模式 — 替换所有当前场景
var handle = await SceneComponent.LoadScene("Assets/Scenes/GameScene.unity");

// Additive 模式 — 在当前场景之上叠加加载
var handle = await SceneComponent.LoadScene(
    "Assets/Scenes/UIOverlay.unity",
    LoadSceneMode.Additive
);
```

### 卸载场景

```csharp
SceneComponent.UnloadScene("Assets/Scenes/UIOverlay.unity");
```

### 设置场景优先级

通过分配排序值（数值越高越优先）来控制哪个已加载场景成为活跃场景：

```csharp
// 加载后设置优先级，使其成为活跃场景
SceneComponent.SetSceneOrder("Assets/Scenes/GameScene.unity", 10);
```

### 订阅事件

使用 `EventComponent` 订阅场景生命周期事件：

| 事件 | 触发时机 |
|------|----------|
| `LoadSceneSuccessEventArgs` | 场景加载成功（包含耗时） |
| `LoadSceneFailureEventArgs` | 场景加载失败（包含错误信息） |
| `LoadSceneUpdateEventArgs` | 加载进度更新 |
| `UnloadSceneSuccessEventArgs` | 场景卸载成功 |
| `UnloadSceneFailureEventArgs` | 场景卸载失败 |
| `ActiveSceneChangedEventArgs` | 活跃场景变更（包含旧/新场景） |

```csharp
// 示例：订阅加载成功事件
EventComponent.Subscribe<LoadSceneSuccessEventArgs>(OnSceneLoaded);

void OnSceneLoaded(object sender, LoadSceneSuccessEventArgs e)
{
    Debug.Log($"场景加载完成: {e.SceneAssetName}，耗时 {e.Duration:F2}s");
}
```

### 查询场景状态

```csharp
bool isLoaded = SceneComponent.SceneIsLoaded("Assets/Scenes/GameScene.unity");
bool isLoading = SceneComponent.SceneIsLoading("Assets/Scenes/GameScene.unity");
bool isUnloading = SceneComponent.SceneIsUnloading("Assets/Scenes/GameScene.unity");
```

---

## 更新日志

详见 [CHANGELOG.md](CHANGELOG.md)。


## 依赖

| 包 | 说明 |
|----|------|
| `com.gameframex.unity.asset` | 2.5.0 |
| `com.gameframex.unity.event` | 1.1.0 |

## 文档与资源

- [官方文档](https://gameframex.doc.alianblank.com)

## 社区与支持

- QQ群: 467608841 / 233840761
## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。

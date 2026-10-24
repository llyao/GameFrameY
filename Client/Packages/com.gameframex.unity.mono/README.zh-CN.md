<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Mono

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 项目简介

Game Frame X Mono 是 GameFrameX 框架的 Mono 生命周期组件，用于管理游戏中 MonoBehaviour 的事件和更新周期，例如 FixedUpdate、LateUpdate、OnDestroy 等，并提供了一种简便的方式来添加和移除这些事件的监听。

## 快速开始

### 安装

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
    "com.gameframex.unity.mono": "1.1.1"
  }
}
```

`scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

**其他安装方式：**

1. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {"com.gameframex.unity.mono": "https://github.com/GameFrameX/com.gameframex.unity.mono.git"}
   ```

2. 在 Unity 的 `Packages Manager` 中使用 `Git URL` 的方式添加库，地址为：
   ```
   https://github.com/GameFrameX/com.gameframex.unity.mono.git
   ```

3. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

## 使用方法

### 获取组件

```csharp
var monoComponent = GameEntry.GetComponent<MonoComponent>();
```

### 注册生命周期监听

MonoComponent 支持注册 Unity `MonoBehaviour` 生命周期事件的回调。所有监听器均可随时添加或移除。

#### Update / FixedUpdate / LateUpdate

这三个监听器接收两个 `float` 参数：
- `elapseSeconds` — 缩放后的增量时间
- `realElapseSeconds` — 未缩放的增量时间

```csharp
private void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 每帧调用
}

private void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 固定间隔调用（物理）
}

private void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 所有 Update 调用完毕后执行
}

// 注册
monoComponent.AddUpdateListener(OnUpdate);
monoComponent.AddFixedUpdateListener(OnFixedUpdate);
monoComponent.AddLateUpdateListener(OnLateUpdate);

// 不再需要时移除
monoComponent.RemoveUpdateListener(OnUpdate);
monoComponent.RemoveFixedUpdateListener(OnFixedUpdate);
monoComponent.RemoveLateUpdateListener(OnLateUpdate);
```

#### OnDestroy

```csharp
private void OnDestroyCallback()
{
    // MonoComponent 所在 GameObject 销毁时调用
}

monoComponent.AddDestroyListener(OnDestroyCallback);
monoComponent.RemoveDestroyListener(OnDestroyCallback);
```

#### OnApplicationFocus / OnApplicationPause

这两个监听器接收一个 `bool` 参数，并支持**双重通知模式** — 既可以使用直接监听器，也可以通过事件总线订阅。

**直接监听方式：**

```csharp
private void OnApplicationFocus(bool isFocus)
{
    // isFocus: true = 应用获得焦点, false = 失去焦点
}

private void OnApplicationPause(bool isPause)
{
    // isPause: true = 应用暂停, false = 恢复
}

monoComponent.AddOnApplicationFocusListener(OnApplicationFocus);
monoComponent.AddOnApplicationPauseListener(OnApplicationPause);

monoComponent.RemoveOnApplicationFocusListener(OnApplicationFocus);
monoComponent.RemoveOnApplicationPauseListener(OnApplicationPause);
```

**事件总线方式**（通过 `EventComponent`）：

```csharp
var eventComponent = GameEntry.GetComponent<EventComponent>();

eventComponent.Subscribe(OnApplicationFocusChangedEventArgs.EventId, OnFocusChanged);
eventComponent.Subscribe(OnApplicationPauseChangedEventArgs.EventId, OnPauseChanged);

private void OnFocusChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationFocusChangedEventArgs)e;
    if (args.IsFocus)
    {
        // 应用获得焦点
    }
}

private void OnPauseChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationPauseChangedEventArgs)e;
    if (args.IsPause)
    {
        // 应用暂停
    }
}
```

### 注意事项

- 监听器注册是**线程安全**的。
- 监听器在回调执行期间可以安全地添加或移除其他监听器（快照派发机制）。
- 回调中的异常会被捕获并记录日志，**不会**中断其他监听器的执行。
- 不再需要时务必移除监听器，避免内存泄漏。

## 文档与资源

- 文档地址: https://gameframex.doc.alianblank.com
- 仓库地址: https://github.com/GameFrameX/com.gameframex.unity.mono
- 问题反馈: https://github.com/GameFrameX/com.gameframex.unity.mono/issues


## 依赖

| 包 | 说明 |
|----|------|
| `com.gameframex.unity.event` | 1.1.0 |

## 社区与支持

- QQ群: 467608841 / 233840761

## 更新日志

查看 [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.mono/releases) 了解更新日志。
## 开源协议

详细信息请查看 [LICENSE](LICENSE.md) 文件。

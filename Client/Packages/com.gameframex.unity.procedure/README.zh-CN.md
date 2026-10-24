<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Procedure

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 语言

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

---

## 项目简介

基于有限状态机（FSM）的 Unity 游戏流程管理包。通过可切换的流程状态驱动游戏生命周期阶段（闪屏、预加载、登录、主菜单等），提供完整生命周期回调，并支持灵活的状态切换机制。

## 架构设计

```
ProcedureComponent (MonoBehaviour)
  └─ IProcedureManager (接口)
       └─ ProcedureManager (管理 FSM)
            └─ ProcedureBase (抽象类，每个流程的逻辑)
```

- **ProcedureComponent** — Unity 组件，通过 Inspector 注册流程和入口流程，支持两种启动模式。
- **ProcedureManager** — 核心管理器，通过 `IFsmManager` 创建内部 FSM 驱动流程状态切换。
- **ProcedureBase** — 抽象基类，提供生命周期回调：`OnInit`、`OnEnter`、`OnUpdate`、`OnFixedUpdate`、`OnLeave`、`OnDestroy`。

## 依赖关系

- [com.gameframex.unity.fsm](https://github.com/GameFrameX/com.gameframex.unity.fsm) — 有限状态机

## 快速开始

### 安装

选择以下任一方式：

1. 编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
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
       "com.gameframex.unity.procedure": "1.2.2"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

2. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {
      "com.gameframex.unity.procedure": "https://github.com/gameframex/com.gameframex.unity.procedure.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为：`https://github.com/gameframex/com.gameframex.unity.procedure.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

### 创建流程类

```csharp
public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 加载资源、配置等
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 检查加载进度，完成后切换到下一个流程
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}

public class ProcedureMain : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 显示主菜单
    }
}
```

### 配置组件

1. 将 `ProcedureComponent` 添加到游戏对象（通过 `GameFrameX > Procedure`）。
2. 在 Inspector 中配置可用流程和入口流程。
3. 勾选 **UseStartupRunner** 可切换到 StartupRunner 模式（见下文核心概念）。

## 核心概念

### 流程生命周期

每个流程类继承 `ProcedureBase`，拥有以下生命周期回调：

| 回调 | 说明 |
|------|------|
| `OnInit` | 流程被创建时调用，用于初始化数据。 |
| `OnEnter` | 进入流程时调用，可在此执行进入动画、资源加载等。 |
| `OnUpdate` | 每帧更新时调用，适合放置帧逻辑。 |
| `OnFixedUpdate` | 固定频率更新时调用，适合物理逻辑。 |
| `OnLeave` | 离开流程时调用，可在此执行清理或退出动画。 |
| `OnDestroy` | 流程被销毁时调用，用于释放资源。 |

### 状态切换

使用 `ChangeToState<T>` 方法切换到指定流程：

```csharp
protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
{
    if (someCondition)
    {
        ChangeToState<ProcedureNext>(procedureOwner);
    }
}
```

### UseStartupRunner 模式

`ProcedureComponent` 支持两种启动模式，通过 Inspector 中的 **UseStartupRunner** 开关切换：

| 模式 | 说明 |
|------|------|
| **传统模式**（默认） | 在 `Start()` 中读取 Inspector 配置的 `Available Procedures` 和 `Entrance Procedure`，自动初始化并启动流程。 |
| **UseStartupRunner 模式** | 勾选后，本组件仅在 `Awake()` 中注册 `IProcedureManager`，不调用初始化和启动。由 `ApplicationStartupEntry` 通过 `StartupRunner.Run` 接管整个流程的初始化与启动。 |

**UseStartupRunner 模式适用场景：**

- 多模块统一入口管理（将流程启动纳入整体启动序列）
- 避免与其他框架组件同时调用 `Initialize` 导致 `Already exist FSM` 冲突
- 需要在流程启动前执行其他初始化步骤

**配置步骤：**

1. 在 Inspector 中勾选 `UseStartupRunner`。
2. 确保存在 `ApplicationStartupEntry` 组件。
3. 通过 `StartupRunner.Run` 注册并启动流程。

## API 参考

### ProcedureComponent

| 方法/属性 | 说明 |
|-----------|------|
| `Procedure` | 获取流程管理器接口。 |
| `CurrentProcedure` | 获取当前正在执行的流程实例。 |
| `CurrentProcedureTime` | 获取当前流程已持续的时间（秒）。 |
| `HasProcedure<T>()` | 检查指定类型的流程是否存在。 |
| `GetProcedure<T>()` | 获取指定类型的流程实例。 |
| `DestroyProcedures()` | 销毁所有流程并清空状态机。 |
| `ReinitializeProcedures(procedures, entranceProcedure)` | 使用新流程重新初始化。 |

### ProcedureBase

| 方法 | 说明 |
|------|------|
| `ChangeToState<T>` | 切换到指定类型的流程状态。 |

## 使用示例

### 定义流程类

```csharp
public class ProcedureSplash : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 显示闪屏
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 闪屏持续一定时间后进入预加载
        ChangeToState<ProcedurePreload>(procedureOwner);
    }
}

public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 加载游戏资源
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 检查加载进度，完成后进入主菜单
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}

public class ProcedureMain : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 显示主菜单界面
    }
}
```

### 运行时管理

```csharp
// 获取 ProcedureComponent 实例
ProcedureComponent procedureComponent = GameEntry.GetComponent<ProcedureComponent>();

// 检查流程是否存在
bool has = procedureComponent.HasProcedure<ProcedureMain>();

// 获取流程实例
ProcedureMain main = procedureComponent.GetProcedure<ProcedureMain>();

// 获取当前流程信息
ProcedureBase current = procedureComponent.CurrentProcedure;
float time = procedureComponent.CurrentProcedureTime;

// 销毁所有流程并使用新流程重新初始化
procedureComponent.DestroyProcedures();
procedureComponent.ReinitializeProcedures(newProcedures, entranceProcedure);
```

## 文档与资源

- [官方文档](https://gameframex.doc.alianblank.com)

## 社区与支持

- QQ群: 467608841 / 233840761

## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。
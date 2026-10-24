<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX FSM

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 项目简介

Unity 泛型有限状态机包。管理类型化状态机的创建、生命周期和状态转换，支持每个 FSM 的数据字典。

### 功能

- **类型安全 FSM** — 每个 FSM 以拥有者类型 `T` 参数化。同类型多个 FSM 可通过可选名称共存。
- **状态生命周期** — 六个虚方法钩子：`OnInit`、`OnEnter`、`OnUpdate`、`OnFixedUpdate`、`OnLeave`、`OnDestroy`。
- **状态转换** — 在任意状态内调用 `ChangeState<TState>()` 切换状态。
- **变量存储** — 通过 `GetData<TData>(name)` / `SetData(name, value)` 实现跨状态键值数据，对象池化零 GC。
- **动态状态管理** — 运行时 `AddState` / `RemoveState`。
- **Reset 支持** — `Reset()` 清除数据并退出当前状态，保留已注册状态。
- **FixedUpdate 轮询** — 双 `Update` + `FixedUpdate` 驱动路径。
- **运行时 Inspector** — 自定义编辑器在 Play 模式下实时显示 FSM 状态和经过时间。

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
       "com.gameframex.unity.fsm": "1.1.1"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

2. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {
      "com.gameframex.unity.fsm": "https://github.com/gameframex/com.gameframex.unity.fsm.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为：`https://github.com/gameframex/com.gameframex.unity.fsm.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

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
    "com.gameframex.unity.fsm": "1.0.4"
  }
}
```

`scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

## 使用示例

### 定义状态

继承 `FsmState<T>` 并重写生命周期方法：

```csharp
public class IdleState : FsmState<Player>
{
    protected override void OnEnter(IFsm<Player> fsm)
    {
        // 当此状态被激活时调用
    }

    protected override void OnUpdate(IFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        // 激活期间每帧调用
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState<MoveState>(fsm);
        }
    }

    protected override void OnLeave(IFsm<Player> fsm, bool isShutdown)
    {
        // 离开此状态时调用
    }
}
```

### 创建并启动 FSM

```csharp
// 标准方式：通过 GameEntry（不依赖 com.gameframex.unity.entry）
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
IFsm<Player> fsm = fsmComponent.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();

// 快捷方式：通过 GameApp（需要 com.gameframex.unity.entry）
IFsm<Player> fsm = GameApp.Fsm.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();
```

### 存取跨状态数据

```csharp
// 在任意状态的 OnEnter / OnUpdate / ... 中
fsm.SetData("Health", 100);
int hp = fsm.GetData<int>("Health");

if (fsm.HasData("Health"))
{
    fsm.RemoveData("Health");
}
```

### 动态状态管理

```csharp
// 向运行中的 FSM 添加新状态
fsm.AddState(new JumpState());

// 移除状态（不能移除当前状态）
fsm.RemoveState<IdleState>();
```

### 重置 FSM

```csharp
// 退出当前状态，清除所有数据，保留已注册状态
fsm.Reset();
// 可通过 Start<TState>() 重新启动
fsm.Start<IdleState>();
```

### 销毁 FSM

```csharp
// 标准方式：通过 GameEntry（不依赖 com.gameframex.unity.entry）
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
fsmComponent.DestroyFsm(fsm);

// 快捷方式：通过 GameApp（需要 com.gameframex.unity.entry）
GameApp.Fsm.DestroyFsm(fsm);
```

## 文档与资源

- [文档](https://gameframex.doc.alianblank.com)

## 社区与支持

- QQ群: 467608841 / 233840761

## 更新日志

查看 [Releases](https://github.com/gameframex/com.gameframex.unity.fsm/releases) 了解更新日志。


## 依赖

| 包 | 说明 |
|----|------|
| (无) | - |

## 开源协议

详见 [LICENSE.md](LICENSE.md) 文件。

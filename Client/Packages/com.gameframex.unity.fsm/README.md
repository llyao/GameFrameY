<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX FSM

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Project Overview

A generic finite state machine package for Unity. Manages creation, lifecycle, and state transitions of typed FSMs with per-FSM data dictionaries.

### Features

- **Type-safe FSMs** — Each FSM is parameterized by an owner type `T`. Multiple FSMs of the same type can coexist with optional names.
- **State lifecycle** — Six virtual hooks: `OnInit`, `OnEnter`, `OnUpdate`, `OnFixedUpdate`, `OnLeave`, `OnDestroy`.
- **State transitions** — `ChangeState<TState>()` from within any state.
- **Variable storage** — Cross-state key-value data via `GetData<TData>(name)` / `SetData(name, value)`, pooled for zero GC.
- **Dynamic state management** — `AddState` / `RemoveState` on running FSMs.
- **Reset support** — `Reset()` clears data and exits the current state while keeping registered states intact.
- **FixedUpdate polling** — Dual `Update` + `FixedUpdate` tick paths.
- **Runtime Inspector** — Custom editor shows live FSM state and elapsed time during Play mode.

## Quick Start

### Installation

Choose one of the following methods:

1. Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:
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

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.fsm": "https://github.com/gameframex/com.gameframex.unity.fsm.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.fsm.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.

Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:

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

`scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

## Usage Examples

### Define States

Subclass `FsmState<T>` and override lifecycle methods:

```csharp
public class IdleState : FsmState<Player>
{
    protected override void OnEnter(IFsm<Player> fsm)
    {
        // Called when this state becomes active
    }

    protected override void OnUpdate(IFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        // Called every frame while active
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState<MoveState>(fsm);
        }
    }

    protected override void OnLeave(IFsm<Player> fsm, bool isShutdown)
    {
        // Called when transitioning away
    }
}
```

### Create and Start an FSM

```csharp
// Standard: via GameEntry (no dependency on com.gameframex.unity.entry)
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
IFsm<Player> fsm = fsmComponent.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();

// Shortcut: via GameApp (requires com.gameframex.unity.entry)
IFsm<Player> fsm = GameApp.Fsm.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();
```

### Store and Read Cross-State Data

```csharp
// In any state's OnEnter / OnUpdate / ...
fsm.SetData("Health", 100);
int hp = fsm.GetData<int>("Health");

if (fsm.HasData("Health"))
{
    fsm.RemoveData("Health");
}
```

### Dynamic State Management

```csharp
// Add a new state to a running FSM
fsm.AddState(new JumpState());

// Remove a state (cannot remove the current state)
fsm.RemoveState<IdleState>();
```

### Reset an FSM

```csharp
// Exits current state, clears all data, keeps registered states
fsm.Reset();
// Can be restarted with Start<TState>()
fsm.Start<IdleState>();
```

### Destroy an FSM

```csharp
// Standard: via GameEntry (no dependency on com.gameframex.unity.entry)
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
fsmComponent.DestroyFsm(fsm);

// Shortcut: via GameApp (requires com.gameframex.unity.entry)
GameApp.Fsm.DestroyFsm(fsm);
```

## Documentation & Resources

- [Documentation](https://gameframex.doc.alianblank.com)

## Community & Support

- QQ Group: 467608841 / 233840761

## Changelog

See [Releases](https://github.com/gameframex/com.gameframex.unity.fsm/releases) for changelog.


## Dependencies

| Package | Description |
|---------|-------------|
| (无) | - |

## License

See [LICENSE.md](LICENSE.md) for license information.

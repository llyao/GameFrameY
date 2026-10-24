<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Procedure

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.procedure?include_prereleases)](https://github.com/GameFrameX/com.gameframex.unity.procedure/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

[**English**](#) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Language

[**English**](#) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

---

## Project Overview

GameFrameX Procedure is an FSM-based game flow management package for Unity. It drives game lifecycle stages (splash, preload, login, main menu, etc.) through swappable procedure states with full lifecycle callbacks.

## Architecture

```
ProcedureComponent (MonoBehaviour)
  └─ IProcedureManager (interface)
       └─ ProcedureManager (manages FSM)
            └─ ProcedureBase (abstract, per-state logic)
```

- **ProcedureComponent** — Unity component that registers procedures and manages startup modes (traditional Inspector-based or UseStartupRunner).
- **ProcedureManager** — Core manager that creates an internal FSM via `IFsmManager` to drive procedure state transitions.
- **ProcedureBase** — Abstract base class with six lifecycle callbacks: `OnInit`, `OnEnter`, `OnUpdate`, `OnFixedUpdate`, `OnLeave`, `OnDestroy`.

## Dependencies

- [com.gameframex.unity.fsm](https://github.com/GameFrameX/com.gameframex.unity.fsm) — Finite State Machine (version 1.0.3 or above)

## Quick Start

### Installation

Choose one of the following four methods:

**Method 1: Edit `manifest.json` with scoped registry**

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
    "com.gameframex.unity.procedure": "1.2.3"
  }
}
```

The `scopes` field controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

**Method 2: Add directly to `manifest.json` dependencies**

```json
{
  "dependencies": {
    "com.gameframex.unity.procedure": "https://github.com/gameframex/com.gameframex.unity.procedure.git"
  }
}
```

**Method 3: Use Unity Package Manager with Git URL**

Open Window > Package Manager in Unity, click the `+` button, and select "Add package from git URL...". Enter:

```
https://github.com/gameframex/com.gameframex.unity.procedure.git
```

**Method 4: Clone into Packages directory**

Clone the repository into your Unity project's `Packages` directory:

```bash
git clone https://github.com/gameframex/com.gameframex.unity.procedure.git Packages/com.gameframex.unity.procedure
```

The package will be loaded automatically by Unity's Package Manager.

### Creating Procedures

Define your procedure classes by inheriting from `ProcedureBase`. Each procedure override the lifecycle methods as needed:

```csharp
using GameFrameX.Fsm.Runtime;
using GameFrameX.Procedure.Runtime;

public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnInit(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnInit(procedureOwner);
    }

    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        // Check loading progress, switch to next procedure when done
        ChangeToState<ProcedureMain>(procedureOwner);
    }

    protected internal override void OnFixedUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnFixedUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
    }

    protected internal override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
    }

    protected internal override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnDestroy(procedureOwner);
    }
}
```

### Configuring the Component

1. Add `ProcedureComponent` to a game object in your scene via GameFrameX > Procedure in the Unity menu.
2. Choose one of two startup modes:

**Traditional Mode (default)**

- In the Inspector, expand the `Available Procedures` list and add your procedure classes.
- Select the `Entrance Procedure` from the dropdown (e.g., `ProcedurePreload`).
- Leave `Use Startup Runner` unchecked.

**UseStartupRunner Mode**

- Check the `Use Startup Runner` checkbox in the Inspector.
- When enabled, `ProcedureComponent` only registers `IProcedureManager` in `Awake` and does NOT call `Initialize` or `StartProcedure`.
- The actual initialization and startup is taken over by `ApplicationStartupEntry` + `StartupRunner.Run`.
- This avoids the "Already exist FSM" error that would occur if both paths tried to initialize.

## Core Concepts

### Procedure Lifecycle

Each `ProcedureBase` subclass can override the following lifecycle callbacks:

| Callback | Description |
|----------|-------------|
| `OnInit` | Called when the procedure state is initialized. |
| `OnEnter` | Called when entering the procedure state. |
| `OnUpdate` | Called every frame while the procedure is active. |
| `OnFixedUpdate` | Called every fixed update frame while the procedure is active. |
| `OnLeave` | Called when leaving the procedure state. |
| `OnDestroy` | Called when the procedure state is destroyed. |

### State Transitions

Use `ChangeToState<T>` to transition from one procedure to another within any lifecycle callback:

```csharp
protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
{
    base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

    if (m_IsLoadComplete)
    {
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}
```

### UseStartupRunner Mode

The `UseStartupRunner` mode integrates with the GameFrameX startup system:

- **When disabled (default):** `ProcedureComponent.Start()` reads Inspector configuration (`m_AvailableProcedureTypeNames`, `m_EntranceProcedureTypeName`), creates procedure instances, initializes the FSM, and starts the entrance procedure.

- **When enabled:** `ProcedureComponent.Start()` yields early. The `ApplicationStartupEntry` + `StartupRunner.Run` pipeline takes full control of initialization and startup. This is the recommended mode when using GameFrameX's unified startup entry point.

## API Reference

### ProcedureComponent

| Method | Description |
|--------|-------------|
| `bool HasProcedure<T>() where T : ProcedureBase` | Checks if a procedure of type T exists. |
| `bool HasProcedure(Type procedureType)` | Checks if a procedure of the specified type exists. |
| `ProcedureBase GetProcedure<T>() where T : ProcedureBase` | Gets the procedure instance of type T. |
| `ProcedureBase GetProcedure(Type procedureType)` | Gets the procedure instance of the specified type. |
| `void DestroyProcedures()` | Destroys the current FSM and clears all registered procedures. |
| `void ReinitializeProcedures(ProcedureBase[] procedures, ProcedureBase entranceProcedure)` | Destroys the current FSM and reinitializes with new procedures. |
| `IProcedureManager Procedure` | Gets the procedure manager interface. |
| `ProcedureBase CurrentProcedure` | Gets the currently active procedure. |
| `float CurrentProcedureTime` | Gets the time elapsed since the current procedure started. |

### IProcedureManager

| Method | Description |
|--------|-------------|
| `void Initialize(IFsmManager fsmManager, params ProcedureBase[] procedures)` | Initializes the procedure manager with a list of procedures. |
| `void StartProcedure<T>() where T : ProcedureBase` | Starts the specified procedure type. |
| `void StartProcedure(Type procedureType)` | Starts the specified procedure type. |
| `bool HasProcedure<T>() where T : ProcedureBase` | Checks if a procedure of type T exists. |
| `bool HasProcedure(Type procedureType)` | Checks if a procedure of the specified type exists. |
| `ProcedureBase GetProcedure<T>() where T : ProcedureBase` | Gets the procedure instance of type T. |
| `ProcedureBase GetProcedure(Type procedureType)` | Gets the procedure instance of the specified type. |
| `void DestroyProcedures()` | Destroys the current FSM and clears all registered procedures. |
| `void ReinitializeProcedures(params ProcedureBase[] procedures)` | Destroys the current FSM and reinitializes with new procedures. |
| `ProcedureBase CurrentProcedure` | Gets the currently active procedure. |
| `float CurrentProcedureTime` | Gets the time elapsed since the current procedure started. |

## Usage Examples

### Defining Procedures

```csharp
using GameFrameX.Fsm.Runtime;
using GameFrameX.Procedure.Runtime;

public class ProcedureSplash : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        // Show splash screen
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        // Auto-transition after splash duration
        if (m_SplashDuration > 2.0f)
        {
            ChangeToState<ProcedurePreload>(procedureOwner);
        }
    }
}

public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        // Initialize game data, load assets
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        // Check loading progress
        if (m_LoadProgress >= 1.0f)
        {
            ChangeToState<ProcedureLogin>(procedureOwner);
        }
    }
}

public class ProcedureLogin : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        // Show login UI
    }
}

public class ProcedureMain : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        // Show main menu
    }
}
```

### Runtime Management

```csharp
using GameFrameX.Procedure.Runtime;
using UnityEngine;

public class ProcedureTest : MonoBehaviour
{
    [SerializeField] private ProcedureComponent m_ProcedureComponent;

    private void Start()
    {
        // Check if a procedure exists
        bool hasPreload = m_ProcedureComponent.HasProcedure<ProcedurePreload>();

        // Get a procedure instance
        ProcedurePreload preload = m_ProcedureComponent.GetProcedure<ProcedurePreload>();

        // Get current procedure info
        ProcedureBase current = m_ProcedureComponent.CurrentProcedure;
        float time = m_ProcedureComponent.CurrentProcedureTime;

        // Access the procedure manager directly
        IProcedureManager manager = m_ProcedureComponent.Procedure;
    }

    private void SwitchToMain()
    {
        // Get the manager and start a new procedure
        IProcedureManager manager = m_ProcedureComponent.Procedure;
        manager.StartProcedure<ProcedureMain>();
    }

    private void ResetProcedures()
    {
        // Destroy all procedures and reinitialize with new ones
        ProcedureBase[] newProcedures = new ProcedureBase[]
        {
            new ProcedurePreload(),
            new ProcedureMain()
        };

        m_ProcedureComponent.ReinitializeProcedures(newProcedures, newProcedures[0]);
    }
}
```

## Documentation & Resources

- [Official Documentation](https://gameframex.doc.alianblank.com)
- [GameFrameX FSM Package](https://github.com/GameFrameX/com.gameframex.unity.fsm)

## Community & Support

- QQ Group: 467608841 / 233840761

## License

This project is dual-licensed under the MIT License and Apache License 2.0. See [LICENSE.md](LICENSE.md) for details.
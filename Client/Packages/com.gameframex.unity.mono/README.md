<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Mono

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Project Overview

Game Frame X Mono is a Mono lifecycle component for the GameFrameX framework. It manages MonoBehaviour events and update cycles in games, such as FixedUpdate, LateUpdate, OnDestroy, etc., and provides a convenient way to add and remove event listeners.

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
       "com.gameframex.unity.mono": "1.1.3"
     }
   }
   ```

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.mono": "https://github.com/gameframex/com.gameframex.unity.mono.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.mono.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.
## Usage Examples

### Getting the Component

```csharp
var monoComponent = GameEntry.GetComponent<MonoComponent>();
```

### Registering Lifecycle Listeners

MonoComponent allows registering callbacks for Unity's `MonoBehaviour` lifecycle events. All listeners can be added or removed at any time.

#### Update / FixedUpdate / LateUpdate

These three listeners receive two `float` parameters:
- `elapseSeconds` — scaled delta time
- `realElapseSeconds` — unscaled delta time

```csharp
private void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    // Called every frame
}

private void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
{
    // Called at fixed intervals (physics)
}

private void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
{
    // Called after all Update calls
}

// Register
monoComponent.AddUpdateListener(OnUpdate);
monoComponent.AddFixedUpdateListener(OnFixedUpdate);
monoComponent.AddLateUpdateListener(OnLateUpdate);

// Unregister when no longer needed
monoComponent.RemoveUpdateListener(OnUpdate);
monoComponent.RemoveFixedUpdateListener(OnFixedUpdate);
monoComponent.RemoveLateUpdateListener(OnLateUpdate);
```

#### OnDestroy

```csharp
private void OnDestroyCallback()
{
    // Called when the MonoComponent's GameObject is destroyed
}

monoComponent.AddDestroyListener(OnDestroyCallback);
monoComponent.RemoveDestroyListener(OnDestroyCallback);
```

#### OnApplicationFocus / OnApplicationPause

These listeners receive a `bool` parameter and support a **dual notification pattern** — you can use either direct listeners or the event bus.

**Direct listener approach:**

```csharp
private void OnApplicationFocus(bool isFocus)
{
    // isFocus: true = app gained focus, false = lost focus
}

private void OnApplicationPause(bool isPause)
{
    // isPause: true = app paused, false = resumed
}

monoComponent.AddOnApplicationFocusListener(OnApplicationFocus);
monoComponent.AddOnApplicationPauseListener(OnApplicationPause);

monoComponent.RemoveOnApplicationFocusListener(OnApplicationFocus);
monoComponent.RemoveOnApplicationPauseListener(OnApplicationPause);
```

**Event bus approach** (via `EventComponent`):

```csharp
var eventComponent = GameEntry.GetComponent<EventComponent>();

eventComponent.Subscribe(OnApplicationFocusChangedEventArgs.EventId, OnFocusChanged);
eventComponent.Subscribe(OnApplicationPauseChangedEventArgs.EventId, OnPauseChanged);

private void OnFocusChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationFocusChangedEventArgs)e;
    if (args.IsFocus)
    {
        // App gained focus
    }
}

private void OnPauseChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationPauseChangedEventArgs)e;
    if (args.IsPause)
    {
        // App paused
    }
}
```

### Important Notes

- Listener registration is **thread-safe**.
- Listeners can safely add or remove other listeners during callback invocation (snapshot dispatch).
- Exceptions in callbacks are caught and logged, and do **not** interrupt other listeners.
- Always unregister listeners when no longer needed to avoid memory leaks.

## Documentation & Resources

- Documentation: https://gameframex.doc.alianblank.com
- Repository: https://github.com/GameFrameX/com.gameframex.unity.mono
- Issues: https://github.com/GameFrameX/com.gameframex.unity.mono/issues


## Dependencies

| Package | Description |
|---------|-------------|
| `com.gameframex.unity.event` | 1.1.0 |

## Community & Support

- QQ Group: 467608841 / 233840761

## Changelog

See [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.mono/releases) for changelog.
## License

See [LICENSE](LICENSE.md) for details.

<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Scene

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Project Overview

GameFrameX Scene is a Unity scene management package built on [YooAsset](https://github.com/tuyoogame/YooAsset). It provides async scene loading/unloading with event-driven state notifications, progress tracking, and an active-scene ordering system.

### Features

- **Async Scene Loading** — Load and unload scenes asynchronously with `Task`-based API
- **LoadSceneMode Support** — Switch between `Single` (replace all) and `Additive` (overlay) modes
- **Progress Tracking** — Real-time loading progress events for loading UI
- **Active Scene Ordering** — Priority-based system to control which loaded scene becomes the active scene
- **Auto Camera Refresh** — Automatically refreshes `Camera.main` reference when the active scene changes
- **Event Notifications** — Subscribe to load/unload success, failure, and update events
- **State Queries** — Check whether a scene is loaded, loading, or unloading at any time
- **Editor Inspector** — Custom inspector for `SceneComponent` configuration

---

## Quick Start

### Dependencies

| Package | Version |
|---------|---------|
| `com.gameframex.unity.asset` | >= 2.5.0 |
| `com.gameframex.unity.event` | >= 1.1.0 |

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
       "com.gameframex.unity.scene": "2.2.2"
     }
   }
   ```

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.scene": "https://github.com/gameframex/com.gameframex.unity.scene.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.scene.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.
## Usage Examples

### Setup

Add the `SceneComponent` to your GameEntry GameObject (via `Add Component > GameFrameX > Scene`).

### Load a Scene

```csharp
// Single mode — replaces all current scenes
var handle = await SceneComponent.LoadScene("Assets/Scenes/GameScene.unity");

// Additive mode — loads on top of current scenes
var handle = await SceneComponent.LoadScene(
    "Assets/Scenes/UIOverlay.unity",
    LoadSceneMode.Additive
);
```

### Unload a Scene

```csharp
SceneComponent.UnloadScene("Assets/Scenes/UIOverlay.unity");
```

### Set Scene Priority

Control which loaded scene becomes the active scene by assigning an order value (higher = active):

```csharp
// After loading, set priority to make it the active scene
SceneComponent.SetSceneOrder("Assets/Scenes/GameScene.unity", 10);
```

### Subscribe to Events

Use `EventComponent` to subscribe to scene lifecycle events:

| Event | Trigger |
|-------|---------|
| `LoadSceneSuccessEventArgs` | Scene loaded successfully (includes duration) |
| `LoadSceneFailureEventArgs` | Scene failed to load (includes error message) |
| `LoadSceneUpdateEventArgs` | Loading progress updated |
| `UnloadSceneSuccessEventArgs` | Scene unloaded successfully |
| `UnloadSceneFailureEventArgs` | Scene failed to unload |
| `ActiveSceneChangedEventArgs` | Active scene changed (includes old/new scene) |

```csharp
// Example: subscribe to load success
EventComponent.Subscribe<LoadSceneSuccessEventArgs>(OnSceneLoaded);

void OnSceneLoaded(object sender, LoadSceneSuccessEventArgs e)
{
    Debug.Log($"Scene loaded: {e.SceneAssetName} in {e.Duration:F2}s");
}
```

### Query Scene State

```csharp
bool isLoaded = SceneComponent.SceneIsLoaded("Assets/Scenes/GameScene.unity");
bool isLoading = SceneComponent.SceneIsLoading("Assets/Scenes/GameScene.unity");
bool isUnloading = SceneComponent.SceneIsUnloading("Assets/Scenes/GameScene.unity");
```

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for release history.


## Dependencies

| Package | Description |
|---------|-------------|
| `com.gameframex.unity.asset` | 2.5.0 |
| `com.gameframex.unity.event` | 1.1.0 |

## Documentation & Resources

- [Documentation](https://gameframex.doc.alianblank.com)

## Community & Support

- QQ Group: 467608841 / 233840761
## License

See [LICENSE.md](LICENSE.md) for license information.

<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX Setting

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.setting)](https://github.com/GameFrameX/com.gameframex.unity.setting/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.setting)](https://github.com/GameFrameX/com.gameframex.unity.setting/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Language

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

---

## Features

- Typed key-value storage: `bool`, `int`, `float`, `string`, serializable objects (via JSON)
- Two built-in storage backends:
  - **PlayerPrefsSettingHelper** (default) — uses Unity PlayerPrefs; storage adapters for Douyin, WeChat, Kuaishou, and Alipay mini-games are provided by separate channel packages (see "Supported Platforms")
  - **DefaultSettingHelper** — file-based binary storage at `Application.persistentDataPath`
- Pluggable helper architecture — implement `ISettingHelper` for custom backends
- Auto-load on start, auto-save on shutdown
- Safe parsing with `TryParse` — corrupted values return defaults with a warning log instead of crashing

## Architecture

```
SettingComponent (MonoBehaviour)
  └─ SettingManager (ISettingManager)
       └─ ISettingHelper
            ├─ PlayerPrefsSettingHelper (default)
            └─ DefaultSettingHelper
```

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
       "com.gameframex.unity.setting": "3.0.0"
     }
   }
   ```

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.setting": "https://github.com/gameframex/com.gameframex.unity.setting.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.setting.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.
### Basic Usage

```csharp
// The SettingComponent is automatically available after adding to a GameObject.
// Access it via GameFramework entry or GetComponent.

SettingComponent setting = ...;

// Write settings
setting.SetBool("FullScreen", true);
setting.SetInt("ResolutionWidth", 1920);
setting.SetFloat("Volume", 0.8f);
setting.SetString("PlayerName", "PlayerOne");

// Persist to storage
setting.Save();
```

### Read Settings

```csharp
// With default fallback (returns default if key is missing or value is corrupted)
float volume = setting.GetFloat("Volume", 0.5f);

// Check existence
if (setting.HasSetting("PlayerName"))
{
    string name = setting.GetString("PlayerName");
}
```

### Remove Settings

```csharp
setting.RemoveSetting("PlayerName");
setting.RemoveAllSettings();
```

### Switch Storage Backend

In the Inspector for `SettingComponent`, change **Setting Helper Type Name** to:

- `GameFrameX.Setting.Runtime.PlayerPrefsSettingHelper` (default)
- `GameFrameX.Setting.Runtime.DefaultSettingHelper` (file-based)

Or assign a custom `ISettingHelper` via the **Custom Setting Helper** field.

## Supported Platforms

| Backend | Standard Unity | Douyin | WeChat | Kuaishou | Alipay |
|---------|:-:|:-:|:-:|:-:|:-:|
| PlayerPrefsSettingHelper | PlayerPrefs | TTStorage | WX SDK | KS SDK | Alipay SDK |
| DefaultSettingHelper | File I/O | File I/O | File I/O | File I/O | File I/O |

Since 3.0.0 the four mini-game storage backends ship as separate channel packages: `com.gameframex.unity.setting.wechatminigame`, `com.gameframex.unity.setting.douyinminigame`, `com.gameframex.unity.setting.kuaishouminigame`, and `com.gameframex.unity.setting.alipayminigame` (each gated by asmdef `defineConstraints`: `UNITY_WEBGL + ENABLE_*_MINI_GAME`, so they do not compile in non-mini-game projects). The common `GameFrameX.Setting.Runtime` assembly only depends on `GameFrameX.Runtime`.

### Breaking Changes in 3.0.0

- The mini-game storage adapters embedded in the main package `Runtime/MiniGame/` in 2.x have moved into the four channel packages above. **Mini-game projects must add the channel package matching their platform** (together with the platform mini-game SDK) to `manifest.json`; no other code changes are required — the adapter still self-registers during `SubsystemRegistration` (reference implies registration).
- `ISettingStorageBackend` and `SettingStorageBackendFactory` are now `public`: custom storage backends can implement the contract and call `RegisterMiniGameBackend` directly, without `InternalsVisibleTo` bridging.
- `RegisterMiniGameBackend` keeps the first registration and logs a warning on duplicates (at most one storage backend per runtime environment).
- Standard Unity projects (non-mini-game) need no changes: without a registered channel package the behavior is identical to 2.x (Editor and device both fall back to Unity PlayerPrefs).

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Count` | `int` | Number of stored settings (-1 for PlayerPrefs backend) |

### Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Load()` | `bool` | Load settings from storage |
| `Save()` | `bool` | Save settings to storage |
| `HasSetting(name)` | `bool` | Check if a setting exists |
| `RemoveSetting(name)` | `bool` | Remove a single setting |
| `RemoveAllSettings()` | `void` | Clear all settings |
| `GetAllSettingNames()` | `string[]` | Get all setting key names |
| `GetBool` / `SetBool` | `bool` | Boolean accessors |
| `GetInt` / `SetInt` | `int` | Integer accessors |
| `GetFloat` / `SetFloat` | `float` | Float accessors (invariant culture) |
| `GetString` / `SetString` | `string` | String accessors |
| `GetObject<T>` / `SetObject<T>` | `T` | JSON-serialized object accessors |

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for release history.


## Dependencies

| Package | Description |
|---------|-------------|
| `com.gameframex.unity` | 1.1.1 |

## Documentation & Resources

- [Documentation](https://gameframex.doc.alianblank.com)

## Community & Support

- QQ Group: 467608841 / 233840761
## License

Dual-licensed under [MIT](LICENSE.md) and [Apache-2.0](LICENSE.md).

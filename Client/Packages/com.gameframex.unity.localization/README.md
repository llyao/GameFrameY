<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Localization

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Quick Start](#quick-start)
  - [System Requirements](#system-requirements)
  - [Installation](#installation)
  - [Component Setup](#component-setup)
- [Usage](#usage)
  - [Getting the Component](#getting-the-component)
  - [Language Management](#language-management)
  - [Getting Localized Strings](#getting-localized-strings)
  - [Parameterized Formatting](#parameterized-formatting)
  - [Dictionary Management](#dictionary-management)
  - [Listening for Language Changes](#listening-for-language-changes)
  - [Language Code Constants](#language-code-constants)
- [Editor Inspector](#editor-inspector)
- [Event Reference](#event-reference)
- [Extending the Helper](#extending-the-helper)
- [Dependencies](#dependencies)
- [Documentation & Resources](#documentation--resources)
- [License](#license)

## Project Overview

Game Frame X Localization is a Unity localization package based on the GameFrameX framework. It provides a complete multi-language localization solution with dictionary-based string translation, parameterized formatting (up to 16 typed parameters), runtime language switching with event notifications, automatic fallback to system or default language, and persistent language preference via the Setting system.

## Features

- **Dictionary-based localization** — Add, query, and remove key-value translation pairs at runtime
- **Parameterized string formatting** — Generic `GetString` overloads with 1 to 16 typed parameters, plus a `params object[]` overload
- **Runtime language switching** — Change the active language at any time; all listeners are notified via events
- **System language detection** — Automatically detects the device's system language via `CultureInfo`
- **Language preference persistence** — The selected language and default language are saved through the Setting component
- **Editor mode support** — Optionally override the language in the Unity Editor for testing purposes
- **180+ language code constants** — Built-in `LocalizationCode` static class covering languages from all major regions
- **Custom helper support** — Extend `LocalizationHelperBase` to override system language detection logic

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
       "com.gameframex.unity.localization": "2.3.0"
     }
   }
   ```

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.

2. Add to `manifest.json` dependencies:
   ```json
   {
      "com.gameframex.unity.localization": "https://github.com/gameframex/com.gameframex.unity.localization.git"
   }
   ```
3. Use **Package Manager** in Unity with **Git URL**: `https://github.com/gameframex/com.gameframex.unity.localization.git`
4. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.
## Usage Examples

### Getting the Component

```csharp
using GameFrameX.Localization.Runtime;

// Standard: via GameEntry
var localization = GameEntry.GetComponent<LocalizationComponent>();
```

### Language Management

```csharp
// Get the current active language (returns system language if none has been set)
string currentLang = localization.Language;

// Set the active language (triggers change event, persists to settings)
localization.Language = "zh_CN";
localization.Language = "en_US";
localization.Language = "ja_JP";

// Get the default/fallback language
string defaultLang = localization.DefaultLanguage;

// Set a new default language
localization.DefaultLanguage = "en_US";

// Get the system language detected from the device
string sysLang = localization.SystemLanguage;

// Get the total number of loaded dictionary entries
int count = localization.DictionaryCount;
```

### Getting Localized Strings

```csharp
// Simple key lookup
// If key is not found, returns "<NoKey>{key}"
string okText = localization.GetString("UI.Button.OK");
string cancelText = localization.GetString("UI.Button.Cancel");

// Check if a key exists before getting the value
if (localization.HasRawString("UI.Button.Confirm"))
{
    string confirmText = localization.GetString("UI.Button.Confirm");
}
```

### Parameterized Formatting

The package provides multiple `GetString` overloads for parameterized formatting. Dictionary values should use standard .NET format placeholders (`{0}`, `{1}`, etc.).

```csharp
// Assuming dictionary contains:
// "UI.Message.Welcome" = "Welcome, {0}!"
// "UI.Info.Score"     = "Player {0} scored {1} points at level {2}!"
// "UI.Shop.Buy"       = "Buy {0} x {1} for {2} coins, discount {3:P}, total {4:C}"

// Using params object[] overload
string welcome = localization.GetString("UI.Message.Welcome", playerName);

// Using typed generic overloads (recommended — avoids boxing for value types)
string info = localization.GetString<string, int, int>("UI.Info.Score", playerName, score, level);

// Up to 16 typed parameters
string shopMsg = localization.GetString<string, int, int, float, decimal>(
    "UI.Shop.Buy", itemName, quantity, unitPrice, discount, total);
```

**Error handling:** If the format string's placeholders don't match the provided arguments, the result will begin with `<Error>` followed by diagnostic information. If the key doesn't exist, the result will be `<NoKey>{key}`.

### Dictionary Management

Manage translation entries at runtime:

```csharp
// Add a new entry (returns false if key already exists or key is null/empty)
bool added = localization.AddRawString("UI.Button.NewButton", "New Button");
if (!added)
{
    // Key already existed or was invalid
}

// Check existence
bool exists = localization.HasRawString("UI.Button.NewButton");

// Get the raw (unformatted) value (returns null if not found)
string rawText = localization.GetRawString("UI.Button.NewButton");

// Remove a specific entry (returns false if key didn't exist)
bool removed = localization.RemoveRawString("UI.Button.NewButton");

// Remove all entries
localization.RemoveAllRawStrings();
```

### Listening for Language Changes

When the active language changes (via `localization.Language = ...`), the component fires a `LocalizationLanguageChangeEventArgs` event through the Event system.

```csharp
using GameFrameX.Event.Runtime;
using GameFrameX.Localization.Runtime;

public class MyLanguageHandler : MonoBehaviour
{
    private EventComponent m_EventComponent;
    private LocalizationComponent m_LocalizationComponent;

    void Start()
    {
        m_EventComponent = GameEntry.GetComponent<EventComponent>();
        m_LocalizationComponent = GameEntry.GetComponent<LocalizationComponent>();

        // Subscribe to language change event
        m_EventComponent.Subscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    void OnDestroy()
    {
        // Unsubscribe when done
        m_EventComponent.Unsubscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    private void OnLanguageChanged(object sender, GameEventArgs e)
    {
        var args = (LocalizationLanguageChangeEventArgs)e;

        // Old language code (e.g. "zh_CN")
        string oldLang = args.OldLanguage;

        // New language code (e.g. "en_US")
        string newLang = args.Language;

        // Refresh all UI text here
        RefreshAllText();
    }

    private void RefreshAllText()
    {
        // Re-fetch all localized strings with the new language
        // e.g. myText.text = m_LocalizationComponent.GetString("UI.MainMenu.Title");
    }
}
```

### Language Code Constants

The `LocalizationCode` static class provides 180+ predefined language code constants, organized by region. These follow the `language_REGION` format (e.g. `zh_CN`, `en_US`).

```csharp
using GameFrameX.Localization.Runtime;

// East Asian
localization.Language = LocalizationCode.ChineseSimplified;    // "zh_CN"
localization.Language = LocalizationCode.ChineseTraditionalTW; // "zh_TW"
localization.Language = LocalizationCode.Japanese;             // "ja_JP"
localization.Language = LocalizationCode.Korean;               // "ko_KR"

// English variants
localization.Language = LocalizationCode.English;    // "en_US"
localization.Language = LocalizationCode.EnglishUK;  // "en_GB"
localization.Language = LocalizationCode.EnglishAU;  // "en_AU"

// European
localization.Language = LocalizationCode.French;     // "fr_FR"
localization.Language = LocalizationCode.German;     // "de_DE"
localization.Language = LocalizationCode.Italian;    // "it_IT"
localization.Language = LocalizationCode.Spanish;    // "es_ES"
localization.Language = LocalizationCode.Portuguese; // "pt_PT"
localization.Language = LocalizationCode.Russian;    // "ru_RU"

// Middle East & West Asian
localization.Language = LocalizationCode.Arabic;  // "ar_SA"
localization.Language = LocalizationCode.Hebrew;   // "he_IL"
localization.Language = LocalizationCode.Persian;  // "fa_IR"
localization.Language = LocalizationCode.Turkish;  // "tr_TR"

// South & Southeast Asian
localization.Language = LocalizationCode.Hindi;      // "hi_IN"
localization.Language = LocalizationCode.Thai;       // "th_TH"
localization.Language = LocalizationCode.Vietnamese; // "vi_VN"
localization.Language = LocalizationCode.Indonesian; // "id_ID"

// African
localization.Language = LocalizationCode.Swahili; // "sw_TZ"

// Oceanian
localization.Language = LocalizationCode.Maori; // "mi_NZ"
```

Many languages have regional variants available (e.g. `SpanishMX`, `PortugueseBR`, `ArabicEG`, `FrenchCA`, etc.). See `LocalizationCode.cs` for the full list.

## Editor Inspector

The `LocalizationComponentInspector` provides a custom Inspector with the following options:

| Field | Description |
|-------|-------------|
| **Localization Helper** | Select the helper type or assign a custom helper instance |
| **Default Language** | Fallback language code |
| **Enable Editor Mode** | When checked, uses Editor Language instead of system language in Play Mode |
| **Editor Language** | Language code used in Editor Mode |
| **Language** (play mode only) | Displays the current active language at runtime |
| **System Language** (play mode only) | Displays the detected system language at runtime |

## Event Reference

| Event | Class | Trigger | Key Fields |
|-------|-------|---------|------------|
| Language changed | `LocalizationLanguageChangeEventArgs` | `Language` property is set | `OldLanguage`, `Language` |
| Dictionary load success | `LoadDictionarySuccessEventArgs` | Dictionary asset loaded | `DictionaryAssetName`, `Duration`, `UserData` |
| Dictionary load failure | `LoadDictionaryFailureEventArgs` | Dictionary asset failed to load | `DictionaryAssetName`, `ErrorMessage`, `UserData` |
| Dictionary load progress | `LoadDictionaryUpdateEventArgs` | Dictionary asset loading progress | `DictionaryAssetName`, `Progress`, `UserData` |

All event classes use the GameFrameX reference pool for zero-alloc event raising.

## Extending the Helper

To customize system language detection (e.g. reading from a backend service or a custom config), create a class that extends `LocalizationHelperBase`:

```csharp
using GameFrameX.Localization.Runtime;

public class CustomLocalizationHelper : LocalizationHelperBase
{
    public override string SystemLanguage
    {
        get
        {
            // Your custom logic here
            // e.g. read from a config file, remote API, etc.
            return "en_US";
        }
    }
}
```

Then assign it in the Inspector's **Localization Helper** field, or configure `LocalizationOptions.LocalizationHelperFactory` with a factory returning your helper instance.

## Dependencies

| Package | Version | Description |
|---------|---------|-------------|
| `com.gameframex.unity` | 1.1.1+ | GameFrameX core framework |
| `com.gameframex.unity.asset` | 1.0.6+ | Asset management module |
| `com.gameframex.unity.event` | 1.0.0+ | Event system module |
| `com.gameframex.unity.setting` | 1.5.0+ | Setting persistence module |

## Documentation & Resources

- Documentation: https://gameframex.doc.alianblank.com
- Repository: https://github.com/GameFrameX/com.gameframex.unity.localization
- Issues: https://github.com/GameFrameX/com.gameframex.unity.localization/issues


## Community & Support

- QQ Group: 467608841 / 233840761

## Changelog

See [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.localization/releases) for changelog.
## License

See [LICENSE.md](LICENSE.md) for license information.

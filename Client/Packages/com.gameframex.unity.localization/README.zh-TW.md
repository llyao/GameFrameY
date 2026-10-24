<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Localization

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 目錄

- [專案簡介](#專案簡介)
- [功能特性](#功能特性)
- [快速開始](#快速開始)
  - [系統要求](#系統要求)
  - [安裝](#安裝)
  - [組件配置](#組件配置)
- [使用指南](#使用指南)
  - [獲取組件](#獲取組件)
  - [語言管理](#語言管理)
  - [獲取本地化字串](#獲取本地化字串)
  - [參數化格式化](#參數化格式化)
  - [字典管理](#字典管理)
  - [監聽語言變更](#監聽語言變更)
  - [語言代碼常量](#語言代碼常量)
- [編輯器 Inspector](#編輯器-inspector)
- [事件參考](#事件參考)
- [擴展 Helper](#擴展-helper)
- [依賴項](#依賴項)
- [文檔與資源](#文檔與資源)
- [開源協議](#開源協議)

## 專案簡介

Game Frame X Localization 是一個基於 GameFrameX 框架的 Unity 本地化功能包。它提供基於字典的字串翻譯、參數化格式化（最多支援 16 個型別化參數）、執行時語言切換與事件通知、自動回退到系統語言或預設語言，以及透過 Setting 系統持久化語言偏好等完整的多語言本地化解決方案。

## 功能特性

- **基於字典的本地化** — 執行時新增、查詢和刪除鍵值對翻譯條目
- **參數化字串格式化** — 提供 1 到 16 個型別化參數的泛型 `GetString` 多載，以及 `params object[]` 多載
- **執行時語言切換** — 隨時切換當前語言，透過事件通知所有監聽者
- **系統語言偵測** — 透過 `CultureInfo` 自動偵測裝置系統語言
- **語言偏好持久化** — 選定的語言和預設語言透過 Setting 組件自動儲存
- **編輯器模式支援** — 可在 Unity 編輯器中覆蓋語言設定，方便測試
- **180+ 語言代碼常量** — 內建 `LocalizationCode` 靜態類別，覆蓋全球主要地區的語言
- **自訂 Helper 支援** — 繼承 `LocalizationHelperBase` 即可覆蓋系統語言偵測邏輯

## 快速開始

### 安裝

選擇以下任一方式：

1. 編輯 Unity 專案的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
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

   `scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。

2. 直接在 `manifest.json` 的 `dependencies` 節點下添加以下內容：
   ```json
   {
      "com.gameframex.unity.localization": "https://github.com/gameframex/com.gameframex.unity.localization.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加庫，地址為：`https://github.com/gameframex/com.gameframex.unity.localization.git`
4. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動載入識別。
## 使用指南

### 獲取組件

```csharp
using GameFrameX.Localization.Runtime;

// 標準方式：透過 GameEntry 獲取
var localization = GameEntry.GetComponent<LocalizationComponent>();
```

### 語言管理

```csharp
// 獲取當前語言（未設定時返回系統語言）
string currentLang = localization.Language;

// 設定當前語言（觸發變更事件，自動持久化）
localization.Language = "zh_TW";
localization.Language = "en_US";
localization.Language = "ja_JP";

// 獲取預設/回退語言
string defaultLang = localization.DefaultLanguage;

// 設定新的預設語言
localization.DefaultLanguage = "en_US";

// 獲取裝置偵測到的系統語言
string sysLang = localization.SystemLanguage;

// 獲取已載入的字典條目總數
int count = localization.DictionaryCount;
```

### 獲取本地化字串

```csharp
// 簡單鍵查找
// 如果 key 不存在，返回 "<NoKey>{key}"
string okText = localization.GetString("UI.Button.OK");
string cancelText = localization.GetString("UI.Button.Cancel");

// 獲取前先檢查 key 是否存在
if (localization.HasRawString("UI.Button.Confirm"))
{
    string confirmText = localization.GetString("UI.Button.Confirm");
}
```

### 參數化格式化

本包提供多個 `GetString` 多載用於參數化格式化。字典值應使用標準 .NET 格式佔位符（`{0}`、`{1}` 等）。

```csharp
// 假設字典中包含：
// "UI.Message.Welcome" = "歡迎，{0}！"
// "UI.Info.Score"     = "玩家 {0} 在第 {2} 關獲得了 {1} 分！"
// "UI.Shop.Buy"       = "購買 {0} x {1}，單價 {2} 金幣，折扣 {3:P}，合計 {4:C}"

// 使用 params object[] 多載
string welcome = localization.GetString("UI.Message.Welcome", playerName);

// 使用泛型多載（推薦——避免值型別的裝箱）
string info = localization.GetString<string, int, int>("UI.Info.Score", playerName, score, level);

// 最多支援 16 個型別化參數
string shopMsg = localization.GetString<string, int, int, float, decimal>(
    "UI.Shop.Buy", itemName, quantity, unitPrice, discount, total);
```

**錯誤處理：** 如果格式字串的佔位符與提供的參數不匹配，結果將以 `<Error>` 開頭並附帶診斷資訊。如果 key 不存在，結果為 `<NoKey>{key}`。

### 字典管理

在執行時管理翻譯條目：

```csharp
// 新增新條目（key 已存在或為 null/空時返回 false）
bool added = localization.AddRawString("UI.Button.NewButton", "新按鈕");
if (!added)
{
    // key 已存在或無效
}

// 檢查是否存在
bool exists = localization.HasRawString("UI.Button.NewButton");

// 獲取原始（未格式化的）值（未找到時返回 null）
string rawText = localization.GetRawString("UI.Button.NewButton");

// 移除指定條目（key 不存在時返回 false）
bool removed = localization.RemoveRawString("UI.Button.NewButton");

// 移除所有條目
localization.RemoveAllRawStrings();
```

### 監聽語言變更

當活躍語言發生變化時（透過 `localization.Language = ...`），組件會透過事件系統觸發 `LocalizationLanguageChangeEventArgs` 事件。

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

        // 訂閱語言變更事件
        m_EventComponent.Subscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    void OnDestroy()
    {
        // 取消訂閱
        m_EventComponent.Unsubscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    private void OnLanguageChanged(object sender, GameEventArgs e)
    {
        var args = (LocalizationLanguageChangeEventArgs)e;

        // 變更前的語言代碼（如 "zh_TW"）
        string oldLang = args.OldLanguage;

        // 變更後的語言代碼（如 "en_US"）
        string newLang = args.Language;

        // 在此處重新整理所有 UI 文字
        RefreshAllText();
    }

    private void RefreshAllText()
    {
        // 使用新語言重新獲取所有本地化字串
        // 如：myText.text = m_LocalizationComponent.GetString("UI.MainMenu.Title");
    }
}
```

### 語言代碼常量

`LocalizationCode` 靜態類別提供了 180+ 個預定義語言代碼常量，按地區分類組織。格式遵循 `語言_地區` 標準（如 `zh_TW`、`en_US`）。

```csharp
using GameFrameX.Localization.Runtime;

// 東亞
localization.Language = LocalizationCode.ChineseSimplified;    // "zh_CN"
localization.Language = LocalizationCode.ChineseTraditionalTW; // "zh_TW"
localization.Language = LocalizationCode.Japanese;             // "ja_JP"
localization.Language = LocalizationCode.Korean;               // "ko_KR"

// 英語變體
localization.Language = LocalizationCode.English;    // "en_US"
localization.Language = LocalizationCode.EnglishUK;  // "en_GB"
localization.Language = LocalizationCode.EnglishAU;  // "en_AU"

// 歐洲語言
localization.Language = LocalizationCode.French;     // "fr_FR"
localization.Language = LocalizationCode.German;     // "de_DE"
localization.Language = LocalizationCode.Italian;    // "it_IT"
localization.Language = LocalizationCode.Spanish;    // "es_ES"
localization.Language = LocalizationCode.Portuguese; // "pt_PT"
localization.Language = LocalizationCode.Russian;    // "ru_RU"

// 中東和西亞
localization.Language = LocalizationCode.Arabic;  // "ar_SA"
localization.Language = LocalizationCode.Hebrew;   // "he_IL"
localization.Language = LocalizationCode.Persian;  // "fa_IR"
localization.Language = LocalizationCode.Turkish;  // "tr_TR"

// 南亞和東南亞
localization.Language = LocalizationCode.Hindi;      // "hi_IN"
localization.Language = LocalizationCode.Thai;       // "th_TH"
localization.Language = LocalizationCode.Vietnamese; // "vi_VN"
localization.Language = LocalizationCode.Indonesian; // "id_ID"

// 非洲
localization.Language = LocalizationCode.Swahili; // "sw_TZ"

// 大洋洲
localization.Language = LocalizationCode.Maori; // "mi_NZ"
```

許多語言還有地區變體（如 `SpanishMX`、`PortugueseBR`、`ArabicEG`、`FrenchCA` 等）。完整列表請參見 `LocalizationCode.cs`。

## 編輯器 Inspector

`LocalizationComponentInspector` 提供了自訂 Inspector，包含以下選項：

| 欄位 | 說明 |
|------|------|
| **Localization Helper** | 選擇 Helper 型別或分配自訂 Helper 實例 |
| **Default Language** | 回退語言代碼 |
| **Enable Editor Mode** | 勾選後，在 Play Mode 中使用 Editor Language 替代系統語言 |
| **Editor Language** | 編輯器模式下使用的語言代碼 |
| **Language**（僅執行時） | 顯示當前執行時的活躍語言 |
| **System Language**（僅執行時） | 顯示偵測到的系統語言 |

## 事件參考

| 事件 | 類別 | 觸發條件 | 關鍵欄位 |
|------|------|----------|----------|
| 語言變更 | `LocalizationLanguageChangeEventArgs` | 設定 `Language` 屬性時 | `OldLanguage`、`Language` |
| 字典載入成功 | `LoadDictionarySuccessEventArgs` | 字典資源載入完成 | `DictionaryAssetName`、`Duration`、`UserData` |
| 字典載入失敗 | `LoadDictionaryFailureEventArgs` | 字典資源載入失敗 | `DictionaryAssetName`、`ErrorMessage`、`UserData` |
| 字典載入進度 | `LoadDictionaryUpdateEventArgs` | 字典資源載入進度更新 | `DictionaryAssetName`、`Progress`、`UserData` |

所有事件類別均使用 GameFrameX 參照池，實現零分配事件觸發。

## 擴展 Helper

如需自訂系統語言偵測（例如從後端服務或自訂配置讀取），建立一個繼承 `LocalizationHelperBase` 的類別：

```csharp
using GameFrameX.Localization.Runtime;

public class CustomLocalizationHelper : LocalizationHelperBase
{
    public override string SystemLanguage
    {
        get
        {
            // 在此實作自訂邏輯
            // 例如從配置檔案、遠端 API 等讀取
            return "en_US";
        }
    }
}
```

然後在 Inspector 的 **Localization Helper** 欄位中分配，或透過 `LocalizationOptions.LocalizationHelperFactory` 配置返回自訂輔助器實例的工廠。

## 依賴項

| 套件 | 版本 | 說明 |
|------|------|------|
| `com.gameframex.unity` | 1.1.1+ | GameFrameX 核心框架 |
| `com.gameframex.unity.asset` | 1.0.6+ | 資源管理模組 |
| `com.gameframex.unity.event` | 1.0.0+ | 事件系統模組 |
| `com.gameframex.unity.setting` | 1.5.0+ | 設定持久化模組 |

## 文檔與資源

- 文檔地址: https://gameframex.doc.alianblank.com
- 倉庫地址: https://github.com/GameFrameX/com.gameframex.unity.localization
- 問題回饋: https://github.com/GameFrameX/com.gameframex.unity.localization/issues


## 依賴

| 套件 | 說明 |
|------|------|
| `com.gameframex.unity` | 1.1.1 |
| `com.gameframex.unity.asset` | 1.0.6 |
| `com.gameframex.unity.event` | 1.0.0 |
| `com.gameframex.unity.setting` | 1.5.0 |

## 社區與支援

- QQ群: 467608841 / 233840761

## 更新日誌

查看 [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.localization/releases) 了解更新日誌。
## 開源協議

本專案遵循 MIT 許可證。詳細資訊請查看 [LICENSE](LICENSE.md) 檔案。

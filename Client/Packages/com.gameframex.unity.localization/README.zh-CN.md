<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Localization

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 目录

- [项目简介](#项目简介)
- [功能特性](#功能特性)
- [快速开始](#快速开始)
  - [系统要求](#系统要求)
  - [安装](#安装)
  - [组件配置](#组件配置)
- [使用指南](#使用指南)
  - [获取组件](#获取组件)
  - [语言管理](#语言管理)
  - [获取本地化字符串](#获取本地化字符串)
  - [参数化格式化](#参数化格式化)
  - [字典管理](#字典管理)
  - [监听语言变更](#监听语言变更)
  - [语言代码常量](#语言代码常量)
- [编辑器 Inspector](#编辑器-inspector)
- [事件参考](#事件参考)
- [扩展 Helper](#扩展-helper)
- [依赖项](#依赖项)
- [文档与资源](#文档与资源)
- [开源协议](#开源协议)

## 项目简介

Game Frame X Localization 是一个基于 GameFrameX 框架的 Unity 本地化功能包。它提供基于字典的字符串翻译、参数化格式化（最多支持 16 个类型化参数）、运行时语言切换与事件通知、自动回退到系统语言或默认语言，以及通过 Setting 系统持久化语言偏好等完整的多语言本地化解决方案。

## 功能特性

- **基于字典的本地化** — 运行时添加、查询和删除键值对翻译条目
- **参数化字符串格式化** — 提供 1 到 16 个类型化参数的泛型 `GetString` 重载，以及 `params object[]` 重载
- **运行时语言切换** — 随时切换当前语言，通过事件通知所有监听者
- **系统语言检测** — 通过 `CultureInfo` 自动检测设备系统语言
- **语言偏好持久化** — 选定的语言和默认语言通过 Setting 组件自动保存
- **编辑器模式支持** — 可在 Unity 编辑器中覆盖语言设置，方便测试
- **180+ 语言代码常量** — 内置 `LocalizationCode` 静态类，覆盖全球主要地区的语言
- **自定义 Helper 支持** — 继承 `LocalizationHelperBase` 即可覆盖系统语言检测逻辑

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
       "com.gameframex.unity.localization": "2.3.0"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

2. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {
      "com.gameframex.unity.localization": "https://github.com/gameframex/com.gameframex.unity.localization.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为：`https://github.com/gameframex/com.gameframex.unity.localization.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。
## 使用指南

### 获取组件

```csharp
using GameFrameX.Localization.Runtime;

// 标准方式：通过 GameEntry 获取
var localization = GameEntry.GetComponent<LocalizationComponent>();
```

### 语言管理

```csharp
// 获取当前语言（未设置时返回系统语言）
string currentLang = localization.Language;

// 设置当前语言（触发变更事件，自动持久化）
localization.Language = "zh_CN";
localization.Language = "en_US";
localization.Language = "ja_JP";

// 获取默认/回退语言
string defaultLang = localization.DefaultLanguage;

// 设置新的默认语言
localization.DefaultLanguage = "en_US";

// 获取设备检测到的系统语言
string sysLang = localization.SystemLanguage;

// 获取已加载的字典条目总数
int count = localization.DictionaryCount;
```

### 获取本地化字符串

```csharp
// 简单键查找
// 如果 key 不存在，返回 "<NoKey>{key}"
string okText = localization.GetString("UI.Button.OK");
string cancelText = localization.GetString("UI.Button.Cancel");

// 获取前先检查 key 是否存在
if (localization.HasRawString("UI.Button.Confirm"))
{
    string confirmText = localization.GetString("UI.Button.Confirm");
}
```

### 参数化格式化

本包提供多个 `GetString` 重载用于参数化格式化。字典值应使用标准 .NET 格式占位符（`{0}`、`{1}` 等）。

```csharp
// 假设字典中包含：
// "UI.Message.Welcome" = "欢迎，{0}！"
// "UI.Info.Score"     = "玩家 {0} 在第 {2} 关获得了 {1} 分！"
// "UI.Shop.Buy"       = "购买 {0} x {1}，单价 {2} 金币，折扣 {3:P}，合计 {4:C}"

// 使用 params object[] 重载
string welcome = localization.GetString("UI.Message.Welcome", playerName);

// 使用泛型重载（推荐——避免值类型的装箱）
string info = localization.GetString<string, int, int>("UI.Info.Score", playerName, score, level);

// 最多支持 16 个类型化参数
string shopMsg = localization.GetString<string, int, int, float, decimal>(
    "UI.Shop.Buy", itemName, quantity, unitPrice, discount, total);
```

**错误处理：** 如果格式字符串的占位符与提供的参数不匹配，结果将以 `<Error>` 开头并附带诊断信息。如果 key 不存在，结果为 `<NoKey>{key}`。

### 字典管理

在运行时管理翻译条目：

```csharp
// 添加新条目（key 已存在或为 null/空时返回 false）
bool added = localization.AddRawString("UI.Button.NewButton", "新按钮");
if (!added)
{
    // key 已存在或无效
}

// 检查是否存在
bool exists = localization.HasRawString("UI.Button.NewButton");

// 获取原始（未格式化的）值（未找到时返回 null）
string rawText = localization.GetRawString("UI.Button.NewButton");

// 移除指定条目（key 不存在时返回 false）
bool removed = localization.RemoveRawString("UI.Button.NewButton");

// 移除所有条目
localization.RemoveAllRawStrings();
```

### 监听语言变更

当活跃语言发生变化时（通过 `localization.Language = ...`），组件会通过事件系统触发 `LocalizationLanguageChangeEventArgs` 事件。

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

        // 订阅语言变更事件
        m_EventComponent.Subscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    void OnDestroy()
    {
        // 取消订阅
        m_EventComponent.Unsubscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    private void OnLanguageChanged(object sender, GameEventArgs e)
    {
        var args = (LocalizationLanguageChangeEventArgs)e;

        // 变更前的语言代码（如 "zh_CN"）
        string oldLang = args.OldLanguage;

        // 变更后的语言代码（如 "en_US"）
        string newLang = args.Language;

        // 在此处刷新所有 UI 文本
        RefreshAllText();
    }

    private void RefreshAllText()
    {
        // 使用新语言重新获取所有本地化字符串
        // 如：myText.text = m_LocalizationComponent.GetString("UI.MainMenu.Title");
    }
}
```

### 语言代码常量

`LocalizationCode` 静态类提供了 180+ 个预定义语言代码常量，按地区分类组织。格式遵循 `语言_地区` 标准（如 `zh_CN`、`en_US`）。

```csharp
using GameFrameX.Localization.Runtime;

// 东亚
localization.Language = LocalizationCode.ChineseSimplified;    // "zh_CN"
localization.Language = LocalizationCode.ChineseTraditionalTW; // "zh_TW"
localization.Language = LocalizationCode.Japanese;             // "ja_JP"
localization.Language = LocalizationCode.Korean;               // "ko_KR"

// 英语变体
localization.Language = LocalizationCode.English;    // "en_US"
localization.Language = LocalizationCode.EnglishUK;  // "en_GB"
localization.Language = LocalizationCode.EnglishAU;  // "en_AU"

// 欧洲语言
localization.Language = LocalizationCode.French;     // "fr_FR"
localization.Language = LocalizationCode.German;     // "de_DE"
localization.Language = LocalizationCode.Italian;    // "it_IT"
localization.Language = LocalizationCode.Spanish;    // "es_ES"
localization.Language = LocalizationCode.Portuguese; // "pt_PT"
localization.Language = LocalizationCode.Russian;    // "ru_RU"

// 中东和西亚
localization.Language = LocalizationCode.Arabic;  // "ar_SA"
localization.Language = LocalizationCode.Hebrew;   // "he_IL"
localization.Language = LocalizationCode.Persian;  // "fa_IR"
localization.Language = LocalizationCode.Turkish;  // "tr_TR"

// 南亚和东南亚
localization.Language = LocalizationCode.Hindi;      // "hi_IN"
localization.Language = LocalizationCode.Thai;       // "th_TH"
localization.Language = LocalizationCode.Vietnamese; // "vi_VN"
localization.Language = LocalizationCode.Indonesian; // "id_ID"

// 非洲
localization.Language = LocalizationCode.Swahili; // "sw_TZ"

// 大洋洲
localization.Language = LocalizationCode.Maori; // "mi_NZ"
```

许多语言还有地区变体（如 `SpanishMX`、`PortugueseBR`、`ArabicEG`、`FrenchCA` 等）。完整列表请参见 `LocalizationCode.cs`。

## 编辑器 Inspector

`LocalizationComponentInspector` 提供了自定义 Inspector，包含以下选项：

| 字段 | 说明 |
|------|------|
| **Localization Helper** | 选择 Helper 类型或分配自定义 Helper 实例 |
| **Default Language** | 回退语言代码 |
| **Enable Editor Mode** | 勾选后，在 Play Mode 中使用 Editor Language 替代系统语言 |
| **Editor Language** | 编辑器模式下使用的语言代码 |
| **Language**（仅运行时） | 显示当前运行时的活跃语言 |
| **System Language**（仅运行时） | 显示检测到的系统语言 |

## 事件参考

| 事件 | 类名 | 触发条件 | 关键字段 |
|------|------|----------|----------|
| 语言变更 | `LocalizationLanguageChangeEventArgs` | 设置 `Language` 属性时 | `OldLanguage`、`Language` |
| 字典加载成功 | `LoadDictionarySuccessEventArgs` | 字典资源加载完成 | `DictionaryAssetName`、`Duration`、`UserData` |
| 字典加载失败 | `LoadDictionaryFailureEventArgs` | 字典资源加载失败 | `DictionaryAssetName`、`ErrorMessage`、`UserData` |
| 字典加载进度 | `LoadDictionaryUpdateEventArgs` | 字典资源加载进度更新 | `DictionaryAssetName`、`Progress`、`UserData` |

所有事件类均使用 GameFrameX 引用池，实现零分配事件触发。

## 扩展 Helper

如需自定义系统语言检测（例如从后端服务或自定义配置读取），创建一个继承 `LocalizationHelperBase` 的类：

```csharp
using GameFrameX.Localization.Runtime;

public class CustomLocalizationHelper : LocalizationHelperBase
{
    public override string SystemLanguage
    {
        get
        {
            // 在此实现自定义逻辑
            // 例如从配置文件、远程 API 等读取
            return "en_US";
        }
    }
}
```

然后在 Inspector 的 **Localization Helper** 字段中分配，或通过 `LocalizationOptions.LocalizationHelperFactory` 配置返回自定义辅助器实例的工厂。

## 依赖

| 包名 | 版本 | 说明 |
|------|------|------|
| `com.gameframex.unity` | 1.1.1+ | GameFrameX 核心框架 |
| `com.gameframex.unity.asset` | 1.0.6+ | 资源管理模块 |
| `com.gameframex.unity.event` | 1.0.0+ | 事件系统模块 |
| `com.gameframex.unity.setting` | 1.5.0+ | 设置持久化模块 |

## 文档与资源

- 文档地址: https://gameframex.doc.alianblank.com
- 仓库地址: https://github.com/GameFrameX/com.gameframex.unity.localization
- 问题反馈: https://github.com/GameFrameX/com.gameframex.unity.localization/issues


## 社区与支持

- QQ群: 467608841 / 233840761

## 更新日志

查看 [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.localization/releases) 了解更新日志。
## 开源协议

本项目遵循 MIT 许可证。详细信息请查看 [LICENSE](LICENSE.md) 文件。

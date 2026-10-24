<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX Setting

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.setting)](https://github.com/GameFrameX/com.gameframex.unity.setting/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.setting)](https://github.com/GameFrameX/com.gameframex.unity.setting/releases)
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

## 功能特性

- 类型化键值存储：`bool`、`int`、`float`、`string`、可序列化对象（JSON）
- 两种内置存储后端：
  - **PlayerPrefsSettingHelper**（默认）— 使用 Unity PlayerPrefs；抖音、微信、快手、支付宝小游戏平台的存储适配由独立渠道包提供（见下文"平台支持"）
  - **DefaultSettingHelper** — 基于 `Application.persistentDataPath` 的文件二进制存储
- 可插拔 Helper 架构 — 实现 `ISettingHelper` 接口即可自定义后端
- 启动时自动加载，关闭时自动保存
- 安全解析（`TryParse`）— 数据损坏时返回默认值并记录警告，不会崩溃

## 架构概览

```
SettingComponent (MonoBehaviour)
  └─ SettingManager (ISettingManager)
       └─ ISettingHelper
            ├─ PlayerPrefsSettingHelper (默认)
            └─ DefaultSettingHelper
```

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
       "com.gameframex.unity.setting": "3.0.0"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。

2. 直接在 `manifest.json` 的 `dependencies` 节点下添加以下内容：
   ```json
   {
      "com.gameframex.unity.setting": "https://github.com/gameframex/com.gameframex.unity.setting.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加库，地址为：`https://github.com/gameframex/com.gameframex.unity.setting.git`
4. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。
### 基本使用

```csharp
// SettingComponent 添加到 GameObject 后自动可用。
// 通过 GameFramework 入口或 GetComponent 获取。

SettingComponent setting = ...;

// 写入设置
setting.SetBool("FullScreen", true);
setting.SetInt("ResolutionWidth", 1920);
setting.SetFloat("Volume", 0.8f);
setting.SetString("PlayerName", "PlayerOne");

// 持久化到存储
setting.Save();
```

### 读取设置

```csharp
// 带默认值回退（键不存在或值损坏时返回默认值）
float volume = setting.GetFloat("Volume", 0.5f);

// 检查是否存在
if (setting.HasSetting("PlayerName"))
{
    string name = setting.GetString("PlayerName");
}
```

### 删除设置

```csharp
setting.RemoveSetting("PlayerName");
setting.RemoveAllSettings();
```

### 切换存储后端

在 `SettingComponent` 的 Inspector 面板中，修改 **Setting Helper Type Name**：

- `GameFrameX.Setting.Runtime.PlayerPrefsSettingHelper`（默认）
- `GameFrameX.Setting.Runtime.DefaultSettingHelper`（文件存储）

也可以通过 **Custom Setting Helper** 字段指定自定义的 `ISettingHelper` 实现。

## 平台支持

| 后端 | 标准 Unity | 抖音 | 微信 | 快手 | 支付宝 |
|------|:-:|:-:|:-:|:-:|:-:|
| PlayerPrefsSettingHelper | PlayerPrefs | TTStorage | WX SDK | KS SDK | Alipay SDK |
| DefaultSettingHelper | 文件 I/O | 文件 I/O | 文件 I/O | 文件 I/O | 文件 I/O |

自 3.0.0 起，四个小游戏存储后端拆分为独立渠道包：`com.gameframex.unity.setting.wechatminigame`、`com.gameframex.unity.setting.douyinminigame`、`com.gameframex.unity.setting.kuaishouminigame`、`com.gameframex.unity.setting.alipayminigame`（各自携带 asmdef `defineConstraints`：`UNITY_WEBGL + ENABLE_*_MINI_GAME`，非小游戏工程不编译）。公共程序集 `GameFrameX.Setting.Runtime` 仅依赖 `GameFrameX.Runtime`。

### 3.0.0 破坏性变更与迁移

- 2.x 内嵌在主包 `Runtime/MiniGame/` 的小游戏存储适配器已移至上述 4 个渠道包。**小游戏工程需在 manifest 按平台加装对应渠道包**（连同平台小游戏 SDK）；除此之外无需代码改动——适配器仍在 `SubsystemRegistration` 阶段自注册，引用即注册。
- `ISettingStorageBackend` / `SettingStorageBackendFactory` 契约改为 `public`：自定义存储后端可直接实现契约并调用 `RegisterMiniGameBackend` 注册，不再依赖 `InternalsVisibleTo` 桥接。
- `RegisterMiniGameBackend` 重复注册时保留首个并输出 Warning（存储后端为"每运行环境至多一个"的互斥语义）。
- 非小游戏的标准 Unity 工程无需任何改动：未注册渠道包时行为与 2.x 完全一致（Editor 与真机均回落 Unity PlayerPrefs）。

## API 参考

### 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Count` | `int` | 已存储的设置数量（PlayerPrefs 后端返回 -1） |

### 方法

| 方法 | 返回值 | 说明 |
|------|--------|------|
| `Load()` | `bool` | 从存储加载设置 |
| `Save()` | `bool` | 保存设置到存储 |
| `HasSetting(name)` | `bool` | 检查设置是否存在 |
| `RemoveSetting(name)` | `bool` | 移除单个设置 |
| `RemoveAllSettings()` | `void` | 清空所有设置 |
| `GetAllSettingNames()` | `string[]` | 获取所有设置的键名 |
| `GetBool` / `SetBool` | `bool` | 布尔类型存取 |
| `GetInt` / `SetInt` | `int` | 整数类型存取 |
| `GetFloat` / `SetFloat` | `float` | 浮点数类型存取（不变区域性） |
| `GetString` / `SetString` | `string` | 字符串类型存取 |
| `GetObject<T>` / `SetObject<T>` | `T` | JSON 序列化对象存取 |

## 更新日志

详见 [CHANGELOG.md](CHANGELOG.md)。


## 依赖

| 包 | 说明 |
|----|------|
| `com.gameframex.unity` | 1.1.1 |

## 文档与资源

- [官方文档](https://gameframex.doc.alianblank.com)

## 社区与支持

- QQ群: 467608841 / 233840761
## 开源协议

双重许可：[MIT](LICENSE.md) 和 [Apache-2.0](LICENSE.md)。

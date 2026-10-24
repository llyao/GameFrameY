<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Scene

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 項目簡介

GameFrameX Scene 是基於 [YooAsset](https://github.com/tuyoogame/YooAsset) 的 Unity 場景管理套件。提供非同步場景載入/卸載、事件驅動的狀態通知、進度追蹤和活躍場景排序系統。

### 功能特性

- **非同步場景載入** — 基於 `Task` 的非同步 API，支援載入和卸載場景
- **LoadSceneMode 支援** — 支援 `Single`（替換全部）和 `Additive`（疊加）兩種模式
- **進度追蹤** — 即時載入進度事件，可用於載入介面
- **活躍場景排序** — 基於優先級的排序系統，控制哪個已載入場景成為活躍場景
- **自動相機刷新** — 活躍場景切換時自動刷新 `Camera.main` 引用
- **事件通知** — 訂閱載入/卸載的成功、失敗和更新事件
- **狀態查詢** — 隨時查詢場景是否已載入、正在載入或正在卸載
- **編輯器擴展** — `SceneComponent` 的自訂 Inspector 面板

---

## 快速開始

### 依賴

| 套件 | 版本 |
|------|------|
| `com.gameframex.unity.asset` | >= 2.5.0 |
| `com.gameframex.unity.event` | >= 1.1.0 |

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
       "com.gameframex.unity.scene": "2.2.2"
     }
   }
   ```

   `scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。

2. 直接在 `manifest.json` 的 `dependencies` 節點下添加以下內容：
   ```json
   {
      "com.gameframex.unity.scene": "https://github.com/gameframex/com.gameframex.unity.scene.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加庫，地址為：`https://github.com/gameframex/com.gameframex.unity.scene.git`
4. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動載入識別。
## 使用範例

### 初始化

在 GameEntry GameObject 上添加 `SceneComponent`（透過 `Add Component > GameFrameX > Scene`）。

### 載入場景

```csharp
// Single 模式 — 替換所有當前場景
var handle = await SceneComponent.LoadScene("Assets/Scenes/GameScene.unity");

// Additive 模式 — 在當前場景之上疊加載入
var handle = await SceneComponent.LoadScene(
    "Assets/Scenes/UIOverlay.unity",
    LoadSceneMode.Additive
);
```

### 卸載場景

```csharp
SceneComponent.UnloadScene("Assets/Scenes/UIOverlay.unity");
```

### 設置場景優先級

透過分配排序值（數值越高越優先）來控制哪個已載入場景成為活躍場景：

```csharp
// 載入後設置優先級，使其成為活躍場景
SceneComponent.SetSceneOrder("Assets/Scenes/GameScene.unity", 10);
```

### 訂閱事件

使用 `EventComponent` 訂閱場景生命週期事件：

| 事件 | 觸發時機 |
|------|----------|
| `LoadSceneSuccessEventArgs` | 場景載入成功（包含耗時） |
| `LoadSceneFailureEventArgs` | 場景載入失敗（包含錯誤訊息） |
| `LoadSceneUpdateEventArgs` | 載入進度更新 |
| `UnloadSceneSuccessEventArgs` | 場景卸載成功 |
| `UnloadSceneFailureEventArgs` | 場景卸載失敗 |
| `ActiveSceneChangedEventArgs` | 活躍場景變更（包含舊/新場景） |

```csharp
// 範例：訂閱載入成功事件
EventComponent.Subscribe<LoadSceneSuccessEventArgs>(OnSceneLoaded);

void OnSceneLoaded(object sender, LoadSceneSuccessEventArgs e)
{
    Debug.Log($"場景載入完成: {e.SceneAssetName}，耗時 {e.Duration:F2}s");
}
```

### 查詢場景狀態

```csharp
bool isLoaded = SceneComponent.SceneIsLoaded("Assets/Scenes/GameScene.unity");
bool isLoading = SceneComponent.SceneIsLoading("Assets/Scenes/GameScene.unity");
bool isUnloading = SceneComponent.SceneIsUnloading("Assets/Scenes/GameScene.unity");
```

---

## 更新日誌

詳見 [CHANGELOG.md](CHANGELOG.md)。


## 依賴

| 套件 | 說明 |
|------|------|
| `com.gameframex.unity.asset` | 2.5.0 |
| `com.gameframex.unity.event` | 1.1.0 |

## 文檔與資源

- [官方文檔](https://gameframex.doc.alianblank.com)

## 社區與支援

- QQ群: 467608841 / 233840761
## 開源協議

詳見 [LICENSE.md](LICENSE.md) 檔案。

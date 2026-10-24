<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Mono

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 項目簡介

Game Frame X Mono 是 GameFrameX 框架的 Mono 生命週期組件，用於管理遊戲中 MonoBehaviour 的事件和更新週期，例如 FixedUpdate、LateUpdate、OnDestroy 等，並提供了一種簡便的方式來添加和移除這些事件的監聽。

## 快速開始

### 安裝

編輯 Unity 專案的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：

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
    "com.gameframex.unity.mono": "1.1.1"
  }
}
```

`scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。

**其他安裝方式：**

1. 直接在 `manifest.json` 的 `dependencies` 節點下添加以下內容：
   ```json
   {"com.gameframex.unity.mono": "https://github.com/GameFrameX/com.gameframex.unity.mono.git"}
   ```

2. 在 Unity 的 `Packages Manager` 中使用 `Git URL` 的方式添加庫，地址為：
   ```
   https://github.com/GameFrameX/com.gameframex.unity.mono.git
   ```

3. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動加載識別。

## 使用方法

### 取得元件

```csharp
var monoComponent = GameEntry.GetComponent<MonoComponent>();
```

### 註冊生命週期監聽

MonoComponent 支援註冊 Unity `MonoBehaviour` 生命週期事件的回呼。所有監聽器均可隨時新增或移除。

#### Update / FixedUpdate / LateUpdate

這三個監聽器接收兩個 `float` 參數：
- `elapseSeconds` — 縮放後的增量時間
- `realElapseSeconds` — 未縮放的增量時間

```csharp
private void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 每幀呼叫
}

private void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 固定間隔呼叫（物理）
}

private void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 所有 Update 呼叫完畢後執行
}

// 註冊
monoComponent.AddUpdateListener(OnUpdate);
monoComponent.AddFixedUpdateListener(OnFixedUpdate);
monoComponent.AddLateUpdateListener(OnLateUpdate);

// 不再需要時移除
monoComponent.RemoveUpdateListener(OnUpdate);
monoComponent.RemoveFixedUpdateListener(OnFixedUpdate);
monoComponent.RemoveLateUpdateListener(OnLateUpdate);
```

#### OnDestroy

```csharp
private void OnDestroyCallback()
{
    // MonoComponent 所在 GameObject 銷毀時呼叫
}

monoComponent.AddDestroyListener(OnDestroyCallback);
monoComponent.RemoveDestroyListener(OnDestroyCallback);
```

#### OnApplicationFocus / OnApplicationPause

這兩個監聽器接收一個 `bool` 參數，並支援**雙重通知模式** — 可以使用直接監聽器，也可以透過事件匯流排訂閱。

**直接監聽方式：**

```csharp
private void OnApplicationFocus(bool isFocus)
{
    // isFocus: true = 應用程式取得焦點, false = 失去焦點
}

private void OnApplicationPause(bool isPause)
{
    // isPause: true = 應用程式暫停, false = 恢復
}

monoComponent.AddOnApplicationFocusListener(OnApplicationFocus);
monoComponent.AddOnApplicationPauseListener(OnApplicationPause);

monoComponent.RemoveOnApplicationFocusListener(OnApplicationFocus);
monoComponent.RemoveOnApplicationPauseListener(OnApplicationPause);
```

**事件匯流排方式**（透過 `EventComponent`）：

```csharp
var eventComponent = GameEntry.GetComponent<EventComponent>();

eventComponent.Subscribe(OnApplicationFocusChangedEventArgs.EventId, OnFocusChanged);
eventComponent.Subscribe(OnApplicationPauseChangedEventArgs.EventId, OnPauseChanged);

private void OnFocusChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationFocusChangedEventArgs)e;
    if (args.IsFocus)
    {
        // 應用程式取得焦點
    }
}

private void OnPauseChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationPauseChangedEventArgs)e;
    if (args.IsPause)
    {
        // 應用程式暫停
    }
}
```

### 注意事項

- 監聽器註冊是**執行緒安全**的。
- 監聽器在回呼執行期間可以安全地新增或移除其他監聽器（快照派發機制）。
- 回呼中的例外會被捕捉並記錄日誌，**不會**中斷其他監聽器的執行。
- 不再需要時務必移除監聽器，避免記憶體洩漏。

## 文檔與資源

- 文檔地址: https://gameframex.doc.alianblank.com
- 倉庫地址: https://github.com/GameFrameX/com.gameframex.unity.mono
- 問題反饋: https://github.com/GameFrameX/com.gameframex.unity.mono/issues


## 依賴

| 套件 | 說明 |
|------|------|
| `com.gameframex.unity.event` | 1.1.0 |

## 社區與支援

- QQ群: 467608841 / 233840761

## 更新日誌

查看 [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.mono/releases) 了解更新日誌。
## 開源協議

詳細信息請查看 [LICENSE](LICENSE.md) 文件。

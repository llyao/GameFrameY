<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Procedure

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 語言

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

---

## 項目簡介

基於有限狀態機（FSM）的 Unity 遊戲流程管理套件。透過可切換的流程狀態驅動遊戲生命週期階段（閃屏、預載入、登入、主選單等）。

## 架構設計

```
ProcedureComponent (MonoBehaviour)
  └─ IProcedureManager (介面)
       └─ ProcedureManager (管理 FSM)
            └─ ProcedureBase (抽象類別，每個流程的邏輯)
```

- **ProcedureComponent** — Unity 元件，負責註冊流程並提供运行时管理介面。
- **ProcedureManager** — 核心管理器，透過 `IFsmManager` 建立內部 FSM 驅動流程狀態切換。
- **ProcedureBase** — 抽象基底類別，提供生命週期回呼：`OnInit`、`OnEnter`、`OnUpdate`、`OnFixedUpdate`、`OnLeave`、`OnDestroy`。

## 依賴關係

- [com.gameframex.unity.fsm](https://github.com/GameFrameX/com.gameframex.unity.fsm) — 有限狀態機

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
       "com.gameframex.unity.procedure": "1.2.2"
     }
   }
   ```

   `scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。

2. 直接在 `manifest.json` 的 `dependencies` 節點下添加以下內容：
   ```json
   {
      "com.gameframex.unity.procedure": "https://github.com/gameframex/com.gameframex.unity.procedure.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加庫，地址為：`https://github.com/gameframex/com.gameframex.unity.procedure.git`
4. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動載入識別。

### 建立流程類別

定義流程類別，繼承 `ProcedureBase` 並實作生命週期回呼：

```csharp
public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 載入資源、設定等
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 檢查載入進度，完成後切換到下一個流程
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}

public class ProcedureMain : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 顯示主選單
    }
}
```

### 配置組件

1. 將 `ProcedureComponent` 添加到遊戲物件（透過 `GameFrameX > Procedure`）。
2. 在 Inspector 中勾選可用的流程。
3. 選擇入口流程（如 `ProcedurePreload`）。

#### UseStartupRunner 模式

`ProcedureComponent` 支援兩種啟動模式：

- **傳統模式（預設）** — `UseStartupRunner` 為 `false` 時，本組件在 `Start()` 中依據 Inspector 設定的可用流程與入口流程自動初始化 FSM 並啟動第一個流程。

- **UseStartupRunner 模式** — `UseStartupRunner` 為 `true` 時，本組件僅在 `Awake()` 中註冊 `IProcedureManager`，不執行初始化與啟動流程。流程的初始化與啟動由 `ApplicationStartupEntry` 透過 `StartupRunner.Run` 接管。

此模式適合需要與其他啟動項（如網路初始化、數據載入等）協同控制的場景，避免多個入口同時初始化 FSM 導致衝突。

## 核心概念

### 流程生命週期

每個流程類別透過覆寫以下方法來處理生命週期：

| 回呼 | 時機 |
|------|------|
| `OnInit` | 流程首次被 FSM 建立時呼叫 |
| `OnEnter` | 流程進入運行狀態時呼叫 |
| `OnUpdate` | 每幀更新時呼叫（帶已耗費時間） |
| `OnFixedUpdate` | 固定頻率更新時呼叫 |
| `OnLeave` | 流程離開運行狀態時呼叫 |
| `OnDestroy` | 流程被銷毀時呼叫 |

### 狀態切換

使用 `ChangeToState<T>` 方法在流程之間進行切換：

```csharp
protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
{
    if (isLoadComplete)
    {
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}
```

### UseStartupRunner 模式

當啟用 `UseStartupRunner` 模式時：

1. `ProcedureComponent.Awake()` — 僅註冊 `IProcedureManager` 介面
2. 外部透過 `StartupRunner.Run()` 統籌所有啟動項（包括流程初始化）
3. `ProcedureComponent.Start()` — 检测到 `UseStartupRunner` 為 `true` 時直接返回，不執行傳統初始化邏輯

此模式將啟動順序的控制權交給外部協調者，適合自定義啟動序列的專案。

## API 參考

### ProcedureComponent

| 成員 | 說明 |
|------|------|
| `Procedure` | 取得 `IProcedureManager` 實例 |
| `CurrentProcedure` | 取得當前運行的流程實例 |
| `CurrentProcedureTime` | 取得當前流程已運行的時間（秒） |
| `HasProcedure<T>()` | 檢查指定類型的流程是否存在 |
| `GetProcedure<T>()` | 取得指定類型的流程實例 |
| `DestroyProcedures()` | 銷毀所有流程並清除 FSM |
| `ReinitializeProcedures(procedures, entranceProcedure)` | 使用新流程重新初始化 |

### ProcedureBase

| 成員 | 說明 |
|------|------|
| `ChangeToState<T>(procedureOwner)` | 切換到指定類型的流程 |
| `ChangeToState(procedureOwner, targetType)` | 透過類型物件切換流程 |

## 使用範例

### 定義流程類別

```csharp
public class ProcedureSplash : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 播放閃屏動畫
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 動畫播放完畢後進入下一個流程
        if (m_IsAnimationComplete)
        {
            ChangeToState<ProcedurePreload>(procedureOwner);
        }
    }
}

public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 載入資源、初始化配置
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 檢查載入進度
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
        // 顯示登入介面
    }
}
```

### 執行階段流程管理

```csharp
// 檢查流程是否存在
bool has = procedureComponent.HasProcedure<ProcedureMain>();

// 取得流程實例
ProcedureMain main = procedureComponent.GetProcedure<ProcedureMain>();

// 取得當前流程資訊
ProcedureBase current = procedureComponent.CurrentProcedure;
float time = procedureComponent.CurrentProcedureTime;

// 銷毀所有流程並使用新流程重新初始化
procedureComponent.DestroyProcedures();
procedureComponent.ReinitializeProcedures(newProcedures, entranceProcedure);
```

## 文檔與資源

- [官方文檔](https://gameframex.doc.alianblank.com)

## 社區與支援

- QQ群: 467608841 / 233840761

## 開源協議

本套件採用 MIT 與 Apache-2.0 雙許可證分發。詳見 [LICENSE.md](LICENSE.md) 檔案。

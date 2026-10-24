<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX FSM

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 項目簡介

Unity 泛型有限狀態機套件。管理型別化狀態機的建立、生命週期和狀態轉換，支援每個 FSM 的資料字典。

### 功能

- **型別安全 FSM** — 每個 FSM 以擁有者型別 `T` 參數化。同型別多個 FSM 可透過可選名稱共存。
- **狀態生命週期** — 六個虛方法鉤子：`OnInit`、`OnEnter`、`OnUpdate`、`OnFixedUpdate`、`OnLeave`、`OnDestroy`。
- **狀態轉換** — 在任意狀態內呼叫 `ChangeState<TState>()` 切換狀態。
- **變數儲存** — 透過 `GetData<TData>(name)` / `SetData(name, value)` 實現跨狀態鍵值資料，物件池化零 GC。
- **動態狀態管理** — 執行時 `AddState` / `RemoveState`。
- **Reset 支援** — `Reset()` 清除資料並退出當前狀態，保留已註冊狀態。
- **FixedUpdate 輪詢** — 雙 `Update` + `FixedUpdate` 驅動路徑。
- **執行時 Inspector** — 自訂編輯器在 Play 模式下即時顯示 FSM 狀態和經過時間。

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
       "com.gameframex.unity.fsm": "1.1.1"
     }
   }
   ```

   `scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。

2. 直接在 `manifest.json` 的 `dependencies` 節點下添加以下內容：
   ```json
   {
      "com.gameframex.unity.fsm": "https://github.com/gameframex/com.gameframex.unity.fsm.git"
   }
   ```
3. 在 Unity 的 `Package Manager` 中使用 `Git URL` 的方式添加庫，地址為：`https://github.com/gameframex/com.gameframex.unity.fsm.git`
4. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動載入識別。

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
    "com.gameframex.unity.fsm": "1.0.4"
  }
}
```

`scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。

## 使用範例

### 定義狀態

繼承 `FsmState<T>` 並覆寫生命週期方法：

```csharp
public class IdleState : FsmState<Player>
{
    protected override void OnEnter(IFsm<Player> fsm)
    {
        // 當此狀態被啟用時呼叫
    }

    protected override void OnUpdate(IFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        // 啟用期間每幀呼叫
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState<MoveState>(fsm);
        }
    }

    protected override void OnLeave(IFsm<Player> fsm, bool isShutdown)
    {
        // 離開此狀態時呼叫
    }
}
```

### 建立並啟動 FSM

```csharp
// 標準方式：透過 GameEntry（不依賴 com.gameframex.unity.entry）
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
IFsm<Player> fsm = fsmComponent.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();

// 快捷方式：透過 GameApp（需要 com.gameframex.unity.entry）
IFsm<Player> fsm = GameApp.Fsm.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();
```

### 存取跨狀態資料

```csharp
// 在任意狀態的 OnEnter / OnUpdate / ... 中
fsm.SetData("Health", 100);
int hp = fsm.GetData<int>("Health");

if (fsm.HasData("Health"))
{
    fsm.RemoveData("Health");
}
```

### 動態狀態管理

```csharp
// 向執行中的 FSM 添加新狀態
fsm.AddState(new JumpState());

// 移除狀態（不能移除當前狀態）
fsm.RemoveState<IdleState>();
```

### 重置 FSM

```csharp
// 退出當前狀態，清除所有資料，保留已註冊狀態
fsm.Reset();
// 可透過 Start<TState>() 重新啟動
fsm.Start<IdleState>();
```

### 銷毀 FSM

```csharp
// 標準方式：透過 GameEntry（不依賴 com.gameframex.unity.entry）
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
fsmComponent.DestroyFsm(fsm);

// 快捷方式：透過 GameApp（需要 com.gameframex.unity.entry）
GameApp.Fsm.DestroyFsm(fsm);
```

## 文檔與資源

- [文檔](https://gameframex.doc.alianblank.com)

## 社區與支援

- QQ群: 467608841 / 233840761

## 更新日誌

查看 [Releases](https://github.com/gameframex/com.gameframex.unity.fsm/releases) 了解更新日誌。


## 依賴

| 套件 | 說明 |
|------|------|
| (无) | - |

## 開源協議

詳見 [LICENSE.md](LICENSE.md) 檔案。

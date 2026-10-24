<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX FSM

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## プロジェクト概要

Unity向け汎用有限状態マシンパッケージ。型付きFSMの作成、ライフサイクル、状態遷移を管理し、FSMごとのデータ辞書をサポートします。

### 機能

- **型安全な FSM** — 各FSMはオーナー型`T`でパラメータ化されています。オプションの名前で同型の複数FSMを共存可能。
- **状態ライフサイクル** — 6つの仮想フック：`OnInit`、`OnEnter`、`OnUpdate`、`OnFixedUpdate`、`OnLeave`、`OnDestroy`。
- **状態遷移** — 任意の状態内から`ChangeState<TState>()`を呼び出して遷移。
- **変数ストレージ** — `GetData<TData>(name)` / `SetData(name, value)`による状態間キーバリューデータ。オブジェクトプールでゼロGC。
- **動的状態管理** — 実行中のFSMに対する`AddState` / `RemoveState`。
- **リセットサポート** — `Reset()`はデータをクリアし現在の状態を終了しますが、登録済み状態は保持。
- **FixedUpdateポーリング** — `Update` + `FixedUpdate`のデュアル駆動パス。
- **ランタイムインスペクター** — カスタムエディターがPlayモード中にFSMの状態と経過時間をリアルタイム表示。

## クイックスタート

### インストール

以下のいずれかの方法を選択してください：

1. Unity プロジェクトの `Packages/manifest.json` を編集し、`scopedRegistries` セクションを追加してください：
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

   `scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に直接追加：
   ```json
   {
      "com.gameframex.unity.fsm": "https://github.com/gameframex/com.gameframex.unity.fsm.git"
   }
   ```
3. Unity の **Package Manager** で **Git URL** を使用して追加：`https://github.com/gameframex/com.gameframex.unity.fsm.git`
4. リポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンしてください。自動的に読み込まれます。

Unityプロジェクトの`Packages/manifest.json`を編集し、`scopedRegistries`セクションを追加してください：

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

`scopes`は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex`で始まるパッケージのみがこのレジストリから取得されます。

## 使用例

### 状態の定義

`FsmState<T>`を継承し、ライフサイクルメソッドをオーバーライドします：

```csharp
public class IdleState : FsmState<Player>
{
    protected override void OnEnter(IFsm<Player> fsm)
    {
        // この状態がアクティブになった時に呼ばれる
    }

    protected override void OnUpdate(IFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        // アクティブ中毎フレーム呼ばれる
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState<MoveState>(fsm);
        }
    }

    protected override void OnLeave(IFsm<Player> fsm, bool isShutdown)
    {
        // この状態から離脱する時に呼ばれる
    }
}
```

### FSMの作成と開始

```csharp
// 標準: GameEntry経由（com.gameframex.unity.entry非依存）
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
IFsm<Player> fsm = fsmComponent.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();

// ショートカット: GameApp経由（com.gameframex.unity.entryが必要）
IFsm<Player> fsm = GameApp.Fsm.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();
```

### 状態間データの保存と読み取り

```csharp
// 任意の状態のOnEnter / OnUpdate / ...内で
fsm.SetData("Health", 100);
int hp = fsm.GetData<int>("Health");

if (fsm.HasData("Health"))
{
    fsm.RemoveData("Health");
}
```

### 動的状態管理

```csharp
// 実行中のFSMに新しい状態を追加
fsm.AddState(new JumpState());

// 状態の削除（現在の状態は削除不可）
fsm.RemoveState<IdleState>();
```

### FSMのリセット

```csharp
// 現在の状態を終了し、全データをクリア、登録済み状態は保持
fsm.Reset();
// Start<TState>()で再開可能
fsm.Start<IdleState>();
```

### FSMの破棄

```csharp
// 標準: GameEntry経由（com.gameframex.unity.entry非依存）
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
fsmComponent.DestroyFsm(fsm);

// ショートカット: GameApp経由（com.gameframex.unity.entryが必要）
GameApp.Fsm.DestroyFsm(fsm);
```

## ドキュメントとリソース

- [ドキュメント](https://gameframex.doc.alianblank.com)

## コミュニティとサポート

- QQグループ: 467608841 / 233840761

## 変更履歴

[Releases](https://github.com/gameframex/com.gameframex.unity.fsm/releases)で変更履歴を確認してください。


## 依存関係

| パッケージ | 説明 |
|----------|------|
| (无) | - |

## ライセンス

詳しくは [LICENSE.md](LICENSE.md) をご参照ください。

<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Procedure

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## 言語

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

---

## プロジェクト概要

FSM ベースの Unity ゲームフロー管理パッケージです。切り替え可能なプロシージャステートにより、ゲームライフサイクルステージ（スプラッシュ、プリロード、ログイン、メインメニューなど）を駆動します。

## コア機能

- **UseStartupRunner モード** - 新機能。`ApplicationStartupEntry` + `StartupRunner.Run` がフロー起動を接管可能
- **伝統モード** - Inspector で `Available Procedures` と `Entrance Procedure` を構成
- フルライフサイクルコールバック: `OnInit`、`OnEnter`、`OnUpdate`、`OnFixedUpdate`、`OnLeave`、`OnDestroy`
- ランタイムプロシージャ管理: 存在確認、取得、破棄、再初期化

## アーキテクチャ

```
ProcedureComponent (MonoBehaviour)
  └─ IProcedureManager (インターフェース)
       └─ ProcedureManager (FSM を管理)
            └─ ProcedureBase (抽象クラス、各プロシージャのロジック)
```

- **ProcedureComponent** — Unity コンポーネント。Inspector でプロシージャとエントランスプロシージャを登録し、`Start()` 時に自動起動します（`UseStartupRunner` モード無効時）。
- **ProcedureManager** — コアマネージャ。`IFsmManager` を介して内部 FSM を作成し、プロシージャの状態遷移を駆動します。
- **ProcedureBase** — 抽象基底クラス。ライフサイクルコールバックを提供：`OnInit`、`OnEnter`、`OnUpdate`、`OnFixedUpdate`、`OnLeave`、`OnDestroy`。

## 依存関係

- [com.gameframex.unity.fsm](https://github.com/GameFrameX/com.gameframex.unity.fsm) — 有限状態機械

## クイックスタート

### インストール

以下のいずれかの方法を選択してください：

#### 方法 1: UPM (推奨)

Unity プロジェクトの `Packages/manifest.json` を編集し、`scopedRegistries` セクションを追加してください：

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

`scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。

#### 方法 2: Git URL

`manifest.json` の `dependencies` に直接追加：

```json
{
  "com.gameframex.unity.procedure": "https://github.com/gameframex/com.gameframex.unity.procedure.git"
}
```

#### 方法 3: Package Manager

Unity の **Package Manager** で **Git URL** を使用して追加：

```
https://github.com/gameframex/com.gameframex.unity.procedure.git
```

#### 方法 4: ローカルクローン

リポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンしてください。自動的に読み込まれます。

### プロシージャクラスの作成

プロシージャクラスは `ProcedureBase` を継承して作成します：

```csharp
public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // アセット、設定などのロード
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // ロード進捗を確認し、完了したら次のプロシージャに切り替え
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}

public class ProcedureMain : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // メインメニューを表示
    }
}
```

### コンポーネントの設定

1. ゲームオブジェクトに `ProcedureComponent` を追加（`GameFrameX > Procedure` メニューから）。
2. **UseStartupRunner モード**を使用する場合:
   - `UseStartupRunner` チェックボックスをオンにします。
   - このコンポーネントは `IProcedureManager` の登録のみを行い、`Initialize` と `StartProcedure` は `ApplicationStartupEntry` + `StartupRunner.Run` が接管します。
3. **伝統モード**を使用する場合:
   - Inspector の `Available Procedures` 利用可能なプロシージャを追加します。
   - `Entrance Procedure` エントランスプロシージャを選択（例：`ProcedurePreload`）。

## コアコンセプト

### プロシージャライフサイクル

各プロシージャは以下のライフサイクルコールバックを持ちます：

| コールバック | 説明 |
|-------------|------|
| `OnInit` | 状態初期化時に呼び出されます。 |
| `OnEnter` | 状態に入る際に呼び出されます。 |
| `OnUpdate` | 毎フレーム呼び出され、状態遷移の判定などに使用します。 |
| `OnFixedUpdate` | 物理演算更新ごとに呼び出されます。 |
| `OnLeave` | 状態を離れる際に呼び出されます。 |
| `OnDestroy` | 状態が破棄される際に呼び出されます。 |

### 状態遷移

`ChangeToState<T>` メソッドを使用して現在のプロシージャから別のプロシージャに遷移します：

```csharp
ChangeToState<ProcedureMain>(procedureOwner);
```

### UseStartupRunner モード

`UseStartupRunner` を有効にすると、`ProcedureComponent` は以下の点で традиционная モードと異なります：

| 項目 | UseStartupRunner オフ | UseStartupRunner オン |
|------|----------------------|---------------------|
| `Awake` | `IProcedureManager` を登録 | `IProcedureManager` を登録 |
| `Start` | プロシージャを初期化して開始 | 何もしない（yield break） |
| 初期化タイミング | `ProcedureComponent.Start` 内 | `ApplicationStartupEntry` + `StartupRunner.Run` が制御 |
| FSM 登録重複 | 手で制御するため重複なし | `StartupRunner.Run` が完了後に行われる |

このモードは、`ApplicationStartupEntry` を使用してアプリケーション全体の起動フローを集中管理したい場合に便利です。

## API リファレンス

### ProcedureComponent

#### プロパティ

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `Procedure` | `IProcedureManager` | プロシージャマネージャを取得します。 |
| `CurrentProcedure` | `ProcedureBase` | 現在実行中のプロシージャを取得します。 |
| `CurrentProcedureTime` | `float` | 現在のプロシージャが実行されている時間を取得します。 |

#### メソッド

| メソッド | 説明 |
|---------|------|
| `HasProcedure<T>()` | 指定した型のプロシージャが存在するかを取得します。 |
| `GetProcedure<T>()` | 指定した型のプロシージャインスタンスを取得します。 |
| `DestroyProcedures()` | 現在のプロシージャ状態機を破棄し、すべての登録済みプロシージャをクリアします。 |
| `ReinitializeProcedures(procedures, entranceProcedure)` | 現在のプロシージャを破棄し、新しいプロシージャで再初期化します。 |

### ProcedureBase

#### メソッド

| メソッド | 説明 |
|---------|------|
| `ChangeToState<T>(procedureOwner)` | 指定した型のプロシージャに遷移します。 |

### IProcedureManager

| プロパティ/メソッド | 型 | 説明 |
|-------------------|-----|------|
| `CurrentProcedure` | `ProcedureBase` | 現在実行中のプロシージャを取得します。 |
| `CurrentProcedureTime` | `float` | 現在のプロシージャが実行されている時間を取得します。 |
| `HasProcedure<T>()` | `bool` | 指定した型のプロシージャが存在するかを取得します。 |
| `GetProcedure<T>()` | `ProcedureBase` | 指定した型のプロシージャを取得します。 |
| `StartProcedure<T>()` | `void` | 指定した型のプロシージャを開始します。 |
| `DestroyProcedures()` | `void` | プロシージャを破棄します。 |
| `ReinitializeProcedures(procedures)` | `void` | プロシージャを再初期化します。 |

## 使用例

### プロシージャクラスの定義

```csharp
public class ProcedureSplash : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // スプラッシュ画面を表示
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 一定時間経過後、ロード画面へ遷移
        if (elapseSeconds >= 2f)
        {
            ChangeToState<ProcedurePreload>(procedureOwner);
        }
    }
}

public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // アセットの事前ロードを開始
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // ロード完了判定
        if (IsLoadComplete())
        {
            ChangeToState<ProcedureLogin>(procedureOwner);
        }
    }

    private bool IsLoadComplete()
    {
        // ロード進捗の判定ロジック
        return true;
    }
}

public class ProcedureLogin : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // ログイン画面を表示
    }
}
```

### ランタイムプロシージャ管理

```csharp
// ProcedureComponent を取得
ProcedureComponent procedureComponent = gameObject.GetComponent<ProcedureComponent>();

// プロシージャの存在確認
bool hasMain = procedureComponent.HasProcedure<ProcedureMain>();

// プロシージャインスタンスの取得
ProcedureMain main = procedureComponent.GetProcedure<ProcedureMain>();

// 現在のプロシージャ情報の取得
ProcedureBase current = procedureComponent.CurrentProcedure;
float time = procedureComponent.CurrentProcedureTime;

// プロシージャの破棄と再初期化
procedureComponent.DestroyProcedures();
procedureComponent.ReinitializeProcedures(newProcedures, entranceProcedure);
```

## ドキュメントとリソース

- [公式ドキュメント](https://gameframex.doc.alianblank.com)
- [GitHub リポジトリ](https://github.com/GameFrameX/com.gameframex.unity.procedure)
- [CHANGELOG](CHANGELOG.md)

## コミュニティとサポート

- QQグループ: 467608841 / 233840761
- [GitHub Issues](https://github.com/GameFrameX/com.gameframex.unity.procedure/issues)

## ライセンス

このパッケージは MIT ライセンスと Apache License 2.0 の二重ライセンスの下で公開されています。

詳細については [LICENSE.md](LICENSE.md) をご確認ください。
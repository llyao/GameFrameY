<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Scene

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## プロジェクト概要

GameFrameX Scene は [YooAsset](https://github.com/tuyoogame/YooAsset) をベースにした Unity シーン管理パッケージです。非同期シーンのロード/アンロード、イベント駆動の状態通知、進捗追跡、アクティブシーンの順序管理システムを提供します。

### 機能

- **非同期シーンロード** — `Task` ベースの非同期 API でシーンのロード/アンロードをサポート
- **LoadSceneMode サポート** — `Single`（全置換）と `Additive`（オーバーレイ）の両モードを切り替え可能
- **進捗追跡** — ローディング UI 用のリアルタイム進捗イベント
- **アクティブシーン順序管理** — 優先度ベースのシステムで、どのロード済みシーンをアクティブにするかを制御
- **カメラ自動更新** — アクティブシーン切り替え時に `Camera.main` 参照を自動更新
- **イベント通知** — ロード/アンロードの成功・失敗・更新イベントをサブスクライブ可能
- **状態照会** — シーンがロード済み・ロード中・アンロード中かをいつでも確認可能
- **エディタ拡張** — `SceneComponent` のカスタム Inspector

---

## クイックスタート

### 依存関係

| パッケージ | バージョン |
|------------|------------|
| `com.gameframex.unity.asset` | >= 2.5.0 |
| `com.gameframex.unity.event` | >= 1.1.0 |

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
       "com.gameframex.unity.scene": "2.2.2"
     }
   }
   ```

   `scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に直接追加：
   ```json
   {
      "com.gameframex.unity.scene": "https://github.com/gameframex/com.gameframex.unity.scene.git"
   }
   ```
3. Unity の **Package Manager** で **Git URL** を使用して追加：`https://github.com/gameframex/com.gameframex.unity.scene.git`
4. リポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンしてください。自動的に読み込まれます。
## 使用例

### セットアップ

GameEntry GameObject に `SceneComponent` を追加します（`Add Component > GameFrameX > Scene`）。

### シーンのロード

```csharp
// Single モード — 現在の全シーンを置換
var handle = await SceneComponent.LoadScene("Assets/Scenes/GameScene.unity");

// Additive モード — 現在のシーンの上にオーバーレイ
var handle = await SceneComponent.LoadScene(
    "Assets/Scenes/UIOverlay.unity",
    LoadSceneMode.Additive
);
```

### シーンのアンロード

```csharp
SceneComponent.UnloadScene("Assets/Scenes/UIOverlay.unity");
```

### シーンの優先度設定

順序値を割り当てることで（値が大きいほど優先）、どのロード済みシーンをアクティブにするかを制御します：

```csharp
// ロード後に優先度を設定し、アクティブシーンにする
SceneComponent.SetSceneOrder("Assets/Scenes/GameScene.unity", 10);
```

### イベントのサブスクライブ

`EventComponent` を使用してシーンライフサイクルイベントをサブスクライブします：

| イベント | 発生タイミング |
|----------|----------------|
| `LoadSceneSuccessEventArgs` | シーンのロード成功（所要時間を含む） |
| `LoadSceneFailureEventArgs` | シーンのロード失敗（エラーメッセージを含む） |
| `LoadSceneUpdateEventArgs` | ロード進捗の更新 |
| `UnloadSceneSuccessEventArgs` | シーンのアンロード成功 |
| `UnloadSceneFailureEventArgs` | シーンのアンロード失敗 |
| `ActiveSceneChangedEventArgs` | アクティブシーンの変更（旧/新シーンを含む） |

```csharp
// 例：ロード成功イベントのサブスクライブ
EventComponent.Subscribe<LoadSceneSuccessEventArgs>(OnSceneLoaded);

void OnSceneLoaded(object sender, LoadSceneSuccessEventArgs e)
{
    Debug.Log($"シーンロード完了: {e.SceneAssetName}、所要時間 {e.Duration:F2}s");
}
```

### シーン状態の照会

```csharp
bool isLoaded = SceneComponent.SceneIsLoaded("Assets/Scenes/GameScene.unity");
bool isLoading = SceneComponent.SceneIsLoading("Assets/Scenes/GameScene.unity");
bool isUnloading = SceneComponent.SceneIsUnloading("Assets/Scenes/GameScene.unity");
```

---

## 変更履歴

詳細は [CHANGELOG.md](CHANGELOG.md) をご覧ください。


## 依存関係

| パッケージ | 説明 |
|----------|------|
| `com.gameframex.unity.asset` | 2.5.0 |
| `com.gameframex.unity.event` | 1.1.0 |

## ドキュメントとリソース

- [ドキュメント](https://gameframex.doc.alianblank.com)

## コミュニティとサポート

- QQグループ: 467608841 / 233840761
## ライセンス

詳しくは [LICENSE.md](LICENSE.md) をご参照ください。

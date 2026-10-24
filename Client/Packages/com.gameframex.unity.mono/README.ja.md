<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Mono

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## プロジェクト概要

Game Frame X Mono は、GameFrameX フレームワークの Mono ライフサイクルコンポーネントです。FixedUpdate、LateUpdate、OnDestroy などの MonoBehaviour イベントと更新サイクルを管理し、これらのイベントリスナーを簡単に追加・削除する方法を提供します。

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
       "com.gameframex.unity.mono": "1.1.3"
     }
   }
   ```

   `scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に直接追加：
   ```json
   {
      "com.gameframex.unity.mono": "https://github.com/gameframex/com.gameframex.unity.mono.git"
   }
   ```
3. Unity の **Package Manager** で **Git URL** を使用して追加：`https://github.com/gameframex/com.gameframex.unity.mono.git`
4. リポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンしてください。自動的に読み込まれます。
## 使用方法

### コンポーネントの取得

```csharp
var monoComponent = GameEntry.GetComponent<MonoComponent>();
```

### ライフサイクルリスナーの登録

MonoComponent は Unity の `MonoBehaviour` ライフサイクルイベントのコールバック登録をサポートします。すべてのリスナーはいつでも追加・削除できます。

#### Update / FixedUpdate / LateUpdate

これら 3 つのリスナーは 2 つの `float` パラメータを受け取ります：
- `elapseSeconds` — スケール済みデルタタイム
- `realElapseSeconds` — 未スケールのデルタタイム

```csharp
private void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 毎フレーム呼び出し
}

private void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 固定間隔で呼び出し（物理）
}

private void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
{
    // すべての Update 完了後に呼び出し
}

// 登録
monoComponent.AddUpdateListener(OnUpdate);
monoComponent.AddFixedUpdateListener(OnFixedUpdate);
monoComponent.AddLateUpdateListener(OnLateUpdate);

// 不要になったら削除
monoComponent.RemoveUpdateListener(OnUpdate);
monoComponent.RemoveFixedUpdateListener(OnFixedUpdate);
monoComponent.RemoveLateUpdateListener(OnLateUpdate);
```

#### OnDestroy

```csharp
private void OnDestroyCallback()
{
    // MonoComponent の GameObject が破棄された時に呼び出し
}

monoComponent.AddDestroyListener(OnDestroyCallback);
monoComponent.RemoveDestroyListener(OnDestroyCallback);
```

#### OnApplicationFocus / OnApplicationPause

これらのリスナーは `bool` パラメータを 1 つ受け取り、**デュアル通知パターン**をサポートします — 直接リスナーまたはイベントバスのいずれかを使用できます。

**直接リスナー方式：**

```csharp
private void OnApplicationFocus(bool isFocus)
{
    // isFocus: true = アプリがフォーカス取得, false = フォーカス喪失
}

private void OnApplicationPause(bool isPause)
{
    // isPause: true = アプリ一時停止, false = 再開
}

monoComponent.AddOnApplicationFocusListener(OnApplicationFocus);
monoComponent.AddOnApplicationPauseListener(OnApplicationPause);

monoComponent.RemoveOnApplicationFocusListener(OnApplicationFocus);
monoComponent.RemoveOnApplicationPauseListener(OnApplicationPause);
```

**イベントバス方式**（`EventComponent` 経由）：

```csharp
var eventComponent = GameEntry.GetComponent<EventComponent>();

eventComponent.Subscribe(OnApplicationFocusChangedEventArgs.EventId, OnFocusChanged);
eventComponent.Subscribe(OnApplicationPauseChangedEventArgs.EventId, OnPauseChanged);

private void OnFocusChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationFocusChangedEventArgs)e;
    if (args.IsFocus)
    {
        // アプリがフォーカスを取得
    }
}

private void OnPauseChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationPauseChangedEventArgs)e;
    if (args.IsPause)
    {
        // アプリが一時停止
    }
}
```

### 注意事項

- リスナーの登録は**スレッドセーフ**です。
- コールバック実行中に他のリスナーを安全に追加・削除できます（スナップショットディスパッチ機構）。
- コールバック内の例外はキャッチされてログに記録され、他のリスナーの実行を**中断しません**。
- 不要になったリスナーは必ず削除し、メモリリークを防いでください。

## ドキュメントとリソース

- ドキュメント: https://gameframex.doc.alianblank.com
- リポジトリ: https://github.com/GameFrameX/com.gameframex.unity.mono
- Issues: https://github.com/GameFrameX/com.gameframex.unity.mono/issues


## 依存関係

| パッケージ | 説明 |
|----------|------|
| `com.gameframex.unity.event` | 1.1.0 |

## コミュニティとサポート

- QQグループ: 467608841 / 233840761

## 変更履歴

[Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.mono/releases) で変更履歴を確認してください。
## ライセンス

詳細は [LICENSE](LICENSE.md) ファイルを参照してください。

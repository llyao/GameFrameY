<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Localization

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

<br />

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · QQグループ: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

## 目次

- [プロジェクト概要](#プロジェクト概要)
- [機能一覧](#機能一覧)
- [クイックスタート](#クイックスタート)
  - [動作環境](#動作環境)
  - [インストール](#インストール)
  - [コンポーネントの設定](#コンポーネントの設定)
- [使い方](#使い方)
  - [コンポーネントの取得](#コンポーネントの取得)
  - [言語管理](#言語管理)
  - [ローカライズされた文字列の取得](#ローカライズされた文字列の取得)
  - [パラメータ付きフォーマット](#パラメータ付きフォーマット)
  - [辞書管理](#辞書管理)
  - [言語変更の監視](#言語変更の監視)
  - [言語コード定数](#言語コード定数)
- [エディタ Inspector](#エディタ-inspector)
- [イベントリファレンス](#イベントリファレンス)
- [Helper の拡張](#helper-の拡張)
- [依存関係](#依存関係)
- [ドキュメントとリソース](#ドキュメントとリソース)
- [ライセンス](#ライセンス)

## プロジェクト概要

Game Frame X Localization は、GameFrameX フレームワークに基づく Unity ローカリゼーションパッケージです。辞書ベースの文字列翻訳、パラメータ付きフォーマット（最大16個の型付きパラメータ）、イベント通知付きのランタイム言語切り替え、システム言語またはデフォルト言語への自動フォールバック、Setting システムによる言語設定の永続化を備えた、完全な多言語ローカリゼーションソリューションを提供します。

## 機能一覧

- **辞書ベースのローカリゼーション** — ランタイムでキーと値の翻訳ペアを追加、照会、削除
- **パラメータ付き文字列フォーマット** — 1〜16個の型付きパラメータを持つジェネリック `GetString` オーバーロードと、`params object[]` オーバーロードを提供
- **ランタイム言語切り替え** — いつでもアクティブ言語を変更可能、イベントで全リスナーに通知
- **システム言語検出** — `CultureInfo` を使用してデバイスのシステム言語を自動検出
- **言語設定の永続化** — 選択された言語とデフォルト言語は Setting コンポーネントを通じて自動保存
- **エディタモード対応** — Unity エディタでテスト用に言語設定を上書き可能
- **180以上の言語コード定数** — 組み込みの `LocalizationCode` 静的クラスで世界の主要地域の言語をカバー
- **カスタム Helper サポート** — `LocalizationHelperBase` を継承してシステム言語検出ロジックを上書き

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
       "com.gameframex.unity.localization": "2.3.0"
     }
   }
   ```

   `scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。

2. `manifest.json` の `dependencies` に直接追加：
   ```json
   {
      "com.gameframex.unity.localization": "https://github.com/gameframex/com.gameframex.unity.localization.git"
   }
   ```
3. Unity の **Package Manager** で **Git URL** を使用して追加：`https://github.com/gameframex/com.gameframex.unity.localization.git`
4. リポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンしてください。自動的に読み込まれます。
## 使用例

### コンポーネントの取得

```csharp
using GameFrameX.Localization.Runtime;

// 標準: GameEntry 経由で取得
var localization = GameEntry.GetComponent<LocalizationComponent>();
```

### 言語管理

```csharp
// 現在のアクティブ言語を取得（未設定の場合はシステム言語を返す）
string currentLang = localization.Language;

// アクティブ言語を設定（変更イベントを発火、設定に自動保存）
localization.Language = "zh_CN";
localization.Language = "en_US";
localization.Language = "ja_JP";

// デフォルト/フォールバック言語を取得
string defaultLang = localization.DefaultLanguage;

// 新しいデフォルト言語を設定
localization.DefaultLanguage = "en_US";

// デバイスから検出されたシステム言語を取得
string sysLang = localization.SystemLanguage;

// 読み込み済みの辞書エントリ総数を取得
int count = localization.DictionaryCount;
```

### ローカライズされた文字列の取得

```csharp
// 単純なキー検索
// キーが見つからない場合、"<NoKey>{key}" を返す
string okText = localization.GetString("UI.Button.OK");
string cancelText = localization.GetString("UI.Button.Cancel");

// 値を取得する前にキーの存在を確認
if (localization.HasRawString("UI.Button.Confirm"))
{
    string confirmText = localization.GetString("UI.Button.Confirm");
}
```

### パラメータ付きフォーマット

本パッケージは、パラメータ付きフォーマットのための複数の `GetString` オーバーロードを提供します。辞書の値には標準的な .NET フォーマットプレースホルダー（`{0}`、`{1}` など）を使用してください。

```csharp
// 辞書に以下が含まれていると仮定：
// "UI.Message.Welcome" = "ようこそ、{0}さん！"
// "UI.Info.Score"     = "プレイヤー {0} はレベル {2} で {1} 点を獲得しました！"
// "UI.Shop.Buy"       = "{0} x {1} を {2} コインで購入、割引 {3:P}、合計 {4:C}"

// params object[] オーバーロードを使用
string welcome = localization.GetString("UI.Message.Welcome", playerName);

// ジェネリックオーバーロードを使用（推奨 — 値型のボクシングを回避）
string info = localization.GetString<string, int, int>("UI.Info.Score", playerName, score, level);

// 最大16個の型付きパラメータまで対応
string shopMsg = localization.GetString<string, int, int, float, decimal>(
    "UI.Shop.Buy", itemName, quantity, unitPrice, discount, total);
```

**エラー処理：** フォーマット文字列のプレースホルダーと提供された引数が一致しない場合、結果は `<Error>` で始まり、診断情報が付加されます。キーが存在しない場合、結果は `<NoKey>{key}` になります。

### 辞書管理

ランタイムで翻訳エントリを管理：

```csharp
// 新しいエントリを追加（キーが既に存在する、または null/空の場合は false を返す）
bool added = localization.AddRawString("UI.Button.NewButton", "新しいボタン");
if (!added)
{
    // キーが既に存在するか無効
}

// 存在確認
bool exists = localization.HasRawString("UI.Button.NewButton");

// 生の（フォーマットされていない）値を取得（見つからない場合は null）
string rawText = localization.GetRawString("UI.Button.NewButton");

// 特定のエントリを削除（キーが存在しない場合は false）
bool removed = localization.RemoveRawString("UI.Button.NewButton");

// 全エントリを削除
localization.RemoveAllRawStrings();
```

### 言語変更の監視

アクティブ言語が変更されると（`localization.Language = ...` 経由）、コンポーネントはイベントシステムを通じて `LocalizationLanguageChangeEventArgs` イベントを発火します。

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

        // 言語変更イベントを購読
        m_EventComponent.Subscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    void OnDestroy()
    {
        // 購読解除
        m_EventComponent.Unsubscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    private void OnLanguageChanged(object sender, GameEventArgs e)
    {
        var args = (LocalizationLanguageChangeEventArgs)e;

        // 変更前の言語コード（例："ja_JP"）
        string oldLang = args.OldLanguage;

        // 変更後の言語コード（例："en_US"）
        string newLang = args.Language;

        // ここで全 UI テキストを更新
        RefreshAllText();
    }

    private void RefreshAllText()
    {
        // 新しい言語で全ローカライズ文字列を再取得
        // 例：myText.text = m_LocalizationComponent.GetString("UI.MainMenu.Title");
    }
}
```

### 言語コード定数

`LocalizationCode` 静的クラスは、地域別に分類された 180 以上の定義済み言語コード定数を提供します。`言語_地域` 形式に従います（例：`ja_JP`、`en_US`）。

```csharp
using GameFrameX.Localization.Runtime;

// 東アジア
localization.Language = LocalizationCode.ChineseSimplified;    // "zh_CN"
localization.Language = LocalizationCode.ChineseTraditionalTW; // "zh_TW"
localization.Language = LocalizationCode.Japanese;             // "ja_JP"
localization.Language = LocalizationCode.Korean;               // "ko_KR"

// 英語バリアント
localization.Language = LocalizationCode.English;    // "en_US"
localization.Language = LocalizationCode.EnglishUK;  // "en_GB"
localization.Language = LocalizationCode.EnglishAU;  // "en_AU"

// ヨーロッパ
localization.Language = LocalizationCode.French;     // "fr_FR"
localization.Language = LocalizationCode.German;     // "de_DE"
localization.Language = LocalizationCode.Italian;    // "it_IT"
localization.Language = LocalizationCode.Spanish;    // "es_ES"
localization.Language = LocalizationCode.Portuguese; // "pt_PT"
localization.Language = LocalizationCode.Russian;    // "ru_RU"

// 中東・西アジア
localization.Language = LocalizationCode.Arabic;  // "ar_SA"
localization.Language = LocalizationCode.Hebrew;   // "he_IL"
localization.Language = LocalizationCode.Persian;  // "fa_IR"
localization.Language = LocalizationCode.Turkish;  // "tr_TR"

// 南アジア・東南アジア
localization.Language = LocalizationCode.Hindi;      // "hi_IN"
localization.Language = LocalizationCode.Thai;       // "th_TH"
localization.Language = LocalizationCode.Vietnamese; // "vi_VN"
localization.Language = LocalizationCode.Indonesian; // "id_ID"

// アフリカ
localization.Language = LocalizationCode.Swahili; // "sw_TZ"

// オセアニア
localization.Language = LocalizationCode.Maori; // "mi_NZ"
```

多くの言語には地域バリアントが用意されています（例：`SpanishMX`、`PortugueseBR`、`ArabicEG`、`FrenchCA` など）。完全なリストは `LocalizationCode.cs` を参照してください。

## エディタ Inspector

`LocalizationComponentInspector` は以下のオプションを備えたカスタム Inspector を提供します：

| フィールド | 説明 |
|-----------|------|
| **Localization Helper** | Helper の型を選択、またはカスタム Helper インスタンスを割り当て |
| **Default Language** | フォールバック言語コード |
| **Enable Editor Mode** | チェックすると、Play Mode でシステム言語の代わりに Editor Language を使用 |
| **Editor Language** | エディタモードで使用する言語コード |
| **Language**（Play Mode のみ） | ランタイムでの現在のアクティブ言語を表示 |
| **System Language**（Play Mode のみ） | 検出されたシステム言語を表示 |

## イベントリファレンス

| イベント | クラス | 発火条件 | 主なフィールド |
|---------|--------|---------|---------------|
| 言語変更 | `LocalizationLanguageChangeEventArgs` | `Language` プロパティが設定された時 | `OldLanguage`、`Language` |
| 辞書読み込み成功 | `LoadDictionarySuccessEventArgs` | 辞書アセットの読み込み完了 | `DictionaryAssetName`、`Duration`、`UserData` |
| 辞書読み込み失敗 | `LoadDictionaryFailureEventArgs` | 辞書アセットの読み込み失敗 | `DictionaryAssetName`、`ErrorMessage`、`UserData` |
| 辞書読み込み進捗 | `LoadDictionaryUpdateEventArgs` | 辞書アセットの読み込み進捗更新 | `DictionaryAssetName`、`Progress`、`UserData` |

全イベントクラスは GameFrameX 参照プールを使用し、ゼロアロケーションでイベントを発火します。

## Helper の拡張

システム言語検出をカスタマイズする場合（例：バックエンドサービスやカスタム設定からの読み取り）、`LocalizationHelperBase` を継承したクラスを作成します：

```csharp
using GameFrameX.Localization.Runtime;

public class CustomLocalizationHelper : LocalizationHelperBase
{
    public override string SystemLanguage
    {
        get
        {
            // ここにカスタムロジックを実装
            // 例：設定ファイルやリモート API から読み取り
            return "en_US";
        }
    }
}
```

その後、Inspector の **Localization Helper** フィールドで割り当てるか、`LocalizationOptions.LocalizationHelperFactory` にヘルパーインスタンスを返すファクトリーを設定します。

## 依存関係

| パッケージ | バージョン | 説明 |
|-----------|-----------|------|
| `com.gameframex.unity` | 1.1.1+ | GameFrameX コアフレームワーク |
| `com.gameframex.unity.asset` | 1.0.6+ | アセット管理モジュール |
| `com.gameframex.unity.event` | 1.0.0+ | イベントシステムモジュール |
| `com.gameframex.unity.setting` | 1.5.0+ | 設定永続化モジュール |

## ドキュメントとリソース

- ドキュメント: https://gameframex.doc.alianblank.com
- リポジトリ: https://github.com/GameFrameX/com.gameframex.unity.localization
- Issues: https://github.com/GameFrameX/com.gameframex.unity.localization/issues


## コミュニティとサポート

- QQグループ: 467608841 / 233840761

## 変更履歴

[Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.localization/releases) で変更履歴を確認してください。
## ライセンス

詳しくは [LICENSE.md](LICENSE.md) をご参照ください。

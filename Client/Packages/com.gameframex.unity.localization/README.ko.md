<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Localization

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.localization)](https://github.com/GameFrameX/com.gameframex.unity.localization/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ 그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 목차

- [프로젝트 개요](#프로젝트-개요)
- [기능](#기능)
- [빠른 시작](#빠른-시작)
  - [시스템 요구 사항](#시스템-요구-사항)
  - [설치](#설치)
  - [컴포넌트 설정](#컴포넌트-설정)
- [사용법](#사용법)
  - [컴포넌트 가져오기](#컴포넌트-가져오기)
  - [언어 관리](#언어-관리)
  - [현지화된 문자열 가져오기](#현지화된-문자열-가져오기)
  - [매개변수 포맷팅](#매개변수-포맷팅)
  - [사전 관리](#사전-관리)
  - [언어 변경 감시](#언어-변경-감시)
  - [언어 코드 상수](#언어-코드-상수)
- [에디터 Inspector](#에디터-inspector)
- [이벤트 참조](#이벤트-참조)
- [Helper 확장](#helper-확장)
- [의존성](#의존성)
- [문서 및 자료](#문서-및-자료)
- [라이선스](#라이선스)

## 프로젝트 개요

Game Frame X Localization은 GameFrameX 프레임워크 기반의 Unity 현지화 패키지입니다. 사전 기반 문자열 번역, 매개변수 포맷팅(최대 16개 타입 매개변수), 이벤트 알림이 포함된 런타임 언어 전환, 시스템 언어 또는 기본 언어로의 자동 폴백, Setting 시스템을 통한 언어 설정 영속화를 제공하는 완전한 다국어 현지화 솔루션입니다.

## 기능

- **사전 기반 현지화** — 런타임에 키-값 번역 쌍 추가, 조회, 삭제
- **매개변수 문자열 포맷팅** — 1~16개 타입 매개변수를 갖는 제네릭 `GetString` 오버로드와 `params object[]` 오버로드 제공
- **런타임 언어 전환** — 언제든 활성 언어를 변경 가능, 이벤트로 모든 리스너에 알림
- **시스템 언어 감지** — `CultureInfo`를 사용하여 기기의 시스템 언어를 자동 감지
- **언어 설정 영속화** — 선택한 언어와 기본 언어가 Setting 컴포넌트를 통해 자동 저장
- **에디터 모드 지원** — Unity 에디터에서 테스트용으로 언어 설정을 덮어쓸 수 있음
- **180개 이상의 언어 코드 상수** — 내장 `LocalizationCode` 정적 클래스로 전 세계 주요 지역 언어 지원
- **커스텀 Helper 지원** — `LocalizationHelperBase`를 상속하여 시스템 언어 감지 로직을 재정의

## 빠른 시작

### 설치

다음 방법 중 하나를 선택하세요:

1. Unity 프로젝트의 `Packages/manifest.json`을 편집하여 `scopedRegistries` 섹션을 추가하세요:
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

   `scopes`는 이 레지스트리를 통해 어떤 패키지를 해석할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies`에 직접 추가:
   ```json
   {
      "com.gameframex.unity.localization": "https://github.com/gameframex/com.gameframex.unity.localization.git"
   }
   ```
3. Unity의 **Package Manager**에서 **Git URL**을 사용하여 추가: `https://github.com/gameframex/com.gameframex.unity.localization.git`
4. 리포지토리를 Unity 프로젝트의 `Packages` 디렉토리에 클론하세요. 자동으로 로드됩니다.
## 사용 예시

### 컴포넌트 가져오기

```csharp
using GameFrameX.Localization.Runtime;

// 표준: GameEntry를 통해 가져오기
var localization = GameEntry.GetComponent<LocalizationComponent>();
```

### 언어 관리

```csharp
// 현재 활성 언어 가져오기 (설정되지 않은 경우 시스템 언어 반환)
string currentLang = localization.Language;

// 활성 언어 설정 (변경 이벤트 발생, 설정에 자동 저장)
localization.Language = "zh_CN";
localization.Language = "en_US";
localization.Language = "ko_KR";

// 기본/폴백 언어 가져오기
string defaultLang = localization.DefaultLanguage;

// 새 기본 언어 설정
localization.DefaultLanguage = "en_US";

// 기기에서 감지된 시스템 언어 가져오기
string sysLang = localization.SystemLanguage;

// 로드된 사전 항목 총 수 가져오기
int count = localization.DictionaryCount;
```

### 현지화된 문자열 가져오기

```csharp
// 간단한 키 조회
// 키를 찾을 수 없는 경우 "<NoKey>{key}" 반환
string okText = localization.GetString("UI.Button.OK");
string cancelText = localization.GetString("UI.Button.Cancel");

// 값을 가져오기 전에 키 존재 여부 확인
if (localization.HasRawString("UI.Button.Confirm"))
{
    string confirmText = localization.GetString("UI.Button.Confirm");
}
```

### 매개변수 포맷팅

이 패키지는 매개변수 포맷팅을 위한 여러 `GetString` 오버로드를 제공합니다. 사전 값은 표준 .NET 포맷 플레이스홀더(`{0}`, `{1}` 등)를 사용해야 합니다.

```csharp
// 사전에 다음이 포함되어 있다고 가정:
// "UI.Message.Welcome" = "환영합니다, {0}님!"
// "UI.Info.Score"     = "플레이어 {0}이(가) 레벨 {2}에서 {1}점을 획득했습니다!"
// "UI.Shop.Buy"       = "{0} x {1}을(를) {2} 코인에 구매, 할인 {3:P}, 합계 {4:C}"

// params object[] 오버로드 사용
string welcome = localization.GetString("UI.Message.Welcome", playerName);

// 제네릭 오버로드 사용 (권장 — 값 타입의 박싱 방지)
string info = localization.GetString<string, int, int>("UI.Info.Score", playerName, score, level);

// 최대 16개 타입 매개변수까지 지원
string shopMsg = localization.GetString<string, int, int, float, decimal>(
    "UI.Shop.Buy", itemName, quantity, unitPrice, discount, total);
```

**오류 처리:** 포맷 문자열의 플레이스홀더와 제공된 인수가 일치하지 않으면, 결과는 `<Error>`로 시작하고 진단 정보가 포함됩니다. 키가 존재하지 않으면 결과는 `<NoKey>{key}`가 됩니다.

### 사전 관리

런타임에서 번역 항목 관리:

```csharp
// 새 항목 추가 (키가 이미 존재하거나 null/빈 문자열이면 false 반환)
bool added = localization.AddRawString("UI.Button.NewButton", "새 버튼");
if (!added)
{
    // 키가 이미 존재하거나 유효하지 않음
}

// 존재 여부 확인
bool exists = localization.HasRawString("UI.Button.NewButton");

// 원시(포맷되지 않은) 값 가져오기 (찾을 수 없으면 null)
string rawText = localization.GetRawString("UI.Button.NewButton");

// 특정 항목 제거 (키가 없으면 false)
bool removed = localization.RemoveRawString("UI.Button.NewButton");

// 모든 항목 제거
localization.RemoveAllRawStrings();
```

### 언어 변경 감시

활성 언어가 변경되면(`localization.Language = ...`를 통해), 컴포넌트는 이벤트 시스템을 통해 `LocalizationLanguageChangeEventArgs` 이벤트를 발생시킵니다.

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

        // 언어 변경 이벤트 구독
        m_EventComponent.Subscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    void OnDestroy()
    {
        // 구독 해제
        m_EventComponent.Unsubscribe(
            LocalizationLanguageChangeEventArgs.EventId, OnLanguageChanged);
    }

    private void OnLanguageChanged(object sender, GameEventArgs e)
    {
        var args = (LocalizationLanguageChangeEventArgs)e;

        // 변경 전 언어 코드 (예: "ko_KR")
        string oldLang = args.OldLanguage;

        // 변경 후 언어 코드 (예: "en_US")
        string newLang = args.Language;

        // 여기서 모든 UI 텍스트 새로고침
        RefreshAllText();
    }

    private void RefreshAllText()
    {
        // 새 언어로 모든 현지화 문자열 다시 가져오기
        // 예: myText.text = m_LocalizationComponent.GetString("UI.MainMenu.Title");
    }
}
```

### 언어 코드 상수

`LocalizationCode` 정적 클래스는 지역별로 분류된 180개 이상의 미리 정의된 언어 코드 상수를 제공합니다. `언어_지역` 형식을 따릅니다(예: `ko_KR`, `en_US`).

```csharp
using GameFrameX.Localization.Runtime;

// 동아시아
localization.Language = LocalizationCode.ChineseSimplified;    // "zh_CN"
localization.Language = LocalizationCode.ChineseTraditionalTW; // "zh_TW"
localization.Language = LocalizationCode.Japanese;             // "ja_JP"
localization.Language = LocalizationCode.Korean;               // "ko_KR"

// 영어 변형
localization.Language = LocalizationCode.English;    // "en_US"
localization.Language = LocalizationCode.EnglishUK;  // "en_GB"
localization.Language = LocalizationCode.EnglishAU;  // "en_AU"

// 유럽
localization.Language = LocalizationCode.French;     // "fr_FR"
localization.Language = LocalizationCode.German;     // "de_DE"
localization.Language = LocalizationCode.Italian;    // "it_IT"
localization.Language = LocalizationCode.Spanish;    // "es_ES"
localization.Language = LocalizationCode.Portuguese; // "pt_PT"
localization.Language = LocalizationCode.Russian;    // "ru_RU"

// 중동 및 서아시아
localization.Language = LocalizationCode.Arabic;  // "ar_SA"
localization.Language = LocalizationCode.Hebrew;   // "he_IL"
localization.Language = LocalizationCode.Persian;  // "fa_IR"
localization.Language = LocalizationCode.Turkish;  // "tr_TR"

// 남아시아 및 동남아시아
localization.Language = LocalizationCode.Hindi;      // "hi_IN"
localization.Language = LocalizationCode.Thai;       // "th_TH"
localization.Language = LocalizationCode.Vietnamese; // "vi_VN"
localization.Language = LocalizationCode.Indonesian; // "id_ID"

// 아프리카
localization.Language = LocalizationCode.Swahili; // "sw_TZ"

// 오세아니아
localization.Language = LocalizationCode.Maori; // "mi_NZ"
```

많은 언어에 지역 변형이 있습니다(예: `SpanishMX`, `PortugueseBR`, `ArabicEG`, `FrenchCA` 등). 전체 목록은 `LocalizationCode.cs`를 참조하세요.

## 에디터 Inspector

`LocalizationComponentInspector`는 다음 옵션이 있는 커스텀 Inspector를 제공합니다:

| 필드 | 설명 |
|------|------|
| **Localization Helper** | Helper 타입을 선택하거나 커스텀 Helper 인스턴스를 할당 |
| **Default Language** | 폴백 언어 코드 |
| **Enable Editor Mode** | 체크하면 Play Mode에서 시스템 언어 대신 Editor Language를 사용 |
| **Editor Language** | 에디터 모드에서 사용할 언어 코드 |
| **Language** (Play Mode 전용) | 런타임에서 현재 활성 언어를 표시 |
| **System Language** (Play Mode 전용) | 감지된 시스템 언어를 표시 |

## 이벤트 참조

| 이벤트 | 클래스 | 발생 조건 | 주요 필드 |
|--------|--------|-----------|-----------|
| 언어 변경 | `LocalizationLanguageChangeEventArgs` | `Language` 속성이 설정될 때 | `OldLanguage`, `Language` |
| 사전 로드 성공 | `LoadDictionarySuccessEventArgs` | 사전 에셋 로드 완료 | `DictionaryAssetName`, `Duration`, `UserData` |
| 사전 로드 실패 | `LoadDictionaryFailureEventArgs` | 사전 에셋 로드 실패 | `DictionaryAssetName`, `ErrorMessage`, `UserData` |
| 사전 로드 진행률 | `LoadDictionaryUpdateEventArgs` | 사전 에셋 로드 진행률 업데이트 | `DictionaryAssetName`, `Progress`, `UserData` |

모든 이벤트 클래스는 GameFrameX 참조 풀을 사용하여 제로 할당 이벤트 발생을 구현합니다.

## Helper 확장

시스템 언어 감지를 커스터마이즈하려면(예: 백엔드 서비스나 커스텀 설정에서 읽기), `LocalizationHelperBase`를 상속하는 클래스를 만듭니다:

```csharp
using GameFrameX.Localization.Runtime;

public class CustomLocalizationHelper : LocalizationHelperBase
{
    public override string SystemLanguage
    {
        get
        {
            // 여기에 커스텀 로직을 구현
            // 예: 설정 파일, 원격 API 등에서 읽기
            return "en_US";
        }
    }
}
```

그런 다음 Inspector의 **Localization Helper** 필드에서 할당하거나, `LocalizationOptions.LocalizationHelperFactory`에 헬퍼 인스턴스를 반환하는 팩토리를 설정합니다.

## 의존성

| 패키지 | 버전 | 설명 |
|--------|------|------|
| `com.gameframex.unity` | 1.1.1+ | GameFrameX 핵심 프레임워크 |
| `com.gameframex.unity.asset` | 1.0.6+ | 에셋 관리 모듈 |
| `com.gameframex.unity.event` | 1.0.0+ | 이벤트 시스템 모듈 |
| `com.gameframex.unity.setting` | 1.5.0+ | 설정 영속화 모듈 |

## 문서 및 자료

- 문서: https://gameframex.doc.alianblank.com
- 저장소: https://github.com/GameFrameX/com.gameframex.unity.localization
- Issues: https://github.com/GameFrameX/com.gameframex.unity.localization/issues


## 커뮤니티 및 지원

- QQ 그룹: 467608841 / 233840761

## 변경 로그

[Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.localization/releases)에서 변경 로그를 확인하세요.
## 라이선스

자세한 내용은 [LICENSE.md](LICENSE.md) 파일을 참조하세요.

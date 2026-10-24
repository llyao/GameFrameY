<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Mono

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.mono)](https://github.com/GameFrameX/com.gameframex.unity.mono/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ 그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 프로젝트 개요

Game Frame X Mono는 GameFrameX 프레임워크의 Mono 수명 주기 컴포넌트입니다. FixedUpdate, LateUpdate, OnDestroy 등 MonoBehaviour 이벤트와 업데이트 주기를 관리하며, 이벤트 리스너를 간편하게 추가하고 제거하는 방법을 제공합니다.

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
       "com.gameframex.unity.mono": "1.1.3"
     }
   }
   ```

   `scopes`는 이 레지스트리를 통해 어떤 패키지를 해석할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies`에 직접 추가:
   ```json
   {
      "com.gameframex.unity.mono": "https://github.com/gameframex/com.gameframex.unity.mono.git"
   }
   ```
3. Unity의 **Package Manager**에서 **Git URL**을 사용하여 추가: `https://github.com/gameframex/com.gameframex.unity.mono.git`
4. 리포지토리를 Unity 프로젝트의 `Packages` 디렉토리에 클론하세요. 자동으로 로드됩니다.
## 사용 방법

### 컴포넌트 가져오기

```csharp
var monoComponent = GameEntry.GetComponent<MonoComponent>();
```

### 라이프사이클 리스너 등록

MonoComponent는 Unity `MonoBehaviour` 라이프사이클 이벤트의 콜백 등록을 지원합니다. 모든 리스너는 언제든 추가 또는 제거할 수 있습니다.

#### Update / FixedUpdate / LateUpdate

이 세 가지 리스너는 두 개의 `float` 매개변수를 받습니다:
- `elapseSeconds` — 스케일된 델타 타임
- `realElapseSeconds` — 스케일되지 않은 델타 타임

```csharp
private void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 매 프레임 호출
}

private void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 고정 간격으로 호출 (물리)
}

private void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
{
    // 모든 Update 호출 완료 후 실행
}

// 등록
monoComponent.AddUpdateListener(OnUpdate);
monoComponent.AddFixedUpdateListener(OnFixedUpdate);
monoComponent.AddLateUpdateListener(OnLateUpdate);

// 더 이상 필요하지 않을 때 제거
monoComponent.RemoveUpdateListener(OnUpdate);
monoComponent.RemoveFixedUpdateListener(OnFixedUpdate);
monoComponent.RemoveLateUpdateListener(OnLateUpdate);
```

#### OnDestroy

```csharp
private void OnDestroyCallback()
{
    // MonoComponent의 GameObject가 파괴될 때 호출
}

monoComponent.AddDestroyListener(OnDestroyCallback);
monoComponent.RemoveDestroyListener(OnDestroyCallback);
```

#### OnApplicationFocus / OnApplicationPause

이 두 리스너는 `bool` 매개변수를 하나 받으며, **이중 알림 패턴**을 지원합니다 — 직접 리스너 또는 이벤트 버스 중 하나를 선택할 수 있습니다.

**직접 리스너 방식:**

```csharp
private void OnApplicationFocus(bool isFocus)
{
    // isFocus: true = 앱이 포커스 획득, false = 포커스 상실
}

private void OnApplicationPause(bool isPause)
{
    // isPause: true = 앱 일시 정지, false = 재개
}

monoComponent.AddOnApplicationFocusListener(OnApplicationFocus);
monoComponent.AddOnApplicationPauseListener(OnApplicationPause);

monoComponent.RemoveOnApplicationFocusListener(OnApplicationFocus);
monoComponent.RemoveOnApplicationPauseListener(OnApplicationPause);
```

**이벤트 버스 방식** (`EventComponent` 경유):

```csharp
var eventComponent = GameEntry.GetComponent<EventComponent>();

eventComponent.Subscribe(OnApplicationFocusChangedEventArgs.EventId, OnFocusChanged);
eventComponent.Subscribe(OnApplicationPauseChangedEventArgs.EventId, OnPauseChanged);

private void OnFocusChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationFocusChangedEventArgs)e;
    if (args.IsFocus)
    {
        // 앱이 포커스를 획득
    }
}

private void OnPauseChanged(object sender, GameEventArgs e)
{
    var args = (OnApplicationPauseChangedEventArgs)e;
    if (args.IsPause)
    {
        // 앱이 일시 정지
    }
}
```

### 주의 사항

- 리스너 등록은 **스레드 안전**합니다.
- 콜백 실행 중 다른 리스너를 안전하게 추가하거나 제거할 수 있습니다 (스냅샷 디스패치).
- 콜백 내 예외는 포착되어 로그에 기록되며, 다른 리스너의 실행을 **중단하지 않습니다**.
- 더 이상 필요하지 않은 리스너는 반드시 제거하여 메모리 누수를 방지하세요.

## 문서 및 자료

- 문서: https://gameframex.doc.alianblank.com
- 저장소: https://github.com/GameFrameX/com.gameframex.unity.mono
- Issues: https://github.com/GameFrameX/com.gameframex.unity.mono/issues


## 의존성

| 패키지 | 설명 |
|--------|------|
| `com.gameframex.unity.event` | 1.1.0 |

## 커뮤니티 및 지원

- QQ 그룹: 467608841 / 233840761

## 변경 로그

[Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.mono/releases)에서 변경 로그를 확인하세요.
## 라이선스

자세한 내용은 [LICENSE](LICENSE.md) 파일을 참조하세요.

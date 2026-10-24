<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Scene

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.scene)](https://github.com/GameFrameX/com.gameframex.unity.scene/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ 그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 프로젝트 개요

GameFrameX Scene은 [YooAsset](https://github.com/tuyoogame/YooAsset) 기반의 Unity 씬 관리 패키지입니다. 비동기 씬 로드/언로드, 이벤트 기반 상태 알림, 진행률 추적 및 활성 씬 순서 관리 시스템을 제공합니다.

### 기능

- **비동기 씬 로드** — `Task` 기반 비동기 API로 씬 로드/언로드 지원
- **LoadSceneMode 지원** — `Single`(전체 교체) 및 `Additive`(오버레이) 모드 전환
- **진행률 추적** — 로딩 UI용 실시간 진행률 이벤트
- **활성 씬 순서 관리** — 우선순위 기반 시스템으로 어떤 로드된 씬이 활성 씬이 될지 제어
- **자동 카메라 갱신** — 활성 씬 전환 시 `Camera.main` 참조 자동 갱신
- **이벤트 알림** — 로드/언로드 성공, 실패, 업데이트 이벤트 구독 가능
- **상태 조회** — 씬이 로드됨, 로딩 중, 언로딩 중인지 언제든 확인 가능
- **에디터 확장** — `SceneComponent`용 커스텀 Inspector

---

## 빠른 시작

### 종속성

| 패키지 | 버전 |
|--------|------|
| `com.gameframex.unity.asset` | >= 2.5.0 |
| `com.gameframex.unity.event` | >= 1.1.0 |

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
       "com.gameframex.unity.scene": "2.2.2"
     }
   }
   ```

   `scopes`는 이 레지스트리를 통해 어떤 패키지를 해석할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies`에 직접 추가:
   ```json
   {
      "com.gameframex.unity.scene": "https://github.com/gameframex/com.gameframex.unity.scene.git"
   }
   ```
3. Unity의 **Package Manager**에서 **Git URL**을 사용하여 추가: `https://github.com/gameframex/com.gameframex.unity.scene.git`
4. 리포지토리를 Unity 프로젝트의 `Packages` 디렉토리에 클론하세요. 자동으로 로드됩니다.
## 사용 예시

### 설정

GameEntry GameObject에 `SceneComponent`를 추가합니다 (`Add Component > GameFrameX > Scene`).

### 씬 로드

```csharp
// Single 모드 — 현재 모든 씬 교체
var handle = await SceneComponent.LoadScene("Assets/Scenes/GameScene.unity");

// Additive 모드 — 현재 씬 위에 오버레이
var handle = await SceneComponent.LoadScene(
    "Assets/Scenes/UIOverlay.unity",
    LoadSceneMode.Additive
);
```

### 씬 언로드

```csharp
SceneComponent.UnloadScene("Assets/Scenes/UIOverlay.unity");
```

### 씬 우선순위 설정

정렬 값을 할당하여 (값이 클수록 우선) 어떤 로드된 씬이 활성 씬이 될지 제어합니다:

```csharp
// 로드 후 우선순위를 설정하여 활성 씬으로 만들기
SceneComponent.SetSceneOrder("Assets/Scenes/GameScene.unity", 10);
```

### 이벤트 구독

`EventComponent`를 사용하여 씬 수명 주기 이벤트를 구독합니다:

| 이벤트 | 발생 시점 |
|--------|-----------|
| `LoadSceneSuccessEventArgs` | 씬 로드 성공 (소요 시간 포함) |
| `LoadSceneFailureEventArgs` | 씬 로드 실패 (오류 메시지 포함) |
| `LoadSceneUpdateEventArgs` | 로딩 진행률 업데이트 |
| `UnloadSceneSuccessEventArgs` | 씬 언로드 성공 |
| `UnloadSceneFailureEventArgs` | 씬 언로드 실패 |
| `ActiveSceneChangedEventArgs` | 활성 씬 변경 (이전/새 씬 포함) |

```csharp
// 예시: 로드 성공 이벤트 구독
EventComponent.Subscribe<LoadSceneSuccessEventArgs>(OnSceneLoaded);

void OnSceneLoaded(object sender, LoadSceneSuccessEventArgs e)
{
    Debug.Log($"씬 로드 완료: {e.SceneAssetName}, 소요 시간 {e.Duration:F2}s");
}
```

### 씬 상태 조회

```csharp
bool isLoaded = SceneComponent.SceneIsLoaded("Assets/Scenes/GameScene.unity");
bool isLoading = SceneComponent.SceneIsLoading("Assets/Scenes/GameScene.unity");
bool isUnloading = SceneComponent.SceneIsUnloading("Assets/Scenes/GameScene.unity");
```

---

## 변경 로그

자세한 내용은 [CHANGELOG.md](CHANGELOG.md)를 참조하세요.


## 의존성

| 패키지 | 설명 |
|--------|------|
| `com.gameframex.unity.asset` | 2.5.0 |
| `com.gameframex.unity.event` | 1.1.0 |

## 문서 및 자료

- [문서](https://gameframex.doc.alianblank.com)

## 커뮤니티 및 지원

- QQ 그룹: 467608841 / 233840761
## 라이선스

자세한 내용은 [LICENSE.md](LICENSE.md) 파일을 참조하세요.

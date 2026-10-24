<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Procedure

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.procedure)](https://github.com/GameFrameX/com.gameframex.unity.procedure/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 지원

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 언어

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

---

## 프로젝트 개요

Unity용 FSM 기반 게임 플로우 관리 패키지입니다. 전환 가능한 프로시저 스테이트를 통해 스플래시, 프리로드, 로그인, 메인 메뉴 등의 게임 라이프사이클 단계를 구동합니다.

## 아키텍처

```
ProcedureComponent (MonoBehaviour)
  └─ IProcedureManager (인터페이스)
       └─ ProcedureManager (FSM 관리)
            └─ ProcedureBase (추상 클래스, 각 프로시저 로직)
```

- **ProcedureComponent** — Unity 컴포넌트. Inspector에서 프로시저와 엔트런스 프로시저를 등록하고, `Start()` 시 자동 실행됩니다.
- **ProcedureManager** — 코어 매니저. `IFsmManager`를 통해 내부 FSM을 생성하여 프로시저 상태 전이를 관리합니다.
- **ProcedureBase** — 추상 베이스 클래스. 라이프사이클 콜백 제공: `OnInit`, `OnEnter`, `OnUpdate`, `OnFixedUpdate`, `OnLeave`, `OnDestroy`.

## 의존성

- [com.gameframex.unity.fsm](https://github.com/GameFrameX/com.gameframex.unity.fsm) — 유한 상태 머신

## 빠른 시작

### 설치

다음 방법 중 하나를 선택하세요:

1. Unity 프로젝트의 `Packages/manifest.json`을 편집하고 `scopedRegistries` 섹션을 추가하세요:
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

   `scopes`는 어떤 패키지를 이 레지스트리에서 해결할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies`에 직접 추가:
   ```json
   {
      "com.gameframex.unity.procedure": "https://github.com/gameframex/com.gameframex.unity.procedure.git"
   }
   ```
3. Unity의 **Package Manager**에서 **Git URL**을 사용하여 추가: `https://github.com/gameframex/com.gameframex.unity.procedure.git`
4. 레포지토리를 Unity 프로젝트의 `Packages` 디렉토리에 클론하세요. 자동으로 로드됩니다.

### 프로시저 클래스 생성

```csharp
public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 에셋, 설정 등을 로드
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 로드 진행 상황 확인, 완료되면 다음 프로시저로 전환
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}

public class ProcedureMain : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 메인 메뉴 표시
    }
}
```

### 컴포넌트 구성

1. 게임 오브젝트에 `ProcedureComponent`를 추가합니다 (`GameFrameX > Procedure`).
2. Inspector에서 **UseStartupRunner** 스위치를 확인합니다.

| 모드 | 설명 |
|------|------|
| **UseStartupRunner = false** (기본) | 전통 모드. Inspector에서 `Available Procedures`와 `Entrance Procedure`를 구성합니다. |
| **UseStartupRunner = true** | 새 기능 모드. `ApplicationStartupEntry` + `StartupRunner.Run`이 플로우 시작을 장악합니다. 이 경우 `ProcedureComponent`는 `Awake`에서 `IProcedureManager`만 등록하고 `Initialize`와 `StartProcedure`를 호출하지 않습니다. |

## 핵심 개념

### 프로시저 라이프사이클

각 프로시저는 다음 라이프사이클 콜백을 가집니다:

| 콜백 | 설명 |
|------|------|
| `OnInit` | 프로시저가 초기화될 때 호출됩니다. |
| `OnEnter` | 프로시저에 진입할 때 호출됩니다. |
| `OnUpdate` | 매 프레임 업데이트 시 호출됩니다. |
| `OnFixedUpdate` | 물리 업데이트 시 호출됩니다. |
| `OnLeave` | 프로시저를 벗어날 때 호출됩니다. |
| `OnDestroy` | 프로시저가 파괴될 때 호출됩니다. |

### 상태 전이

`ChangeToState<T>` 메서드를 사용하여 프로시저 간 전이를 수행합니다:

```csharp
ChangeToState<ProcedureMain>(procedureOwner);
```

### UseStartupRunner 모드

`UseStartupRunner`가 `true`일 때:

- `ProcedureComponent`는 `Awake`에서 `IProcedureManager`만 등록합니다.
- `Initialize`와 `StartProcedure` 호출은 `ApplicationStartupEntry`가 `StartupRunner.Run()`을 통해 수행합니다.
- 이 모드는 `Already exist FSM` 오류를 방지하고 플로우 시작을 중앙에서 제어할 수 있습니다.

## API 레퍼런스

### ProcedureComponent 속성

| 속성 | 설명 |
|------|------|
| `Procedure` | 프로시저 매니저 인터페이스를 반환합니다. |
| `CurrentProcedure` | 현재 실행 중인 프로시저를 반환합니다. |
| `CurrentProcedureTime` | 현재 프로시저의 경과 시간을 반환합니다. |

### ProcedureComponent 메서드

| 메서드 | 설명 |
|--------|------|
| `HasProcedure<T>()` | 지정된 프로시저가 존재하는지 확인합니다. |
| `GetProcedure<T>()` | 지정된 프로시저 인스턴스를 가져옵니다. |
| `DestroyProcedures()` | 모든 프로시저를 파괴하고 FSM을 정리합니다. |
| `ReinitializeProcedures(procedures, entranceProcedure)` | 현재 FSM을 파괴하고 새 프로시저로 재초기화합니다. |

## 사용 예시

### 프로시저 클래스 정의

```csharp
public class ProcedurePreload : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 에셋, 설정 등의 로드
    }

    protected internal override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        // 로드 진행 상황 확인, 완료되면 다음 프로시저로 전환
        ChangeToState<ProcedureMain>(procedureOwner);
    }
}

public class ProcedureMain : ProcedureBase
{
    protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        // 메인 메뉴 표시
    }
}
```

### 런타임 프로시저 관리

```csharp
// 프로시저 존재 확인
bool has = procedureComponent.HasProcedure<ProcedureMain>();

// 프로시저 인스턴스 가져오기
ProcedureMain main = procedureComponent.GetProcedure<ProcedureMain>();

// 현재 프로시저 정보 가져오기
ProcedureBase current = procedureComponent.CurrentProcedure;
float time = procedureComponent.CurrentProcedureTime;

// 모든 프로시저 파괴 및 새 프로시저로 재초기화
procedureComponent.DestroyProcedures();
procedureComponent.ReinitializeProcedures(newProcedures, entranceProcedure);
```

## 문서 및 자료

- [문서](https://gameframex.doc.alianblank.com)

## 커뮤니티 및 지원

- QQ그룹: 467608841 / 233840761

## 라이선스

자세한 내용은 [LICENSE.md](LICENSE.md)를 참조하세요.
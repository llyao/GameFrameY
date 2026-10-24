<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# GameFrameX FSM

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.fsm)](https://github.com/GameFrameX/com.gameframex.unity.fsm/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ 그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

## 프로젝트 개요

Unity용 제네릭 유한 상태 머신 패키지입니다. 타입화된 FSM의 생성, 라이프사이클, 상태 전환을 관리하며 FSM별 데이터 딕셔너리를 지원합니다.

### 기능

- **타입 안전 FSM** — 각 FSM은 소유자 타입 `T`로 매개변수화됩니다. 선택적 이름으로 동일 타입의 여러 FSM을 공존시킬 수 있습니다.
- **상태 라이프사이클** — 6개의 가상 훅: `OnInit`, `OnEnter`, `OnUpdate`, `OnFixedUpdate`, `OnLeave`, `OnDestroy`.
- **상태 전환** — 임의의 상태 내에서 `ChangeState<TState>()` 호출로 전환.
- **변수 저장소** — `GetData<TData>(name)` / `SetData(name, value)`를 통한 상태 간 키-값 데이터. 객체 풀링으로 제로 GC.
- **동적 상태 관리** — 실행 중인 FSM에 대한 `AddState` / `RemoveState`.
- **리셋 지원** — `Reset()`은 데이터를 지우고 현재 상태를 종료하지만 등록된 상태는 유지합니다.
- **FixedUpdate 폴링** — `Update` + `FixedUpdate` 이중 구동 경로.
- **런타임 인스펙터** — 커스텀 에디터가 Play 모드에서 FSM 상태와 경과 시간을 실시간으로 표시합니다.

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
       "com.gameframex.unity.fsm": "1.1.1"
     }
   }
   ```

   `scopes`는 이 레지스트리를 통해 어떤 패키지를 해석할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

2. `manifest.json`의 `dependencies`에 직접 추가:
   ```json
   {
      "com.gameframex.unity.fsm": "https://github.com/gameframex/com.gameframex.unity.fsm.git"
   }
   ```
3. Unity의 **Package Manager**에서 **Git URL**을 사용하여 추가: `https://github.com/gameframex/com.gameframex.unity.fsm.git`
4. 리포지토리를 Unity 프로젝트의 `Packages` 디렉토리에 클론하세요. 자동으로 로드됩니다.

Unity 프로젝트의 `Packages/manifest.json`을 편집하여 `scopedRegistries` 섹션을 추가하세요:

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

`scopes`는 이 레지스트리를 통해 어떤 패키지를 해석할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.

## 사용 예시

### 상태 정의

`FsmState<T>`를 상속하고 라이프사이클 메서드를 오버라이드합니다:

```csharp
public class IdleState : FsmState<Player>
{
    protected override void OnEnter(IFsm<Player> fsm)
    {
        // 이 상태가 활성화될 때 호출
    }

    protected override void OnUpdate(IFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        // 활성 중 매 프레임 호출
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState<MoveState>(fsm);
        }
    }

    protected override void OnLeave(IFsm<Player> fsm, bool isShutdown)
    {
        // 이 상태에서 벗어날 때 호출
    }
}
```

### FSM 생성 및 시작

```csharp
// 표준: GameEntry를 통해 (com.gameframex.unity.entry 비의존)
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
IFsm<Player> fsm = fsmComponent.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();

// 단축: GameApp을 통해 (com.gameframex.unity.entry 필요)
IFsm<Player> fsm = GameApp.Fsm.CreateFsm(player, new IdleState(), new MoveState());
fsm.Start<IdleState>();
```

### 상태 간 데이터 저장 및 읽기

```csharp
// 임의의 상태의 OnEnter / OnUpdate / ... 에서
fsm.SetData("Health", 100);
int hp = fsm.GetData<int>("Health");

if (fsm.HasData("Health"))
{
    fsm.RemoveData("Health");
}
```

### 동적 상태 관리

```csharp
// 실행 중인 FSM에 새 상태 추가
fsm.AddState(new JumpState());

// 상태 제거 (현재 상태는 제거 불가)
fsm.RemoveState<IdleState>();
```

### FSM 리셋

```csharp
// 현재 상태를 종료하고 모든 데이터를 지움, 등록된 상태는 유지
fsm.Reset();
// Start<TState>()로 재시작 가능
fsm.Start<IdleState>();
```

### FSM 제거

```csharp
// 표준: GameEntry를 통해 (com.gameframex.unity.entry 비의존)
var fsmComponent = GameEntry.GetComponent<FsmComponent>();
fsmComponent.DestroyFsm(fsm);

// 단축: GameApp을 통해 (com.gameframex.unity.entry 필요)
GameApp.Fsm.DestroyFsm(fsm);
```

## 문서 및 자료

- [문서](https://gameframex.doc.alianblank.com)

## 커뮤니티 및 지원

- QQ 그룹: 467608841 / 233840761

## 변경 로그

[Releases](https://github.com/gameframex/com.gameframex.unity.fsm/releases)에서 변경 로그를 확인하세요.


## 의존성

| 패키지 | 설명 |
|--------|------|
| (无) | - |

## 라이선스

자세한 내용은 [LICENSE.md](LICENSE.md) 파일을 참조하세요.

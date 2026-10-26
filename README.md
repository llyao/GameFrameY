# GameFrameY

GameFrameX + YooAsset + Luban + VContainer 的游戏客户端框架单仓，含自研的**模块管理 + 服务发现**层（GameFrameY.Core），服务端采用 GameFrameX.Server 快照。

```
GameFrameY/
├── Client/          # Unity 2022.3 LTS 工程（自研框架在 Packages/com.gameframey.core）
│   └── Packages/    #   全部第三方依赖以源码 vendored（embedded package，共 18 个，无第三方 registry）
├── Config/          # Luban 配置仓：一份定义，双端生成（客户端 cfg / 服务端 GameFrameY.Config）
│   └── Tools/       #   LubanSource/ 生成器全量源码(v5.1.0) + Luban/ 构建产物 + build-luban/vendor 脚本
├── Server/          # GameFrameX.Server 快照（.NET 10，Apache-2.0，来源见 Server/PROVENANCE.md）
├── Protobuf/        # proto3 协议（客户端/服务端共用约定）
├── docs/            # 架构说明 + 第三方源码清单（third-party.md）+ 交互式模块说明系列（site/，双击 site/index.html 打开）
└── docker-compose.yml  # MongoDB（服务端依赖）
```

技术栈：**GameFrameX**（组件底座，源码内置）· **YooAsset**（资源，源码内置，经 AssetComponent）· **Luban v5.1**（配置，生成器源码内置）· **VContainer**（DI 脊柱，源码内置）· **UniTask**（异步，源码内置）。

> 所有第三方组件（GameFrameX 包族 + YooAsset + VContainer + UniTask + Luban）都以**源码**随仓库分发，
> 版本/许可证/同步方式见 [docs/third-party.md](docs/third-party.md)。改 Luban 生成逻辑后跑
> `Config/Tools/build-luban.bat|sh` 重出工具；改 Unity 侧任意包源码，Unity 内直接生效。

## 快速开始

### 1. 打开客户端（Unity 2022.3 LTS）

1. Unity Hub → Open → `Client/`（全部第三方包已 vendored 为源码，无需第三方 registry；仅 ugui/SBP 等 Unity 官方包走官方源。ProjectVersion 标记 2022.3.54f1，用任意 2022.3.x 打开皆可）。
2. 打开 `Assets/Scenes/Boot.unity` → Play。

预期日志（冒烟通过标准）：

```text
[GameFrameY] GameFrameX 组件就绪，开始装配 Bootstrap 模块...
[GameFrameY] 模块就绪: Event (EventModule) ...（Event/Asset/Scene/Config/UI/Network 六个桥接模块）
[GameFrameY] 启动流程已开始（Launch → Patch → LoadConfig → EnterGame）
[GameFrameY] 资源包初始化完成
[GameFrameY] Luban 配置加载完成：TbItem 6 条，TbServerList 3 条
[GameFrameY] 模块就绪: Demo (DemoModule)
[GameFrameY] [Demo] TbItem[10001] = 木剑（新手木剑，聊胜于无）价格 10 品质 White；...
[GameFrameY] 模块就绪: Match3 (Match3Module)
[GameFrameY] [Match3] 第 1 关开始 8x8 颜色 5，目标 1500 分 / 20 步
[GameFrameY] GameFrameY 启动流程完成（GameReady）
[GameFrameY] [Demo] Tick 心跳 #1（每 3 秒一条）
```

屏幕上会出现消消乐棋盘：**拖拽或点选两颗相邻宝石交换**，产生三连即消除得分，级联有 Combo 倍率；达到目标分过关（下一关），步数用完失败（重试）。全部 UI/贴图/关卡数据均为程序生成，无任何二进制美术资源。

### 2. 内置示例：消消乐（Match3）

框架的完整实战用例，全链路走框架服务：

- **玩法**：按关卡 8x8~9x9、4~6 色，限步数冲目标分；三连消除、重力下落、顶部补充、级联 Combo 倍率、死局自动洗牌、过关/失败结算面板、重新开始按钮。
- **代码结构**（`Client/Assets/GameFrameY/Game/Modules/Match3/`）：
  - `Board/` 棋盘**纯逻辑**（无 Unity 依赖，可单测、可复用到服务端）；
  - `Match3UI` 运行时构建 UGUI（无 prefab）+ 拖拽/点选双输入 + UniTask 补间动画；
  - `Match3Assets` 经 **YooAsset** 加载 `Bundles/Atlases` 的宝石贴图，加载失败自动回退程序化生成（保证可玩）；
  - `Match3GameController` 关卡状态机：计分 = 消除数 × 基础分（**Luban** `TbMatch3Const`）× 级联序号，进度经 **IEventService** 广播。
- **改关卡**：`Config/Datas/match3level.json` → `Config/gen-client.*` → 回 Unity 即生效。
- **改美术**：`python tools/gen_match3_art.py`（纯 Python 画 PNG，调色在脚本里）。
- **回归测试**：`cd tools/Match3BoardSmoke && dotnet run`（随机 300 局 × 400 次交换校验棋盘不变量——首版就抓出过一个越界 bug）。
- **Core 模块 smoke**：`cd tools/CoreModuleSmoke && dotnet run`（覆盖稳定拓扑、重复名、缺失依赖、循环依赖和模块扫描过滤）。

### 3. 配置工作流

```bash
cd Config
./gen-client.sh    # 或 cmd: gen-client.bat —— 生成客户端 cfg 代码 + bytes（已入库一次产物）
./gen-server.sh    # 生成服务端 GameFrameY.Config 代码 + bytes
```

改表：`Config/Defines/schema.xml`（结构）+ `Config/Datas/*.json`（数据）→ 跑 gen → 回 Unity 即生效。
要求 .NET SDK 8+（工具已随仓库构建在 `Config/Tools/Luban/`）。

### 4. 服务端

```bash
docker compose up -d                 # MongoDB
cd Server && dotnet run --project GameFrameX.Launcher   # .NET 10 SDK；默认 TCP 29100 / HTTP 28080
cd Server/Config && dotnet build     # 我们的服务端配置库（Luban 生成）单独可编译
```

> `Server/` 为上游快照 + 我们的 `Config/` 增量，同步方式见 `Server/PROVENANCE.md`。

## 核心概念

- **模块**：`[GameModule("Name", Dependencies = new[]{"Asset"}, Startup = ModuleStartup.Deferred)]` 标记的类，
  自动发现 → 依赖拓扑排序 → VContainer 线性子作用域装配；`Dependencies` 用于排序和存在性校验，跨模块定位的服务必须显式导出到 `IServiceRegistry`。
- **服务**：优先构造注入；`IServiceRegistry` 提供中央登记表 + 根作用域兜底、登记/注销事件与 `WaitServiceAsync`（未就绪先等待）。
- **流程**：`GameFrameYGameLauncher` 组装 Launch → Patch → LoadConfig → EnterGame 四流程，并维护 `Created → Bootstrapping → Patching → LoadingConfig → EnteringGame → Ready` 启动状态；流程异常进入 `Failed`，EnterGame 只广播一次 `GameFrameY.GameReady`，模块 Shutdown 进入 `Stopping → Stopped`。

详见 [docs/architecture.md](docs/architecture.md)。

## 模块说明系列（交互式文档）

每个包 / 每个模块的详细说明（API、机制、互动演示、真实代码示例）见 [docs/site/index.html](docs/site/index.html)——
纯静态离线 HTML，双击即开：总览架构图、GameFrameY.Core 模块体系（拓扑排序模拟器）、GameFrameX 组件族、
Asset/YooAsset、UI/场景、网络、UniTask/VContainer 底座、Luban 配置管线、服务端、源码原理深读、端到端实战（从零做一个背包系统），共 11 章；每章按「快速上手 → API → 原理 → 内部/边界」组织。目录见 [docs/site/README.md](docs/site/README.md)。

## 冒烟/排障清单（首次打开工程）

本工程在无 Unity 编辑器的环境搭建，以下为已知低风险点：

| 症状 | 处理 |
|---|---|
| 包解析失败 | manifest 只依赖 Unity 官方源（ugui/内置 modules）；第三方包全部在 `Packages/` 下以源码形式存在，检查目录是否完整 |
| 某 GFX 包报缺依赖 | 该包 package.json 声明的 `com.gameframex.*` 依赖未 vendored——跑 `Config/Tools/vendor_packages.py` 补齐或按 docs/third-party.md 手动克隆 |
| Collector 窗口（YooAsset → Bundle Collector）提示 CollectPath/GUID 不匹配 | Luban 重新生成会重建 `Bundles/Config` 目录导致目录 GUID 变化：窗口里 Fix 即可，或把 `AssetBundleCollectorSetting.asset` 里 CollectorGUID 改为该文件夹 .meta 的 guid |
| EditorSimulate 下加载配置失败 | 确认 AssetBundleCollectorSetting 存在且 Config 组启用；菜单 YooAsset → Bundle Collector 查看 |
| Play 无 GameFrameY 日志 | Console 过滤 `[GameFrameY]`；确认 Boot 场景为 Build Settings 第 0 个 |
| 想切 HostPlayMode | `GameFrameYStartup.ConfigureGameFrameXOptions` 改 `EPlayMode.HostPlayMode`，并填 `GameFrameYLaunchSettings.HostUrl`（挂一个静态 HTTP 服务放 CDN 产物） |

## 路线图（后续基建）

- [ ] HostPlayMode 完整补丁流（静态版本/清单/断点续传/重试 UI）
- [ ] HybridCLR 接入（Deferred 模块移入热更程序集，`InstallDeferredModulesAsync(assembly)` 装配）
- [ ] Protobuf 生成工具接入（GameFrameX.Tools）与 NetworkService 联调
- [ ] VContainer SourceGenerator 启用
- [x] 根级 CI：Luban 生成一致性 + Core/Match3 smoke + Server build/test + package/asmdef 元数据检查
- [ ] Unity 2022.3.54f1 batchmode 编译与 EditMode/PlayMode 启动 smoke（需要匹配 Unity runner）

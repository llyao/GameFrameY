# GameFrameY

GameFrameX + YooAsset + Luban + VContainer 的游戏客户端框架单仓，含自研的**模块管理 + 服务发现**层（GameFrameY.Core），服务端采用 GameFrameX.Server 快照。

```
GameFrameY/
├── Client/          # Unity 2022.3 LTS 工程（自研框架在 Packages/com.gameframey.core）
│   └── Packages/    #   含 vendored 源码包：YooAsset fork（embedded，优先于 registry）
├── Config/          # Luban 配置仓：一份定义，双端生成（客户端 cfg / 服务端 GameFrameY.Config）
│   └── Tools/       #   LubanSource/ 生成器全量源码(v5.1.0) + Luban/ 构建产物 + build-luban 脚本
├── Server/          # GameFrameX.Server 快照（.NET 10，Apache-2.0，来源见 Server/PROVENANCE.md）
├── Protobuf/        # proto3 协议（客户端/服务端共用约定）
├── docs/            # 架构说明 + 第三方源码来源（third-party.md）
└── docker-compose.yml  # MongoDB（服务端依赖）
```

技术栈：**GameFrameX**（组件底座，UPM scoped registry）· **YooAsset**（资源，源码内置，经 AssetComponent）· **Luban v5.1**（配置，生成器源码内置）· **VContainer**（DI 脊柱）· **UniTask**（异步）。

> Luban 与 YooAsset 均以**源码**形式随仓库分发（版本、改动与同步方式见 [docs/third-party.md](docs/third-party.md)）；
> 改 Luban 生成逻辑后跑 `Config/Tools/build-luban.bat|sh` 重出工具，改 YooAsset 源码 Unity 内直接生效。

## 快速开始

### 1. 打开客户端（Unity 2022.3 LTS）

1. Unity Hub → Open → `Client/`（首次打开需要联网拉取 UPM 包；ProjectVersion 标记 2022.3.54f1，用任意 2022.3.x 打开皆可）。
2. 两个 scoped registry 已配置在 `Client/Packages/manifest.json`：GameFrameX（`com.gameframex.unity.*`）与 OpenUPM（VContainer）。
3. 打开 `Assets/Scenes/Boot.unity` → Play。

预期日志（冒烟通过标准）：

```text
[GameFrameY] GameFrameX 组件就绪，开始装配 Bootstrap 模块...
[GameFrameY] 模块就绪: Event (EventModule) ...（Event/Asset/Scene/Config/UI/Network 六个桥接模块）
[GameFrameY] 启动流程已开始（Launch → Patch → LoadConfig → EnterGame）
[GameFrameY] 资源包初始化完成
[GameFrameY] Luban 配置加载完成：TbItem 6 条，TbServerList 3 条
[GameFrameY] 模块就绪: Demo (DemoModule)
[GameFrameY] [Demo] TbItem[10001] = 木剑（新手木剑，聊胜于无）价格 10 品质 White；...
[GameFrameY] [Demo] Hello, GameFrameY! 模块装配 → 服务发现 → 解析 全链路 OK
[GameFrameY] GameFrameY 启动流程完成（GameReady）
[GameFrameY] [Demo] Tick 心跳 #1（每 3 秒一条）
```

### 2. 配置工作流

```bash
cd Config
./gen-client.sh    # 或 cmd: gen-client.bat —— 生成客户端 cfg 代码 + bytes（已入库一次产物）
./gen-server.sh    # 生成服务端 GameFrameY.Config 代码 + bytes
```

改表：`Config/Defines/schema.xml`（结构）+ `Config/Datas/*.json`（数据）→ 跑 gen → 回 Unity 即生效。
要求 .NET SDK 8+（工具已随仓库构建在 `Config/Tools/Luban/`）。

### 3. 服务端

```bash
docker compose up -d                 # MongoDB
cd Server && dotnet run --project GameFrameX.Launcher   # .NET 10 SDK；默认 TCP 29100 / HTTP 28080
cd Server/Config && dotnet build     # 我们的服务端配置库（Luban 生成）单独可编译
```

> `Server/` 为上游快照 + 我们的 `Config/` 增量，同步方式见 `Server/PROVENANCE.md`。

## 核心概念

- **模块**：`[GameModule("Name", Dependencies = new[]{"Asset"}, Startup = ModuleStartup.Deferred)]` 标记的类，
  自动发现 → 依赖拓扑排序 → VContainer 链式子作用域装配（只能注入自己声明依赖的服务，未声明引用装配期即失败）。
- **服务**：优先构造注入；`IServiceRegistry` 提供服务定位（两级查找）、登记/注销事件与 `WaitServiceAsync`（未就绪先等待）。
- **流程**：`GameFrameYGameLauncher` 组装 Launch → Patch → LoadConfig → EnterGame 四流程；EnterGame 装配 Deferred 业务模块并广播 `GameFrameY.GameReady`。

详见 [docs/architecture.md](docs/architecture.md)。

## 冒烟/排障清单（首次打开工程）

本工程在无 Unity 编辑器的环境搭建，以下为已知低风险点：

| 症状 | 处理 |
|---|---|
| 包解析失败 | 确认能访问 `gameframex.upm.alianblank.uk` 与 `package.openupm.com`；Package Manager → Refresh |
| asmdef 引用告警 "YooAsset / YooAsset.Runtime 不存在" | 两个名字按 fork 包实际保留其一即可，另一项告警可忽略（可删 `GameFrameY.Bridge.asmdef` 里多余的那行） |
| Collector 窗口（YooAsset → Bundle Collector）提示 CollectPath/GUID 不匹配 | Luban 重新生成会重建 `Bundles/Config` 目录导致目录 GUID 变化：窗口里 Fix 即可，或把 `AssetBundleCollectorSetting.asset` 里 CollectorGUID 改为该文件夹 .meta 的 guid |
| EditorSimulate 下加载配置失败 | 确认 AssetBundleCollectorSetting 存在且 Config 组启用；菜单 YooAsset → Bundle Collector 查看 |
| Play 无 GameFrameY 日志 | Console 过滤 `[GameFrameY]`；确认 Boot 场景为 Build Settings 第 0 个 |
| 想切 HostPlayMode | `GameFrameYStartup.ConfigureGameFrameXOptions` 改 `EPlayMode.HostPlayMode`，并填 `GameFrameYLaunchSettings.HostUrl`（挂一个静态 HTTP 服务放 CDN 产物） |

## 路线图（后续基建）

- [ ] HostPlayMode 完整补丁流（静态版本/清单/断点续传/重试 UI）
- [ ] HybridCLR 接入（Deferred 模块移入热更程序集，`InstallDeferredModulesAsync(assembly)` 装配）
- [ ] Protobuf 生成工具接入（GameFrameX.Tools）与 NetworkService 联调
- [ ] VContainer SourceGenerator 启用
- [ ] CI（Luban 生成校验 + Server build + Client batchmode 编译检查）

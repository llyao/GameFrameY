# GameFrameY 模块说明系列（交互式文档站）

纯静态 HTML 文档站，**离线可用**（无 CDN/外部依赖，双击 `index.html` 即可在浏览器打开）。
覆盖本仓库全部 20 个 vendored 包 + 自研层 + 配置管线 + 服务端的模块细节，每章带互动演示与真实代码示例。

## 目录

| 章 | 文件 | 内容 |
|---|---|---|
| 00 | `index.html` | 架构分层图（可点击）、20 包清单过滤器、8 步启动时序播放器、快速开始 |
| 01 | `core.html` | GameFrameY.Core：模块体系 / 服务发现 / 六桥接 / 四流程；**拓扑排序模拟器**、**服务注册表 & WaitServiceAsync 演示** |
| 02 | `gameframex.html` | [GameFrameX] 宿主自举动画、Event/FSM/Procedure/Setting/Mono；**事件总线（Fire→下一帧分发）演示** |
| 03 | `asset.html` | Asset/YooAsset：四种播放模式、句柄族；**引用计数演示**、排障清单 |
| 04 | `ui-scene.html` | UI 18 层 UIGroup、IUIForm 生命周期、UGUI 适配、场景管理；**UIGroup 层级栈**、**OpenAsync 流程播放器** |
| 05 | `network.html` | 通道制网络、分包管线、心跳/RPC、WebSocket、Protobuf 约定；**分包管线动画**、**心跳丢跳断连模拟** |
| 06 | `foundation.html` | UniTask/VContainer 底座 + LitJson/ReadAssets/Localization；**PlayerLoop 时机图**、**作用域链解析模拟** |
| 07 | `config.html` | Luban 一份定义双端生成全链路；**group 分组过滤演示**、**表数据浏览器（真实数据）** |
| 08 | `server.html` | GameFrameX.Server：状态-组件-代理三层（可点击）、热更新、部署端口 |

## 使用

- 直接双击 `index.html`（推荐 Chrome/Edge）；
- 侧边栏支持搜索（快捷键 `/`）、明暗主题切换；
- 所有代码块右上角有复制按钮；示例代码全部摘自仓库真实源码（标注了文件路径）。

## 维护

- 内容与代码同步更新：改了对应模块的 API 后顺手更新对应章节；
- 共享样式 `assets/style.css`、共享脚本 `assets/app.js`（导航清单在 app.js 顶部的 `SERIES` 数组，加新章节时改这里）；
- 章节强调色在 `SERIES` 各项的 `accent` 字段。

> 生成时间：2026-10-24 · 基于仓库 main@fc2c6e0 的源码调研

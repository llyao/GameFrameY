# Server 目录来源说明

本目录是 [GameFrameX/GameFrameX.Server](https://github.com/GameFrameX/GameFrameX.Server) 的快照（Apache-2.0）。

- 快照 commit：`638bd52793109edff97e3196b8e639a26567021a`（2026-10-10，main 分支）
- 引入日期：2026-10-23
- 上游架构：.NET 10 控制台 / Actor（TPL DataFlow）/ SuperSocket / MongoDB / State-Component-Agent / HTTP 热重载

## 本仓库的改动

- 新增 `Config/`：GameFrameY 服务端配置 classlib（Luban 生成的 `GameFrameY.Config.Tables` + 运行时），
  由仓库根 `Config/gen-server.bat|sh` 生成，不来自上游。
- 其余文件保持上游原样；后续基建优化建议以 patch 方式进行，便于与上游同步。

## 与上游同步（参考）

```bash
git remote add gfx-server https://github.com/GameFrameX/GameFrameX.Server.git
git fetch gfx-server
# 对比/合并 gfx-server/main 到 Server/（保留本仓库 Config/ 目录）
```

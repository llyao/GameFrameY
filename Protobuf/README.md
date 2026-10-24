# Protobuf 协议目录

客户端（GameFrameX NetworkComponent）与服务端（GameFrameX.Server，SuperSocket + protobuf-net）共用一份 proto3 协议。

## 约定（沿用 GameFrameX.Protobuf 规范）

- 一个功能模块一个 `module_xxx.proto` 文件，文件内声明 `option csharp_namespace`。
- 消息命名：请求 `XxxReq`、响应 `XxxResp`、推送 `XxxNotify`。
- 消息 ID = `(moduleId << 16) + seqId`，`moduleId` 全局唯一（demo 取 10，正式模块从 100 起分配）。
- 字段编号 1~799，预留给框架的字段号从 800 起。

## 代码生成

使用 [GameFrameX.Tools](https://github.com/GameFrameX/GameFrameX.Tools)（proto → C# 服务端 / Unity 客户端 / TypeScript）：

```bash
# 安装后（CLI 或 Avalonia GUI）
gameframex-tools proto gen --input ./proto --output-csharp ../Server/GameFrameX.Proto --output-unity ../Client/Assets/GameFrameY/Proto
```

> 当前仓库仅放置协议与示例；生成工具接入属于后续基建（见根 README 路线图）。

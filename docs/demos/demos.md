---
uid: Guides.Demos
title: 示例项目
---

# 示例项目

## 基础

### [Kook.Net.Samples.SimpleBot](https://github.com/gehongyan/Kook.Net/tree/master/samples/Kook.Net.Samples.SimpleBot)

该项目演示了创建并启动 Bot 的基本方法，并实现了一个简单的 !ping 命令。
当用户调用该命令时，Bot 回复一条包含按钮的卡片消息，用户点击该按钮，Bot 会再回复一条消息。

该示例项目还罗列了 Kook.Net 所公开的所有事件。

### [Kook.Net.Samples.ReactionRoleBot](https://github.com/gehongyan/Kook.Net/blob/master/samples/Kook.Net.Samples.ReactionRoleBot)

该示例项目演示了如何通过订阅到 Kook.Net 中的多个事件，来完成不同的操作。
当用户在指定的消息上添加或撤销回应时，Bot 会为该用户添加或撤销服务器的角色。

## 文本命令框架

### [Kook.Net.Samples.TextCommands](https://github.com/gehongyan/Kook.Net/tree/master/samples/Kook.Net.Samples.TextCommands)

该示例项目演示了如何使用 Kook.Net 中的文本命令框架，来构建一个易于扩展与维护的基于文本的命令交互 Bot。

## 语音

### [Kook.Net.Samples.Audio](https://github.com/gehongyan/Kook.Net/tree/dev/samples/Kook.Net.Samples.Audio)

该示例项目演示了如何使用 Kook.Net 中的语音功能，来构建一个点歌 Bot。

## 语言变体

### [Kook.Net.Samples.FSharp](https://github.com/gehongyan/Kook.Net/tree/dev/samples/Kook.Net.Samples.FSharp)

该示例项目演示了如何使用 F# 构建基于 Kook.Net 的 Bot 的方法。

### [Kook.Net.Samples.VisualBasic](https://github.com/gehongyan/Kook.Net/tree/dev/samples/Kook.Net.Samples.VisualBasic)

该示例项目演示了如何使用 Visual Basic 构建基于 Kook.Net 的 Bot 的方法。

## 部署

### [Kook.Net.Samples.Docker](https://github.com/gehongyan/Kook.Net/tree/dev/samples/Kook.Net.Samples.Docker)

该示例项目演示了如何编写 Dockerfile 来创建封装 Bot 能力的镜像。

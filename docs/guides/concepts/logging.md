---
uid: Guides.Concepts.Logging
title: 日志
---

# 在 Kook.Net 中记录日志

Kook.Net 中提供了一个日志事件，所有的日志消息都会通过此事件传递。

有关 Kook.Net 中的事件，请参阅 [事件] 章节。

[事件]: xref:Guides.Concepts.Events

> [!WARNING]
> Kook.Net 中所有的日志实现处理程序都将在网关线程上同步执行，
> 如果您要将日志消息输出到 Web API 中（例如：Sentry、Stackdriver、KOOK 频道等），
> 建议将输出程序包装在 `Task.Run` 中，以避免网关线程在等待数据日志输出时阻塞。
> 更多有关网关线程的信息，请参阅 [事件](events.md#线程安全性) 章节。

## 在客户端中记录日志

要处理日志，只需将日志处理程序订阅至 @Kook.Rest.BaseKookClient.Log 事件，
日志处理程序需接收一个 [LogMessage] 对象，返回 `Task` 对象。

[LogMessage]: xref:Kook.LogMessage

## 在命令中记录日志

Kook.Net 的 [CommandService] 也提供了 @Kook.Commands.CommandService.Log
事件，其签名与其它日志事件相同。

通过日志事件记录的数据往往与 [CommandException] 相结合使用，其中包含了命令上下文与异常信息。

[CommandService]: xref:Kook.Commands.CommandService

[CommandException]: xref:Kook.Commands.CommandException

## 示例

[!code-csharp[Logging Sample](samples/logging.cs)]

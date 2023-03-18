---
uid: Guides.Concepts.Events
title: 事件
---

# Kook.Net 中的事件

Kook.Net 中的事件采用与标准 .NET 事件模式类似的方式实现，
不同的是，Kook.Net 中的事件类型都为 [Task]，
事件参数传递不使用 [EventArgs]，而是直接传递到事件处理程序中。

这使得事件处理程序可以直接在异步上下文中执行，事件返回类型为
[Task]，而不是 `async void`。

[Task]: https://docs.microsoft.com/dotnet/api/system.threading.tasks.task

[EventArgs]: https://docs.microsoft.com/dotnet/api/system.eventargs

## 用法

要从事件中接收数据，只需通过 C# 委托的事件模式进行订阅。

订阅事件支持命名函数，也支持匿名函数（Lambda 表达式）。

## 线程安全性

所有的事件都被设计为线程安全的，所有的事件都拥有与网关线程相同的上下文，
在网关线程之外的任务上同步运行，

但这样做也会存在副作用，这可能会导致网关现成死锁并终止连接。
经验之谈，任何耗时超过 3 秒的任务都**不应该**直接在事件上下文中等待，
而是应该包装在 `Task.Run` 中执行，或是卸载到另外一个任务中。

这意味着您不应该在与事件相同的上下文中通过 KOOK 网关请求数据，
由于网关线程将等待所有被调用的事件处理程序完成，然后才会处理所有来自网关的任何其它数据，
这将导致一个无法恢复的死锁。

## 常见模式

Kook.Net 中的事件签名都是形如 `Func<T1, ..., Task>` 的模式，没有额外定义名称，
因此，有关方法签名的详细信息，请参考 IntelliSense 智能提示，或直接浏览 API 文档。

不过，Kook.Net 中的事件签名大多遵循类似的模式，还是可以让您从中推断参数定义。

### 实体变更

具有 `Func<Entity, Entity, Task>` 签名的事件处理程序通常表示一个实体中的信息发生了变更，
两个实体中，前者为发生变更前实体的副本，后者为变更执行完成后的实体。

此模式通常仅在 `EntityUpdated` 事件中出现。

### 缓存实体

具有 `Func<Cacheable, Entity, Task>` 签名的事件处理程序则通常表示 API
或网关并未提供实体发生变更前的状态，因此它可以从客户端的缓存中提取或从 API 中下载。

有关此对象的更多信息，请参阅 [Cacheable] 文档。

[Cacheable]: xref:Kook.Cacheable`2

> [!NOTE]
> 许多与消息相关的实体（例如：`MessageUpdated` 和 `ReactionAdded`）依赖于客户端的消息缓存，
> 该特性默认**不启用**，因此，如果您需要使用它，请在 @Kook.WebSocket.KookSocketConfig
> 中通过设置 `MessageCacheSize` 的值来启用该消息缓存。

## 示例

[!code-csharp[Event Sample](samples/events.cs)]

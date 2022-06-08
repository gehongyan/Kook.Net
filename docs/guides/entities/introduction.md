---
uid: Guides.Entities.Intro
title: 实体简介
---

# KaiHeiLa.Net 中的实体

KaiHeiLa.Net 提供了一系列多用途的实体类，来表示来自开黑啦 API 或网关的数据。

> [!TIP]
> 在使用 `Get***` 系列方法来获取包含详细信息的实例实体时，需要为方法指定正确的 ID，
> 因此，建议您启用开黑啦的开发者模式，以便于获取实体的 ID，开发者模式可在
> “用户设置 - 高级设置 - 开发者模式” 处启用。

## 实体的变体与继承

由于同一对象可能会在开黑啦 API 或网关不同的接口或事件中出现，
在不同的数据上下文中包含不同的字段，因此，KaiHeiLa.Net 中的部分实体拥有多种变体，
例如：`IUser` 和 `IGuildUser` 分别表示通用的用户实体和服务器内的用户实体。

尽管部分实体包含的信息很简略，但所有的实体模型都有一个包含尽可能详细的信息的实体变体与之对应。

## Socket 与 REST

REST 实体通过 HTTP API 获取，其在被使用后会被销毁释放。每一个 REST
实体的获取都会引起一次 API 请求，短时间内发起大量 API 请求可能会触发速率限制，
因此，请尽可能少地通过 REST 获取实体。

- [有关 REST 的更多信息](https://restfulapi.net/)

Socket 实体中的大多数都是通过 `KaiHeiLaSocketClient` 的网关连接中的各种事件创建的，
仅当客户端初始化或事件包含的数据过于简略时，客户端才会进一步通过 API 获取的 REST 实体补全信息。
这些 Socket 实体都会进入到客户端的全局缓存中，以在业务逻辑中使用。

以 `MessageReceived` 事件为例，事件所传递的数据为 `SocketMessage` 实体，
实体内指示消息所来源的频道的属性为 `SocketMessageChannel` 实体。
所有的消息都来源于支持发送消息的频道类型，
因此，频道的这种实体变体可以覆盖全部需要发送消息的频道的情况。

但这并不是说消息不能来源于 `SocketTextChannel`（`SocketTextChannel` 表示服务器内的文本频道），
如果要从一个消息实体获取消息所在服务器的信息，则需要将其频道实体类型转换为 `SocketTextChannel`。

> [!NOTE]
> 有关各种实体的继承关系及其定义，请参阅 [实体词汇表](xref:Guides.Entities.Glossary)。

## 导航属性

多数 Socket 实体都有一个内部的导航属性，以便于实体向其父类或派生类的转换与访问。

## 访问 Socket 实体

实体最基本的形式形如 `SocketGuild`、`SocketUser`、`SocketChannel` 等，
这些实体可以从 `KaiHeiLaSocketClient` 的全局缓存中获取，
也可以在 `KaiHeiLaSocketClient` 上通过相应的 `Get***` 方法获取。

在这些实体上进一步调用 `Get***` 方法可以获得包含更多信息的实体变体，例如：
`SocketGuild.GetUser` 可以获得 `SocketGuildUser` 实体，
`SocketGuild.GetChannel` 可以获得 `SocketGuildChannel` 实体。
按业务逻辑的需要，将这些实体进行进一步的类型转换便可获得实体的其它变体。

### 示例

[!code-csharp[Socket Sample](samples/socketentities.cs)]

## 访问 REST 实体

REST 实体的工作方式几乎与 Socket 实体相同，只是在每一次获取时都会发起一次
API 请求，因此较少使用。

访问 REST 实体需要通过 `KaiHeiLaRestClient` 的 `Rest` 属性进行，
或是创建一个新的 [KaiHeiLaRestClient] 实例，可以独立于网关线程。

[KaiHeiLaRestClient]: xref:KaiHeiLa.Rest.KaiHeiLaRestClient

### 示例

[!code-csharp[Rest Sample](samples/restentities.cs)]
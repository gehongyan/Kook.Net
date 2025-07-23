---
uid: Changelog
title: 变更日志
---

# 变更日志

## v0.9.11 [2025-06-25]

### 新增

- 新增对消息模板相关 API 的支持。
- `Debug` 模式的日志级别现在会输出网关会话 ID。

### 变更

- 各个发送消息的方法中 `templateId` 参数的类型从 `int` 变更为 `ulong`，以支持更大的消息 ID 范围。

### 修复

- 修复因角色渐变色数据结构变更导致应用程序启动失败的问题。

### 其它

- 新增消息模板接口相关的快速指南文档。

## v0.9.10 [2025-06-18]

### 修复

- 修复文本命令框架无法正确解析文件、音频、视频的卡片模块作为 `Uri` 参数的问题。

## v0.9.9 [2025-06-01]

### 修复

- 修复导致文本命令框架无法正确注册枚举类型参数的问题。

## v0.9.8 [2025-05-20]

### 新增

- 新增 `BaseSocketClient.UnknownDispatchReceived`，用于处理未知的事件类型。
- 新增 `KookSocketConfig.SuppressUnknownDispatchWarnings` 用于绕过未知事件类型的警告。

### 修复

- 修复 `IIntimacyRelation` 上的 `RelationType` 及 `CreatedAt` 可能为空导致无法获取亲密关系信息的问题，相关属性已变更为可空属性。
- 修复 `SocketMessage.Update` 可能会异常阻塞异步线程的问题。

### 其他

- 更新相关依赖库版本。
- 迁移测试框架至 xunit.v3。

## v0.9.7 [2025-03-09]

### 新增

- 新增 Webhook 客户端多实例注入的支持。

### 修复

- 修复 `AddKookAspNetWebhookClient` 接收 `KookAspNetWebhookConfig` 的重载未能正确注册客户端实例的问题。

### 其他

- 移除对 `FluentAssertions` 的依赖。
- 修正 F# 示例代码中的空引用警告。
- 优化集成测试项目中的实体创建过程，使用 `IAsyncLifetime`。
- 改进 OAuth 示例项目，提供获取所加入服务器的示例代码。

## v0.9.6 [2025-01-05]

### 新增

- 新增对亲密关系管理的支持。
- 管道消息客户端新增支持发送卡片消息，及对消息回复与临时消息可见用户的支持。
- `ICard` 接口添加 `Modules` 属性。

### 修复

- 修正抢先速率限制的速率值。
- 修复消息缓存最大容量可能会溢出的问题。
- 修复服务器频道删除时未能正确更新缓存的问题。

### 其它

- 移除了对 .NET 6.0 目标框架的支持，更新依赖注入、消息队列、主机托管等扩展包的目标框架至 .NET 9.0。
- 更新所有外部引用的 NuGet 包至最新版本。

## v0.9.5 [2024-11-13]

### 新增

- 新增 Kook.Net.Pipe 包，提供 `KookPipeClient`，用于发送管道消息。
- 新增 `TokenType.Pipe`，但仅用于指示内部 API 客户端以管道令牌类型登录，不应用于用户代码的客户端登录。

### 其它

- 移除目标框架 .NET 7.0，添加目标框架 .NET 9.0。

## v0.9.4 [2024-11-12]

### 新增

- 新增支持发送与修改模板消息。
  - `IMessageChannel.SendTextAsync`、`IMessageChannel.SendCardsAsync`、`IDMChannel.SendTextAsync`
    及 `IDMChannel.SendCardsAsync` 方法新增接收参数 `templateId`、`parameters` 及 `jsonSerializerOptions`
    的重载，可用于发送模板消息。
  - `MessageProperties` 新增 `TemplateId`、`Parameters` 及 `JsonSerializerOptions` 属性，用于修改模板消息。
  - 新增 `MessageProperties<T>` 类，用于支持传递泛型 `parameters` 参数。
- 文本命令框架支持注入键控服务，`FromKeyedServicesAttribute` 可用于从键控服务中获取服务实例。

### 修复

- 修复 `SocketGuild.CurrentUser` 为空的问题。
- 修复私聊消息可能无法正确编辑的问题。

## v0.9.3 [2024-10-27]

### 新增

- 新增对帖子频道的基本支持。新增帖子频道类型 `ChannelType.Thread`，新增帖子频道相关实体
  `IThreadChannel`、`RestThreadChannel`、`SocketThreadChannel`，新增对帖子频道的获取、创建、修改、删除操作的支持。
- 新增权限 `GuildPermission.ReplyToPost`、`ChannelPermission.ReplyToPost`
  表示发布帖子回复的权限，并在各与权限相关的方法中添加对帖子频道权限的支持。
- 新增 `SocketRole.Members`，可用于获取缓存中拥有指定角色的所有用户。

### 修复

- 修复 `SocketGuildUser.RemoveRolesAsync` 方法未能处理 `RequestOptions` 的问题。
- 修复 `IChannel.Name` 可能为空的问题。

## v0.9.2 [2024-10-18]

### 新增

- 支持发送无边主题 `CardTheme.Invisible` 卡片
- `DefaultRestClient` 支持传入 `IWebProxy` 实例
- 为 `Color`、`AlphaColor`、`GradientColor` 实现 `IEquatable<T>` 接口

### 修复

- 修复部分 XML 文档中的符号引用错误
- 修复尝试通过发送消息方法返回的可缓存对象获取消息实体时返回 `null` 的问题

### 其它

- 启用 SourceLink

## v0.9.1 [2024-09-07]

### Fixed

- 修复频道创建者字段为空字符串导致的启动失败的问题

## v0.9.0 [2024-08-30]

### 更新路线

本次更新增加了以下新的软件包：

- `Kook.Net.Webhook`：Webhook 支持包
- `Kook.Net.Webhook.HttpListener`：HTTP Listener 的 Webhook 实现包
- `Kook.Net.Webhook.AspNet`：与 ASP.NET 集成的 Webhook 实现包
- `Kook.Net.MessageQueue.InMemory`：内存消息队列支持包
- `Kook.Net.MessageQueue.MassTransit`：MassTransit 消息队列支持包
- `Kook.Net.DependencyInjection.Microsoft`：Microsoft.Extensions.DependencyInjection 依赖注入扩展包
- `Kook.Net.Hosting`：主机服务扩展包

`Kook.Net` 包新增了对 `Kook.Net.Webhook` 包的引用，移除了对 `Kook.Net.CardMarkup`
包的引用，如需继续使用有关通过标记语言构建卡片消息的功能，请单独引用 `Kook.Net.CardMarkup` 包。

语音连接与推流功能已变更为由受官方支持的方式实现，新增的接收语音数据流相关的 API 为实验性功能，不受官方支持。

`Emote.Parse` 在发生错误时会引发的异常从 `ArgumentException` 变更为 `FormatException`。`Emote.ToString` 的返回结果变更为等效于
`ToKMarkdownString` 的结果。`MessageExtensions.GetJumpUrl` 所提供的链接格式进行了调整。
`SocketGuild.ValidBoostSubscriptions` 变更为 `SocketGuild.ActiveBoostSubscriptions`。`Tag<T>` 变更为 `Tag<TKey, TValue>`。

### 新增

- 新增支持自定义消息队列，默认实现为同步消息处理，支持通过安装扩展 NuGet 包并配置 `KookSocketConfig.MessageQueueProvider`
  来设置消息队列提供者。`Kook.Net.MessageQueue.InMemory` 为内存队列支持包，`Kook.Net.MessageQueue.MassTransit` 为
  MassTransit 队列支持包。用法请参考示例。
- 新增支持 Webhook 模式，目前所实现的 Webhook 模式建立在 Socket 实现之上，Kook.Net 所集成的 Webhook 为抽象类
  `KookWebhookClient`。`Kook.Net.Webhook.HttpListener` 为 HTTP Listener 的 Webhook 实现包，`Kook.Net.Webhook.AspNet` 为与
  ASP.NET 集成的 Webhook 实现包。用法请参考示例。
- 新增对 Microsoft.Extensions.DependencyInjection 依赖注入框架的扩展方法包 `Kook.Net.DependencyInjection.Microsoft`
  ，用于支持快捷添加 Kook.Net 中各种客户端的服务，用法请参考示例。
- 新增基于 `IHost` 及 `IHostedService` 扩展的主机服务扩展包 `Kook.Net.Hosting`，用于支持快捷添加 Kook.Net
  中各种客户端的主机服务，用法请参考示例。
- 新增支持接收语音数据流相关的 API。（实验性功能，不受官方支持）
- 文本命令框架新增内置支持对 `DateOnly` 与 `TimeOnly` 类型的参数解析，新增支持对 `Uri` 类型的参数解析，新增对图文混排消息的命令解析。
- `IUserMessage` 上新增扩展方法 `MaybeTextImageMixedMessage`，用于判断是否可能为图文混排消息。
- 公开 `Emote` 的构造函数。
- 新增 `KookComparers` 类，用于支持 KOOK 实体按 ID 比较。
- 新增 `Color` 上的 `Parse` 与 `TryParse` 方法。
- `IKookClient` 提供 `LoginAsync` 与 `LogoutAsync` 方法。
- 新增 `TagUtil`，用于转换 `ITag` 为 `Tag<TKey, TValue>`。

### 修复

- 修复 `AudioClient.ClientDisconnected` 事件未能被正确引发的问题。
- 修复 `Rest/SocketGuildUser` 上 `IsOwner` 可以被确定但值为 `null` 的问题。
- 修复 `RequireRoleAttribute` 可以被错误地添加到不恰当的目标的问题。
- 修复服务器成员更新事件未携带昵称参数时引发异常的问题。
- 修复 `IGuild` 上的实现未公开 `IsAvailable` 属性的问题。
- 修复 `ITag` 中的 `EveryoneMention` 和 `HereMention` 的值可能为 `0` 而非 `0U` 的问题。

### 变更

- 用户代码通过 API 操作服务器成员的角色后，框架会尝试更新缓存，以在不通过 API 更新用户角色信息时，可以通过缓存获得尽量准确的角色信息。
- `Emote.Parse` 在发生错误时会引发的异常从 `ArgumentException` 变更为 `FormatException`。
- `Emote.ToString` 的返回结果变更为等效于 `ToKMarkdownString` 的结果。
- 语音连接与推流功能已变更为由受官方支持的方式实现。
- 变更 `MessageExtensions.GetJumpUrl` 所提供的链接格式，服务器频道更改为官方新增支持的链接格式，私聊频道中的参数调整为使用聊天代码。
- 变更 `Tag<T>` 为 `Tag<TKey, TValue>`。
- 调整 `IUserMessage.Resolve` 的结果，使其更符合 KMarkdown 的格式。
- `ModuleBase.ReplyTextAsync` 上的第一个参数重命名为 `text`。
- 重命名 `SocketGuild.ValidBoostSubscriptions` 为 `SocketGuild.ActiveBoostSubscriptions`。
- `Kook.Net` 包不再包含对 `Kook.Net.CardMarkup` 的引用。

### 移除

- 由于大量非官方接口已被禁用，Kook.Net.Experimental 上的大部分 API 均已移除。

### 其它

- XML 文档已重写为简体中文。
- 新增 MessageQueue、Webhook、OAuth 的用法示例。
- 新增简体中文 README。

## v0.8.0 [2024-05-28]

### 更新路线

由于 KOOK API 变更，Bot 用户现已无法在启动时通过 `/guild/index`
接口一次性获取全部所需的服务器基础信息，而是需要通过 `/guild/view` 接口遍历各个服务器，这会导致加入过多服务器的 Bot
会在启动时消耗过长时间，并大量发起 API 请求。因此，当前版本引入 `KookSocketConfig.StartupCacheFetchMode` 配置项，用于定义
Bot 启动时加载服务器所需基础数据的方式。

- `Synchronous`：同步模式。客户端启动时获取到服务器的简单列表后，会先通过 API
  遍历获取所需服务器的基础数据，全部获取完成后再触发 `Ready` 事件。
- `Asynchronous`：异步模式。客户端启动时在获取到服务器的简单列表后立即触发 `Ready` 事件，再在启动后台任务拉取所有服务器的基础数据。
- `Lazy`：懒模式。客户端启动时在获取到服务器的简单列表后立即触发 `Ready` 事件，不主动拉取服务器基础数据，当网关下发涉及到服务器的事件时，
  会对未获取基础数据的服务器对象通过 API 获取信息。
- `Auto`：自动模式，默认值。客户端的启动模式根据 Bot
  所加入的服务器数量自动判断，当服务器数量达到 `LargeNumberOfGuildsThreshold`（默认为 50）时为 `Lazy`
  ，否则若达到 `SmallNumberOfGuildsThreshold`（默认为 5）时为 `Asynchronous`，否则为 `Synchronous`。该判断将在每次 Bot 连接
  WebSocket 时进行。

在未使用 `Synchronous` 模式时，在 `Ready`
事件之后，未经事件主动访问缓存的服务器实体时，可能会获取到未完整包含服务器基础数据的缓存实体，`IsAvailable`
属性指示该服务器实体是否已经通过 API 完整缓存基础数据。在这种情况下，请主动调用 `UpdateAsync` 方法来通过 API 更新缓存服务器实体。
上述的服务器基础数据主要指服务器的频道、角色、频道权限覆盖、当前用户在服务器内的昵称等信息。

已针对整个框架的代码添加了空引用静态分析诊断的特性，有关可为空引用类型的 C#
概念，请参阅 [可为空引用类型 - C# | Microsoft Learn]。更新至当前版本后，所有可能为空的类型都会被标记为可为空引用类型，
这可能会导致一些代码在编译时产生警告，这些警告应该被视为潜在的空引用异常，应该根据实际情况进行修复。

另外，`IQuote` 新增了一个实现 `MessageReference`，这仅包含要被引用的消息 ID，用于在用户代码调用 API 时传入。原有创建 `Quote`
的用户代码应尽快迁移至 `MessageReference`。

`fileName` 已重命名为 `filename`；事件参数 `Cacheable<SocketMessage, Guid>`
变更为 `Cacheable<IMessage, Guid>`；`SectionAccessoryMode.Unspecified` 现已由 `null` 代替；`Format.StripMarkDown`
被重命名为 `StripMarkdown`；`SendFileAsync` 中接收 `Steam` 类型的重载中的 `filename`
参数现在为必选参数。请注意这些变更可能会导致编译错误，应根据实际情况进行修复。

### 新增

- `KookSocketConfig` 新增 `StartupCacheFetchMode`、`LargeNumberOfGuildsThreshold` 及 `SmallNumberOfGuildsThreshold`
  配置项，用于自定义 Bot 的 Socket 客户端在启动时通过 API 获取缓存所需服务器基础数据的方式
- `KookSocketConfig` 上新增两个配置项 `AutoUpdateRolePositions` 与 `AutoUpdateChannelPositions`，默认为 `false`
  。当启用时，会在相关事件下发时自动通过 API 获取数据，以维护缓存中的角色排序信息与频道排序信息。
- `Embed` 添加了 `CardEmbed`
- 卡片实体与构造器现在实现了 `IEquatable<T>`
- SocketSelfUser 现在实现了 `IUpdateable`
- 添加了 `IGuild.RecommendInfo.Certifications`
- `IQuote` 新增新的实现 `MessageReference`，这仅包含要被引用的消息 ID，用于在用户代码调用 API 时传入
- 添加了对事件类型 `embeds_append`、`sort_channel`、`updated_server_type`、`batch_added_channel`、`batch_updated_channel`、
`batch_deleted_channel`、`live_status_changed`、`PERSON` 类型的 `updated_guild`、`add_guild_mute`、`delete_guild_mute`、
`unread_count_changed` 的支持，但暂时无法确认这些事件是否会实际下发。

### 修复

- 修复私聊消息的作者不正确的问题
- 修复 `SocketUserMessage.Quote.Author` 可能为空的问题
- 修复消息中引用不存在的实体时，Tags 缺失对应值的问题
- 修复语音客户端未能正确处理未定义事件导致推流崩溃的问题
- 修复解析新引入的图文混排消息失败的问题
- 修正用户昵称更新行为不正确的问题

### 变更

- 启用可为空引用类型特性，有关可为空引用类型的 C# 概念，请参阅 [可为空引用类型 - C# | Microsoft Learn]
- 卡片构造器的各种验证已推迟到调用 `Build` 时进行
- 卡片内涉及到列表的属性的类型已变更为 `IList<T>`
- `Quote.Empty` 及其公开构造函数已标记 `Obsolete` 特性，应使用 `MessageReference`
- `fileName` 已重命名为 `filename`
- `SendFileAsync` 中接收 `Steam` 类型的重载中的 `filename` 参数现在为必选参数
- `BaseSocketClient._baseConfig` 重命名为 `BaseConfig`
- 事件参数 `Cacheable<SocketMessage, Guid>` 变更为 `Cacheable<IMessage, Guid>`，以解决下载实体失败的问题
- `SectionAccessoryMode.Unspecified` 现已移除，请使用 `null` 代替
- `Format.StripMarkDown` 被重命名为 `StripMarkdown`，原方法已标记 `Obsolete`
- Format.StripMarkdown 现在会移除连字符 `-`

### 其它

- 在 .NET 7 及以前的目标框架上添加了对 `PolySharp` 的引用，以支持一些新特性在旧框架上的实现
- 新增部分 Socket 事件的集成测试

[可为空引用类型 - C# | Microsoft Learn]: https://learn.microsoft.com/zh-cn/dotnet/csharp/nullable-references

## v0.7.0 [2024-04-02]

### 更新路线

KOOK 客户端现已支持在语音频道内发送消息，与此相关的以下 API 产生了变动：

- `CreateVoiceChannelProperties` 现已继承自 `CreateTextChannelProperties`
- `ModifyVoiceChannelProperties` 现已继承自 `ModifyTextChannelProperties`
- `IVoiceChannel` 现已继承自 `ITextChannel`
- `ChannelPermissions.Voice` 的值已附加文字频道的权限
- `SocketTextChannel.GetMessagesAsync` 系列方法现已为虚方法，以供 `SocketVoiceChannel` 重写

需要注意的是，受限于 KOOK API，语音频道内不支持通过 API 获取历史消息，无置顶消息功能，因此，语音频道上不支持调用
`IMessageChannel` 的 `GetMessagesAsync` 与 `GetPinnedMessagesAsync` 方法。

另外，语音频道在 API 层面支持操作 `Topic` 及 `SlowModeInterval`，但 KOOK 客户端暂无相关表现。

创建频道时不支持立即指定 `Topic`，`CreateTextChannelProperties` 中的 `Topic` 属性不生效，现已移除，
请在创建频道后调用 `ITextChannel.ModifyAsync` 方法进行修改。

### 新增

- 新增支持通过 XML 定义卡片消息
- `IVoiceChannel` 现已继承自 `ITextChannel`，相关实现已更新
- `BaseKookClient` 新增 `SentRequest` 事件
- 为卡片消息解析相关的 `Try*` 方法添加非空结果提示诊断

### 修复

- 修正 `ConnectionManager` 引发 `Disconnected` 时 `State` 不正确的问题

### 变更

- CreateTextChannelProperties 中的 Topic 属性已移除

### 其它

- 标记项目不支持 AOT 及程序集裁剪
- 新增示例项目 Kook.Net.Samples.CardMarkup
- 新增 XML 定义卡片消息的相关文档
- 新增 Logo

## v0.6.0 [2024-02-28]

### 更新路线

KOOK 内不支持在文本消息中提及语音频道，即 `IVoiceChannel` 不可被提及，现已不再派生自 `IMentionable`
接口。有关提及 `IVoiceChannel` 的错误用法应该予以移除或变更。

有关取消令牌的的名称已从 `CancelToken` 更改为 `CancellationToken`，现有关于取消用牌的方法、变量、属性、参数的名称都应该更新。

### 新增

- 新增语音频道的连接与推流

### 修复

- 修复 `KookSocketClient` 转换为 `IKookClient` 或 `BaseKookClient` 时 `ConnectionState` 不正确的问题

### 变更

- `IVoiceChannel` 不再派生自的 `IMentionable` 接口
- 重命名方法、变量、属性、参数的名称 `CancelToken` 为 `CancellationToken`

### 其它

- 更改文档模板，新增示例项目页面，新增语音推流文档

## v0.5.5 [2024-02-02]

### 更新路线

`RequireUserAttribute` 不再支持通过 `IUser` 参数构造，请改用 `IUser.Id` 作为参数。

### 新增

- 文本命令框架先决条件新增 `RequireRoleAttribute`
- 文本命令框架 `CommandAttribute` 新增 `Aliases`、`Summary`、`Remarks` 属性及可选参数

### 修复

- 修复了 Bot 启动时下载数据的异常没有被正确地输出到日志的问题

### 变更

- 移除了 `RequireUserAttribute` 上不切实际地接收 `IUser` 参数的构造函数

## v0.5.4 [2024-01-06]

### 更新路线

`KookRestApiClient` 的 `AuthTokenType` 与 `AuthToken` 属性的 set 访问性已更改为
private，用户代码对这些属性的更改可能会导致框架运行异常，如需使用不同的身份认证登录 KOOK 网关，请在 `LogoutAsync`
后重新 `LoginAsync`。

### 新增

- `IUser` 接口上新增 `HasAnnualBuff`、`IsSystemUser`、`Nameplates` 属性
- `UserTag` 上新增 `BackgroundColor` 属性
- 新增支持解析互动表情

### 变更

- `KookRestApiClient` 的 `AuthTokenType` 与 `AuthToken` 属性的 set 访问性已更改为 private

## v0.5.3 [2023-11-15]

### 更新路线

`IVoiceChannel.ServerUrl` 属性已移动至 `IAudioChannel` 接口，所有使用此属性的用法都需要更新。

### 新增

- 在 `IAudioChannel` 接口上新增语音区域相关属性
- `IVoiceChannel.ModifyAsync` 方法参数新增支持修改语音区域相关属性
- 为卡片、模块、元素构造器相关类新增有参构造函数

### 变更

- `IVoiceChannel.ServerUrl` 属性已移动至 `IAudioChannel` 接口
- 移除了部分属性上的意外提供的公开 set 访问器

### 修复

- 修复了 `SectionModuleBuild` 的构建校验条件不正确的问题

### 其它

- 替换测试 Mock 框架 Moq 为 NSubstitute
- 新增 Docker 使用示例
- 新增面向 .NET 8 的目标框架构建

## v0.5.2 [2023-08-18]

### 更新路线

`ModifyEmoteNameAsync` 方法的参数 `Action<string>` 已变更为 `string`，所有使用此方法的用法都需要更新。

### 修复

- 修复了 `SocketReaction` 的比较不正确导致的回应移除时缓存中的消息的回应未能被正确移除的问题
- 修复了 Socket 客户端断开连接时未能正确发送关闭代码的问题
- 修复了 GetMessagesAsync 方法查询参考消息之后的消息时结果不正确的问题
- 修复了 ModifyEmoteNameAsync 方法未能正确重命名服务器表情的问题

### 其它

- 补全了快速参考指南
- 为示例程序 `SimpleBot` 加入实际的功能，能够响应一个简单的命令，回复按钮，并对按钮点击事件进行响应
- 新增了 Visual Basic 及 F# 的调用示例

## v0.5.1 [2023-07-29]

### 新增

- 添加了 `SocketGuild.GetCategoryChannel` 和 `RestGuild.GetCategoryChannelAsync` 方法

### 修复

- 修复了 `GetJumpUrl` 方法返回结果不正确的问题
- 修复了 `KookConfig.MaxMessagesPerBatch` 设置不正确的问题
- 修复了已删除引用的反序列化不正确的问题
- 修复了 `MoveUsersAsync` 方法的 `RequestOptions` 参数不为可选参数的问题
- 修复了 `MoveUsersAsync` 方法的失败问题
- 修复了在 SocketGuildChannel 上创建频道权限复写后立即修改可能会导致失败且无错误信息的问题

### 优化

- 优化了 `GetDirectMessageAsync` 方法的内部实现
- 统一了注释中 `langword` 的字符串表示方式

## v0.5.0 [2023-05-21]

### 更新路线

出于便利性原因，此版本对 `BaseSocketClient` 中的部分事件所传递的参数类型进行了调整，事件参数所发生的具体变更可参阅文末的附录，
对涉及到的事件的应用都需要进行相应的更新。

`GuildFeature` 与 `GuildFeatures` 中表示重要客户的枚举值与属性已被重命名为更准确的名称；`GuildPermissions`
已被重构为结构体，`RoleProperties.Permissions` 属性的类型也已被相应变更；`RestGuild.Channels`
的类型被错误地声明为值的类型为 `RestChannel` 的字典，已被修正为值的类型为 `RestGuildChannel` 的字典；`KookSocketClient`
上的部分方法的返回类型为 `ValueTask`，现已统一为 `Task`。涉及到以上 `API` 的用法都需要进行相应的更新。

### 新增

- 新增 `GuildFeature.Partner` 枚举值及 `GuildFeatures.IsPartner` 属性
- 新增 `IGuild.Banner` 属性
- 新增 `RestGuild` 上的 `CurrentUserNickname`、`CurrentUserDisplayName` 与 `CurrentUserRoles` 属性
- 新增 `INestedChannel.SyncPermissionsAsync` 方法
- 新增 `BaseSocketClient` 上的 `DownloadVoiceStatesAsync` 与 `DownloadBoostSubscriptionsAsync` 抽象方法
- 新增 `RestGuild` 上的 `TextChannels`、`VoiceChannels` 与 `CategoryChannels` 属性

### 变更

- 重命名 `GuildFeature.Ka` 为 `GuildFeature.KeyAccount`，`GuildFeatures.IsKa` 为 `GuildFeatures.IsKeyAccount`
- 变更 `GuildPermissions` 为结构体，变更 `RoleProperties.Permissions` 为 `GuildPermissions?` 类型
- 出于便利性目的，为部分 `BaseSocketClient` 中的事件变更或新增了事件参数，变更详情参见文末的附录
- 变更 `BaseSocketClient` 上的 `DownloadBoostSubscriptionsAsync` 方法参数都为可选参数
- 变更 `RestGuild.Channels` 的类型为 `ImmutableDictionary<ulong, RestGuildChannel>`
- 变更 `KookSocketClient` 上的 `GetChannelAsync`、`GetDMChannelAsync`、`GetDMChannelsAsync` 与 `GetUserAsync`
  的方法返回类型为 `Task<*>`

### 修复

- 修复部分 API 对 bool 类型返回字符串 `1` 或 `0` 时未能正确解析的问题
- 修复 `IGuild.DefaultChannelId` 未能正确设置为服务器实际配置的默认文字频道的问题
- 修复为下载服务器用户列表时 `SocketGuild.CurrentUser` 为 `null` 的问题
- 修复 `SocketUser` 上的 `IsOnline` 与 `ActiveClient` 可能会抛出空引用异常的问题
- 修复 `MessageType.Poke` 类型的消息未被正确解析的问题
- 修复请求桶未能对 HTTP 429 Too Many Requests 错误进行正确处理的问题

### 优化

- 优化 `Cacheable` 的调试器显示文本
- `FileAttachment.Stream` 现已可以复用
- `SendFileAsync` 与 `ReplyFileAsync` 方法对通过文件或流创建的 `FileAttachment`
  附件进行多次发送前所创建的资产地址将会被缓存，以避免重复上传相同的文件

### 其它

- 修正了一些奇怪的代码缩进
- 变更文档主题
- 新增依赖于 doc 分支的文档更新独立工作流
- 新增 API 快速参考文档
- 补充权限相关单元测试中缺失的权限值

### 附录

**`BaseSocketClient` 中变更参数的事件列表：**

`ReactionAdded` 与 `ReactionRemoved`
- `ISocketMessageChannel` → `SocketTextChannel`
- 新增 `Cacheable<SocketGuildUser, ulong>` 参数表示添加或取消回应的服务器用户

`DirectReactionAdded` 与 `DirectReactionRemoved`
- 新增 `Cacheable<SocketUser, ulong>` 参数表示添加或取消回应的用户

`MessageReceived`
- 新增 `SocketGuildUser` 表示发送消息的服务器用户
- 新增 `SocketTextChannel` 表示消息所在的服务器文字频道

`MessageDeleted`、`MessageUpdated`、`MessagePinned` 与 `MessageUnpinned`
- `ISocketMessageChannel` → `SocketTextChannel`

`DirectMessageReceived`
- 新增 `SocketUser` 表示发送消息的用户
- 新增 `SocketDMChannel` 表示消息所在的私聊频道

`DirectMessageDeleted`
- `Cacheable<IDMChannel, Guid>` → `Cacheable<SocketDMChannel, ulong>`
- 新增 `Cacheable<SocketUser, ulong>` 表示消息的发送者

`DirectMessageUpdated`
- `IDMChannel` → `Cacheable<SocketDMChannel, ulong>`
- 新增 `Cacheable<SocketUser, ulong>` 表示消息的发送者

## v0.4.1 [2023-04-05]

### 修复

- 修复用户离开服务器或角色被删除时，`SocketGuildChannel.UserPermissionOverwrites` 与
  `SocketGuildChannel.RolePermissionOverwrites` 属性未移除对应的权限覆盖项的问题

## v0.4.0 [2023-04-03]

### 更新路线

出于以下几项原因，此版本对 `BaseSocketClient` 中的部分事件所传递的参数类型进行了调整：
- 部分参数必定可取，无需使用 `Cacheable` 进行封装，例如 `MessageDeleted` 与 `UserConnected`；
- 网关所下发的数据不完整，导致部分事件传递的参数存在缺失的情况，此类事件参数已调整为 `Cacheable` 封装的实体，修复实体
  ID 未知无法通过 Rest 客户端发起 API 请求获取完整数据的问题；
- 网关所下发的数据不完整，原处理逻辑为通过 Rest 客户端发起 API
  请求获取完整数据，在大型服务器内可能会导致性能不佳或超速等问题，例如 `GuildMemberOnline` 与
  `GuildMemberOffline`，此类事件参数已调整为 `Cacheable` 封装的实体，可通过 `GetOrDownloadAsync` 方法按需获取完整数据；
- 部分事件所传递的参数类型过于具体，导致类型不匹配而传递空值，例如 `ReactionAdded`；
- 部分事件所传递的参数类型过于宽泛，例如 `MessageDeleted` 与 `UserConnected`，可以避免不必要的模式匹配
- 部分事件传递参数缺失，例如 `UserBanned`
- 部分事件传递了过度冗余的参数，例如 `MessageButtonClicked`

事件参数所发生的具体变更可参阅文末的附录，对涉及到的事件的应用都需要进行相应的更新。

KMarkdown 格式化帮助类 `Format` 中的各格式化方法皆已变更为扩展方法，并新增可选参数
`sanitize`，以支持是否对文本内的特殊字符进行转义，默认为 `true`。扩展方法的调用方式仍然兼容原有的静态方法调用方式。
默认情况下，各格式化方法会对文本内与 KMarkdown 语法冲突的特殊字符进行转义，以避免 KMarkdown 语法解析错误。
此特性默认启用，可通过 `sanitize` 参数禁用。所有涉及到传入此方法的文本参数如已对特殊字符进行转义，
则应将 `sanitize` 参数设置为 `false`，或调整传入参数为未转义的原始文本。另外，`Format.Quote` 与
`Format.BlockQuote` 方法的逻辑已调整，现在会在文本内按需插入换行符与零宽连字符，以保持文本在 KOOK
客户端中的显示效果。其中，`Format.BlockQuote` 方法的格式化结果可以保证整段文本在 KOOK
客户端中的显示为一段引用块，而 `Format.Quote` 方法的格式化结果则将文本按空行分割为多个引用块，
空行不会包括在引用块内。

`CardJsonExtension` 类中的 `Parse` 与 `TryParse` 方法已重命名为 `ParseSingle` 与 `TryParseSingle`，
以避免与解析多个卡片时使用的 `ParseMany` 与 `TryParseMany` 方法产生冲突。所有涉及到此方法的调用都需要进行相应的更新。

`IGuild` 及 `IRecommendInfo` 的 `Features` 属性类型原为 `object[]`，现已实现为 `GuildFeatures`
类型，所有涉及到此属性的调用都需要进行相应的更新。

`RestPresence` 命名空间已修正为 `Kook.Rest`。所有涉及到 `RestPresence` 的调用都需要进行命名空间引用的更新。

### 新增

- 新增好友管理与用户屏蔽管理相关方法，变更详情参见文末的附录
- 为 `Cacheable` 与 `Quote` 类新增调试显示文本
- `KookSocketConfig` 新增 `MaxJoinedGuildDataFetchingRetryTimes` 与 `JoinedGuildDataFetchingRetryDelay`
  属性，控制加入服务器时的数据获取重试次数与重试间隔
- `CardJsonExtension` 新增 `ParseMany` 与 `TryParseMany` 方法
- （实验性功能）新增 `IVoiceRegion.MinimumBoostLevel` 属性
- （实验性功能）`KookRestClient` 新增 `ValidateCardAsync` 与 `ValidateCardsAsync` 方法

### 变更

- 变更 `BaseSocketClient` 中的部分事件所传递的参数类型，变更详情参见文末的附录
- `Format` 帮助类各格式化方法已变更为扩展方法，并新增可选参数 `sanitize`，以支持是否对文本内的特殊字符进行转义，默认为 `true`
- 重命名 `CardJsonExtension` 的 `Parse` 与 `TryParse` 为 `ParseSingle` 与 `TryParseSingle`
- 实现 `IGuild` 及 `IRecommendInfo` 的 `Features` 属性为 `GuildFeatures` 类型
- 修正 `RestPresence` 命名空间为 `Kook.Rest`
- （实验性功能）`KookRestClient.GetAdminGuildsAsync` 方法现已支持 Bot 类型认证

### 修复

- 修复 `Format.Quote` 与 `Format.BlockQuote` 方法结果在 KOOK 中显示异常的问题
- 修复 `CountdownModuleBuilder.Build` 抛出异常时的错误信息错误的问题
- 修复 `BaseSocketClient.DirectMessageUpdated` 可能会传递错误的用户实体的问题
- 修复 `BaseSocketClient` 与消息相关的事件在传递包含引用的消息实体时 `Author` 属性在用户未缓存时为空的问题
- 修复 `IGuild.OwnerId` 为 `0` 的问题
- 修复 `BaseSocketClient.Pinned` 与 `BaseSocketClient.Unpinned` 事件所传递的消息未正确设置 `IsPinned` 属性的问题
- 修复 `IPresence.ActiveClient` 属性可能会被意外清空的问题
- 修复 `IPresence` 的调试显示信息格式不正确的问题
- 修复 `IRestClient` 的默认实现在源代码启用 `DEBUG_REST` 预处理器指令调试高并发请求时可能会抛出异常的问题
- 修复 `Quote.Empty` 不为静态属性的问题

### 优化

- 修复 `KookRestApiClient` 缺失预处理器指令的问题
- 优化 `SocketUser.UpdateIntimacyAsync` 方法对 `IUser` 接口的实现
- 优化 `NumberBooleanConverter` 的使用
- `KookSocketClient` 接收乱序报文或对所接收的事件报文处理不正确时将报文内容到日志
- `KookSocketClient` 打印异常报文时将使用传入的 `serializerOptions` 序列化选项
- 完善 `BaseSocketClient` 事件的 XML 文档

### 其他

- 由于 KOOK 服务端已修复创建角色时部分字段值缺失的问题，相关提交已还原
- 修正部分文档内容错误
- 修正集成测试所创建的测试服务器未被正确删除的问题

### 附录

**新增接口列表：**

- 获取所有好友：`IKookClient.GetFriendsAsync`
- 请求添加好友：`IUser.RequestFriendAsync`
- 删除好友：`IUser.RemoveFriendAsync`
- 获取所有好友请求：`IKookClient.GetFriendRequestsAsync`
- 接受好友请求：`IFriendRequest.AcceptAsync`
- 拒绝好友请求：`IFriendRequest.DeclineAsync`
- 获取所有被屏蔽用户：`IKookClient.GetBlockedUsersAsync`
- 屏蔽用户：`IUser.BlockAsync`
- 取消屏蔽用户：`IUser.UnblockAsync`

**`BaseSocketClient` 中变更参数的事件列表：**

`ReactionAdded` 与 `ReactionRemoved`
- `Cacheable<IUserMessage, Guid>` → `Cacheable<IMessage, ulong>`
- `Cacheable<IMessageChannel, ulong>` → `ISocketMessageChannel`

`DirectReactionAdded` 与 `DirectReactionRemoved`
- `Cacheable<IUserMessage, Guid>` → `Cacheable<IMessage, ulong>`

`MessageDeleted`
- `Cacheable<IMessageChannel, ulong>` → `ISocketMessageChannel`

`MessageUpdated`
- `Cacheable<IMessage, Guid>` → `Cacheable<SocketMessage, Guid>`
- `SocketMessage` → `Cacheable<SocketMessage, Guid>`

`MessagePinned` 与 `MessageUnpinned`
- `Cacheable<IMessage, Guid>` → `Cacheable<SocketMessage, Guid>`
- `SocketMessage` → `Cacheable<SocketMessage, Guid>`
- `SocketGuildUser` → `Cacheable<SocketGuildUser, ulong>`

`DirectMessageUpdated`
- `SocketMessage` → `Cacheable<SocketMessage, Guid>`

`UserLeft`
- `SocketUser` → `Cacheable<SocketUser, ulong>`

`UserBanned`
- `IReadOnlyCollection<SocketUser>` → `IReadOnlyCollection<Cacheable<SocketUser, ulong>>`
- `SocketUser` → `Cacheable<SocketUser, ulong>`
- 新增 `string` 参数表示加入黑名单的原因

`UserUnbanned`
- `IReadOnlyCollection<SocketUser>` → `IReadOnlyCollection<Cacheable<SocketUser, ulong>>`
- `SocketUser` → `Cacheable<SocketUser, ulong>`

`UserUpdated`
- `SocketUser` → `Cacheable<SocketUser, ulong>`

`GuildMemberUpdated`
- `SocketGuildUser` → `Cacheable<SocketGuildUser, ulong>`

`GuildMemberOnline` 与 `GuildMemberOffline`
- `IReadOnlyCollection<SocketGuildUser>` → `IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>`

`UserConnected` 与 `UserDisconnected`
- `SocketUser` → `Cacheable<SocketGuildUser, ulong>`
- 移除 `SocketGuild` 参数，请从 `SocketVoiceChannel.Guild` 获取

`MessageButtonClicked`
- `SocketUser` → `Cacheable<SocketGuildUser, ulong>`
- `IMessage` → `Cacheable<IMessage, Guid>`
- 移除 `SocketGuild` 参数，请从 `SocketTextChannel.Guild` 获取

`DirectMessageButtonClicked`
- `SocketUser` → `Cacheable<SocketUser, ulong>`
- `IMessage` → `Cacheable<IMessage, Guid>`

## v0.3.1 [2023-03-19]

### 新增

- 新增 `ICard.ToJsonString`) 及 `ICardBuilder.ToJsonString`) 以支持卡片 JSON 序列化，新增 `CardJsonExtension.Parse`) 及
  `CardJsonExtension.TryParse`) 以支持卡片 JSON 反序列化

## v0.3.0 [2023-03-19]

### 更新路线

此版本将 `IUserMessage.Resolve` 中的默认参数 `everyoneHandling` 变更为 `TagHandling.Name`，所有使用此方法的用法都需要留意此变更。

### 新增

- `IKookClient` 接口新增方法重载 `GetUserAsync`，并为 `BaseSocketClient` 实现接口 `IKookClient`
- 新增支持通过 `KookConfig.FormatUsersInBidirectionalUnicode` 禁用针对双向 Unicode 格式化用户名字符串

### 修复

- 修复 `IUserMessage.Resolve` 对全体成员与在线成员标签文本化不正确的问题
- 修复 `IGuild.CreateRoleAsync` 反序列化结果失败的问题
- （实验性功能）修复 `BaseKookClient.CreateGuildAsync` 及 `KookSocketClient.CreateGuildAsync` 返回所创建的服务器信息不完整的问题

### 其它

- 为全部公共 API 添加 XML 文档
- 应用代码样式

## v0.2.5 [2023-03-16]

### 更新路线

此版本为 `KookRestClient.GetGamesAsync` 方法签名新增了可选可空参数 `GameCreationSource`，以支持获取指定创建来源的游戏信息。
所有在向此方法传递位置实参 `RequestOptions` 的用法都需要更新。

### 新增

- 为方法 `KookRestClient.GetGamesAsync` 新增可选可空参数 `GameCreationSource`

## v0.2.4 [2023-03-09]

### 新增

- 新增支持解析角色 `IRole` 的颜色类型 `ColorType` 及渐变色信息 `GradientColor`

## v0.2.3 [2023-01-19]

### 新增

- 为 Bearer 类型认证新增支持 `KookRestClient.GetAdminGuildsAsync`
- 新增 `Format.Colorize` 用于 KMarkdown 文本颜色格式化

### 修复

- 修复 `KookRestClient.GetGuildsAsync` 在服务器数量较大时下载数据过慢的问题
- （实验性功能）修复 `KookRestClient.GetGuildsAsync` 在 Bearer 类型认证下构造对象失败的问题
- 修复 `Color` 部分值不正确的问题

## v0.2.2 [2022-12-30]

### 修复

- 修复了 `SocketTextChannel.SendCardAsync` 与 `SocketTextChannel.SendCardsAsync` 设置 `ephemeralUser` 参数不生效的问题

## v0.2.1 [2022-12-25]

### 修复

- 修复了导致启动失败的 JSON 转换器错误

## v0.2.0 [2022-12-25]

### 更新路线

此版本将非官方列出的接口实现分离到单独的包中，即 `INestedChannel.SyncPermissionsAsync` 等，使用此接口的开发者现在应该安装
Kook.Net.Experimental 包。

### 移除

- 移除了接口定义上的方法 `INestedChannel.SyncPermissionsAsync`，接口的实现现已移动至 Kook.Net.Experimental 包中

### 新增

- 新增 Kook.Net.Experimental 包，用于实现非官方列出的接口

### 修复

- 修复了 `IGuild.OpenId` 为空时可能导致的空引用异常

### 其它

- 修正了不正确的代码缩进

## v0.1.2 [2022-12-18]

### 更新路线

此版本将 `SocketGuild.MemberCount` 的类型从 `int` 更改为 `int?`，其中 `null` 值表示未知的服务器成员数量。此外，类似的更改发生也在
`SocketGuild.HasAllMembers` 上。所有依赖这两个属性的用法都需要更新。

### 变更

- `KookSocketConfig.AlwaysDownloadUsers` 也将定义是否在启动时加载服务器成员数量

### 修复

- 修复了修改语音频道时应用的不正确的先决条件
- 修复了不正确的文档

## v0.1.1 [2022-11-20]

### 新增

- 新增 `IGuild.GetActiveBoostSubscriptionsAsync`
- JSON 反序列化失败时将输出报文本体至日志管理器

### 修复

- 修复 `BaseSocketClient.UserUnbanned` 未能正常触发的问题
- 修复 `BaseSocketClient.GuildAvailable` 事件触发时 `DownloadVoiceStatesAsync` 与 `DownloadBoostSubscriptionsAsync`
  被意外绕过的问题

## v0.1.0 [2022-11-15]

首次发布。

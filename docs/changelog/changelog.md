---
uid: Changelog
title: 变更日志
---

# 变更日志

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
- 为 `Cacheable` 与 `Qupte` 类新增调试显示文本
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

- 新增 [ICard.ToJsonString](xref:Kook.Rest.CardJsonExtension.ToJsonString(Kook.ICard,System.Boolean)) 及
  [ICardBuilder.ToJsonString](xref:Kook.Rest.CardJsonExtension.ToJsonString(Kook.ICardBuilder,System.Boolean)) 以支持卡片 JSON
  序列化，新增 [CardJsonExtension.Parse](xref:Kook.Rest.CardJsonExtension.ParseSingle(System.String)) 及
  [CardJsonExtension.TryParse](xref:Kook.Rest.CardJsonExtension.TryParseSingle(System.String,Kook.ICardBuilder@)) 以支持卡片 JSON 反序列化

## v0.3.0 [2023-03-19]

### 更新路线

此版本将 [IUserMessage.Resolve](xref:Kook.IUserMessage.Resolve*) 中的默认参数 `everyoneHandling`
变更为 [TagHandling.Name](xref:Kook.TagHandling.Name)，所有使用此方法的用法都需要留意此变更。

### 新增

- [IKookClient](xref:Kook.IKookClient) 接口新增方法重载
  [GetUserAsync](xref:Kook.IKookClient.GetUserAsync(System.String,System.String,Kook.RequestOptions))，并为
  [BaseSocketClient](xref:Kook.WebSocket.BaseSocketClient) 实现接口 [IKookClient](xref:Kook.IKookClient)
- 新增支持通过 [KookConfig.FormatUsersInBidirectionalUnicode](xref:Kook.KookConfig.FormatUsersInBidirectionalUnicode)
  禁用针对双向 Unicode 格式化用户名字符串

### 修复

- 修复 [IUserMessage.Resolve](xref:Kook.IUserMessage.Resolve*) 对全体成员与在线成员标签文本化不正确的问题
- 修复 [IGuild.CreateRoleAsync](xref:Kook.IGuild.CreateRoleAsync*) 反序列化结果失败的问题
- （实验性功能）修复 [BaseKookClient.CreateGuildAsync](xref:Kook.Rest.KookRestClientExperimentalExtensions.CreateGuildAsync*)
  及 [KookSocketClient.CreateGuildAsync](xref:Kook.WebSocket.BaseSocketClientExperimentalExtensions.CreateGuildAsync*)
  返回所创建的服务器信息不完整的问题

### 其它

- 为全部公共 API 添加 XML 文档
- 应用代码样式

## v0.2.5 [2023-03-16]

### 更新路线

此版本为 [KookRestClient.GetGamesAsync](xref:Kook.Rest.KookRestClient.GetGamesAsync*) 方法签名新增了可选可空参数
[GameCreationSource](xref:Kook.GameCreationSource)，以支持获取指定创建来源的游戏信息。所有在向此方法传递位置实参
[RequestOptions](xref:Kook.RequestOptions) 的用法都需要更新。

### 新增

- 为方法 [KookRestClient.GetGamesAsync](xref:Kook.Rest.KookRestClient.GetGamesAsync*)
  新增可选可空参数 [GameCreationSource](xref:Kook.GameCreationSource)

## v0.2.4 [2023-03-09]

### 新增

- 新增支持解析角色 [IRole](xref:Kook.IRole) 的颜色类型 [ColorType](xref:Kook.ColorType) 及渐变色信息 [GradientColor](xref:Kook.GradientColor)

## v0.2.3 [2023-01-19]

### 新增

- 为 Bearer 类型认证新增支持 [KookRestClient.GetAdminGuildsAsync](xref:Kook.Rest.KookRestClientExperimentalExtensions.GetAdminGuildsAsync*)
- 新增 [Format.Colorize](xref:Kook.Format.Colorize*) 用于 KMarkdown 文本颜色格式化

### 修复

- 修复 [KookRestClient.GetGuildsAsync](xref:Kook.Rest.KookRestClient.GetGuildsAsync*) 在服务器数量较大时下载数据过慢的问题
- （实验性功能）修复 [KookRestClient.GetGuildsAsync](xref:Kook.Rest.KookRestClient.GetGuildsAsync*) 在 Bearer 类型认证下构造对象失败的问题
- 修复 [Color](xref:Kook.Color) 部分值不正确的问题

## v0.2.2 [2022-12-30]

### 修复

- 修复了 [SocketTextChannel.SendCardAsync](xref:Kook.WebSocket.SocketTextChannel.SendCardAsync*)
  与 [SocketTextChannel.SendCardsAsync](xref:Kook.WebSocket.SocketTextChannel.SendCardsAsync*)
  设置 `ephemeralUser` 参数不生效的问题

## v0.2.1 [2022-12-25]

### 修复

- 修复了导致启动失败的 JSON 转换器错误

## v0.2.0 [2022-12-25]

### 更新路线

此版本将非官方列出的接口实现分离到单独的包中，即
[INestedChannel.SyncPermissionsAsync](xref:Kook.Rest.RestTextChannelExperimentalExtensions.SyncPermissionsAsync*)
等，使用此接口的开发者现在应该安装 Kook.Net.Experimental 包。

### 移除

- 移除了接口定义上的方法 `INestedChannel.SyncPermissionsAsync`，接口的实现现已移动至 Kook.Net.Experimental 包中

### 新增

- 新增 Kook.Net.Experimental 包，用于实现非官方列出的接口

### 修复

- 修复了 [IGuild.OpenId](xref:Kook.IGuild.OpenId) 为空时可能导致的空引用异常

### 其它

- 修正了不正确的代码缩进

## v0.1.2 [2022-12-18]

### 更新路线

此版本将 [SocketGuild.MemberCount](xref:Kook.WebSocket.SocketGuild.MemberCount) 的类型从 `int` 更改为 `int?`，其中 `null` 值表示未知的服务器成员数量。此外，类似的更改发生也在
[SocketGuild.HasAllMembers](xref:Kook.WebSocket.SocketGuild.HasAllMembers) 上。所有依赖这两个属性的用法都需要更新。

### 变更

- [KookSocketConfig.AlwaysDownloadUsers](xref:Kook.WebSocket.KookSocketConfig.AlwaysDownloadUsers) 也将定义是否在启动时加载服务器成员数量

### 修复

- 修复了修改语音频道时应用的不正确的先决条件
- 修复了不正确的文档

## v0.1.1 [2022-11-20]

### 新增

- 新增 [IGuild.GetActiveBoostSubscriptionsAsync](xref:Kook.IGuild.GetActiveBoostSubscriptionsAsync*)
- JSON 反序列化失败时将输出报文本体至日志管理器

### 修复

- 修复 [BaseSocketClient.UserUnbanned](xref:Kook.WebSocket.BaseSocketClient.UserUnbanned) 未能正常触发的问题
- 修复 [BaseSocketClient.GuildAvailable](xref:Kook.WebSocket.BaseSocketClient.GuildAvailable) 事件触发时
  [DownloadVoiceStatesAsync](xref:Kook.WebSocket.KookSocketClient.DownloadVoiceStatesAsync*)
  与 [DownloadBoostSubscriptionsAsync](xref:Kook.WebSocket.KookSocketClient.DownloadBoostSubscriptionsAsync*)
  被意外绕过的问题

## v0.1.0 [2022-11-15]

首次发布。

---
uid: Changelog
title: 变更日志
---

# 变更日志

## v0.3.1 [2023-03-19]

### 新增

- 新增 [ICard.ToJsonString](xref:Kook.Rest.CardJsonExtension.ToJsonString(Kook.ICard,System.Boolean)) 及
  [ICardBuilder.ToJsonString](xref:Kook.Rest.CardJsonExtension.ToJsonString(Kook.ICardBuilder,System.Boolean)) 以支持卡片 JSON
  序列化，新增 [CardJsonExtension.Parse](xref:Kook.Rest.CardJsonExtension.Parse(System.String)) 及
  [CardJsonExtension.TryParse](xref:Kook.Rest.CardJsonExtension.TryParse(System.String,Kook.ICardBuilder@)) 以支持卡片 JSON 反序列化

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

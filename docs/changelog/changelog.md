---
uid: Changelog
title: 变更日志
---

# 变更日志

## v0.2.5 [2023-03-16]

### 新增

- 为方法 `Kook.GetGamesAsync` 新增可选参数 `Nullable<GameCreationSource>`

## v0.2.4 [2023-03-09]

### 新增

- 新增支持解析角色 `IRole` 的颜色类型 `ColorType` 及渐变色信息 `GradientColor`

## v0.2.3 [2023-01-19]

### 新增

- 为 Bearer 类型认证新增支持 `KookRestClient.GetAdminGuildsAsync`
- 新增 `Format.Colorize` 用于 KMarkdown 文本颜色格式化

### 修复

- 修复 `GetGuildsAsync` 在服务器数量较大时下载数据过慢的问题
- 修复 `GetGuildsAsync` 在 Bearer 类型认证下构造对象失败的问题
- 修复 `Color` 部分值不正确的问题

## v0.2.2 [2022-12-30]

### 修复

- 修复了 `SocketTextChannel.SendCard(s)Async` 设置 `ephemeralUser` 参数不生效的问题

## v0.2.1 [2022-12-25]

### 修复

- 修复了导致启动失败的 JSON 转换器错误

## v0.2.0 [2022-12-25]

### 更新路线

此版本将非官方列出的接口实现分离到单独的包中，即 `INestedChannel.SyncPermissionsAsync`，使用此接口的开发者现在应该安装
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

- `AlwaysDownloadUsers` 也将定义是否在启动时加载服务器成员数量

### 修复

- 修复了修改语音频道时应用的不正确的先决条件
- 修复了不正确的文档

## v0.1.1 [2022-11-20]

### 新增

- 新增 @Kook.IGuild.GetActiveBoostSubscriptionsAsync
- JSON 反序列化失败时将输出报文本体至日志管理器

### 修复

- 修复 @Kook.WebSocket.BaseSocketClient.UserUnbanned 未能正常触发的问题
- 修复 @Kook.WebSocket.BaseSocketClient.GuildAvailable 事件触发时 @Kook.WebSocket.KookSocketClient.DownloadVoiceStatesAsync*
  与 @Kook.WebSocket.KookSocketClient.DownloadBoostSubscriptionsAsync* 被意外绕过的问题

## v0.1.0 [2022-11-15]

首次发布。

---
uid: Changelog
title: 变更日志
---

# 变更日志

## v0.2.0 [2022-12-25]

## 更新路线

此版本将非官方列出的接口实现分离到单独的包中，即 `INestedChannel.SyncPermissionsAsync`，使用此接口的开发者现在应该安装
Kook.Net.Experimental 包。

## 移除

- 移除了接口定义上的方法 `INestedChannel.SyncPermissionsAsync`，接口的实现现已移动至 Kook.Net.Experimental 包中

## 新增

- 新增 Kook.Net.Experimental 包，用于实现非官方列出的接口

## 修复

- 修复了 `IGuild.OpenId` 为空时可能导致的空引用异常

## 其它

- 修复了不正确的代码缩进


## v0.1.2 [2022-12-18]

## 更新路线

此版本将 `SocketGuild.MemberCount` 的类型从 `int` 更改为 `int?`，其中 `null` 值表示未知的服务器成员数量。此外，类似的更改发生也在
`SocketGuild.HasAllMembers` 上。所有依赖这两个属性的用法都需要更新。

## 变更

- `AlwaysDownloadUsers` 也将定义是否在启动时加载服务器成员数量

## 修复

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

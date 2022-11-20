---
uid: Changelog
title: 变更日志
---

# 变更日志

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

---
uid: Guides.QuickReference.Event.Guild
title: 服务器相关事件
---

# 服务器角色相关事件

预声明变量

```csharp
readonly KookSocketClient _client = null;
```

### [服务器角色增加]

```csharp
_client.RoleCreated += role => Task.CompletedTask;
```

### [服务器角色删除]

```csharp
_client.RoleDeleted += role => Task.CompletedTask;
```

### [服务器角色更新]

```csharp
_client.RoleUpdated += (before, after) => Task.CompletedTask;
```

[服务器角色增加]: https://developer.kookapp.cn/doc/event/guild-role#服务器角色增加
[服务器角色删除]: https://developer.kookapp.cn/doc/event/guild-role#服务器角色删除
[服务器角色更新]: https://developer.kookapp.cn/doc/event/guild-role#服务器角色更新

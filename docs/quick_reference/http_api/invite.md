---
uid: Guides.QuickReference.Http.Invite
title: 邀请相关接口
---

# 邀请相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

IGuild guild = null;
```

### [获取邀请列表]

GET `/api/v3/invite/list`

```csharp
// API 请求
IReadOnlyCollection<IInvite> invites = await guild.GetInvitesAsync();
```

### [创建邀请链接]

POST `/api/v3/invite/create`

```csharp
InviteMaxAge maxAge = default; // 有效期
InviteMaxUses maxUses = default; // 最大使用次数

// API 请求
IInvite invite = await guild.CreateInviteAsync(maxAge, maxUses);
```

### [删除邀请链接]

POST `/api/v3/invite/delete`

```csharp
IInvite invite = null; // 邀请

// API 请求
await invite.DeleteAsync();
```

[获取邀请列表]: https://developer.kookapp.cn/doc/http/invite#获取邀请列表
[创建邀请链接]: https://developer.kookapp.cn/doc/http/invite#创建邀请链接
[删除邀请链接]: https://developer.kookapp.cn/doc/http/invite#删除邀请链接

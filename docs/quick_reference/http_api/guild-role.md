---
uid: Guides.QuickReference.Http.GuildRole
title: 服务器角色权限相关接口
---

# 服务器角色权限相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketGuild socketGuild = null;

RestGuild restGuild = null;

IGuild guild = null;
IRole role = null;
```

### [获取服务器角色列表]

GET `/api/v3/guild-role/list`

```csharp
// 获取服务器的角色列表
IReadOnlyCollection<SocketRole> socketRoles = socketGuild.Roles;
IReadOnlyCollection<RestRole> restRoles = restGuild.Roles;
IReadOnlyCollection<IRole> roles = guild.Roles;

// 获取服务器的指定角色 ID 的信息
SocketRole socketRole = socketGuild.GetRole(roleId);
RestRole restRole = restGuild.GetRole(roleId);
IRole role = guild.GetRole(roleId);
```

### [创建服务器角色]

POST `/api/v3/guild-role/create`

```csharp
string roleName = null; // 角色名称

// API 请求
RestRole restRoleFromSocket = await socketGuild.CreateRoleAsync(roleName);
RestRole restRoleFromRest = await restGuild.CreateRoleAsync(roleName);
IRole role = await guild.CreateRoleAsync(roleName);
```

### [更新服务器角色]

POST `/api/v3/guild-role/update`

```csharp
string roleName = null; // 角色名称
Color color = default; // 角色颜色
bool hoist = default; // 是否分离显示
bool mentionable = default; // 是否可被提及
GuildPermissions permissions = default; // 角色权限

// API 请求
await role.ModifyAsync(x =>
{
    x.Name = roleName;
    x.Color = color;
    x.Hoist = hoist;
    x.Mentionable = mentionable;
    x.Permissions = permissions;
});
```

### [删除服务器角色]

POST `/api/v3/guild-role/delete`

```csharp
// API 请求
await role.DeleteAsync();
```

### [赋予用户角色]

POST `/api/v3/guild-role/grant`

```csharp
ulong guildId = default; // 服务器 ID
IRole role = null; // 角色
uint roleId = default; // 角色 ID
IEnumerable<IRole> roles = null; // 多个角色
IEnumerable<uint> roleIds = null; // 多个角色 ID
ulong userId = default; // 用户 ID

// API 请求赋予角色
await guildUser.AddRoleAsync(role);
await guildUser.AddRoleAsync(roleId);

// API 请求批量赋予角色
await guildUser.AddRolesAsync(roles);
await guildUser.AddRolesAsync(roleIds);

// API 请求，无实体操作
await _socketClient.Rest.AddRoleAsync(guildId, userId, roleId);
```

### [删除用户角色]

POST `/api/v3/guild-role/revoke`

```csharp
ulong guildId = default; // 服务器 ID
IRole role = null; // 角色
uint roleId = default; // 角色 ID
IEnumerable<IRole> roles = null; // 多个角色
IEnumerable<uint> roleIds = null; // 多个角色 ID
ulong userId = default; // 用户 ID

// API 请求赋予角色
await guildUser.RemoveRoleAsync(role);
await guildUser.RemoveRoleAsync(roleId);

// API 请求批量赋予角色
await guildUser.RemoveRolesAsync(roles);
await guildUser.RemoveRolesAsync(roleIds);

// API 请求，无实体操作
await _socketClient.Rest.RemoveRoleAsync(guildId, userId, roleId);
```

[获取服务器角色列表]: https://developer.kookapp.cn/doc/http/guild-role#获取服务器角色列表
[创建服务器角色]: https://developer.kookapp.cn/doc/http/guild-role#创建服务器角色
[更新服务器角色]: https://developer.kookapp.cn/doc/http/guild-role#更新服务器角色
[删除服务器角色]: https://developer.kookapp.cn/doc/http/guild-role#删除服务器角色
[赋予用户角色]: https://developer.kookapp.cn/doc/http/guild-role#赋予用户角色
[删除用户角色]: https://developer.kookapp.cn/doc/http/guild-role#删除用户角色

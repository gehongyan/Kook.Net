---
uid: Guides.QuickReference.Http.Badge
title: Badge 相关接口
---

# Badge 相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

IGuild guild = null;
```

### [获取服务器 Badge]

GET `/api/v3/badge/guild`

```csharp
BadgeStyle style = default; // 样式

// API 请求
Stream badge = await guild.GetBadgeAsync(style);
```

[获取服务器 Badge]: https://developer.kookapp.cn/doc/http/badge#获取服务器%20Badge

---
uid: Guides.QuickReference.Http.OAuth
title: OAuth 2.0 相关接口
---

# 频道用户相关

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
```

### [获取 AccessToken]

POST `/api/oauth2/token`

无公开 API，请使用 `AspNet.Security.OAuth.Kook`，请参考：

*  NuGet: <https://www.nuget.org/packages/AspNet.Security.OAuth.Kook/>
*  GitHub: <https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers>
*  文档：<https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/blob/dev/docs/kook.md>

[获取 AccessToken]: https://developer.kookapp.cn/doc/http/oauth#获取AccessToken

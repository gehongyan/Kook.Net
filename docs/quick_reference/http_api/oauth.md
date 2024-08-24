---
uid: Guides.QuickReference.Http.OAuth
title: OAuth 2.0 相关接口
---

# OAuth 2.0 相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
```

### [获取 AccessToken]

POST `/api/oauth2/token`

无公开 API。可参考以下类库：

- **AspNet.Security.OAuth.Kook**：[NuGet](https://www.nuget.org/packages/AspNet.Security.OAuth.Kook/)，[GitHub](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers)，[文档](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers/blob/dev/docs/kook.md)
- **OpenIddict**：[NuGet](https://www.nuget.org/packages/OpenIddict/)，[GitHub](https://github.com/openiddict/openiddict-core)，[文档](https://documentation.openiddict.com/)

[获取 AccessToken]: https://developer.kookapp.cn/doc/http/oauth#获取AccessToken

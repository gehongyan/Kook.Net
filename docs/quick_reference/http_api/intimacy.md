---
uid: Guides.QuickReference.Http.Intimacy
title: 亲密度相关接口
---

# 亲密度相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketUser socketUser = null;

RestUser restUser = null;

IUser user = null;
```

### [获取用户的亲密度]

GET `/api/v3/intimacy/index`

```csharp
// API 请求
RestIntimacy intimacyFromSocket = await socketUser.GetIntimacyAsync();
RestIntimacy intimacyFromRest = await restUser.GetIntimacyAsync();
IIntimacy intimacy = await user.GetIntimacyAsync();
```

### [更新用户的亲密度]

POST `/api/v3/intimacy/update`

```csharp
string socialInfo = null; // 社交信息
uint imageId = default; // 形象图片 ID
int score = default; // 亲密度

// API 请求
await user.UpdateIntimacyAsync(x =>
{
    x.SocialInfo = socialInfo;
    x.ImageId = imageId;
    x.Score = score;
});
```

[获取用户的亲密度]: https://developer.kookapp.cn/doc/http/intimacy#获取用户的亲密度
[更新用户的亲密度]: https://developer.kookapp.cn/doc/http/intimacy#更新用户的亲密度

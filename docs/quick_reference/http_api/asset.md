---
uid: Guides.QuickReference.Http.Asset
title: 媒体接口
---

# 媒体接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
```

### [上传文件/图片]

POST `/api/v3/asset/create`

```csharp
Stream stream = null; // 文件流
string path = null; // 文件路径
string filename = null; // 文件名

// API 请求
string assertUri = await _socketClient.Rest.CreateAssetAsync(stream, filename);
string assertUri = await _socketClient.Rest.CreateAssetAsync(path, filename);
string assertUri = await _restClient.CreateAssetAsync(stream, filename);
string assertUri = await _restClient.CreateAssetAsync(path, filename);
```


[上传文件/图片]: https://developer.kookapp.cn/doc/http/asset#%上传媒体文件

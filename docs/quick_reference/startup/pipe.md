---
uid: Guides.QuickReference.Startup.Pipe
title: Pipe 客户端
---

# Pipe 客户端

预声明变量

```csharp
readonly KookPipeClient _pipeClient;
```

```csharp
string accessToken; // 管道访问令牌
Uri pipeUrl; // 管道回调地址

// 使用访问令牌及默认配置创建 Pipe 客户端
_pipeClient = new KookPipeClient(accessToken);
// 使用回调地址及默认配置创建 Pipe 客户端
_pipeClient = new KookPipeClient(pipeUrl);
// 使用访问令牌及自定义配置创建 Pipe 客户端
_pipeClient = new KookPipeClient(accessToken, new KookRestConfig
{
    // KookRestConfig 的全部配置项参见 Rest 客户端页面
});
// 使用回调地址及自定义配置创建 Pipe 客户端
_pipeClient = new KookPipeClient(pipeUrl, new KookRestConfig
{
    // KookRestConfig 的全部配置项参见 Rest 客户端页面
});

// Pipe 客户端不实现 IKookClient 接口，创建即登录
```

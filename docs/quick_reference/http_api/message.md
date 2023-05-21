---
uid: Guides.QuickReference.Http.Message
title: 消息相关接口
---

# 消息相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;
```

### [获取频道聊天消息列表]

GET `/api/v3/message/list`

```csharp

```

### [获取频道聊天消息详情]

GET `/api/v3/message/view`

```csharp

```

### [发送频道聊天消息]

POST `/api/v3/message/create`

```csharp

```

### [更新频道聊天消息]

POST `/api/v3/message/update`

```csharp

```

### [删除频道聊天消息]

POST `/api/v3/message/delete`

```csharp

```

### [获取频道消息某个回应的用户列表]

GET `/api/v3/message/reaction-list`

```csharp

```

### [给某个消息添加回应]

POST `/api/v3/message/add-reaction`

```csharp

```

### [删除消息的某个回应]

POST `/api/v3/message/delete-reaction`

```csharp

```

[获取频道聊天消息列表]: https://developer.kookapp.cn/doc/http/message#获取频道聊天消息列表
[获取频道聊天消息详情]: https://developer.kookapp.cn/doc/http/message#获取频道聊天消息详情
[发送频道聊天消息]: https://developer.kookapp.cn/doc/http/message#发送频道聊天消息
[更新频道聊天消息]: https://developer.kookapp.cn/doc/http/message#更新频道聊天消息
[删除频道聊天消息]: https://developer.kookapp.cn/doc/http/message#删除频道聊天消息
[获取频道消息某个回应的用户列表]: https://developer.kookapp.cn/doc/http/message#获取频道消息某个回应的用户列表
[给某个消息添加回应]: https://developer.kookapp.cn/doc/http/message#给某个消息添加回应
[删除消息的某个回应]: https://developer.kookapp.cn/doc/http/message#删除消息的某个回应

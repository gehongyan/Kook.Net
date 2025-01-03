---
uid: Guides.QuickReference.Http.Friend
title: 好友相关接口
---

# 好友相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

IUser user = null;
```

### [好友列表]

GET `/api/v3/friend`

```csharp
// API 请求，获取好友列表
IReadOnlyCollection<RestUser> friends = await _restClient.GetFriendsAsync();

// API 请求，获取好友请求列表
IReadOnlyCollection<RestFriendRequest> friendRequests = await _restClient.GetFriendRequestsAsync();

// API 请求，获取屏蔽用户列表
IReadOnlyCollection<RestUser> blockedUsers = await _restClient.GetBlockedUsersAsync();

// API 请求，获取亲密关系列表
IReadOnlyCollection<RestIntimacyRelation> intimacyRequests = await _restClient.GetIntimacyRelationsAsync();

// API 请求，获取亲密关系请求列表
IReadOnlyCollection<RestIntimacyRelationRequest> intimacyRequests = await _restClient.GetIntimacyRelationRequestsAsync();
```

### [好友申请]

POST `/api/v3/friend/request`

```csharp
// API 请求
await user.RequestFriendAsync();
```

### [处理好友申请]

POST `/api/v3/friend/handle-request`

```csharp
RestFriendRequest friendRequest = null; // 待处理的好友申请

// API 请求，接受好友申请
await friendRequest.AcceptAsync();

// API 请求，拒绝好友申请
await friendRequest.DeclineAsync();
```

### [删除好友]

POST `/api/v3/friend/delete`

```csharp
// API 请求
await user.RemoveFriendAsync();
```

### 亲密关系申请

POST `/api/v3/friend/create-relation`

```csharp
IntimacyRelationType relationType = default; // 亲密关系类型

// API 请求
await user.RequestIntimacyRelationAsync(relationType);
```

### 处理亲密关系申请

POST `/api/v3/friend/handle-relation`

```csharp
IIntimacyRelationRequest intimacyRequest = null; // 待处理的亲密关系申请

// API 请求，接受亲密关系申请
await intimacyRequest.AcceptAsync();

// API 请求，拒绝亲密关系申请
await intimacyRequest.DeclineAsync();
```

### 解除亲密关系

POST `/api/v3/friend/unravel-relation`

```csharp
IIntimacyRelation intimacyRelation = null; // 待解除的亲密关系
bool removeFriend = false; // 是否同时删除好友

// API 请求，在亲密关系实体上调用
await intimacyRelation.UnravelAsync(removeFriend);
// API 请求，在用户实体上调用
await user.UnravelIntimacyRelationAsync(removeFriend);
```

### [屏蔽用户]

POST `/api/v3/friend/block`

```csharp
// API 请求
await user.BlockAsync();
```

### [取消屏蔽用户]

POST `/api/v3/friend/unblock`

```csharp
// API 请求
await user.UnblockAsync();
```

###

[好友列表]: https://developer.kookapp.cn/doc/http/friend#好友列表
[好友申请]: https://developer.kookapp.cn/doc/http/friend#好友申请
[处理好友申请]: https://developer.kookapp.cn/doc/http/friend#处理好友申请
[删除好友]: https://developer.kookapp.cn/doc/http/friend#删除好友
[屏蔽用户]: https://developer.kookapp.cn/doc/http/friend#屏蔽用户
[取消屏蔽用户]: https://developer.kookapp.cn/doc/http/friend#取消屏蔽用户

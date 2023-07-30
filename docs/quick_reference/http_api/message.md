---
uid: Guides.QuickReference.Http.Message
title: 消息相关接口
---

# 消息相关接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketTextChannel socketTextChannel = null;

RestTextChannel restTextChannel = null;

ITextChannel textChannel = null;
IUserMessage userMessage = null;
```

### [获取频道聊天消息列表]

GET `/api/v3/message/list`

```csharp
IMessage referenceMessage = null; // 获取消息列表所根据的参考消息位置的
Guid referenceMessageId = default; // 获取消息列表所根据的参考消息位置的 ID
Direction direction = Direction.Before; // 获取消息列表的方向
int limit = 25; // 获取消息列表的数量

// 要缓存文字频道聊天消息，请设置 MessageCacheSize 的值
// 缓存获取文字频道的聊天消息列表
IReadOnlyCollection<SocketMessage> cachedMessages = socketTextChannel.CachedMessages;
IReadOnlyCollection<SocketMessage> conditionalCachedMessages = socketTextChannel.GetCachedMessages(referenceMessage, direction, limit);
IReadOnlyCollection<SocketMessage> conditionalCachedMessagesById = socketTextChannel.GetCachedMessages(referenceMessageId, direction, limit);

// API 请求
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessages = socketTextChannel.GetMessagesAsync(referenceMessage, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessagesById = socketTextChannel.GetMessagesAsync(referenceMessageId, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<RestMessage>> pagedRestMessages = restTextChannel.GetMessagesAsync(referenceMessage, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<RestMessage>> pagedRestMessagesById = restTextChannel.GetMessagesAsync(referenceMessageId, direction, limit);

// 在 ITextChannel 上进行调用
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessages = textChannel.GetMessagesAsync(referenceMessage, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessagesById = textChannel.GetMessagesAsync(referenceMessageId, direction, limit);
```

### [获取频道聊天消息详情]

GET `/api/v3/message/view`

```csharp
Guid messageId = default; // 要获取详情的消息 ID

// 要缓存文字频道聊天消息，请设置 MessageCacheSize 的值
// 缓存获取文字频道的聊天消息详情
SocketMessage cachedMessage = socketTextChannel.GetCachedMessage(messageId);

// 调用异步方法获取消息详情，在缓存中包含指定的消息对象的情况下，直接返回缓存的消息对象，否则会发起 API 请求获取消息对象
IMessage messageAsync = await socketTextChannel.GetMessageAsync(messageId);

// API 请求
IMessage message = await restTextChannel.GetMessageAsync(messageId);

// 在 ITextChannel 上进行调用
IMessage message = await textChannel.GetMessageAsync(messageId);
```

### [发送频道聊天消息]

POST `/api/v3/message/create`

```csharp
string text = null; // 要发送的文字消息
IQuote quote = null; // 被引用的消息
IUser ephemeralUser = null; // 瞬态消息的可见用户
string filePath = null; // 要发送的文件的路径
string fileName = null; // 要发送的文件的名称
AttachmentType fileType = default; // 要发送的文件的类型
Stream stream = null; // 要发送的文件的流
FileAttachment attachment = default; // 要发送的文件的附件
ICard card = null; // 要发送的卡片
IEnumerable<ICard> cards = null; // 要发送的卡片列表

// API 请求，发送文本消息
Cacheable<IUserMessage,Guid> textMessage = await textChannel.SendTextAsync(text, quote, ephemeralUser);
// API 请求，发送文件消息
Cacheable<IUserMessage,Guid> fileMessageFromPath = await textChannel.SendFileAsync(filePath, fileName, fileType, quote, ephemeralUser);
Cacheable<IUserMessage,Guid> fileMessageFromStream = await textChannel.SendFileAsync(stream, fileName, fileType, quote, ephemeralUser);
Cacheable<IUserMessage,Guid> fileMessageFromAttachment = await textChannel.SendFileAsync(attachment, fileName, fileType, quote, ephemeralUser);
// API 请求，发送单卡片消息
Cacheable<IUserMessage,Guid> cardMessage = await textChannel.SendCardAsync(card, quote, ephemeralUser);
// API 请求，发送多卡片消息
Cacheable<IUserMessage,Guid> cardsMessage = await textChannel.SendCardsAsync(cards, quote, ephemeralUser);
```

### [更新频道聊天消息]

POST `/api/v3/message/update`

```csharp
string content = null; // 要更新的消息的文本
IEnumerable<ICard> cards = null; // 要更新的消息的卡片
IQuote quote = null; // 要更新的消息的引用
IUser ephemeralUser = null; // 要更新的瞬态消息的可见用户

// API 请求
await userMessage.ModifyAsync(x =>
{
    x.Content = content;
    x.Cards = cards;
    x.Quote = quote; // 要清除引用，请设置为 Quote.Empty
    x.EphemeralUser = ephemeralUser;
});
```

### [删除频道聊天消息]

POST `/api/v3/message/delete`

```csharp
// API 请求
await userMessage.DeleteAsync();
```

### [获取频道消息某个回应的用户列表]

GET `/api/v3/message/reaction-list`

```csharp
IEmote emoji = null; // 要获取用户列表的回应的表情

// 缓存获取消息的回应概要，但不包含用户列表
IReadOnlyDictionary<IEmote,ReactionMetadata> cachedReactions = socketUserMessage.Reactions;

// API 请求
IReadOnlyCollection<IUser> reactionUsers = await userMessage.GetReactionUsersAsync(emoji);
```

### [给某个消息添加回应]

POST `/api/v3/message/add-reaction`

有关 Emoji 的详细信息，请参考 [表情符号](xref:Guides.Emoji)。

```
IEmote emoji = null; // 要添加的回应的表情
IEnumerable<IEmote> emojis = null; // 要添加的回应的表情列表

// API 请求
await userMessage.AddReactionAsync(emoji);
await userMessage.AddReactionsAsync(emojis);
```

### [删除消息的某个回应]

POST `/api/v3/message/delete-reaction`

```csharp
IEmote emoji = null;
IEnumerable<IEmote> emojis = null;
IUser user = null;

// API 请求
await userMessage.RemoveReactionAsync(emoji, user);
await userMessage.RemoveReactionsAsync(user, emojis);
```

[获取频道聊天消息列表]: https://developer.kookapp.cn/doc/http/message#获取频道聊天消息列表
[获取频道聊天消息详情]: https://developer.kookapp.cn/doc/http/message#获取频道聊天消息详情
[发送频道聊天消息]: https://developer.kookapp.cn/doc/http/message#发送频道聊天消息
[更新频道聊天消息]: https://developer.kookapp.cn/doc/http/message#更新频道聊天消息
[删除频道聊天消息]: https://developer.kookapp.cn/doc/http/message#删除频道聊天消息
[获取频道消息某个回应的用户列表]: https://developer.kookapp.cn/doc/http/message#获取频道消息某个回应的用户列表
[给某个消息添加回应]: https://developer.kookapp.cn/doc/http/message#给某个消息添加回应
[删除消息的某个回应]: https://developer.kookapp.cn/doc/http/message#删除消息的某个回应

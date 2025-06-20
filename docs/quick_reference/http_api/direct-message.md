---
uid: Guides.QuickReference.Http.DirectMessage
title: 用户私聊消息接口
---

# 用户私聊消息接口

预声明变量

```csharp
readonly KookSocketClient _socketClient = null;
readonly KookRestClient _restClient = null;

SocketDMChannel socketDmChannel = null;

RestDMChannel restDmChannel = null;

IDMChannel dmChannel = null;
IUserMessage userMessage = null;
```

### [获取私信聊天消息列表]

GET `/api/v3/direct-message/list`

```csharp
IMessage referenceMessage = null; // 获取消息列表所根据的参考消息位置的
Guid referenceMessageId = default; // 获取消息列表所根据的参考消息位置的 ID
Direction direction = default; // 获取消息列表的方向
int limit = default; // 获取消息列表的数量

// 要缓存文字频道聊天消息，请设置 MessageCacheSize 的值
// 缓存获取文字频道的聊天消息列表
IReadOnlyCollection<SocketMessage> cachedMessages = socketDmChannel.CachedMessages;
IReadOnlyCollection<SocketMessage> conditionalCachedMessages = socketDmChannel.GetCachedMessages(referenceMessage, direction, limit);
IReadOnlyCollection<SocketMessage> conditionalCachedMessagesById = socketDmChannel.GetCachedMessages(referenceMessageId, direction, limit);

// API 请求
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessages = socketDmChannel.GetMessagesAsync(referenceMessage, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessagesById = socketDmChannel.GetMessagesAsync(referenceMessageId, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<RestMessage>> pagedRestMessages = restDmChannel.GetMessagesAsync(referenceMessage, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<RestMessage>> pagedRestMessagesById = restDmChannel.GetMessagesAsync(referenceMessageId, direction, limit);

// 在 ITextChannel 上进行调用
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessages = dmChannel.GetMessagesAsync(referenceMessage, direction, limit);
IAsyncEnumerable<IReadOnlyCollection<IMessage>> pagedMessagesById = dmChannel.GetMessagesAsync(referenceMessageId, direction, limit);
```

### [获取私信聊天会话详情]

POST `/api/v3/user-chat/view`

```csharp
// 要缓存文字频道聊天消息，请设置 MessageCacheSize 的值
// 缓存获取文字频道的聊天消息详情
SocketMessage cachedMessage = socketDmChannel.GetCachedMessage(messageId);

// 调用异步方法获取消息详情，在缓存中包含指定的消息对象的情况下，直接返回缓存的消息对象，否则会发起 API 请求获取消息对象
IMessage messageAsync = await socketDmChannel.GetMessageAsync(messageId);

// API 请求
IMessage message = await restDmChannel.GetMessageAsync(messageId);

// 在 IDMChannel 上进行调用
IMessage message = await dmChannel.GetMessageAsync(messageId);
```

### [发送私信聊天消息]

POST `/api/v3/direct-message/create`

```csharp
string text = null; // 要发送的文字消息
IQuote quote = null; // 被引用的消息
string filePath = null; // 要发送的文件的路径
string fileName = null; // 要发送的文件的名称
AttachmentType fileType = default; // 要发送的文件的类型
Stream stream = null; // 要发送的文件的流
FileAttachment attachment = default; // 要发送的文件的附件
ICard card = null; // 要发送的卡片
IEnumerable<ICard> cards = null; // 要发送的卡片列表
ulong templateId = default; // 要发送的模板消息的模板 ID
T parameters = default; // 要发送的模板消息的参数
JsonSerializerOptions jsonSerializerOptions = null; // 要发送的模板消息的参数的序列化选项

// API 请求，发送文本消息
Cacheable<IUserMessage, Guid> textMessage = await dmChannel.SendTextAsync(text, quote);
Cacheable<IUserMessage, Guid> textMessageFromTemplate = await dmChannel.SendTextAsync(templateId, parameters, quote, jsonSerializerOptions);
// API 请求，发送文件消息
Cacheable<IUserMessage, Guid> fileMessageFromPath = await dmChannel.SendFileAsync(filePath, fileName, fileType, quote);
Cacheable<IUserMessage, Guid> fileMessageFromStream = await dmChannel.SendFileAsync(stream, fileName, fileType, quote);
Cacheable<IUserMessage, Guid> fileMessageFromAttachment = await dmChannel.SendFileAsync(attachment, quote);
// API 请求，发送单卡片消息
Cacheable<IUserMessage, Guid> cardMessage = await dmChannel.SendCardAsync(card, quote);
// API 请求，发送多卡片消息
Cacheable<IUserMessage, Guid> cardsMessage = await dmChannel.SendCardsAsync(cards, quote);
Cacheable<IUserMessage, Guid> cardsMessageFromTemplate = await dmChannel.SendCardsAsync(templateId, parameters, quote, jsonSerializerOptions);
```

### [更新私信聊天消息]

POST `/api/v3/direct-message/update`

```csharp
string content = null; // 要更新的消息的文本
IEnumerable<ICard> cards = null; // 要更新的消息的卡片
IQuote quote = null; // 要更新的消息的引用
ulong templateId = default; // 要发送的模板消息的模板 ID
T parameters = default; // 要发送的模板消息的参数
JsonSerializerOptions jsonSerializerOptions = null; // 要发送的模板消息的参数的序列化选项

// API 请求
// 在更新模板消息时，指定泛型参数 T 可以提高序列化性能，省略泛型参数 T 时，序列化器将以序列化 object 的方式进行序列化
await userMessage.ModifyAsync(x =>
{
    x.Content = content;
    x.Cards = cards;
    x.Quote = quote; // 要清除引用，请设置为 Quote.Empty
    x.TemplateId = templateId;
    x.Parameters = parameters;
    x.JsonSerializerOptions = jsonSerializerOptions;
});
```

### [删除私信聊天消息]

POST `/api/v3/direct-message/delete`

```csharp
// API 请求
await userMessage.DeleteAsync();
```

### [获取频道消息某回应的用户列表]

GET `/api/v3/direct-message/reaction-list`

```csharp
IEmote emoji = null; // 要获取用户列表的回应的表情

// 缓存获取消息的回应概要，但不包含用户列表
IReadOnlyDictionary<IEmote,ReactionMetadata> cachedReactions = socketUserMessage.Reactions;

// API 请求
IReadOnlyCollection<IUser> reactionUsers = await userMessage.GetReactionUsersAsync(emoji);
```

### [给某个消息添加回应]

POST `/api/v3/direct-message/add-reaction`

有关如何构造 Emoji，请参考 [表情符号](xref:Guides.Emoji)。

```
IEmote emoji = null; // 要添加的回应的表情
IEnumerable<IEmote> emojis = null; // 要添加的回应的表情列表

// API 请求
await userMessage.AddReactionAsync(emoji);
await userMessage.AddReactionsAsync(emojis);
```

### [删除消息的某个回应]

POST `/api/v3/direct-message/delete-reaction`

```csharp
IEmote emoji = null;
IEnumerable<IEmote> emojis = null;
IUser user = null;

// API 请求
await userMessage.RemoveReactionAsync(emoji, user);
await userMessage.RemoveReactionsAsync(user, emojis);
```

[获取私信聊天消息列表]: https://developer.kookapp.cn/doc/http/direct-message#获取私信聊天消息列表
[获取私信聊天会话详情]: https://developer.kookapp.cn/doc/http/user-chat#%获取私信聊天会话详情
[发送私信聊天消息]: https://developer.kookapp.cn/doc/http/direct-message#发送私信聊天消息
[更新私信聊天消息]: https://developer.kookapp.cn/doc/http/direct-message#更新私信聊天消息
[删除私信聊天消息]: https://developer.kookapp.cn/doc/http/direct-message#删除私信聊天消息
[获取频道消息某回应的用户列表]: https://developer.kookapp.cn/doc/http/direct-message#获取频道消息某回应的用户列表
[给某个消息添加回应]: https://developer.kookapp.cn/doc/http/direct-message#给某个消息添加回应
[删除消息的某个回应]: https://developer.kookapp.cn/doc/http/direct-message#删除消息的某个回应

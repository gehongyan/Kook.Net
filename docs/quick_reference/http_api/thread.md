---
uid: Guides.QuickReference.Http.Thread
title: 帖子相关接口
---

# 帖子相关接口

预声明变量

```csharp
// 帖子接口对于 SocketClient 和 RestClient 的实现是相同的
readonly RestThreadChannel _channel = null;
readonly RestThreadCategory _threadCategory = null;
readonly RestThread _thread = null;
readonly RestThreadPost _post = null;
readonly RestThreadReply _reply = null;
```

### [获取帖子分区列表]

GET `/api/v3/category/list`

```csharp
// API 请求
IReadOnlyCollection<RestThreadCategory> threadCategories = await _channel.GetThreadCategoriesAsync();
```

### [创建帖子]

POST `/api/v3/thread/create`

```csharp
string title = null; // 要发布的帖子的标题
string content = null; // 要发布的帖子的正文内容
bool isKMarkdown = false; // 是否使用 KMarkdown 格式的正文内容
ICard card = null; // 要发布的帖子的卡片内容
IEnumerable<ICard> cards = null; // 要发布的帖子的卡片内容列表
string? cover = null; // 要发布的帖子的封面图片链接
IEnumerable<ThreadTag>? tags = null; // 要发布的帖子的话题标签列表

// API 请求，在帖子频道中发布帖子
RestThread thread = await _channel.CreateThreadAsync(title, content, isKMarkdown, cover, threadCategory, tags);
RestThread thread = await _channel.CreateThreadAsync(title, card, cover, threadCategory, tags);
RestThread thread = await _channel.CreateThreadAsync(title, cards, cover, threadCategory, tags);

// API 请求，在帖子分区中发布帖子
RestThread thread = await _threadCategory.CreateThreadAsync(title, content, isKMarkdown, cover, tags);
RestThread thread = await _threadCategory.CreateThreadAsync(title, card, cover, tags);
RestThread thread = await _threadCategory.CreateThreadAsync(title, cards, cover, tags);
```

### [评论/回复]

POST `/api/v3/thread/reply`

```csharp
string content = null; // 要发布的评论或回复的正文内容
bool isKMarkdown = false; // 是否使用 KMarkdown 格式的正文内容
ICard card = null; // 要发布的评论或回复的卡片内容
IEnumerable<ICard> cards = null; // 要发布的评论或回复的卡片内容列表

// API 请求，在帖子中发布评论
RestThreadPost post = await _thread.CreatePostAsync(content, isKMarkdown);
RestThreadPost post = await _thread.CreatePostAsync(card);
RestThreadPost post = await _thread.CreatePostAsync(cards);

// API 请求，在帖子中发布对评论的回复
RestThreadReply reply = await _post.CreateReplyAsync(content, isKMarkdown);
RestThreadReply reply = await _post.CreateReplyAsync(card);
RestThreadReply reply = await _post.CreateReplyAsync(cards);

// API 请求，在帖子中发布对回复的回复
RestThreadReply reply = await _reply.ReplyAsync(content, isKMarkdown);
RestThreadReply reply = await _reply.ReplyAsync(card);
RestThreadReply reply = await _reply.ReplyAsync(cards);
```

### [帖子详情]

GET `/api/v3/thread/view`

```csharp
ulong id = default; // 要获取详情的帖子 ID

// API 请求
RestThread thread = await _channel.GetThreadAsync(id);
```

### [帖子列表]

GET `/api/v3/thread/list`

```csharp
IThread referenceThread = null; // 获取帖子列表所根据的参考帖子位置的
DateTimeOffset referenceTimestamp = default; // 获取帖子列表所根据的参考帖子位置的创建时间或最后活跃时间，由 sortOrder 决定
ThreadSortOrder sortOrder = default; // 获取帖子列表的排序方式
IThreadCategory? category = null; // 获取帖子帖子所在的分区
int limit = default; // 获取帖子列表的数量

// API 请求
IAsyncEnumerable<IReadOnlyCollection<RestThread>> threads = _channel.GetThreadsAsync(limit, category);
IAsyncEnumerable<IReadOnlyCollection<RestThread>> threads = _channel.GetThreadsAsync(referenceThread, sortOrder, limit, category);
IAsyncEnumerable<IReadOnlyCollection<RestThread>> threads = _channel.GetThreadsAsync(referenceTimestamp, sortOrder, limit, category);
```

### [帖子/评论/回复删除]

POST `/api/v3/thread/delete`

```csharp
IThread thread = null; // 帖子
ulong threadId = default; // 帖子 ID
IThreadPost post = null; // 评论
ulong postId = default; // 评论 ID
IThreadReply reply = null; // 回复
ulong replyId = default; // 回复 ID

// API 请求，删除帖子
await _thread.DeleteAsync();
await _channel.DeleteThreadAsync(thread);
await _channel.DeleteThreadAsync(threadId);

// API 请求，删除帖子的主楼内容
await _thread.DeleteContentAsync();
await _channel.DeleteThreadContentAsync(thread);
await _channel.DeleteThreadContentAsync(threadId);

// API 请求，删除评论
await _post.DeleteAsync();
await _thread.DeletePostAsync(post);
await _thread.DeletePostAsync(postId);

// API 请求，删除评论回复
await _reply.DeleteAsync();
await _post.DeleteReplyAsync(reply);
await _post.DeleteReplyAsync(replyId);
```

### [回复列表]

GET `/api/v3/thread/post`

```csharp
int limit = default; // 获取评论或回复列表的数量
IThreadPost referencePost = null; // 获取评论列表所根据的参考评论位置的
IThreadReply referenceReply = null; // 获取回复列表所根据的参考回复位置的
DateTimeOffset referenceTimestamp = default; // 获取评论或回复列表所根据的参考评论或回复位置的创建时间
SortMode sortMode = default; // 获取评论或回复列表的排序方式

// API 请求，获取帖子评论列表
IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> posts = _thread.GetPostsAsync(limit);
IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> posts = _thread.GetPostsAsync(referencePost, sortMode, limit);
IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> posts = _thread.GetPostsAsync(referenceTimestamp, sortMode, limit);

// API 请求，获取评论回复列表
IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> replies = _post.GetRepliesAsync(limit);
IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> replies = _post.GetRepliesAsync(referenceReply, sortMode, limit);
IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> replies = _post.GetRepliesAsync(referenceTimestamp, sortMode, limit);
```


[获取帖子分区列表]: https://developer.kookapp.cn/doc/http/thread#获取帖子分区列表
[创建帖子]: https://developer.kookapp.cn/doc/http/thread#创建帖子
[评论/回复]: https://developer.kookapp.cn/doc/http/thread#评论/回复
[帖子详情]: https://developer.kookapp.cn/doc/http/thread#帖子详情
[帖子列表]: https://developer.kookapp.cn/doc/http/thread#帖子列表
[帖子/评论/回复删除]: https://developer.kookapp.cn/doc/http/thread#帖子/评论/回复删除
[回复列表]: https://developer.kookapp.cn/doc/http/thread#回复列表

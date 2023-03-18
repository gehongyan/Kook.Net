---
uid: Guides.Emoji
title: 表情符号
---

# Kook.Net 中的表情符号

KOOK 中的表情符号包含两种形式，即 Emoji 表情与服务器自定义表情，在 Kook.Net
中分别以 @Kook.Emoji 与 @Kook.Emote 表示，这两个类都实现了 @Kook.IEmote 接口。

## Emoji

Emoji 是一种通用的表情符号，是 Unicode 标准中的一部分，可以在任何支持 Unicode
标准的平台上使用。因此，诸如 `👌`、`♥`、`👀` 等的字符串都会被 KOOK 解析为 Emoji
表情符号。

每个被 KOOK 所支持的 Emoji 表情符号都有其对应的短代码，其为由两个冒号及其所包围的别名组成，例如
`👌`、`♥`、`👀` 可分别被表示为 `:ok_hand:`、`:heart:`、`:eyes:`。KOOK API 处理有关
Emoji 表情符号的消息时会自动将短代码转换为对应的 Emoji 表情符号。

有关 KOOK 所受支持的短代码，可参考[此页面](xref:Guides.Emoji.EmojiList)。

### 声明 Emoji

要创建 @Kook.Emoji 对象，可将 Emoji 表情符号或其 Unicode 传入 @Kook.Emoji 的构造函数中，例如
`new Emoji("👌");` 或 `new Emoji("\uD83D\uDC4C");`，也可以通过使用 @Kook.Emoji.Parse* 或
@Kook.Emoji.TryParse* 解析 Emoji 表情符号或其短代码的方式创建 @Kook.Emoji 对象，例如
`Emoji.Parse("👌")`、`Emoji.Parse(":ok_hand:")`、`Emoji.TryParse(":ok_hand:", out var emoji)`。

为消息添加 Emoji 表情符号的代码示例：
[!code-csharp[Emoji Sample](samples/emoji-sample.cs)]

## Emote

Kook.Net 中的 Emote 指代 KOOK 中添加到服务器内的自定义表情符号，其在 KMarkdown 中的完全限定形式形如：
`(emj)kook-logo(emj)[1591057729615250/9nG5PxHkZE074074]`。
其中，`kook-logo` 为表情符号的别名，`1591057729615250/9nG5PxHkZE074074` 为表情符号的 ID。

目前 KOOK 中尚未提供便捷获取自定义表情符号的完全限定形式的方法，要获取自定义表情符号的完全限定形式，可通过一下几种方式：

1. Kook.Net 对消息体中的表情符号部分进行了解析，可通过 @Kook.IMessage.Tags 获取 @Kook.ITag.Type 为
   @Kook.TagType.Emoji 的 @Kook.Emote 对象，通过 @Kook.Emote.ToKMarkdownString* 方法获取完全限定形式。
2. 通过 @Kook.WebSocket.SocketGuild.Emotes 属性获取服务器中的全部自定义表情符号，从而获取其完全限定形式。
3. 通过 @Kook.WebSocket.SocketGuild.GetEmotesAsync* 方法获取服务器中的全部自定义表情符号，从而获取其完全限定形式。
4. 在 KOOK 网页端或桌面客户端中启用开发者工具 (Ctrl+Alt+Shift+O)，通过跟踪与表情符号相关的网络请求负载或相应获取其完全限定形式。

### 声明 Emote

要通过服务器自定义表情符号的完全限定形式创建 @Kook.Emote 对象，请使用 @Kook.Emote.Parse* 或
@Kook.Emote.TryParse* 方法，例如 `Emote.Parse("(emj)kook-logo(emj)[1591057729615250/9nG5PxHkZE074074]")`。

[!code[Emote Sample](samples/emote-sample.cs)]

> [!TIP]
> 要在 Socket 客户端中通过表情符号名称获取指定的表情符号，可以访问 @Kook.WebSocket.SocketGuild.Emotes 属性。
> [!code-csharp[Socket emote sample](samples/socket-emote-sample.cs)]

> [!TIP]
> KOOK 中，激活 BUFF 的用户可以跨服务器使用表情符号，因此，Bot 可能会收到来自 Bot 自身并未加入的服务器的表情符号。
> 虽然 KOOK Bot API 允许 Bot 跨服务器使用表情符号，但不能使用未加入的服务器的表情符号。
> 为了安全起见，若在 Socket 客户端中要在消息中引用、或添加新回应时采用未知来源的表情符号，应当访问所有服务器的
> @Kook.WebSocket.SocketGuild.Emotes 属性来确定表情符号是否存在于 Bot 所加入的服务器中，或捕获可能的异常。

## 更多信息

要进一步了解如何使用 Emoji 和 Emote，请参阅 @Kook.IEmote 文档。

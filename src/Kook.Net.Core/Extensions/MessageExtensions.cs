using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kook;

/// <summary>
///     提供用于各种消息实体的扩展方法。
/// </summary>
public static class MessageExtensions
{
    /// <summary>
    ///     获取一个跳转到消息的 URL。
    /// </summary>
    /// <param name="msg"> 要获取跳转 URL 的消息。 </param>
    /// <returns> 一个包含用于在聊天中跳转到消息的 URL 的字符串。 </returns>
    public static string GetJumpUrl(this IMessage msg)
    {
        IMessageChannel channel = msg.Channel;
        return channel switch
        {
            IDMChannel dmChannel => $"https://www.kookapp.cn/app/home/privatemessage/{dmChannel.ChatCode}/{msg.Id}",
            IGuildChannel guildChannel => $"https://www.kookapp.cn/direct/anchor/{guildChannel.GuildId}/{channel.Id}/{msg.Id}",
            _ => throw new ArgumentException("Message must be in a guild or a DM channel.", nameof(msg))
        };
    }

    /// <summary>
    ///     向消息添加多个回应。
    /// </summary>
    /// <remarks>
    ///     此方法会对每个要添加的回应分别发送请求。
    /// </remarks>
    /// <param name="msg"> 要添加回应的消息。 </param>
    /// <param name="reactions"> 要用于向此消息添加回应的所有表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步添加操作的任务。 </returns>
    /// <seealso cref="Kook.IMessage.AddReactionAsync(Kook.IEmote,Kook.RequestOptions)"/>
    public static async Task AddReactionsAsync(this IUserMessage msg, IEnumerable<IEmote> reactions,
        RequestOptions? options = null)
    {
        foreach (IEmote emote in reactions)
            await msg.AddReactionAsync(emote, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     移除消息上的多个回应。
    /// </summary>
    /// <remarks>
    ///     此方法会对每个要移除的回应分别发送请求。
    /// </remarks>
    /// <param name="msg"> 要添加回应的消息。 </param>
    /// <param name="user"> 要删除其回应的用户。 </param>
    /// <param name="reactions"> 要从此消息移除的回应的所有表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    /// <seealso cref="Kook.IMessage.RemoveReactionAsync(Kook.IEmote,Kook.IUser,Kook.RequestOptions)"/>
    public static async Task RemoveReactionsAsync(this IUserMessage msg, IUser user, IEnumerable<IEmote> reactions,
        RequestOptions? options = null)
    {
        foreach (IEmote emote in reactions)
            await msg.RemoveReactionAsync(emote, user, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     向消息所属的频道回复文件消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        string path, string? filename = null, AttachmentType type = AttachmentType.File, bool isQuote = false,
        bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(path, filename, type,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复文件消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        Stream stream, string filename, AttachmentType type = AttachmentType.File, bool isQuote = false,
        bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(stream, filename, type,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复文件消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(this IUserMessage message,
        FileAttachment attachment, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendFileAsync(attachment,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复文字消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="content"> 要发送的文本。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyTextAsync(this IUserMessage message,
        string content, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendTextAsync(content,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复文字消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="templateId"> 消息模板的 ID。 </param>
    /// <param name="parameters"> 传入消息模板的参数。 </param>
    /// <param name="isQuote"> 是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="jsonSerializerOptions"> 序列化模板参数时要使用的序列化选项。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <typeparam name="T"> 参数的类型。 </typeparam>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyTextAsync<T>(this IUserMessage message,
        ulong templateId, T parameters, bool isQuote = false, bool isEphemeral = false,
        JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        await message.Channel.SendTextAsync(templateId, parameters,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                jsonSerializerOptions, options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复卡片消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="isQuote">是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral">是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardsAsync(this IUserMessage message,
        IEnumerable<ICard> cards, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendCardsAsync(cards,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复卡片消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="templateId"> 消息模板的 ID。 </param>
    /// <param name="parameters"> 传入消息模板的参数。 </param>
    /// <param name="isQuote">是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral">是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="jsonSerializerOptions"> 序列化模板参数时要使用的序列化选项。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <typeparam name="T"> 参数的类型。 </typeparam>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardsAsync<T>(this IUserMessage message,
        ulong templateId, T parameters, bool isQuote = false, bool isEphemeral = false,
        JsonSerializerOptions? jsonSerializerOptions = null, RequestOptions? options = null) =>
        await message.Channel.SendCardsAsync(templateId, parameters,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                jsonSerializerOptions, options)
            .ConfigureAwait(false);

    /// <summary>
    ///     向消息所属的频道回复卡片消息。
    /// </summary>
    /// <param name="message"> 要回复的消息。 </param>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="isQuote">是否在回复消息时引用被回复的消息。 </param>
    /// <param name="isEphemeral">是否以临时消息的方式回复。如果设置为 <c>true</c>，则仅该被回复的消息的作者可以看到此回复消息，否则所有人都可以看到此回复消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    public static async Task<Cacheable<IUserMessage, Guid>> ReplyCardAsync(this IUserMessage message,
        ICard card, bool isQuote = false, bool isEphemeral = false, RequestOptions? options = null) =>
        await message.Channel.SendCardAsync(card,
                new MessageReference(isQuote ? message.Id : Guid.Empty, message.Id),
                isEphemeral ? message.Author : null,
                options)
            .ConfigureAwait(false);

    /// <summary>
    ///     获取此消息是否可能是图文混排消息。
    /// </summary>
    /// <param name="msg"> 要判断的消息。 </param>
    /// <returns> 如果此消息可能是图文混排消息，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool MaybeTextImageMixedMessage(this IUserMessage msg)
    {
        if (msg.Cards.Count != 1) return false;
        if (msg.Cards.First() is not Card card) return false;
        if (card.Theme != CardTheme.Invisible) return false;
        if (card.Modules.Length == 0) return false;
        return card.Modules.All(x => x is
            SectionModule { Text: PlainTextElement or KMarkdownElement }
            or ContainerModule { Elements: [not null] }
            or IMediaModule { Source.Length: > 0 }
        );
    }

    /// <summary>
    ///     尝试将消息内卡片的内容展开为单个字符串。
    /// </summary>
    /// <param name="msg"> 要展开的消息。 </param>
    /// <param name="extractedContent"> 展开的内容。 </param>
    /// <returns> 如果成功展开，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    /// <remarks>
    ///     仅图文混排消息可以用于内容展开，参见 <see cref="MaybeTextImageMixedMessage"/>。
    /// </remarks>
    public static bool TryExtractCardContent(this IUserMessage msg,
        [NotNullWhen(true)] out string? extractedContent)
    {
        // TODO: 支持设置 TagHandling
        if (!msg.MaybeTextImageMixedMessage())
        {
            extractedContent = null;
            return false;
        }

        string result = string.Join(" ", EnumerateCardModuleContents(msg.Cards));
        if (string.IsNullOrWhiteSpace(result))
        {
            extractedContent = null;
            return false;
        }

        extractedContent = result;
        return true;
    }

    internal static IEnumerable<string> EnumerateCardModuleContents(IEnumerable<ICard> cards) => cards
        .OfType<Card>()
        .SelectMany(x => x.Modules)
        .Select(x => x switch
        {
            SectionModule { Text: PlainTextElement or KMarkdownElement } sectionModule =>
                sectionModule.Text.ToString(),
            ContainerModule { Elements: [{ } element] } => element.Source,
            IMediaModule { Source: { Length: > 0 } mediaSource } => mediaSource,
            _ => null
        })
        .OfType<string>()
        .Where(x => !string.IsNullOrWhiteSpace(x));
}

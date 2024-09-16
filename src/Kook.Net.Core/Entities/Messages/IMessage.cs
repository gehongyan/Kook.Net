namespace Kook;

/// <summary>
///     表示一个通用的消息。
/// </summary>
public interface IMessage : IEntity<Guid>, IDeletable
{
    #region General

    /// <summary>
    ///     获取此消息的类型。
    /// </summary>
    MessageType Type { get; }

    /// <summary>
    ///     获取此消息的来源。
    /// </summary>
    MessageSource Source { get; }

    /// <summary>
    ///     获取此消息是否被置顶。
    /// </summary>
    bool? IsPinned { get; }

    /// <summary>
    ///     获取此消息的来源频道。
    /// </summary>
    IMessageChannel Channel { get; }

    /// <summary>
    ///     获取此消息的作者。
    /// </summary>
    IUser Author { get; }

    /// <summary>
    ///     获取此消息的内容。
    /// </summary>
    /// <remarks>
    ///     如果消息不是文本消息，则此属性可能为空或包含原始代码。
    /// </remarks>
    string Content { get; }

    /// <summary>
    ///     获取此消息的纯净内容。
    /// </summary>
    /// <returns>
    ///     此属性会对 <see cref="Content"/> 的内容进行两步操作： <br />
    ///     1. 使用 <see cref="Kook.IUserMessage.Resolve(Kook.TagHandling,Kook.TagHandling,Kook.TagHandling,Kook.TagHandling,Kook.TagHandling)"/>
    ///     方法解析所有标签的完整名称； <br />
    ///     2. 使用 <see cref="Kook.Format.StripMarkdown(System.String)"/> 清理所有 KMarkdown 格式字符。
    /// </returns>
    /// <seealso cref="Kook.IUserMessage.Resolve(Kook.TagHandling,Kook.TagHandling,Kook.TagHandling,Kook.TagHandling,Kook.TagHandling)"/>
    /// <seealso cref="Kook.Format.StripMarkdown(System.String)"/>
    string CleanContent { get; }

    /// <summary>
    ///     获取此消息的发送时间。
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    ///     获取此消息最后一次编辑的时间。
    /// </summary>
    /// <remarks>
    ///     如果此消息从未被编辑过，则此属性的值为 <see langword="null"/>。
    /// </remarks>
    DateTimeOffset? EditedTimestamp { get; }

    /// <summary>
    ///     获取此消息中提及的所有用户的 ID。
    /// </summary>
    IReadOnlyCollection<ulong> MentionedUserIds { get; }

    /// <summary>
    ///     获取此消息中提及的所有角色的 ID。
    /// </summary>
    IReadOnlyCollection<uint> MentionedRoleIds { get; }

    /// <summary>
    ///     获取此消息是否提及了全体成员。
    /// </summary>
    bool MentionedEveryone { get; }

    /// <summary>
    ///     获取此消息是否提及了在线成员。
    /// </summary>
    bool MentionedHere { get; }

    /// <summary>
    ///     获取此消息中解析出的所有标签。
    /// </summary>
    IReadOnlyCollection<ITag> Tags { get; }

    /// <summary>
    ///     获取此消息中包含的所有附件。
    /// </summary>
    /// <remarks>
    ///     此属性也会包含从卡片中解析出来的附件信息。
    /// </remarks>
    IReadOnlyCollection<IAttachment> Attachments { get; }

    /// <summary>
    ///     获取此消息中包含的所有卡片。
    /// </summary>
    IReadOnlyCollection<ICard> Cards { get; }

    /// <summary>
    ///     获取此消息中包含的所有嵌入式内容。
    /// </summary>
    IReadOnlyCollection<IEmbed> Embeds { get; }

    /// <summary>
    ///     获取此消息中包含的所有 POKE。
    /// </summary>
    IReadOnlyCollection<IPokeAction> Pokes { get; }

    #endregion

    #region Reactions

    /// <summary>
    ///     获取此消息中包含的所有回应。
    /// </summary>
    IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }

    /// <summary>
    ///     向此消息添加一个回应。
    /// </summary>
    /// <param name="emote"> 要用于向此消息添加回应的表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示添加添加异步操作的任务。 </returns>
    Task AddReactionAsync(IEmote emote, RequestOptions? options = null);

    /// <summary>
    ///     从此消息中移除一个回应。
    /// </summary>
    /// <param name="emote"> 要从此消息移除的回应的表情符号。 </param>
    /// <param name="user"> 要移除其回应的用户。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions? options = null);

    /// <summary>
    ///     从此消息中移除一个回应。
    /// </summary>
    /// <param name="emote"> 要从此消息移除的回应的表情符号。 </param>
    /// <param name="userId"> 要移除其回应的用户的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步移除操作的任务。 </returns>
    Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions? options = null);

    /// <summary>
    ///     获取所有对消息使用给定表情符号进行回应的用户。
    /// </summary>
    /// <param name="emote"> 要获取其回应用户的表情符号。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含对消息使用给定表情符号进行回应的所有用户。 </returns>
    Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emote, RequestOptions? options = null);

    #endregion
}

using Kook.Commands.Builders;

namespace Kook.Commands;

/// <summary>
///     表示一个模块基类。
/// </summary>
public abstract class ModuleBase : ModuleBase<ICommandContext>;

/// <summary>
///     表示一个模块基类。
/// </summary>
/// <typeparam name="T"> 模块的上下文类型。 </typeparam>
public abstract class ModuleBase<T> : IModuleBase
    where T : class, ICommandContext
{
    #region ModuleBase

    /// <summary>
    ///     获取此命令的上下文。
    /// </summary>
    public T Context { get; private set; } = null!; // 将由 SetContext 方法设置。

    /// <summary>
    ///     发送文件到此命令消息所在的频道。
    /// </summary>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> 是否引用源消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的形式发送给命令调用者。如果设置为 <c>true</c>，则仅该用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, bool isQuote = true, bool isEphemeral = false,
        RequestOptions? options = null) =>
        await Context.Channel.SendFileAsync(path, filename, type,
                isQuote ? new MessageReference(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options)
            .ConfigureAwait(false);

    /// <summary>
    ///     发送文件到此命令消息所在的频道。
    /// </summary>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="isQuote"> 是否引用源消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的形式发送给命令调用者。如果设置为 <c>true</c>，则仅该用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, bool isQuote = true, bool isEphemeral = false,
        RequestOptions? options = null) =>
        await Context.Channel.SendFileAsync(stream, filename, type,
                isQuote ? new MessageReference(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options)
            .ConfigureAwait(false);

    /// <summary>
    ///     发送文件到此命令消息所在的频道。
    /// </summary>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="isQuote"> 是否引用源消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的形式发送给命令调用者。如果设置为 <c>true</c>，则仅该用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(FileAttachment attachment,
        bool isQuote = true, bool isEphemeral = false, RequestOptions? options = null) =>
        await Context.Channel.SendFileAsync(attachment,
                isQuote ? new MessageReference(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options)
            .ConfigureAwait(false);

    /// <summary>
    ///     发送文本消息到此命令消息所在的频道。
    /// </summary>
    /// <param name="text"> 要发送的文本。 </param>
    /// <param name="isQuote"> 是否引用源消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的形式发送给命令调用者。如果设置为 <c>true</c>，则仅该用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyTextAsync(string text, bool isQuote = true,
        bool isEphemeral = false, RequestOptions? options = null) =>
        await Context.Channel.SendTextAsync(text,
                isQuote ? new MessageReference(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options)
            .ConfigureAwait(false);

    /// <summary>
    ///     发送卡片消息到此命令消息所在的频道。
    /// </summary>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="isQuote"> 是否引用源消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的形式发送给命令调用者。如果设置为 <c>true</c>，则仅该用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyCardsAsync(IEnumerable<ICard> cards,
        bool isQuote = true, bool isEphemeral = false, RequestOptions? options = null) =>
        await Context.Channel.SendCardsAsync(cards,
                isQuote ? new MessageReference(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options)
            .ConfigureAwait(false);

    /// <summary>
    ///     发送卡片消息到此命令消息所在的频道。
    /// </summary>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="isQuote"> 是否引用源消息。 </param>
    /// <param name="isEphemeral"> 是否以临时消息的形式发送给命令调用者。如果设置为 <c>true</c>，则仅该用户可以看到此消息，否则所有人都可以看到此消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。 </returns>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyCardAsync(ICard card,
        bool isQuote = true, bool isEphemeral = false, RequestOptions? options = null) =>
        await Context.Channel.SendCardAsync(card,
                isQuote ? new MessageReference(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options)
            .ConfigureAwait(false);

    /// <inheritdoc cref="Kook.Commands.IModuleBase.BeforeExecuteAsync(Kook.Commands.CommandInfo)" />
    protected virtual Task BeforeExecuteAsync(CommandInfo command) => Task.CompletedTask;

    /// <inheritdoc cref="Kook.Commands.IModuleBase.BeforeExecute(Kook.Commands.CommandInfo)" />
    protected virtual void BeforeExecute(CommandInfo command)
    {
    }

    /// <inheritdoc cref="Kook.Commands.IModuleBase.AfterExecuteAsync(Kook.Commands.CommandInfo)" />
    protected virtual Task AfterExecuteAsync(CommandInfo command) => Task.CompletedTask;

    /// <inheritdoc cref="Kook.Commands.IModuleBase.AfterExecute(Kook.Commands.CommandInfo)" />
    protected virtual void AfterExecute(CommandInfo command)
    {
    }

    /// <inheritdoc cref="Kook.Commands.IModuleBase.OnModuleBuilding(Kook.Commands.CommandService,Kook.Commands.Builders.ModuleBuilder)" />
    protected virtual void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
    {
    }

    #endregion

    #region IModuleBase

    void IModuleBase.SetContext(ICommandContext context)
    {
        T? newValue = context as T;
        Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}.");
    }

    Task IModuleBase.BeforeExecuteAsync(CommandInfo command) => BeforeExecuteAsync(command);
    void IModuleBase.BeforeExecute(CommandInfo command) => BeforeExecute(command);
    Task IModuleBase.AfterExecuteAsync(CommandInfo command) => AfterExecuteAsync(command);
    void IModuleBase.AfterExecute(CommandInfo command) => AfterExecute(command);

    void IModuleBase.OnModuleBuilding(CommandService commandService, ModuleBuilder builder) =>
        OnModuleBuilding(commandService, builder);

    #endregion
}

using Kook.Commands.Builders;

namespace Kook.Commands;

/// <summary>
///     Provides a base class for a command module to inherit from.
/// </summary>
public abstract class ModuleBase : ModuleBase<ICommandContext> { }

/// <summary>
///     Provides a base class for a command module to inherit from.
/// </summary>
/// <typeparam name="T">A class that implements <see cref="ICommandContext"/>.</typeparam>
public abstract class ModuleBase<T> : IModuleBase
    where T : class, ICommandContext
{
    #region ModuleBase
    /// <summary>
    ///     The underlying context of the command.
    /// </summary>
    /// <seealso cref="T:Kook.Commands.ICommandContext" />
    /// <seealso cref="T:Kook.Commands.CommandContext" />
    public T Context { get; private set; }

    /// <summary>
    ///     Sends a file to the source channel.
    /// </summary>
    /// <param name="path">
    ///     The file path of the file.
    /// </param>
    /// <param name="fileName">
    ///     The name of the file.
    /// </param>
    /// <param name="type">The type of the attachment.</param>
    /// <param name="isQuote">
    ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
    /// </param>
    /// <param name="isEphemeral">
    ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
    /// </param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(string path, string fileName = null,
        AttachmentType type = AttachmentType.File, bool isQuote = true, bool isEphemeral = false, RequestOptions options = null)
    {
        return await Context.Channel.SendFileAsync(path, fileName, type, isQuote ? new Quote(Context.Message.Id) : null,
            isEphemeral ? Context.User : null, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a file to the source channel.
    /// </summary>
    /// <param name="stream">
    ///     Stream of the file to be sent.
    /// </param>
    /// <param name="fileName">
    ///     The name of the file.
    /// </param>
    /// <param name="type">The type of the attachment.</param>
    /// <param name="isQuote">
    ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
    /// </param>
    /// <param name="isEphemeral">
    ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
    /// </param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(Stream stream, string fileName = null,
        AttachmentType type = AttachmentType.File, bool isQuote = true, bool isEphemeral = false, RequestOptions options = null)
    {
        return await Context.Channel.SendFileAsync(stream, fileName, type, isQuote ? new Quote(Context.Message.Id) : null,
            isEphemeral ? Context.User : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     Sends a file to the source channel.
    /// </summary>
    /// <param name="attachment">The attachment containing the file.</param>
    /// <param name="isQuote">
    ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
    /// </param>
    /// <param name="isEphemeral">
    ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
    /// </param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyFileAsync(FileAttachment attachment, bool isQuote = true,
        bool isEphemeral = false, RequestOptions options = null)
    {
        return await Context.Channel.SendFileAsync(attachment, isQuote ? new Quote(Context.Message.Id) : null,
            isEphemeral ? Context.User : null, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a text message to the source channel.
    /// </summary>
    /// <param name="message">
    ///     Contents of the message.
    /// </param>
    /// <param name="isQuote">
    ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
    /// </param>
    /// <param name="isEphemeral">
    ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
    /// </param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyTextAsync(string message, bool isQuote = true,
        bool isEphemeral = false, RequestOptions options = null)
    {
        return await Context.Channel.SendTextAsync(message, isQuote ? new Quote(Context.Message.Id) : null,
            isEphemeral ? Context.User : null, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a card message to the source channel.
    /// </summary>
    /// <param name="cards">
    ///     The cards to be sent.
    /// </param>
    /// <param name="isQuote">
    ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
    /// </param>
    /// <param name="isEphemeral">
    ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
    /// </param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyCardsAsync(IEnumerable<ICard> cards, bool isQuote = true,
        bool isEphemeral = false, RequestOptions options = null)
    {
        return await Context.Channel.SendCardsAsync(cards, isQuote ? new Quote(Context.Message.Id) : null,
            isEphemeral ? Context.User : null, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends a card message to the source channel.
    /// </summary>
    /// <param name="card">
    ///     The card to be sent.
    /// </param>
    /// <param name="isQuote">
    ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
    /// </param>
    /// <param name="isEphemeral">
    ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
    /// </param>
    /// <param name="options">The request options for this <see langword="async"/> request.</param>
    protected virtual async Task<Cacheable<IUserMessage, Guid>> ReplyCardAsync(ICard card,
        bool isQuote = true,
        bool isEphemeral = false, RequestOptions options = null)
    {
        return await Context.Channel.SendCardAsync(card, isQuote ? new Quote(Context.Message.Id) : null,
            isEphemeral ? Context.User : null, options).ConfigureAwait(false);
    }
    /// <summary>
    ///     The method to execute asynchronously before executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual Task BeforeExecuteAsync(CommandInfo command) => Task.CompletedTask;
    /// <summary>
    ///     The method to execute before executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual void BeforeExecute(CommandInfo command)
    {
    }
    /// <summary>
    ///     The method to execute asynchronously after executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual Task AfterExecuteAsync(CommandInfo command) => Task.CompletedTask;
    /// <summary>
    ///     The method to execute after executing the command.
    /// </summary>
    /// <param name="command">The <see cref="CommandInfo"/> of the command to be executed.</param>
    protected virtual void AfterExecute(CommandInfo command)
    {
    }

    /// <summary>
    ///     The method to execute when building the module.
    /// </summary>
    /// <param name="commandService">The <see cref="CommandService"/> used to create the module.</param>
    /// <param name="builder">The builder used to build the module.</param>
    protected virtual void OnModuleBuilding(CommandService commandService, ModuleBuilder builder)
    {
    }
    #endregion

    #region IModuleBase
    void IModuleBase.SetContext(ICommandContext context)
    {
        var newValue = context as T;
        Context = newValue ?? throw new InvalidOperationException($"Invalid context type. Expected {typeof(T).Name}, got {context.GetType().Name}.");
    }
    Task IModuleBase.BeforeExecuteAsync(CommandInfo command) => BeforeExecuteAsync(command);
    void IModuleBase.BeforeExecute(CommandInfo command) => BeforeExecute(command);
    Task IModuleBase.AfterExecuteAsync(CommandInfo command) => AfterExecuteAsync(command);
    void IModuleBase.AfterExecute(CommandInfo command) => AfterExecute(command);
    void IModuleBase.OnModuleBuilding(CommandService commandService, ModuleBuilder builder) => OnModuleBuilding(commandService, builder);
    #endregion
}

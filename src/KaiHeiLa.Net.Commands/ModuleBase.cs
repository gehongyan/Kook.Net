using System;
using System.Threading.Tasks;
using KaiHeiLa.Commands.Builders;

namespace KaiHeiLa.Commands
{
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
        /// <seealso cref="T:KaiHeiLa.Commands.ICommandContext" />
        /// <seealso cref="T:KaiHeiLa.Commands.CommandContext" />
        public T Context { get; private set; }

        /// <summary>
        ///     Sends a plain text message to the source channel.
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
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyTextAsync(string message, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendTextMessageAsync(message, isQuote ? new Quote(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends an image to the source channel.
        /// </summary>
        /// <param name="path">
        ///     The file path of the image file.
        /// </param>
        /// <param name="fileName">
        ///     The name of the image file.
        /// </param>
        /// <param name="isQuote">
        ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        /// </param>
        /// <param name="isEphemeral">
        ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        /// </param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyImageAsync(string path, string fileName = null, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendImageMessageAsync(path, fileName, isQuote ? new Quote(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends an image to the source channel.
        /// </summary>
        /// <param name="stream">
        ///     Stream of the image file to be sent.
        /// </param>
        /// <param name="fileName">
        ///     The name of the image file.
        /// </param>
        /// <param name="isQuote">
        ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        /// </param>
        /// <param name="isEphemeral">
        ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        /// </param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyImageAsync(Stream stream, string fileName = null, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendImageMessageAsync(stream, fileName, isQuote ? new Quote(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a video to the source channel.
        /// </summary>
        /// <param name="path">
        ///     The file path of the video file.
        /// </param>
        /// <param name="fileName">
        ///     The name of the video file.
        /// </param>
        /// <param name="isQuote">
        ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        /// </param>
        /// <param name="isEphemeral">
        ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        /// </param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyVideoAsync(string path, string fileName = null, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendVideoMessageAsync(path, fileName, isQuote ? new Quote(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a video to the source channel.
        /// </summary>
        /// <param name="stream">
        ///     Stream of the video file to be sent.
        /// </param>
        /// <param name="fileName">
        ///     The name of the video file.
        /// </param>
        /// <param name="isQuote">
        ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        /// </param>
        /// <param name="isEphemeral">
        ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        /// </param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyVideoAsync(Stream stream, string fileName = null, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendVideoMessageAsync(stream, fileName, isQuote ? new Quote(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        }

        // /// <summary>
        // ///     Sends an audio to the source channel.
        // /// </summary>
        // /// <param name="path">
        // ///     The audio file path of the file.
        // /// </param>
        // /// <param name="fileName">
        // ///     The name of the file.
        // /// </param>
        // /// <param name="isQuote">
        // ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        // /// </param>
        // /// <param name="isEphemeral">
        // ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        // /// </param>
        // /// <param name="options">The request options for this <see langword="async"/> request.</param>
        // protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyAudioAsync(string path, string fileName = null, bool isQuote = false,
        //     bool isEphemeral = false, RequestOptions options = null)
        // {
        //     return await Context.Channel.SendAudioMessageAsync(path, fileName, isQuote ? new Quote(Context.Message.Id) : null,
        //         isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        // }
        // /// <summary>
        // ///     Sends an audio to the source channel.
        // /// </summary>
        // /// <param name="stream">
        // ///     Stream of the audio file to be sent.
        // /// </param>
        // /// <param name="fileName">
        // ///     The name of the file.
        // /// </param>
        // /// <param name="isQuote">
        // ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        // /// </param>
        // /// <param name="isEphemeral">
        // ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        // /// </param>
        // /// <param name="options">The request options for this <see langword="async"/> request.</param>
        // protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyAudioAsync(Stream stream, string fileName = null, bool isQuote = false,
        //     bool isEphemeral = false, RequestOptions options = null)
        // {
        //     return await Context.Channel.SendAudioMessageAsync(stream, fileName, isQuote ? new Quote(Context.Message.Id) : null,
        //         isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        // }

        /// <summary>
        ///     Sends a file to the source channel.
        /// </summary>
        /// <param name="path">
        ///     The file path of the file.
        /// </param>
        /// <param name="fileName">
        ///     The name of the file.
        /// </param>
        /// <param name="isQuote">
        ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        /// </param>
        /// <param name="isEphemeral">
        ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        /// </param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyFileAsync(string path, string fileName = null, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendFileMessageAsync(path, fileName, isQuote ? new Quote(Context.Message.Id) : null,
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
        /// <param name="isQuote">
        ///     <c>true</c> if the source message will be quoted in this message; otherwise, <c>false</c>.
        /// </param>
        /// <param name="isEphemeral">
        ///     <c>true</c> if the message to be sent can be seen only by the command invoker; otherwise, <c>false</c>.
        /// </param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyFileAsync(Stream stream, string fileName = null, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendFileMessageAsync(stream, fileName, isQuote ? new Quote(Context.Message.Id) : null,
                isEphemeral ? Context.User : null, options).ConfigureAwait(false);
        }
        
        /// <summary>
        ///     Sends a KMarkdown message to the source channel.
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
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyKMarkdownAsync(string message, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendKMarkdownMessageAsync(message, isQuote ? new Quote(Context.Message.Id) : null,
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
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyCardsAsync(IEnumerable<ICard> cards, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendCardMessageAsync(cards, isQuote ? new Quote(Context.Message.Id) : null,
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
        protected virtual async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyCardsAsync(ICard card, bool isQuote = false,
            bool isEphemeral = false, RequestOptions options = null)
        {
            return await Context.Channel.SendCardMessageAsync(new[] { card }, isQuote ? new Quote(Context.Message.Id) : null,
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
}

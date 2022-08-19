using System;
using System.Threading.Tasks;
using System.IO;

namespace KaiHeiLa
{
    /// <summary> An extension class for various Discord user objects. </summary>
    public static class UserExtensions
    {
        /// <summary>
        ///     Sends a message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(this IUser user,
            string text,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendTextMessageAsync(text, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends an image via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="path">The file path of the image.</param>
        /// <param name="fileName">The name of the image.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(this IUser user,
            string path, 
            string fileName = null,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendImageMessageAsync(path, fileName, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends an image via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="stream">The stream of the image.</param>
        /// <param name="fileName">The name of the image.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(this IUser user,
            Stream stream, 
            string fileName = null,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendImageMessageAsync(stream, fileName, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends an image via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="uri">The URI of the image.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(this IUser user,
            Uri uri, 
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendImageMessageAsync(uri, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a video via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="path">The file path of the video.</param>
        /// <param name="fileName">The name of the video.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(this IUser user,
            string path, 
            string fileName = null,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendVideoMessageAsync(path, fileName, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a video via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="stream">The stream of the video.</param>
        /// <param name="fileName">The name of the video.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(this IUser user,
            Stream stream, 
            string fileName = null,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendVideoMessageAsync(stream, fileName, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a video via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="uri">The URI of the video.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(this IUser user,
            Uri uri, 
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendVideoMessageAsync(uri, quote, options).ConfigureAwait(false);
        }
        // /// <summary>
        // ///     Sends an audio via DM.
        // /// </summary>
        // /// <param name="user">The user to send the DM to.</param>
        // /// <param name="path">The file path of the audio.</param>
        // /// <param name="fileName">The name of the audio.</param>
        // /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        // /// <param name="options">The options to be used when sending the request.</param>
        // public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(this IUser user,
        //     string path, 
        //     string fileName = null,
        //     IQuote quote = null,
        //     RequestOptions options = null)
        // {
        //     return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendAudioMessageAsync(path, fileName, quote, options).ConfigureAwait(false);
        // }
        // /// <summary>
        // ///     Sends an audio via DM.
        // /// </summary>
        // /// <param name="user">The user to send the DM to.</param>
        // /// <param name="stream">The stream of the audio.</param>
        // /// <param name="fileName">The name of the audio.</param>
        // /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        // /// <param name="options">The options to be used when sending the request.</param>
        // public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(this IUser user,
        //     Stream stream, 
        //     string fileName = null,
        //     IQuote quote = null,
        //     RequestOptions options = null)
        // {
        //     return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendAudioMessageAsync(stream, fileName, quote, options).ConfigureAwait(false);
        // }
        // /// <summary>
        // ///     Sends an audio via DM.
        // /// </summary>
        // /// <param name="user">The user to send the DM to.</param>
        // /// <param name="uri">The URI of the audio.</param>
        // /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        // /// <param name="options">The options to be used when sending the request.</param>
        // public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(this IUser user,
        //     Uri uri, 
        //     IQuote quote = null,
        //     RequestOptions options = null)
        // {
        //     return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendAudioMessageAsync(uri, quote, options).ConfigureAwait(false);
        // }
        /// <summary>
        ///     Sends a file via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="path">The file path of the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(this IUser user,
            string path, 
            string fileName = null,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileMessageAsync(path, fileName, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a file via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="stream">The stream of the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(this IUser user,
            Stream stream, 
            string fileName = null,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileMessageAsync(stream, fileName, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a file via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="uri">The URI of the file.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(this IUser user,
            Uri uri, 
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileMessageAsync(uri, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a KMarkdown message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="content">The KMarkdown content to be sent.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(this IUser user,
            string content,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendKMarkdownMessageAsync(content, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a card message message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="cards">The cards to be sent.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyCardsAsync(this IUser user, 
            IEnumerable<ICard> cards, IQuote quote = null, RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendCardMessageAsync(cards, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a card message message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="card">The card to be sent.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> ReplyCardsAsync(this IUser user, 
            ICard card, IQuote quote = null, RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendCardMessageAsync(new[] { card }, quote, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Bans the user from the guild and optionally prunes their recent messages.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="pruneDays">The number of days to remove messages from this <paramref name="user"/> for - must be between [0, 7]</param>
        /// <param name="reason">The reason of the ban to be written in the audit log.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <exception cref="ArgumentException"><paramref name="pruneDays" /> is not between 0 to 7.</exception>
        /// <returns>
        ///     A task that represents the asynchronous operation for banning a user.
        /// </returns>
        public static Task BanAsync(this IGuildUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
            => user.Guild.AddBanAsync(user, pruneDays, reason, options);
    }
}

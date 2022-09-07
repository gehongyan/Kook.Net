using System;
using System.Threading.Tasks;
using System.IO;

namespace Kook
{
    /// <summary> An extension class for various Kook user objects. </summary>
    public static class UserExtensions
    {
        /// <summary>
        ///     Sends a file via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="path">The file path of the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="type">The type of the file.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
            string path, string fileName = null, AttachmentType type = AttachmentType.File, IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(path, fileName, type, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a file via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="stream">The stream of the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="type">The type of the file.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
            Stream stream, string fileName = null, AttachmentType type = AttachmentType.File, IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false))
                .SendFileAsync(stream, fileName, type, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a file via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="attachment">The attachment containing the file.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(this IUser user,
            FileAttachment attachment, IQuote quote = null, RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendFileAsync(attachment, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a text message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="content">The KMarkdown content to be sent.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        public static async Task<Cacheable<IUserMessage, Guid>> SendTextAsync(this IUser user,
            string content,
            IQuote quote = null,
            RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendTextAsync(content, quote, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Sends a card message message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="cards">The cards to be sent.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        public static async Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(this IUser user, 
            IEnumerable<ICard> cards, IQuote quote = null, RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendCardsAsync(cards, quote, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends a card message message via DM.
        /// </summary>
        /// <param name="user">The user to send the DM to.</param>
        /// <param name="card">The card to be sent.</param>
        /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
        /// <param name="options">The request options for this <see langword="async"/> request.</param>
        public static async Task<Cacheable<IUserMessage, Guid>> SendCardAsync(this IUser user,
            ICard card, IQuote quote = null, RequestOptions options = null)
        {
            return await (await user.CreateDMChannelAsync().ConfigureAwait(false)).SendCardAsync(card, quote, options).ConfigureAwait(false);
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

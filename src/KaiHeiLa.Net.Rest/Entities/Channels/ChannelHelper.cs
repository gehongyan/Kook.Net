using KaiHeiLa.API;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Rest;

internal static class ChannelHelper
{
    #region General

    public static async Task DeleteGuildChannelAsync(IGuildChannel channel, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteGuildChannelAsync(channel.Id, options).ConfigureAwait(false);
    }
    
    public static async Task DeleteDMChannelAsync(IDMChannel channel, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteUserChatAsync(channel.Recipient.Id, options).ConfigureAwait(false);
    }

    #endregion

    #region Messages
    
    public static async Task<RestMessage> GetMessageAsync(IMessageChannel channel, BaseKaiHeiLaClient client,
        Guid id, RequestOptions options)
    {
        var guildId = (channel as IGuildChannel)?.GuildId;
        var guild = guildId != null ? await (client as IKaiHeiLaClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).ConfigureAwait(false) : null;
        var model = await client.ApiClient.GetMessageAsync(id, options).ConfigureAwait(false);
        if (model == null)
            return null;
        var author = MessageHelper.GetAuthor(client, guild, model.Author);
        return RestMessage.Create(client, channel, author, model);
    }
    
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendMessageAsync(IMessageChannel channel, 
        BaseKaiHeiLaClient client, MessageType messageType, string content, RequestOptions options, IQuote quote = null, IUser ephemeralUser = null)
    {
        CreateMessageParams args = new(messageType, channel.Id, content)
        {
            QuotedMessageId = quote?.QuotedMessageId,
            EphemeralUserId = ephemeralUser?.Id
        };
        CreateMessageResponse model = await client.ApiClient.CreateMessageAsync(args, options).ConfigureAwait(false);
        return (model.MessageId, model.MessageTimestamp);
    }

    public static Task DeleteMessageAsync(IMessageChannel channel, Guid messageId, BaseKaiHeiLaClient client,
        RequestOptions options)
        => MessageHelper.DeleteAsync(messageId, client, options);

    public static Task DeleteDirectMessageAsync(IMessageChannel channel, Guid messageId, BaseKaiHeiLaClient client,
        RequestOptions options)
        => MessageHelper.DeleteDirectAsync(messageId, client, options);

    
    public static async Task ModifyMessageAsync(IMessageChannel channel, Guid messageId, Action<MessageProperties> func,
        BaseKaiHeiLaClient client, RequestOptions options)
        => await MessageHelper.ModifyAsync(messageId, client, func, options).ConfigureAwait(false);
    
    #endregion

    #region Direct Messages

    public static async Task<RestMessage> GetDirectMessageAsync(IDMChannel channel, BaseKaiHeiLaClient client,
        Guid id, RequestOptions options)
    {
        var model = await client.ApiClient.GetDirectMessageAsync(id, chatCode: channel.Id, options: options).ConfigureAwait(false);
        if (model == null)
            return null;
        User userModel = await client.ApiClient.GetUserAsync(model.AuthorId, options);
        var author = MessageHelper.GetAuthor(client, null, userModel);
        return RestMessage.Create(client, channel, author, model);
    }
    
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendDirectMessageAsync(IDMChannel channel, 
        BaseKaiHeiLaClient client, MessageType messageType, string content, RequestOptions options, IQuote quote = null)
    {
        CreateDirectMessageParams args = new(messageType, channel.Recipient.Id, content)
        {
            QuotedMessageId = quote?.QuotedMessageId,
        };
        CreateDirectMessageResponse model = await client.ApiClient.CreateDirectMessageAsync(args, options).ConfigureAwait(false);
        return (model.MessageId, model.MessageTimestamp);
    }

    public static async Task ModifyDirectMessageAsync(IDMChannel channel, Guid messageId, Action<MessageProperties> func,
        BaseKaiHeiLaClient client, RequestOptions options)
        => await MessageHelper.ModifyDirectAsync(messageId, client, func, options).ConfigureAwait(false);
    #endregion
}
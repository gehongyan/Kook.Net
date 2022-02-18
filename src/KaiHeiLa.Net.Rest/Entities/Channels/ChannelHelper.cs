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
    
    public static async Task<(Guid Messageid, DateTimeOffset MessageTimestamp)> SendMessageAsync(IMessageChannel channel, 
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

    #endregion
}
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Rest;

internal static class ChannelHelper
{
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
}
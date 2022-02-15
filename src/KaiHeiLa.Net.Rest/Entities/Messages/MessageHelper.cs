namespace KaiHeiLa.Rest;

internal static class MessageHelper
{
    public static Task DeleteAsync(IMessage msg, BaseKaiHeiLaClient client, RequestOptions options)
        => DeleteAsync(msg.Channel.Id, msg.Id, client, options);
    
    public static async Task DeleteAsync(ulong channelId, Guid msgId, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteMessageAsync(msgId, options).ConfigureAwait(false);
    }
}
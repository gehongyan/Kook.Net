namespace KaiHeiLa.Rest;
using Model = KaiHeiLa.API.Message;
using UserModel = KaiHeiLa.API.User;

internal static class MessageHelper
{
    public static Task DeleteAsync(IMessage msg, BaseKaiHeiLaClient client, RequestOptions options)
        => DeleteAsync(msg.Channel.Id, msg.Id, client, options);
    
    public static async Task DeleteAsync(ulong channelId, Guid msgId, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteMessageAsync(msgId, options).ConfigureAwait(false);
    }
    
    public static MessageSource GetSource(Model msg)
    {
        if (msg.Author.Bot ?? false)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }
    
    public static IUser GetAuthor(BaseKaiHeiLaClient client, IGuild guild, UserModel model)
    {
        IUser author = null;
        if (guild != null)
            author = guild.GetUserAsync(model.Id, CacheMode.CacheOnly).Result;
        if (author == null)
            author = RestUser.Create(client, guild, model);
        return author;
    }
}
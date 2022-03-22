using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Rest
{
    internal static class InviteHelper
    {
        public static async Task DeleteAsync(IInvite invite, BaseKaiHeiLaClient client, 
            RequestOptions options)
        {
            DeleteGuildInviteParams args = new()
            {
                GuildId = invite.GuildId,
                ChannelId = invite.ChannelId,
                UrlCode = invite.Code
            };
            await client.ApiClient.DeleteGuildInviteAsync(args, options).ConfigureAwait(false);
        }
    }
}

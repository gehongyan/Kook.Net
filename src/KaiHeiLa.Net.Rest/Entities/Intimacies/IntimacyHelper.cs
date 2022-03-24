using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Rest
{
    internal static class IntimacyHelper
    {
        public static async Task UpdateAsync(IIntimacy intimacy, BaseKaiHeiLaClient client,
            Action<IntimacyProperties> func,
            RequestOptions options)
        {
            IntimacyProperties properties = new()
            {
                Score = intimacy.Score,
                SocialInfo = intimacy.SocialInfo
            };
            func(properties);
            var args = new UpdateIntimacyValueParams()
            {
                UserId = intimacy.User.Id,
                Score = properties.Score,
                SocialInfo = properties.SocialInfo,
                ImageId = properties.ImageId
            };
            await client.ApiClient.UpdateIntimacyValueAsync(args, options).ConfigureAwait(false);
        }
    }
}

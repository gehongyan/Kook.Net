using Kook.API.Rest;

namespace Kook.Rest;

internal static class IntimacyHelper
{
    public static async Task UpdateAsync(IIntimacy intimacy, BaseKookClient client,
        Action<IntimacyProperties> func,
        RequestOptions? options)
    {
        IntimacyProperties properties = new(intimacy.SocialInfo, intimacy.Score);
        func(properties);
        UpdateIntimacyValueParams args = new()
        {
            UserId = intimacy.User.Id,
            Score = properties.Score,
            SocialInfo = properties.SocialInfo,
            ImageId = properties.ImageId
        };
        await client.ApiClient.UpdateIntimacyValueAsync(args, options).ConfigureAwait(false);
    }
}

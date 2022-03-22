using Model = KaiHeiLa.API.Role;

namespace KaiHeiLa.Rest;

internal static class RoleHelper
{
    #region General

    public static async Task<Model> ModifyAsync(IRole role, BaseKaiHeiLaClient client,
        Action<RoleProperties> func, RequestOptions options)
    {
        var args = new RoleProperties();
        func(args);

        var apiArgs = new API.Rest.ModifyGuildRoleParams
        {
            GuildId = role.Guild.Id,
            Name = args.Name,
            Color = args.Color?.RawValue,
            Hoist = args.Hoist switch
            {
                true => 1,
                false => 0,
                null => null
            },
            Mentionable = args.Mentionable switch
            {
                true => 1,
                false => 0,
                null => null
            },
            Permissions = args.Permissions?.RawValue,
        };

        var model = await client.ApiClient.ModifyGuildRoleAsync(apiArgs, options).ConfigureAwait(false);
        return model;
    }

    #endregion
}
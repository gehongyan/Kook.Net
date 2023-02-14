using Model = Kook.API.Role;

namespace Kook.Rest;

internal static class RoleHelper
{
    #region General

    public static async Task DeleteAsync(IRole role, BaseKookClient client,
        RequestOptions options)
    {
        var args = new API.Rest.DeleteGuildRoleParams()
        {
            Id = role.Id,
            GuildId = role.Guild.Id
        };
        await client.ApiClient.DeleteGuildRoleAsync(args, options).ConfigureAwait(false);
    }
    public static async Task<Model> ModifyAsync(IRole role, BaseKookClient client,
        Action<RoleProperties> func, RequestOptions options)
    {
        var args = new RoleProperties();
        func(args);

        var apiArgs = new API.Rest.ModifyGuildRoleParams
        {
            GuildId = role.Guild.Id,
            RoleId = role.Id,
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

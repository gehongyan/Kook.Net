using Kook.API.Rest;
using Model = Kook.API.Role;

namespace Kook.Rest;

internal static class RoleHelper
{
    #region General

    public static async Task DeleteAsync(IRole role, BaseKookClient client, RequestOptions? options)
    {
        DeleteGuildRoleParams args = new()
        {
            Id = role.Id,
            GuildId = role.Guild.Id
        };
        await client.ApiClient.DeleteGuildRoleAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<Model> ModifyAsync(IRole role, BaseKookClient client,
        Action<RoleProperties> func, RequestOptions? options)
    {
        RoleProperties args = new();
        func(args);
        ModifyGuildRoleParams apiArgs = new()
        {
            GuildId = role.Guild.Id,
            RoleId = role.Id,
            Name = args.Name,
            Color = args.Color,
            IsHoisted = args.IsHoisted switch
            {
                true => 1,
                false => 0,
                _ => null
            },
            IsMentionable = args.IsMentionable switch
            {
                true => 1,
                false => 0,
                _ => null
            },
            Permissions = args.Permissions?.RawValue
        };

        Model model = await client.ApiClient.ModifyGuildRoleAsync(apiArgs, options).ConfigureAwait(false);
        return model;
    }

    #endregion
}

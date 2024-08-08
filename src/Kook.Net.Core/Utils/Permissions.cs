using System.Runtime.CompilerServices;

namespace Kook;

internal static class Permissions
{
    public const int MaxBits = 29;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PermValue GetValue(ulong allow, ulong deny, ChannelPermission flag) =>
        GetValue(allow, deny, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PermValue GetValue(ulong allow, ulong deny, GuildPermission flag) =>
        GetValue(allow, deny, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PermValue GetValue(ulong allow, ulong deny, ulong flag)
    {
        if (HasFlag(allow, flag))
            return PermValue.Allow;
        if (HasFlag(deny, flag))
            return PermValue.Deny;
        return PermValue.Inherit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetValue(ulong value, ChannelPermission flag) =>
        GetValue(value, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetValue(ulong value, GuildPermission flag) =>
        GetValue(value, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetValue(ulong value, ulong flag) => HasFlag(value, flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValue(ref ulong rawValue, bool? value, ChannelPermission flag) =>
        SetValue(ref rawValue, value, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValue(ref ulong rawValue, bool? value, GuildPermission flag) =>
        SetValue(ref rawValue, value, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValue(ref ulong rawValue, bool? value, ulong flag)
    {
        if (value.HasValue)
        {
            if (value == true)
                SetFlag(ref rawValue, flag);
            else
                UnsetFlag(ref rawValue, flag);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, ChannelPermission flag) =>
        SetValue(ref allow, ref deny, value, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, GuildPermission flag) =>
        SetValue(ref allow, ref deny, value, (ulong)flag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetValue(ref ulong allow, ref ulong deny, PermValue? value, ulong flag)
    {
        if (value.HasValue)
            switch (value)
            {
                case PermValue.Allow:
                    SetFlag(ref allow, flag);
                    UnsetFlag(ref deny, flag);
                    break;
                case PermValue.Deny:
                    UnsetFlag(ref allow, flag);
                    SetFlag(ref deny, flag);
                    break;
                default:
                    UnsetFlag(ref allow, flag);
                    UnsetFlag(ref deny, flag);
                    break;
            }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasFlag(ulong value, ulong flag) => (value & flag) == flag;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetFlag(ref ulong value, ulong flag) => value |= flag;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnsetFlag(ref ulong value, ulong flag) => value &= ~flag;

    public static ChannelPermissions ToChannelPerms(IGuildChannel channel, ulong guildPermissions) =>
        new(guildPermissions & ChannelPermissions.All(channel).RawValue);

    public static ulong ResolveGuild(IGuild guild, IGuildUser user)
    {
        ulong resolvedPermissions = 0;

        if (user.Id == guild.OwnerId)
            resolvedPermissions = GuildPermissions.All.RawValue; //Owners always have all permissions
        else
        {
            foreach (uint roleId in user.RoleIds)
                resolvedPermissions |= guild.GetRole(roleId)?.Permissions.RawValue ?? 0;

            if (GetValue(resolvedPermissions, GuildPermission.Administrator))
                resolvedPermissions = GuildPermissions.All.RawValue; //Administrators always have all permissions
        }

        return resolvedPermissions;
    }

    /*public static ulong ResolveChannel(IGuildUser user, IGuildChannel channel)
    {
        return ResolveChannel(user, channel, ResolveGuild(user));
    }*/
    public static ulong ResolveChannel(IGuild guild, IGuildUser user, IGuildChannel channel, ulong guildPermissions)
    {
        ulong resolvedPermissions;

        ulong mask = ChannelPermissions.All(channel).RawValue;
        if (GetValue(guildPermissions, GuildPermission.Administrator)) //Includes owner
            resolvedPermissions = mask;                                    //Owners and administrators always have all permissions
        else
        {
            //Start with this user's guild permissions
            resolvedPermissions = guildPermissions;

            //Give/Take Everyone permissions
            if (channel.GetPermissionOverwrite(guild.EveryoneRole) is {} everyoneRolePerms)
            {
                resolvedPermissions &= ~everyoneRolePerms.DenyValue;
                resolvedPermissions |= everyoneRolePerms.AllowValue;
            }

            //Give/Take Role permissions
            ulong deniedPermissions = 0UL;
            ulong allowedPermissions = 0UL;
            foreach (uint roleId in user.RoleIds)
            {
                if (roleId == guild.EveryoneRole.Id
                    || guild.GetRole(roleId) is not { } role
                    || channel.GetPermissionOverwrite(role) is not { } rolePerms)
                    continue;
                allowedPermissions |= rolePerms.AllowValue;
                deniedPermissions |= rolePerms.DenyValue;
            }

            resolvedPermissions &= ~deniedPermissions;
            resolvedPermissions |= allowedPermissions;

            //Give/Take User permissions
            if (channel.GetPermissionOverwrite(user) is { } userPerms)
            {
                resolvedPermissions &= ~userPerms.DenyValue;
                resolvedPermissions |= userPerms.AllowValue;
            }

            if (channel is ITextChannel)
            {
                if (!GetValue(resolvedPermissions, ChannelPermission.ViewChannel))
                    //No read permission on a text channel removes all other permissions
                    resolvedPermissions = 0;
                else
                {
                    //No send permissions on a text channel removes all send-related permissions
                    if (!GetValue(resolvedPermissions, ChannelPermission.SendMessages))
                    {
                        resolvedPermissions &= ~(ulong)ChannelPermission.MentionEveryone;
                        resolvedPermissions &= ~(ulong)ChannelPermission.AttachFiles;
                    }
                    // Connect permission overrides passive voice connect permissions
                    if (GetValue(resolvedPermissions, ChannelPermission.Connect))
                        resolvedPermissions |= (ulong)ChannelPermission.PassiveConnect;
                }
            }

            //Ensure we didn't get any permissions this channel doesn't support (from guildPerms, for example)
            resolvedPermissions &= mask;
        }

        return resolvedPermissions;
    }
}

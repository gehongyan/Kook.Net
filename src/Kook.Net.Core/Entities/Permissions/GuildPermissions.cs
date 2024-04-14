using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a set of permissions for a guild.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct GuildPermissions
{
    /// <summary> Gets a blank <see cref="GuildPermissions"/> that grants no permissions. </summary>
    public static readonly GuildPermissions None = new();

    /// <summary> Gets a <see cref="GuildPermissions"/> that grants all guild permissions. </summary>
    public static readonly GuildPermissions All = new(0b1_1111_1111_1111_1111_1111_1111_1111);

    /// <summary> Gets a packed value representing all the permissions in this <see cref="GuildPermissions"/>. </summary>
    public ulong RawValue { get; }

    /// <summary> If <c>true</c>, a user is granted all permissions, and cannot have them revoked via channel permissions. </summary>
    public bool Administrator => Permissions.GetValue(RawValue, GuildPermission.Administrator);

    /// <summary> If <c>true</c>, a user may adjust guild properties. </summary>
    public bool ManageGuild => Permissions.GetValue(RawValue, GuildPermission.ManageGuild);

    /// <summary> If <c>true</c>, a user may view the audit log. </summary>
    public bool ViewAuditLog => Permissions.GetValue(RawValue, GuildPermission.ViewAuditLog);

    /// <summary> If <c>true</c>, a user may create invites. </summary>
    public bool CreateInvites => Permissions.GetValue(RawValue, GuildPermission.CreateInvites);

    /// <summary> If <c>true</c>, a user may view and revoke invites. </summary>
    public bool ManageInvites => Permissions.GetValue(RawValue, GuildPermission.ManageInvites);

    /// <summary> If <c>true</c>, a user may create, delete and modify channels. </summary>
    public bool ManageChannels => Permissions.GetValue(RawValue, GuildPermission.ManageChannels);

    /// <summary> If <c>true</c>, a user may kick users from the guild. </summary>
    public bool KickMembers => Permissions.GetValue(RawValue, GuildPermission.KickMembers);

    /// <summary> If <c>true</c>, a user may ban users from the guild. </summary>
    public bool BanMembers => Permissions.GetValue(RawValue, GuildPermission.BanMembers);

    /// <summary> If <c>true</c>, a user may edit the emojis for this guild. </summary>
    public bool ManageEmojis => Permissions.GetValue(RawValue, GuildPermission.ManageEmojis);

    /// <summary> If <c>true</c>, a user may change their own nickname. </summary>
    public bool ChangeNickname => Permissions.GetValue(RawValue, GuildPermission.ChangeNickname);

    /// <summary> If <c>true</c>, a user may adjust roles. </summary>
    public bool ManageRoles => Permissions.GetValue(RawValue, GuildPermission.ManageRoles);

    /// <summary> If <c>true</c>, a user may view channels. </summary>
    public bool ViewChannel => Permissions.GetValue(RawValue, GuildPermission.ViewChannel);

    /// <summary> If <c>true</c>, a user may send messages. </summary>
    public bool SendMessages => Permissions.GetValue(RawValue, GuildPermission.SendMessages);

    /// <summary> If <c>true</c>, a user may delete messages. </summary>
    public bool ManageMessages => Permissions.GetValue(RawValue, GuildPermission.ManageMessages);

    /// <summary> If <c>true</c>, a user may send files. </summary>
    public bool AttachFiles => Permissions.GetValue(RawValue, GuildPermission.AttachFiles);

    /// <summary> If <c>true</c>, a user may connect to a voice channel. </summary>
    public bool Connect => Permissions.GetValue(RawValue, GuildPermission.Connect);

    /// <summary> If <c>true</c>, a user may kick other users from voice channels, and move other users between voice channels. </summary>
    public bool ManageVoice => Permissions.GetValue(RawValue, GuildPermission.ManageVoice);

    /// <summary> If <c>true</c>, a user may mention all users. </summary>
    public bool MentionEveryone => Permissions.GetValue(RawValue, GuildPermission.MentionEveryone);

    /// <summary> If <c>true</c>, a user may add reactions. </summary>
    public bool AddReactions => Permissions.GetValue(RawValue, GuildPermission.AddReactions);

    /// <summary> If <c>true</c>, a user may follow added reactions. </summary>
    public bool FollowReactions => Permissions.GetValue(RawValue, GuildPermission.FollowReactions);

    /// <summary> If <c>true</c>, a user may connect to a voice channel only when the user is invited or moved by other users. </summary>
    public bool PassiveConnect => Permissions.GetValue(RawValue, GuildPermission.PassiveConnect);

    /// <summary> If <c>true</c>, a user may speak only via push-to-talk. </summary>
    public bool OnlyPushToTalk => Permissions.GetValue(RawValue, GuildPermission.OnlyPushToTalk);

    /// <summary> If <c>true</c>, a user may use voice activation. </summary>
    public bool UseVoiceActivity => Permissions.GetValue(RawValue, GuildPermission.UseVoiceActivity);

    /// <summary> If <c>true</c>, a user may speak in a voice channel. </summary>
    public bool Speak => Permissions.GetValue(RawValue, GuildPermission.Speak);

    /// <summary> If <c>true</c>, a user may deafen users. </summary>
    public bool DeafenMembers => Permissions.GetValue(RawValue, GuildPermission.DeafenMembers);

    /// <summary> If <c>true</c>, a user may mute users. </summary>
    public bool MuteMembers => Permissions.GetValue(RawValue, GuildPermission.MuteMembers);

    /// <summary> If <c>true</c>, a user may change the nickname of other users. </summary>
    public bool ManageNicknames => Permissions.GetValue(RawValue, GuildPermission.ManageNicknames);

    /// <summary> If <c>true</c>, a user may play soundtracks in a voice channel. </summary>
    public bool PlaySoundtrack => Permissions.GetValue(RawValue, GuildPermission.PlaySoundtrack);

    /// <summary> If <c>true</c>, a user may share screen in a voice channel. </summary>
    public bool ShareScreen => Permissions.GetValue(RawValue, GuildPermission.ShareScreen);

    /// <summary> Creates a new <see cref="GuildPermissions"/> with the provided packed value. </summary>
    public GuildPermissions(ulong rawValue) => RawValue = rawValue;

    /// <summary> Creates a new <see cref="GuildPermissions"/> with the provided packed value after converting to ulong. </summary>
    public GuildPermissions(string rawValue) => RawValue = ulong.Parse(rawValue);

    private GuildPermissions(ulong initialValue,
        bool? administrator = null,
        bool? manageGuild = null,
        bool? viewAuditLog = null,
        bool? createInvites = null,
        bool? manageInvites = null,
        bool? manageChannels = null,
        bool? kickMembers = null,
        bool? banMembers = null,
        bool? manageEmojis = null,
        bool? changeNickname = null,
        bool? manageRoles = null,
        bool? viewChannel = null,
        bool? sendMessages = null,
        bool? manageMessages = null,
        bool? attachFiles = null,
        bool? connect = null,
        bool? manageVoice = null,
        bool? mentionEveryone = null,
        bool? addReactions = null,
        bool? followReactions = null,
        bool? passiveConnect = null,
        bool? onlyPushToTalk = null,
        bool? useVoiceActivity = null,
        bool? speak = null,
        bool? deafenMembers = null,
        bool? muteMembers = null,
        bool? manageNicknames = null,
        bool? playSoundtrack = null,
        bool? shareScreen = null
    )
    {
        ulong value = initialValue;

        Permissions.SetValue(ref value, administrator, GuildPermission.Administrator);
        Permissions.SetValue(ref value, manageGuild, GuildPermission.ManageGuild);
        Permissions.SetValue(ref value, viewAuditLog, GuildPermission.ViewAuditLog);
        Permissions.SetValue(ref value, createInvites, GuildPermission.CreateInvites);
        Permissions.SetValue(ref value, manageInvites, GuildPermission.ManageInvites);
        Permissions.SetValue(ref value, manageChannels, GuildPermission.ManageChannels);
        Permissions.SetValue(ref value, kickMembers, GuildPermission.KickMembers);
        Permissions.SetValue(ref value, banMembers, GuildPermission.BanMembers);
        Permissions.SetValue(ref value, manageEmojis, GuildPermission.ManageEmojis);
        Permissions.SetValue(ref value, changeNickname, GuildPermission.ChangeNickname);
        Permissions.SetValue(ref value, manageRoles, GuildPermission.ManageRoles);
        Permissions.SetValue(ref value, viewChannel, GuildPermission.ViewChannel);
        Permissions.SetValue(ref value, sendMessages, GuildPermission.SendMessages);
        Permissions.SetValue(ref value, manageMessages, GuildPermission.ManageMessages);
        Permissions.SetValue(ref value, attachFiles, GuildPermission.AttachFiles);
        Permissions.SetValue(ref value, connect, GuildPermission.Connect);
        Permissions.SetValue(ref value, manageVoice, GuildPermission.ManageVoice);
        Permissions.SetValue(ref value, mentionEveryone, GuildPermission.MentionEveryone);
        Permissions.SetValue(ref value, addReactions, GuildPermission.AddReactions);
        Permissions.SetValue(ref value, followReactions, GuildPermission.FollowReactions);
        Permissions.SetValue(ref value, passiveConnect, GuildPermission.PassiveConnect);
        Permissions.SetValue(ref value, onlyPushToTalk, GuildPermission.OnlyPushToTalk);
        Permissions.SetValue(ref value, useVoiceActivity, GuildPermission.UseVoiceActivity);
        Permissions.SetValue(ref value, speak, GuildPermission.Speak);
        Permissions.SetValue(ref value, deafenMembers, GuildPermission.DeafenMembers);
        Permissions.SetValue(ref value, muteMembers, GuildPermission.MuteMembers);
        Permissions.SetValue(ref value, manageNicknames, GuildPermission.ManageNicknames);
        Permissions.SetValue(ref value, playSoundtrack, GuildPermission.PlaySoundtrack);
        Permissions.SetValue(ref value, shareScreen, GuildPermission.ShareScreen);

        RawValue = value;
    }

    /// <summary> Creates a new <see cref="GuildPermissions"/> structure with the provided permissions. </summary>
    public GuildPermissions(
        bool administrator = false,
        bool manageGuild = false,
        bool viewAuditLog = false,
        bool createInvites = false,
        bool manageInvites = false,
        bool manageChannels = false,
        bool kickMembers = false,
        bool banMembers = false,
        bool manageEmojis = false,
        bool changeNickname = false,
        bool manageRoles = false,
        bool viewChannel = false,
        bool sendMessages = false,
        bool manageMessages = false,
        bool attachFiles = false,
        bool connect = false,
        bool manageVoice = false,
        bool mentionEveryone = false,
        bool addReactions = false,
        bool followReactions = false,
        bool passiveConnect = false,
        bool onlyPushToTalk = false,
        bool useVoiceActivity = false,
        bool speak = false,
        bool deafenMembers = false,
        bool muteMembers = false,
        bool manageNicknames = false,
        bool playSoundtrack = false,
        bool shareScreen = false)
        : this(0, administrator, manageGuild, viewAuditLog, createInvites, manageInvites, manageChannels, kickMembers,
            banMembers, manageEmojis, changeNickname, manageRoles, viewChannel, sendMessages, manageMessages, attachFiles,
            connect, manageVoice, mentionEveryone, addReactions, followReactions, passiveConnect, onlyPushToTalk,
            useVoiceActivity, speak, deafenMembers, muteMembers, manageNicknames, playSoundtrack, shareScreen)
    {
    }

    /// <summary> Creates a new <see cref="GuildPermissions"/> from this one, changing the provided non-null permissions. </summary>
    public GuildPermissions Modify(
        bool? administrator = null,
        bool? manageGuild = null,
        bool? viewAuditLog = null,
        bool? createInvites = null,
        bool? manageInvites = null,
        bool? manageChannels = null,
        bool? kickMembers = null,
        bool? banMembers = null,
        bool? manageEmojis = null,
        bool? changeNickname = null,
        bool? manageRoles = null,
        bool? viewChannel = null,
        bool? sendMessages = null,
        bool? manageMessages = null,
        bool? attachFiles = null,
        bool? connect = null,
        bool? manageVoice = null,
        bool? mentionEveryone = null,
        bool? addReactions = null,
        bool? followReactions = null,
        bool? passiveConnect = null,
        bool? onlyPushToTalk = null,
        bool? useVoiceActivity = null,
        bool? speak = null,
        bool? deafenMembers = null,
        bool? muteMembers = null,
        bool? manageNicknames = null,
        bool? playSoundtrack = null,
        bool? shareScreen = null)
        => new(RawValue, administrator, manageGuild, viewAuditLog, createInvites, manageInvites,
            manageChannels, kickMembers, banMembers, manageEmojis, changeNickname, manageRoles, viewChannel,
            sendMessages, manageMessages, attachFiles, connect, manageVoice, mentionEveryone, addReactions,
            followReactions, passiveConnect, onlyPushToTalk, useVoiceActivity, speak, deafenMembers, muteMembers,
            manageNicknames, playSoundtrack, shareScreen);

    /// <summary>
    ///     Returns a value that indicates if a specific <see cref="GuildPermission"/> is enabled
    ///     in these permissions.
    /// </summary>
    /// <param name="permission">The permission value to check for.</param>
    /// <returns><c>true</c> if the permission is enabled, <c>false</c> otherwise.</returns>
    public bool Has(GuildPermission permission) => Permissions.GetValue(RawValue, permission);

    /// <summary>
    ///     Returns a <see cref="List{T}"/> containing all of the <see cref="GuildPermission"/>
    ///     flags that are enabled.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> containing <see cref="GuildPermission"/> flags. Empty if none are enabled.</returns>
    public List<GuildPermission> ToList()
    {
        List<GuildPermission> perms = new();

        // bitwise operations on raw value
        // each of the GuildPermissions increments by 2^i from 0 to MaxBits
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = (ulong)1 << i;
            if ((RawValue & flag) != 0) perms.Add((GuildPermission)flag);
        }

        return perms;
    }

    internal void Ensure(GuildPermission permissions)
    {
        if (!Has(permissions))
        {
            IEnumerable<GuildPermission> vals = Enum.GetValues(typeof(GuildPermission)).Cast<GuildPermission>();
            ulong currentValues = RawValue;
            IEnumerable<GuildPermission> missingValues = vals.Where(x => permissions.HasFlag(x) && !Permissions.GetValue(currentValues, x));

            throw new InvalidOperationException(
                $"Missing required guild permission{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
        }
    }

    /// <summary>
    ///     Gets the raw value of the permissions.
    /// </summary>
    public override string ToString() => RawValue.ToString();

    private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
}

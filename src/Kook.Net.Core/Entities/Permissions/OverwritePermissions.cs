using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a container for a series of overwrite permissions.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct OverwritePermissions
{
    /// <summary>
    ///     Gets a blank <see cref="OverwritePermissions" /> that inherits all permissions.
    /// </summary>
    public static OverwritePermissions InheritAll { get; } = new OverwritePermissions();
    /// <summary>
    ///     Gets a <see cref="OverwritePermissions" /> that grants all permissions for the given channel.
    /// </summary>
    /// <exception cref="ArgumentException">Unknown channel type.</exception>
    public static OverwritePermissions AllowAll(IChannel channel)
        => new OverwritePermissions(ChannelPermissions.All(channel).RawValue, 0);
    /// <summary>
    ///     Gets a <see cref="OverwritePermissions" /> that denies all permissions for the given channel.
    /// </summary>
    /// <exception cref="ArgumentException">Unknown channel type.</exception>
    public static OverwritePermissions DenyAll(IChannel channel)
        => new OverwritePermissions(0, ChannelPermissions.All(channel).RawValue);

    /// <summary>
    ///     Gets a packed value representing all the allowed permissions in this <see cref="OverwritePermissions"/>.
    /// </summary>
    public ulong AllowValue { get; }
    /// <summary>
    ///     Gets a packed value representing all the denied permissions in this <see cref="OverwritePermissions"/>.
    /// </summary>
    public ulong DenyValue { get; }

    /// <summary> If Allowed, a user may create invites. </summary>
    public PermValue CreateInvites => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.CreateInvites);
    /// <summary> If Allowed, a user may create, delete and modify channels. </summary>
    public PermValue ManageChannels => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageChannels);
    /// <summary> If Allowed, a user may adjust roles. </summary>
    public PermValue ManageRoles => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageRoles);
    /// <summary> If Allowed, a user may view channels. </summary>
    public PermValue ViewChannel => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ViewChannel);
    /// <summary> If Allowed, a user may send messages. </summary>
    public PermValue SendMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.SendMessages);
    /// <summary> If Allowed, a user may delete messages. </summary>
    public PermValue ManageMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageMessages);
    /// <summary> If Allowed, a user may send files. </summary>
    public PermValue AttachFiles => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.AttachFiles);
    /// <summary> If Allowed, a user may connect to a voice channel. </summary>
    public PermValue Connect => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.Connect);
    /// <summary> If Allowed, a user may kick other users from voice channels, and move other users between voice channels. </summary>
    public PermValue ManageVoice => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageVoice);
    /// <summary> If Allowed, a user may mention all users. </summary>
    public PermValue MentionEveryone => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MentionEveryone);
    /// <summary> If Allowed, a user may add reactions. </summary>
    public PermValue AddReactions => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.AddReactions);
    /// <summary> If Allowed, a user may connect to a voice channel only when the user is invited or moved by other users. </summary>
    public PermValue PassiveConnect => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.PassiveConnect);
    /// <summary> If Allowed, a user may use voice activation. </summary>
    public PermValue UseVoiceActivity => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.UseVoiceActivity);
    /// <summary> If Allowed, a user may speak in a voice channel. </summary>
    public PermValue Speak => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.Speak);
    /// <summary> If Allowed, a user may deafen users. </summary>
    public PermValue DeafenMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.DeafenMembers);
    /// <summary> If Allowed, a user may mute users. </summary>
    public PermValue MuteMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MuteMembers);
    /// <summary> If Allowed, a user may play soundtracks in a voice channel. </summary>
    public PermValue PlaySoundtrack => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.PlaySoundtrack);

    /// <summary> Creates a new OverwritePermissions with the provided allow and deny packed values. </summary>
    public OverwritePermissions(ulong allowValue, ulong denyValue)
    {
        AllowValue = allowValue;
        DenyValue = denyValue;
    }

    /// <summary> Creates a new OverwritePermissions with the provided allow and deny packed values after converting to ulong. </summary>
    public OverwritePermissions(string allowValue, string denyValue)
    {
        AllowValue = uint.Parse(allowValue);
        DenyValue = uint.Parse(denyValue);
    }

    private OverwritePermissions(ulong allowValue, ulong denyValue,
        PermValue? createInvites = null,
        PermValue? manageChannels = null,
        PermValue? manageRoles = null,
        PermValue? viewChannel = null,
        PermValue? sendMessages = null,
        PermValue? manageMessages = null,
        PermValue? attachFiles = null,
        PermValue? connect = null,
        PermValue? manageVoice = null,
        PermValue? mentionEveryone = null,
        PermValue? addReactions = null,
        PermValue? passiveConnect = null,
        PermValue? useVoiceActivity = null,
        PermValue? speak = null,
        PermValue? deafenMembers = null,
        PermValue? muteMembers = null,
        PermValue? playSoundtrack = null)
    {
        Permissions.SetValue(ref allowValue, ref denyValue, createInvites, ChannelPermission.CreateInvites);
        Permissions.SetValue(ref allowValue, ref denyValue, manageChannels, ChannelPermission.ManageChannels);
        Permissions.SetValue(ref allowValue, ref denyValue, manageRoles, ChannelPermission.ManageRoles);
        Permissions.SetValue(ref allowValue, ref denyValue, viewChannel, ChannelPermission.ViewChannel);
        Permissions.SetValue(ref allowValue, ref denyValue, sendMessages, ChannelPermission.SendMessages);
        Permissions.SetValue(ref allowValue, ref denyValue, manageMessages, ChannelPermission.ManageMessages);
        Permissions.SetValue(ref allowValue, ref denyValue, attachFiles, ChannelPermission.AttachFiles);
        Permissions.SetValue(ref allowValue, ref denyValue, connect, ChannelPermission.Connect);
        Permissions.SetValue(ref allowValue, ref denyValue, manageVoice, ChannelPermission.ManageVoice);
        Permissions.SetValue(ref allowValue, ref denyValue, mentionEveryone, ChannelPermission.MentionEveryone);
        Permissions.SetValue(ref allowValue, ref denyValue, addReactions, ChannelPermission.AddReactions);
        Permissions.SetValue(ref allowValue, ref denyValue, passiveConnect, ChannelPermission.PassiveConnect);
        Permissions.SetValue(ref allowValue, ref denyValue, useVoiceActivity, ChannelPermission.UseVoiceActivity);
        Permissions.SetValue(ref allowValue, ref denyValue, speak, ChannelPermission.Speak);
        Permissions.SetValue(ref allowValue, ref denyValue, deafenMembers, ChannelPermission.DeafenMembers);
        Permissions.SetValue(ref allowValue, ref denyValue, muteMembers, ChannelPermission.MuteMembers);
        Permissions.SetValue(ref allowValue, ref denyValue, playSoundtrack, ChannelPermission.PlaySoundtrack);

        AllowValue = allowValue;
        DenyValue = denyValue;
    }

    /// <summary>
    ///     Initializes a new <see cref="ChannelPermissions"/> struct with the provided permissions.
    /// </summary>
    public OverwritePermissions(
        PermValue createInvites = PermValue.Inherit,
        PermValue manageChannels = PermValue.Inherit,
        PermValue manageRoles = PermValue.Inherit,
        PermValue viewChannel = PermValue.Inherit,
        PermValue sendMessages = PermValue.Inherit,
        PermValue manageMessages = PermValue.Inherit,
        PermValue attachFiles = PermValue.Inherit,
        PermValue connect = PermValue.Inherit,
        PermValue manageVoice = PermValue.Inherit,
        PermValue mentionEveryone = PermValue.Inherit,
        PermValue addReactions = PermValue.Inherit,
        PermValue passiveConnect = PermValue.Inherit,
        PermValue useVoiceActivity = PermValue.Inherit,
        PermValue speak = PermValue.Inherit,
        PermValue deafenMembers = PermValue.Inherit,
        PermValue muteMembers = PermValue.Inherit,
        PermValue playSoundtrack = PermValue.Inherit)
        : this(0, 0, createInvites, manageChannels, manageRoles, viewChannel, sendMessages, manageMessages,
            attachFiles, connect, manageVoice, mentionEveryone, addReactions, passiveConnect, useVoiceActivity, speak,
            deafenMembers, muteMembers, playSoundtrack)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="OverwritePermissions" /> from the current one, changing the provided
    ///     non-null permissions.
    /// </summary>
    public OverwritePermissions Modify(
        PermValue? createInvites = null,
        PermValue? manageChannels = null,
        PermValue? manageRoles = null,
        PermValue? viewChannel = null,
        PermValue? sendMessages = null,
        PermValue? manageMessages = null,
        PermValue? attachFiles = null,
        PermValue? connect = null,
        PermValue? manageVoice = null,
        PermValue? mentionEveryone = null,
        PermValue? addReactions = null,
        PermValue? passiveConnect = null,
        PermValue? useVoiceActivity = null,
        PermValue? speak = null,
        PermValue? deafenMembers = null,
        PermValue? muteMembers = null,
        PermValue? playSoundtrack = null)
        => new OverwritePermissions(AllowValue, DenyValue, createInvites, manageChannels, manageRoles,
            viewChannel, sendMessages, manageMessages, attachFiles, connect, manageVoice, mentionEveryone,
            addReactions, passiveConnect, useVoiceActivity, speak, deafenMembers, muteMembers, playSoundtrack);

    /// <summary>
    ///     Creates a <see cref="List{T}"/> of all the <see cref="ChannelPermission"/> values that are allowed.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> of all allowed <see cref="ChannelPermission"/> flags. If none, the list will be empty.</returns>
    public List<ChannelPermission> ToAllowList()
    {
        List<ChannelPermission> perms = new();
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            // first operand must be long or ulong to shift >31 bits
            ulong flag = ((ulong) 1 << i);
            if ((AllowValue & flag) != 0)
                perms.Add((ChannelPermission) flag);
        }

        return perms;
    }

    /// <summary>
    ///     Creates a <see cref="List{T}"/> of all the <see cref="ChannelPermission"/> values that are denied.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> of all denied <see cref="ChannelPermission"/> flags. If none, the list will be empty.</returns>
    public List<ChannelPermission> ToDenyList()
    {
        List<ChannelPermission> perms = new();
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = ((ulong) 1 << i);
            if ((DenyValue & flag) != 0)
                perms.Add((ChannelPermission) flag);
        }

        return perms;
    }

    public override string ToString() => $"Allow {AllowValue}, Deny {DenyValue}";

    private string DebuggerDisplay =>
        $"Allow {string.Join(", ", ToAllowList())}, " +
        $"Deny {string.Join(", ", ToDenyList())}";
}
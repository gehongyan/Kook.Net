using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a set of permissions for a channel.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct ChannelPermissions
{
    /// <summary> Gets a blank <see cref="ChannelPermissions"/> that grants no permissions.</summary>
    public static readonly ChannelPermissions None = new();

    /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for text channels.</summary>
    public static readonly ChannelPermissions Text = new(0b0_0000_0000_0110_0111_1100_0010_1000);

    /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for voice channels.</summary>
    public static readonly ChannelPermissions Voice = new(0b1_1011_1101_0001_1000_1100_0010_1000);

    /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for category channels.</summary>
    public static readonly ChannelPermissions Category = new(0b1_1011_1101_0111_1111_1100_0010_1000);

    /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for direct message channels.</summary>
    public static readonly ChannelPermissions DM = new(0b0_0000_0000_0100_0101_1000_0000_0000);

    /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for a given channel type.</summary>
    /// <exception cref="ArgumentException">Unknown channel type.</exception>
    public static ChannelPermissions All(IChannel channel) =>
        channel switch
        {
            ITextChannel => Text,
            IVoiceChannel => Voice,
            ICategoryChannel => Category,
            IDMChannel => DM,
            _ => throw new ArgumentException("Unknown channel type.", nameof(channel))
        };

    /// <summary> Gets a packed value representing all the permissions in this <see cref="ChannelPermissions"/>.</summary>
    public ulong RawValue { get; }

    /// <summary> If <see langword="true"/>, a user may create invites. </summary>
    public bool CreateInvites => Permissions.GetValue(RawValue, ChannelPermission.CreateInvites);

    /// <summary> If <see langword="true"/>, a user may view and revoke invites. </summary>
    public bool ManageChannels => Permissions.GetValue(RawValue, ChannelPermission.ManageChannels);

    /// <summary> If <see langword="true"/>, a user may adjust roles. </summary>
    public bool ManageRoles => Permissions.GetValue(RawValue, ChannelPermission.ManageRoles);

    /// <summary> If <see langword="true"/>, a user may view channels. </summary>
    public bool ViewChannel => Permissions.GetValue(RawValue, ChannelPermission.ViewChannel);

    /// <summary> If <see langword="true"/>, a user may send messages. </summary>
    public bool SendMessages => Permissions.GetValue(RawValue, ChannelPermission.SendMessages);

    /// <summary> If <see langword="true"/>, a user may delete messages. </summary>
    public bool ManageMessages => Permissions.GetValue(RawValue, ChannelPermission.ManageMessages);

    /// <summary> If <see langword="true"/>, a user may send files. </summary>
    public bool AttachFiles => Permissions.GetValue(RawValue, ChannelPermission.AttachFiles);

    /// <summary> If <see langword="true"/>, a user may connect to a voice channel. </summary>
    public bool Connect => Permissions.GetValue(RawValue, ChannelPermission.Connect);

    /// <summary> If <see langword="true"/>, a user may kick other users from voice channels, and move other users between voice channels. </summary>
    public bool ManageVoice => Permissions.GetValue(RawValue, ChannelPermission.ManageVoice);

    /// <summary> If <see langword="true"/>, a user may mention all users. </summary>
    public bool MentionEveryone => Permissions.GetValue(RawValue, ChannelPermission.MentionEveryone);

    /// <summary> If <see langword="true"/>, a user may add reactions. </summary>
    public bool AddReactions => Permissions.GetValue(RawValue, ChannelPermission.AddReactions);

    /// <summary> If <see langword="true"/>, a user may connect to a voice channel only when the user is invited or moved by other users. </summary>
    public bool PassiveConnect => Permissions.GetValue(RawValue, ChannelPermission.PassiveConnect);

    /// <summary> If <see langword="true"/>, a user may use voice activation. </summary>
    public bool UseVoiceActivity => Permissions.GetValue(RawValue, ChannelPermission.UseVoiceActivity);

    /// <summary> If <see langword="true"/>, a user may speak in a voice channel. </summary>
    public bool Speak => Permissions.GetValue(RawValue, ChannelPermission.Speak);

    /// <summary> If <see langword="true"/>, a user may deafen users. </summary>
    public bool DeafenMembers => Permissions.GetValue(RawValue, ChannelPermission.DeafenMembers);

    /// <summary> If <see langword="true"/>, a user may mute users. </summary>
    public bool MuteMembers => Permissions.GetValue(RawValue, ChannelPermission.MuteMembers);

    /// <summary> If <see langword="true"/>, a user may play soundtracks in a voice channel. </summary>
    public bool PlaySoundtrack => Permissions.GetValue(RawValue, ChannelPermission.PlaySoundtrack);

    /// <summary> If <see langword="true"/>, a user may share screen in a voice channel. </summary>
    public bool ShareScreen => Permissions.GetValue(RawValue, ChannelPermission.ShareScreen);

    /// <summary> Creates a new <see cref="ChannelPermissions"/> with the provided packed value.</summary>
    public ChannelPermissions(ulong rawValue) => RawValue = rawValue;

    private ChannelPermissions(ulong initialValue,
        bool? createInvites = null,
        bool? manageChannels = null,
        bool? manageRoles = null,
        bool? viewChannel = null,
        bool? sendMessages = null,
        bool? manageMessages = null,
        bool? attachFiles = null,
        bool? connect = null,
        bool? manageVoice = null,
        bool? mentionEveryone = null,
        bool? addReactions = null,
        bool? passiveConnect = null,
        bool? useVoiceActivity = null,
        bool? speak = null,
        bool? deafenMembers = null,
        bool? muteMembers = null,
        bool? playSoundtrack = null,
        bool? shareScreen = null)
    {
        ulong value = initialValue;

        Permissions.SetValue(ref value, createInvites, ChannelPermission.CreateInvites);
        Permissions.SetValue(ref value, manageChannels, ChannelPermission.ManageChannels);
        Permissions.SetValue(ref value, manageRoles, ChannelPermission.ManageRoles);
        Permissions.SetValue(ref value, viewChannel, ChannelPermission.ViewChannel);
        Permissions.SetValue(ref value, sendMessages, ChannelPermission.SendMessages);
        Permissions.SetValue(ref value, manageMessages, ChannelPermission.ManageMessages);
        Permissions.SetValue(ref value, attachFiles, ChannelPermission.AttachFiles);
        Permissions.SetValue(ref value, connect, ChannelPermission.Connect);
        Permissions.SetValue(ref value, manageVoice, ChannelPermission.ManageVoice);
        Permissions.SetValue(ref value, mentionEveryone, ChannelPermission.MentionEveryone);
        Permissions.SetValue(ref value, addReactions, ChannelPermission.AddReactions);
        Permissions.SetValue(ref value, passiveConnect, ChannelPermission.PassiveConnect);
        Permissions.SetValue(ref value, useVoiceActivity, ChannelPermission.UseVoiceActivity);
        Permissions.SetValue(ref value, speak, ChannelPermission.Speak);
        Permissions.SetValue(ref value, deafenMembers, ChannelPermission.DeafenMembers);
        Permissions.SetValue(ref value, muteMembers, ChannelPermission.MuteMembers);
        Permissions.SetValue(ref value, playSoundtrack, ChannelPermission.PlaySoundtrack);
        Permissions.SetValue(ref value, shareScreen, ChannelPermission.ShareScreen);

        RawValue = value;
    }

    /// <summary> Creates a new <see cref="ChannelPermissions"/> with the provided permissions.</summary>
    public ChannelPermissions(
        bool? createInvites = false,
        bool? manageChannels = false,
        bool? manageRoles = false,
        bool? viewChannel = false,
        bool? sendMessages = false,
        bool? manageMessages = false,
        bool? attachFiles = false,
        bool? connect = false,
        bool? manageVoice = false,
        bool? mentionEveryone = false,
        bool? addReactions = false,
        bool? passiveConnect = false,
        bool? useVoiceActivity = false,
        bool? speak = false,
        bool? deafenMembers = false,
        bool? muteMembers = false,
        bool? playSoundtrack = false,
        bool? shareScreen = false)
        : this(0, createInvites, manageChannels, manageRoles, viewChannel, sendMessages, manageMessages, attachFiles,
            connect, manageVoice, mentionEveryone, addReactions, passiveConnect, useVoiceActivity, speak, deafenMembers,
            muteMembers, playSoundtrack, shareScreen)
    {
    }

    /// <summary> Creates a new <see cref="ChannelPermissions"/> from this one, changing the provided non-null permissions.</summary>
    public ChannelPermissions Modify(
        bool? createInvites = null,
        bool? manageChannels = null,
        bool? manageRoles = null,
        bool? viewChannel = null,
        bool? sendMessages = null,
        bool? manageMessages = null,
        bool? attachFiles = null,
        bool? connect = null,
        bool? manageVoice = null,
        bool? mentionEveryone = null,
        bool? addReactions = null,
        bool? passiveConnect = null,
        bool? useVoiceActivity = null,
        bool? speak = null,
        bool? deafenMembers = null,
        bool? muteMembers = null,
        bool? playSoundtrack = null,
        bool? shareScreen = null)
        => new(RawValue,
            createInvites,
            manageChannels,
            manageRoles,
            viewChannel,
            sendMessages,
            manageMessages,
            attachFiles,
            connect,
            manageVoice,
            mentionEveryone,
            addReactions,
            passiveConnect,
            useVoiceActivity,
            speak,
            deafenMembers,
            muteMembers,
            playSoundtrack,
            shareScreen);

    /// <summary>
    ///     Returns a value that indicates if a specific <see cref="ChannelPermission"/> is enabled
    ///     in these permissions.
    /// </summary>
    /// <param name="permission">The permission value to check for.</param>
    /// <returns><see langword="true"/> if the permission is enabled, <c>false</c> otherwise.</returns>
    public bool Has(ChannelPermission permission) => Permissions.GetValue(RawValue, permission);

    /// <summary>
    ///     Returns a <see cref="List{T}"/> containing all of the <see cref="ChannelPermission"/>
    ///     flags that are enabled.
    /// </summary>
    /// <returns>A <see cref="List{T}"/> containing <see cref="ChannelPermission"/> flags. Empty if none are enabled.</returns>
    public List<ChannelPermission> ToList()
    {
        List<ChannelPermission> perms = new();

        // bitwise operations on raw value
        // each of the ChannelPermissions increments by 2^i from 0 to MaxBits
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = (ulong)1 << i;
            if ((RawValue & flag) != 0) perms.Add((ChannelPermission)flag);
        }

        return perms;
    }

    /// <summary>
    ///     Gets the raw value of the permissions.
    /// </summary>
    public override string ToString() => RawValue.ToString();

    private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
}

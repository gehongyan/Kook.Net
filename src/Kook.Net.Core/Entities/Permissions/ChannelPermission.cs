namespace Kook;

/// <summary>
///     Represents a permission can be granted to a channel.
/// </summary>
[Flags]
public enum ChannelPermission : uint
{
    /// <summary>
    ///     Allows creation of invites.
    /// </summary>
    CreateInvites = 1 << 3,
    /// <summary>
    ///     Allows management and editing of channels.
    /// </summary>
    ManageChannels = 1 << 5,
    /// <summary>
    ///     Allows management and editing of roles.
    /// </summary>
    ManageRoles = 1 << 10,
    /// <summary>
    ///     Allows guild members to view a channel, which includes reading messages in text channels.
    /// </summary>
    ViewChannel = 1 << 11,
    /// <summary>
    ///     Allows for sending messages in a channel
    /// </summary>
    SendMessages = 1 << 12,
    /// <summary>
    ///     Allows for deletion of other users messages.
    /// </summary>
    ManageMessages = 1 << 13,
    /// <summary>
    ///     Allows for uploading images and files.
    /// </summary>
    AttachFiles = 1 << 14,
    /// <summary>
    ///     Allows for joining of a voice channel.
    /// </summary>
    Connect = 1 << 15,
    /// <summary>
    ///     Allows for disconnection of members, and moving of members between voice channels.
    /// </summary>
    /// <remarks>
    ///     Such movements are allowed to channels that both the user to be moved and the operator have permissions to.
    /// </remarks>
    ManageVoice = 1 << 16,
    /// <summary>
    ///     Allows for using the everyone mention tag to notify all users in a channel,
    ///     and the online mention tag to notify all online users in a channel.
    /// </summary>
    MentionEveryone = 1 << 17,
    /// <summary>
    ///     Allows for the addition of reactions to messages.
    /// </summary>
    AddReactions = 1 << 18,
    /// <summary>
    ///     Limits the user to connecting to a voice channel only when the user is invited or moved by others.
    /// </summary>
    PassiveConnect = 1 << 20,
    /// <summary>
    ///     Allows for speaking in voice a channel without having to press the speaking key.
    /// </summary>
    UseVoiceActivity = 1 << 22,
    /// <summary>
    ///     Allows for speaking in a voice channel.
    /// </summary>
    Speak = 1 << 23,
    /// <summary>
    ///     Allows for deafening of members in a voice channel.
    /// </summary>
    DeafenMembers = 1 << 24,
    /// <summary>
    ///     Allows for muting members in a voice channel.
    /// </summary>
    MuteMembers = 1 << 25,
    /// <summary>
    ///     Allows for playing soundtracks in a voice channel.
    /// </summary>
    PlaySoundtrack = 1 << 27,
    /// <summary>
    ///     Allows for screen share.
    /// </summary>
    ShareScreen = 1 << 28
}

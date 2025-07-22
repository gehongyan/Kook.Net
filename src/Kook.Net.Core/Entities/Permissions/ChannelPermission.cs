namespace Kook;

/// <summary>
///     表示可以为角色或用户设置的频道级别的权限。
/// </summary>
[Flags]
public enum ChannelPermission : uint
{
    /// <inheritdoc cref="Kook.GuildPermission.CreateInvites" />
    CreateInvites = 1 << 3,

    /// <inheritdoc cref="Kook.GuildPermission.ManageChannels" />
    ManageChannels = 1 << 5,

    /// <inheritdoc cref="Kook.GuildPermission.ManageRoles" />
    ManageRoles = 1 << 10,

    /// <inheritdoc cref="Kook.GuildPermission.ViewChannel" />
    ViewChannel = 1 << 11,

    /// <inheritdoc cref="Kook.GuildPermission.SendMessages" />
    SendMessages = 1 << 12,

    /// <inheritdoc cref="Kook.GuildPermission.ManageMessages" />
    ManageMessages = 1 << 13,

    /// <inheritdoc cref="Kook.GuildPermission.AttachFiles" />
    AttachFiles = 1 << 14,

    /// <inheritdoc cref="Kook.GuildPermission.Connect" />
    Connect = 1 << 15,

    /// <inheritdoc cref="Kook.GuildPermission.ManageVoice" />
    ManageVoice = 1 << 16,

    /// <inheritdoc cref="Kook.GuildPermission.MentionEveryone" />
    MentionEveryone = 1 << 17,

    /// <inheritdoc cref="Kook.GuildPermission.AddReactions" />
    AddReactions = 1 << 18,

    /// <inheritdoc cref="Kook.GuildPermission.PassiveConnect" />
    PassiveConnect = 1 << 20,

    /// <inheritdoc cref="Kook.GuildPermission.UseVoiceActivity" />
    UseVoiceActivity = 1 << 22,

    /// <inheritdoc cref="Kook.GuildPermission.Speak" />
    Speak = 1 << 23,

    /// <inheritdoc cref="Kook.GuildPermission.DeafenMembers" />
    DeafenMembers = 1 << 24,

    /// <inheritdoc cref="Kook.GuildPermission.MuteMembers" />
    MuteMembers = 1 << 25,

    /// <inheritdoc cref="Kook.GuildPermission.PlaySoundtrack" />
    PlaySoundtrack = 1 << 27,

    /// <inheritdoc cref="Kook.GuildPermission.ShareScreen" />
    ShareScreen = 1 << 28,

    /// <inheritdoc cref="Kook.GuildPermission.ReplyToPost" />
    ReplyToPost = 1 << 29,

    /// <inheritdoc cref="Kook.GuildPermission.RecordAudio" />
    RecordAudio = 1 << 30,
}

namespace Kook;

/// <summary>
///     表示可以为角色或用户设置的频道级别的权限。
/// </summary>
[Flags]
public enum ChannelPermission : uint
{
    /// <inheritdoc cref="F:Kook.GuildPermission.CreateInvites" />
    CreateInvites = 1 << 3,

    /// <inheritdoc cref="F:Kook.GuildPermission.ManageChannels" />
    ManageChannels = 1 << 5,

    /// <inheritdoc cref="F:Kook.GuildPermission.ManageRoles" />
    ManageRoles = 1 << 10,

    /// <inheritdoc cref="F:Kook.GuildPermission.ViewChannel" />
    ViewChannel = 1 << 11,

    /// <inheritdoc cref="F:Kook.GuildPermission.SendMessages" />
    SendMessages = 1 << 12,

    /// <inheritdoc cref="F:Kook.GuildPermission.ManageMessages" />
    ManageMessages = 1 << 13,

    /// <inheritdoc cref="F:Kook.GuildPermission.AttachFiles" />
    AttachFiles = 1 << 14,

    /// <inheritdoc cref="F:Kook.GuildPermission.Connect" />
    Connect = 1 << 15,

    /// <inheritdoc cref="F:Kook.GuildPermission.ManageVoice" />
    ManageVoice = 1 << 16,

    /// <inheritdoc cref="F:Kook.GuildPermission.MentionEveryone" />
    MentionEveryone = 1 << 17,

    /// <inheritdoc cref="F:Kook.GuildPermission.AddReactions" />
    AddReactions = 1 << 18,

    /// <inheritdoc cref="F:Kook.GuildPermission.PassiveConnect" />
    PassiveConnect = 1 << 20,

    /// <inheritdoc cref="F:Kook.GuildPermission.UseVoiceActivity" />
    UseVoiceActivity = 1 << 22,

    /// <inheritdoc cref="F:Kook.GuildPermission.Speak" />
    Speak = 1 << 23,

    /// <inheritdoc cref="F:Kook.GuildPermission.DeafenMembers" />
    DeafenMembers = 1 << 24,

    /// <inheritdoc cref="F:Kook.GuildPermission.MuteMembers" />
    MuteMembers = 1 << 25,

    /// <inheritdoc cref="F:Kook.GuildPermission.PlaySoundtrack" />
    PlaySoundtrack = 1 << 27,

    /// <inheritdoc cref="F:Kook.GuildPermission.ShareScreen" />
    ShareScreen = 1 << 28
}

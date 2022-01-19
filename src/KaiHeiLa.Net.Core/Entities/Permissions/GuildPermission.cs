namespace KaiHeiLa;

[Flags]
public enum GuildPermission : uint
{
    Administrator        = 1 << 0,
    ManageGuild          = 1 << 1,
    ViewAuditLog         = 1 << 2,
    CreateInvites        = 1 << 3,
    ManageInvites        = 1 << 4,
    ManageChannels       = 1 << 5,
    KickMembers          = 1 << 6,
    BanMembers           = 1 << 7,
    ManageEmojis         = 1 << 8,
    ChangeNickname       = 1 << 9,
    ManageRoles          = 1 << 10,
    ViewChannels         = 1 << 11,
    SendMessages         = 1 << 12,
    ManageMessages       = 1 << 13,
    AttachFiles          = 1 << 14,
    Connect              = 1 << 15,
    ManageVoice          = 1 << 16,
    MentionEveryone      = 1 << 17,
    AddReactions         = 1 << 18,
    FollowReactions      = 1 << 19,
    PassiveConnect       = 1 << 20,
    OnlyPressToTalk      = 1 << 21,
    UseVoiceActivity     = 1 << 22,
    Speak                = 1 << 23,
    DeafenMembers        = 1 << 24,
    MuteMembers          = 1 << 25,
    ManageNicknames      = 1 << 26,
    PlaySoundtrack       = 1 << 27
}
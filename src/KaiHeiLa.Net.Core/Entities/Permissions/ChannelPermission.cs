namespace KaiHeiLa;

[Flags]
public enum ChannelPermission : uint
{
    CreateInvites        = 1 << 3,
    ManageChannels       = 1 << 5,
    ManageRoles          = 1 << 10,
    ViewChannels         = 1 << 11,
    SendMessages         = 1 << 12,
    ManageMessages       = 1 << 13,
    AttachFiles          = 1 << 14,
    Connect              = 1 << 15,
    ManageVoice          = 1 << 16,
    MentionEveryone      = 1 << 17,
    AddReactions         = 1 << 18,
    PassiveConnect       = 1 << 20,
    UseVoiceActivity     = 1 << 22,
    Speak                = 1 << 23,
    DeafenMembers        = 1 << 24,
    MuteMembers          = 1 << 25,
    PlaySoundtrack       = 1 << 27
}
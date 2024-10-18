namespace Kook;

/// <summary>
///     表示可以为角色或用户设置的服务器级别的服务器权限。
/// </summary>
[Flags]
public enum GuildPermission : uint
{
    /// <summary>
    ///     管理员。
    /// </summary>
    /// <remarks>
    ///     拥有此权限会获得完整的管理权，不受所有其他权限的限制。
    /// </remarks>
    Administrator = 1 << 0,

    /// <summary>
    ///     管理服务器。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以修改服务器名称、头像和区域等设置项，以及未明确的概况设置项，并对服务器安全进行设置。
    /// </remarks>
    ManageGuild = 1 << 1,

    /// <summary>
    ///     查看管理日志。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以查看服务器的管理日志。
    /// </remarks>
    ViewAuditLog = 1 << 2,

    /// <summary>
    ///     创建邀请。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以创建邀请链接。
    /// </remarks>
    CreateInvites = 1 << 3,

    /// <summary>
    ///     管理邀请。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以管理服务器邀请。
    /// </remarks>
    ManageInvites = 1 << 4,

    /// <summary>
    ///     频道管理。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以创建新的频道以及编辑或删除已存在的频道。
    /// </remarks>
    ManageChannels = 1 << 5,

    /// <summary>
    ///     踢出成员。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以踢出其他成员。
    /// </remarks>
    KickMembers = 1 << 6,

    /// <summary>
    ///     加入服务器黑名单。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以将其他成员加入服务器黑名单、查看服务器黑名单。
    /// </remarks>
    BanMembers = 1 << 7,

    /// <summary>
    ///     管理自定义表情。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以管理自定义表情。
    /// </remarks>
    ManageEmojis = 1 << 8,

    /// <summary>
    ///     修改昵称。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的用户可以更改自己的昵称。
    /// </remarks>
    ChangeNickname = 1 << 9,

    /// <summary>
    ///     管理角色权限。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以创建新的角色，编辑或删除位次低于该角色的角色。
    /// </remarks>
    ManageRoles = 1 << 10,

    /// <summary>
    ///     查看文字与语音频道。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以查看文字、语音频道。
    /// </remarks>
    ViewChannel = 1 << 11,

    /// <summary>
    ///     发送文字消息。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以在文字频道中发送消息。
    /// </remarks>
    SendMessages = 1 << 12,

    /// <summary>
    ///     消息管理。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以删除其他成员发出的消息和置顶消息。
    /// </remarks>
    ManageMessages = 1 << 13,

    /// <summary>
    ///     上传文件。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以上传文件（包括图片）。
    /// </remarks>
    AttachFiles = 1 << 14,

    /// <summary>
    ///     语音连接。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以连接到语音频道。
    /// </remarks>
    Connect = 1 << 15,

    /// <summary>
    ///     语音管理。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可修改频道发言模式，管理频道成员上麦，将频道成员转移至其他频道和踢出频道。
    /// </remarks>
    ManageVoice = 1 << 16,

    /// <summary>
    ///     提及全体成员、在线成员和所有角色。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以使用@全体成员，@在线成员提及该频道中的所有成员，该权限可以绕开"允许任何人@提及此角色"的限制。
    /// </remarks>
    MentionEveryone = 1 << 17,

    /// <summary>
    ///     添加回应。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以对消息添加新的回应。
    /// </remarks>
    AddReactions = 1 << 18,

    /// <summary>
    ///     跟随添加回应。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以跟随使用已经添加的回应。
    /// </remarks>
    FollowReactions = 1 << 19,

    /// <summary>
    ///     被动连接语音频道。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员在没有语音连接权限时，可以被动邀请或被人移动进入语音频道。
    /// </remarks>
    PassiveConnect = 1 << 20,

    /// <summary>
    ///     仅使用按键说话。
    /// </summary>
    /// <remarks>
    ///     拥有此限制的成员加入语音频道后，只能使用按键说话。
    /// </remarks>
    OnlyPushToTalk = 1 << 21,

    /// <summary>
    ///     使用自由麦。
    /// </summary>
    /// <remarks>
    ///     没有此权限的成员，必须在频道内使用按键说话。
    /// </remarks>
    UseVoiceActivity = 1 << 22,

    /// <summary>
    ///     发言。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以在语音频道中发言。
    /// </remarks>
    Speak = 1 << 23,

    /// <summary>
    ///     服务器静音。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以限制其他成员在服务器中的语音接收。
    /// </remarks>
    DeafenMembers = 1 << 24,

    /// <summary>
    ///     服务器闭麦。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可以限制其他成员在语音频道中发言和共享计算机音频。
    /// </remarks>
    MuteMembers = 1 << 25,

    /// <summary>
    ///     修改他人昵称。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的用户可以更改他人的昵称。
    /// </remarks>
    ManageNicknames = 1 << 26,

    /// <summary>
    ///     共享计算机音频。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可在语音频道中共享计算机音频。
    /// </remarks>
    PlaySoundtrack = 1 << 27,

    /// <summary>
    ///     屏幕分享。
    /// </summary>
    /// <remarks>
    ///     拥有此权限的成员可在语音频道中共享计算机画面。
    /// </remarks>
    ShareScreen = 1 << 28
}

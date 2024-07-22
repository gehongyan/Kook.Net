using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个服务器的权限集。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct GuildPermissions
{
    /// <summary>
    ///     获取一个空的 <see cref="GuildPermissions"/>，不包含任何权限。
    /// </summary>
    public static readonly GuildPermissions None = new();

    /// <summary>
    ///     获取一个包含所有可以为服务器设置的权限的 <see cref="GuildPermissions"/>。
    /// </summary>
    public static readonly GuildPermissions All = new(0b1_1111_1111_1111_1111_1111_1111_1111);

    /// <summary>
    ///     获取此权限集的原始值。
    /// </summary>
    public ulong RawValue { get; }

    /// <summary>
    ///     获取此权限集的相关用户是否为服务器管理员。
    /// </summary>
    public bool Administrator => Permissions.GetValue(RawValue, GuildPermission.Administrator);

    /// <summary>
    ///     获取此权限集是否允许相关用户管理服务器。
    /// </summary>
    public bool ManageGuild => Permissions.GetValue(RawValue, GuildPermission.ManageGuild);

    /// <summary>
    ///     获取此权限集是否允许相关用户查看管理日志。
    /// </summary>
    public bool ViewAuditLog => Permissions.GetValue(RawValue, GuildPermission.ViewAuditLog);

    /// <summary>
    ///     获取此权限集是否允许相关用户创建邀请。
    /// </summary>
    public bool CreateInvites => Permissions.GetValue(RawValue, GuildPermission.CreateInvites);

    /// <summary>
    ///     获取此权限集是否允许相关用户管理邀请。
    /// </summary>
    public bool ManageInvites => Permissions.GetValue(RawValue, GuildPermission.ManageInvites);

    /// <summary>
    ///     获取此权限集是否允许相关用户管理频道。
    /// </summary>
    public bool ManageChannels => Permissions.GetValue(RawValue, GuildPermission.ManageChannels);

    /// <summary>
    ///     获取此权限集是否允许相关用户踢出其他用户。
    /// </summary>
    public bool KickMembers => Permissions.GetValue(RawValue, GuildPermission.KickMembers);

    /// <summary>
    ///     获取此权限集是否允许相关用户封禁其他用户。
    /// </summary>
    public bool BanMembers => Permissions.GetValue(RawValue, GuildPermission.BanMembers);

    /// <summary>
    ///     获取此权限集是否允许相关用户管理自定义表情。
    /// </summary>
    public bool ManageEmojis => Permissions.GetValue(RawValue, GuildPermission.ManageEmojis);

    /// <summary>
    ///     获取此权限集是否允许相关用户修改昵称。
    /// </summary>
    public bool ChangeNickname => Permissions.GetValue(RawValue, GuildPermission.ChangeNickname);

    /// <summary>
    ///     获取此权限集是否允许相关用户管理角色。
    /// </summary>
    public bool ManageRoles => Permissions.GetValue(RawValue, GuildPermission.ManageRoles);

    /// <summary>
    ///     获取此权限集是否允许相关用户查看文字与语音频道。
    /// </summary>
    public bool ViewChannel => Permissions.GetValue(RawValue, GuildPermission.ViewChannel);

    /// <summary>
    ///     获取此权限集是否允许相关用户发送文字消息。
    /// </summary>
    public bool SendMessages => Permissions.GetValue(RawValue, GuildPermission.SendMessages);

    /// <summary>
    ///     获取此权限集是否允许相关用户管理消息。
    /// </summary>
    public bool ManageMessages => Permissions.GetValue(RawValue, GuildPermission.ManageMessages);

    /// <summary>
    ///     获取此权限集是否允许相关用户上传文件。
    /// </summary>
    public bool AttachFiles => Permissions.GetValue(RawValue, GuildPermission.AttachFiles);

    /// <summary>
    ///     获取此权限集是否允许相关用户连接语音。
    /// </summary>
    public bool Connect => Permissions.GetValue(RawValue, GuildPermission.Connect);

    /// <summary>
    ///     获取此权限集是否允许相关用户管理语音频道。
    /// </summary>
    public bool ManageVoice => Permissions.GetValue(RawValue, GuildPermission.ManageVoice);

    /// <summary>
    ///     获取此权限集是否允许相关用户提及全体成员、在线成员和所有角色。
    /// </summary>
    public bool MentionEveryone => Permissions.GetValue(RawValue, GuildPermission.MentionEveryone);

    /// <summary>
    ///     获取此权限集是否允许相关用户添加回应。
    /// </summary>
    public bool AddReactions => Permissions.GetValue(RawValue, GuildPermission.AddReactions);

    /// <summary>
    ///     获取此权限集是否允许相关用户跟随添加回应。
    /// </summary>
    public bool FollowReactions => Permissions.GetValue(RawValue, GuildPermission.FollowReactions);

    /// <summary>
    ///     获取此权限集是否允许相关用户被动连接到语音频道。
    /// </summary>
    public bool PassiveConnect => Permissions.GetValue(RawValue, GuildPermission.PassiveConnect);

    /// <summary>
    ///     获取此权限集是否要求相关用户仅可使用按键说话。
    /// </summary>
    public bool OnlyPushToTalk => Permissions.GetValue(RawValue, GuildPermission.OnlyPushToTalk);

    /// <summary>
    ///     获取此权限集是否允许相关用户使用自由麦。
    /// </summary>
    public bool UseVoiceActivity => Permissions.GetValue(RawValue, GuildPermission.UseVoiceActivity);

    /// <summary>
    ///     获取此权限集是否允许相关用户在语音频道中发言。
    /// </summary>
    public bool Speak => Permissions.GetValue(RawValue, GuildPermission.Speak);

    /// <summary>
    ///     获取此权限集是否允许相关用户使其他用户被服务器静音。
    /// </summary>
    public bool DeafenMembers => Permissions.GetValue(RawValue, GuildPermission.DeafenMembers);

    /// <summary>
    ///     获取此权限集是否允许相关用户使其他用户被服务器闭麦。
    /// </summary>
    public bool MuteMembers => Permissions.GetValue(RawValue, GuildPermission.MuteMembers);

    /// <summary>
    ///     获取此权限集是否允许相关用户修改他人昵称。
    /// </summary>
    public bool ManageNicknames => Permissions.GetValue(RawValue, GuildPermission.ManageNicknames);

    /// <summary>
    ///     获取此权限集是否允许相关用户共享计算机音频。
    /// </summary>
    public bool PlaySoundtrack => Permissions.GetValue(RawValue, GuildPermission.PlaySoundtrack);

    /// <summary>
    ///     获取此权限集是否允许相关用户共享计算机画面。
    /// </summary>
    public bool ShareScreen => Permissions.GetValue(RawValue, GuildPermission.ShareScreen);

    /// <summary>
    ///     使用指定的权限原始值创建一个 <see cref="GuildPermissions"/> 的新实例。
    /// </summary>
    /// <param name="rawValue"> 权限原始值。 </param>
    public GuildPermissions(ulong rawValue)
    {
        RawValue = rawValue;
    }

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

    /// <summary>
    ///     使用指定的权限位信息创建一个 <see cref="GuildPermissions"/> 的新实例。
    /// </summary>
    /// <param name="administrator"> 管理员。 </param>
    /// <param name="manageGuild"> 管理服务器。 </param>
    /// <param name="viewAuditLog"> 查看管理日志。 </param>
    /// <param name="createInvites"> 创建邀请。 </param>
    /// <param name="manageInvites"> 管理邀请。 </param>
    /// <param name="manageChannels"> 频道管理。 </param>
    /// <param name="kickMembers"> 踢出成员。 </param>
    /// <param name="banMembers"> 加入服务器黑名单。 </param>
    /// <param name="manageEmojis"> 管理自定义表情。 </param>
    /// <param name="changeNickname"> 修改昵称。 </param>
    /// <param name="manageRoles"> 管理角色权限。 </param>
    /// <param name="viewChannel"> 查看文字与语音频道。 </param>
    /// <param name="sendMessages"> 发送文字消息。 </param>
    /// <param name="manageMessages"> 消息管理。 </param>
    /// <param name="attachFiles"> 上传文件。 </param>
    /// <param name="connect"> 语音连接。 </param>
    /// <param name="manageVoice"> 语音管理。 </param>
    /// <param name="mentionEveryone"> 提及全体成员、在线成员和所有角色。 </param>
    /// <param name="addReactions"> 添加回应。 </param>
    /// <param name="followReactions"> 跟随添加回应。 </param>
    /// <param name="passiveConnect"> 被动连接语音频道。 </param>
    /// <param name="onlyPushToTalk"> 仅使用按键说话。 </param>
    /// <param name="useVoiceActivity"> 使用自由麦。 </param>
    /// <param name="speak"> 发言。 </param>
    /// <param name="deafenMembers"> 服务器静音。 </param>
    /// <param name="muteMembers"> 服务器闭麦。 </param>
    /// <param name="manageNicknames"> 修改他人昵称。 </param>
    /// <param name="playSoundtrack"> 共享计算机音频。 </param>
    /// <param name="shareScreen"> 屏幕分享。 </param>
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

    /// <summary>
    ///     以当前权限集为基础，更改指定的权限，返回一个 <see cref="GuildPermissions"/> 的新实例。
    /// </summary>
    /// <param name="administrator"> 管理员。 </param>
    /// <param name="manageGuild"> 管理服务器。 </param>
    /// <param name="viewAuditLog"> 查看管理日志。 </param>
    /// <param name="createInvites"> 创建邀请。 </param>
    /// <param name="manageInvites"> 管理邀请。 </param>
    /// <param name="manageChannels"> 频道管理。 </param>
    /// <param name="kickMembers"> 踢出成员。 </param>
    /// <param name="banMembers"> 加入服务器黑名单。 </param>
    /// <param name="manageEmojis"> 管理自定义表情。 </param>
    /// <param name="changeNickname"> 修改昵称。 </param>
    /// <param name="manageRoles"> 管理角色权限。 </param>
    /// <param name="viewChannel"> 查看文字与语音频道。 </param>
    /// <param name="sendMessages"> 发送文字消息。 </param>
    /// <param name="manageMessages"> 消息管理。 </param>
    /// <param name="attachFiles"> 上传文件。 </param>
    /// <param name="connect"> 语音连接。 </param>
    /// <param name="manageVoice"> 语音管理。 </param>
    /// <param name="mentionEveryone"> 提及全体成员、在线成员和所有角色。 </param>
    /// <param name="addReactions"> 添加回应。 </param>
    /// <param name="followReactions"> 跟随添加回应。 </param>
    /// <param name="passiveConnect"> 被动连接语音频道。 </param>
    /// <param name="onlyPushToTalk"> 仅使用按键说话。 </param>
    /// <param name="useVoiceActivity"> 使用自由麦。 </param>
    /// <param name="speak"> 发言。 </param>
    /// <param name="deafenMembers"> 服务器静音。 </param>
    /// <param name="muteMembers"> 服务器闭麦。 </param>
    /// <param name="manageNicknames"> 修改他人昵称。 </param>
    /// <param name="playSoundtrack"> 共享计算机音频。 </param>
    /// <param name="shareScreen"> 屏幕分享。 </param>
    /// <returns> 更改了指定权限的新的权限集。 </returns>
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
        bool? shareScreen = null) =>
        new(RawValue, administrator, manageGuild, viewAuditLog, createInvites, manageInvites,
            manageChannels, kickMembers, banMembers, manageEmojis, changeNickname, manageRoles, viewChannel,
            sendMessages, manageMessages, attachFiles, connect, manageVoice, mentionEveryone, addReactions,
            followReactions, passiveConnect, onlyPushToTalk, useVoiceActivity, speak, deafenMembers, muteMembers,
            manageNicknames, playSoundtrack, shareScreen);

    /// <summary>
    ///     获取当前权限集是否包含指定的权限。
    /// </summary>
    /// <param name="permission"> 要检查的权限。 </param>
    /// <returns> 如果当前权限集包含了所有指定的权限信息，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public bool Has(GuildPermission permission) => Permissions.GetValue(RawValue, permission);

    /// <summary>
    ///     获取一个包含当前权限集所包含的所有已设置的 <see cref="GuildPermission"/> 独立位标志枚举值的集合。
    /// </summary>
    /// <returns> 一个包含当前权限集所包含的所有已设置的 <see cref="GuildPermission"/> 独立位标志枚举值的集合；如果当前权限集未包含任何已设置的权限位，则会返回一个空集合。 </returns>
    public List<GuildPermission> ToList()
    {
        List<GuildPermission> perms = [];

        // bitwise operations on raw value
        // each of the GuildPermissions increments by 2^i from 0 to MaxBits
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = (ulong)1 << i;
            if ((RawValue & flag) != 0)
                perms.Add((GuildPermission)flag);
        }

        return perms;
    }

    internal void Ensure(GuildPermission permissions)
    {
        if (!Has(permissions))
        {
            IEnumerable<GuildPermission> vals = Enum
                .GetValues(typeof(GuildPermission))
                .Cast<GuildPermission>();
            ulong currentValues = RawValue;
            IEnumerable<GuildPermission> missingValues = vals
                .Where(x => permissions.HasFlag(x) && !Permissions.GetValue(currentValues, x))
                .ToList();

            throw new InvalidOperationException(
                $"Missing required guild permission{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
        }
    }

    /// <summary>
    ///     获取此权限集原始值的字符串表示。
    /// </summary>
    /// <returns> 此权限集原始值的字符串表示。 </returns>
    public override string ToString() => RawValue.ToString();

    private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
}

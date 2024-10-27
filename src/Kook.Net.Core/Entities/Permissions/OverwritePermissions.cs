using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一组权限重写配置。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct OverwritePermissions
{
    /// <summary>
    ///     获取一个空的 <see cref="OverwritePermissions"/>，继承所有权限。
    /// </summary>
    public static OverwritePermissions InheritAll { get; } = new();

    /// <summary>
    ///     获取一个在权限重写配置中为指定频道重写允许所有权限的 <see cref="OverwritePermissions"/>。
    /// </summary>
    /// <exception cref="ArgumentException"> 未知的频道类型。 </exception>
    public static OverwritePermissions AllowAll(IChannel channel) =>
        new(ChannelPermissions.All(channel).RawValue, 0);

    /// <summary>
    ///     获取一个在权限重写配置中为指定频道重写禁止所有权限的 <see cref="OverwritePermissions"/>。
    /// </summary>
    /// <exception cref="ArgumentException"> 未知的频道类型。 </exception>
    public static OverwritePermissions DenyAll(IChannel channel) =>
        new(0, ChannelPermissions.All(channel).RawValue);

    /// <summary>
    ///     获取一个表示此重写中所有允许的权限的原始值。
    /// </summary>
    public ulong AllowValue { get; }

    /// <summary>
    ///     获取一个表示此重写中所有禁止的权限的原始值。
    /// </summary>
    public ulong DenyValue { get; }

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.CreateInvites"/> 的重写配置。
    /// </summary>
    public PermValue CreateInvites => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.CreateInvites);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.ManageChannels"/> 的重写配置。
    /// </summary>
    public PermValue ManageChannels => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageChannels);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.ManageRoles"/> 的重写配置。
    /// </summary>
    public PermValue ManageRoles => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageRoles);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.ViewChannel"/> 的重写配置。
    /// </summary>
    public PermValue ViewChannel => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ViewChannel);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.SendMessages"/> 的重写配置。
    /// </summary>
    public PermValue SendMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.SendMessages);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.ManageMessages"/> 的重写配置。
    /// </summary>
    public PermValue ManageMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageMessages);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.AttachFiles"/> 的重写配置。
    /// </summary>
    public PermValue AttachFiles => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.AttachFiles);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.Connect"/> 的重写配置。
    /// </summary>
    public PermValue Connect => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.Connect);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.ManageVoice"/> 的重写配置。
    /// </summary>
    public PermValue ManageVoice => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageVoice);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.MentionEveryone"/> 的重写配置。
    /// </summary>
    public PermValue MentionEveryone => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MentionEveryone);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.AddReactions"/> 的重写配置。
    /// </summary>
    public PermValue AddReactions => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.AddReactions);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.PassiveConnect"/> 的重写配置。
    /// </summary>
    public PermValue PassiveConnect => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.PassiveConnect);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.UseVoiceActivity"/> 的重写配置。
    /// </summary>
    public PermValue UseVoiceActivity => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.UseVoiceActivity);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.Speak"/> 的重写配置。
    /// </summary>
    public PermValue Speak => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.Speak);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.DeafenMembers"/> 的重写配置。
    /// </summary>
    public PermValue DeafenMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.DeafenMembers);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.MuteMembers"/> 的重写配置。
    /// </summary>
    public PermValue MuteMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MuteMembers);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.PlaySoundtrack"/> 的重写配置。
    /// </summary>
    public PermValue PlaySoundtrack => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.PlaySoundtrack);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.ShareScreen"/> 的重写配置。
    /// </summary>
    public PermValue ShareScreen => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ShareScreen);

    /// <summary>
    ///     获取此权限重写配置对频道权限位 <see cref="Kook.ChannelPermission.ReplyToPost"/> 的重写配置。
    /// </summary>
    public PermValue ReplyToPost => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ReplyToPost);

    /// <summary>
    ///     使用指定的原始值初始化一个 <see cref="OverwritePermissions"/> 结构的新实例。
    /// </summary>
    /// <param name="allowValue"> 重写允许的权限的原始值。 </param>
    /// <param name="denyValue"> 重写禁止的权限的原始值。 </param>
    public OverwritePermissions(ulong allowValue, ulong denyValue)
    {
        AllowValue = allowValue;
        DenyValue = denyValue;
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
        PermValue? playSoundtrack = null,
        PermValue? shareScreen = null,
        PermValue? replyToPost = null)
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
        Permissions.SetValue(ref allowValue, ref denyValue, shareScreen, ChannelPermission.ShareScreen);
        Permissions.SetValue(ref allowValue, ref denyValue, replyToPost, ChannelPermission.ReplyToPost);

        AllowValue = allowValue;
        DenyValue = denyValue;
    }

    /// <summary>
    ///     使用指定的权限重写信息创建一个 <see cref="OverwritePermissions"/> 结构的新实例。
    /// </summary>
    /// <param name="createInvites"> 创建邀请。 </param>
    /// <param name="manageChannels"> 频道管理。 </param>
    /// <param name="manageRoles"> 管理角色权限。 </param>
    /// <param name="viewChannel"> 查看文字与语音频道。 </param>
    /// <param name="sendMessages"> 发送文字消息。 </param>
    /// <param name="manageMessages"> 消息管理。 </param>
    /// <param name="attachFiles"> 上传文件。 </param>
    /// <param name="connect"> 语音连接。 </param>
    /// <param name="manageVoice"> 语音管理。 </param>
    /// <param name="mentionEveryone"> 提及全体成员、在线成员和所有角色。 </param>
    /// <param name="addReactions"> 添加回应。 </param>
    /// <param name="passiveConnect"> 被动连接语音频道。 </param>
    /// <param name="useVoiceActivity"> 使用自由麦。 </param>
    /// <param name="speak"> 发言。 </param>
    /// <param name="deafenMembers"> 服务器静音。 </param>
    /// <param name="muteMembers"> 服务器闭麦。 </param>
    /// <param name="playSoundtrack"> 共享计算机音频。 </param>
    /// <param name="shareScreen"> 屏幕分享。 </param>
    /// <param name="replyToPost"> 发布帖子回复。 </param>
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
        PermValue playSoundtrack = PermValue.Inherit,
        PermValue shareScreen = PermValue.Inherit,
        PermValue replyToPost = PermValue.Inherit)
        : this(0, 0, createInvites, manageChannels, manageRoles, viewChannel, sendMessages, manageMessages,
            attachFiles, connect, manageVoice, mentionEveryone, addReactions, passiveConnect, useVoiceActivity, speak,
            deafenMembers, muteMembers, playSoundtrack, shareScreen, replyToPost)
    {
    }

    /// <summary>
    ///     以当前权限重写配置为基础，更改指定的重写，返回一个 <see cref="OverwritePermissions"/> 结构的新实例。
    /// </summary>
    /// <param name="createInvites"> 创建邀请。 </param>
    /// <param name="manageChannels"> 频道管理。 </param>
    /// <param name="manageRoles"> 管理角色权限。 </param>
    /// <param name="viewChannel"> 查看文字与语音频道。 </param>
    /// <param name="sendMessages"> 发送文字消息。 </param>
    /// <param name="manageMessages"> 消息管理。 </param>
    /// <param name="attachFiles"> 上传文件。 </param>
    /// <param name="connect"> 语音连接。 </param>
    /// <param name="manageVoice"> 语音管理。 </param>
    /// <param name="mentionEveryone"> 提及全体成员、在线成员和所有角色。 </param>
    /// <param name="addReactions"> 添加回应。 </param>
    /// <param name="passiveConnect"> 被动连接语音频道。 </param>
    /// <param name="useVoiceActivity"> 使用自由麦。 </param>
    /// <param name="speak"> 发言。 </param>
    /// <param name="deafenMembers"> 服务器静音。 </param>
    /// <param name="muteMembers"> 服务器闭麦。 </param>
    /// <param name="playSoundtrack"> 共享计算机音频。 </param>
    /// <param name="shareScreen"> 屏幕分享。 </param>
    /// <param name="replyToPost"> 发布帖子回复。 </param>
    /// <returns> 更改了指定权限的新的权限集。 </returns>
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
        PermValue? playSoundtrack = null,
        PermValue? shareScreen = null,
        PermValue? replyToPost = null) =>
        new(AllowValue, DenyValue, createInvites, manageChannels, manageRoles, viewChannel, sendMessages,
            manageMessages, attachFiles, connect, manageVoice, mentionEveryone, addReactions, passiveConnect,
            useVoiceActivity, speak, deafenMembers, muteMembers, playSoundtrack, shareScreen, replyToPost);

    /// <summary>
    ///     获取一个包含当前权限重写配置所包含的所有重写允许的 <see cref="ChannelPermission"/> 独立位标志枚举值的集合。
    /// </summary>
    /// <returns> 一个包含当前权限重写配置所包含的所有重写允许的 <see cref="ChannelPermission"/> 独立位标志枚举值的集合；如果当前权限重写配置未包含任何重写允许的权限位，则会返回一个空集合。 </returns>
    public List<ChannelPermission> ToAllowList()
    {
        List<ChannelPermission> perms = [];
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            // first operand must be long or ulong to shift >31 bits
            ulong flag = (ulong)1 << i;
            if ((AllowValue & flag) != 0)
                perms.Add((ChannelPermission)flag);
        }

        return perms;
    }

    /// <summary>
    ///     获取一个包含当前权限重写配置所包含的所有重写禁止的 <see cref="ChannelPermission"/> 独立位标志枚举值的集合。
    /// </summary>
    /// <returns> 一个包含当前权限重写配置所包含的所有重写禁止的 <see cref="ChannelPermission"/> 独立位标志枚举值的集合；如果当前权限重写配置未包含任何重写禁止的权限位，则会返回一个空集合。 </returns>
    public List<ChannelPermission> ToDenyList()
    {
        List<ChannelPermission> perms = new();
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = (ulong)1 << i;
            if ((DenyValue & flag) != 0)
                perms.Add((ChannelPermission)flag);
        }

        return perms;
    }

    /// <summary>
    ///     获取此权限重写配置所重写允许与重写禁止的权限的原始值的字符串表示。
    /// </summary>
    /// <returns> 此权限重写配置所重写允许与重写禁止的权限的原始值的字符串表示。 </returns>
    public override string ToString() => $"Allow {AllowValue}, Deny {DenyValue}";

    private string DebuggerDisplay =>
        $"Allow {string.Join(", ", ToAllowList())}, " + $"Deny {string.Join(", ", ToDenyList())}";
}

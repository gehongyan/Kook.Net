using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个频道的权限集。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct ChannelPermissions
{
    /// <summary>
    ///     获取一个空的 <see cref="ChannelPermissions"/>，不包含任何权限。
    /// </summary>
    public static readonly ChannelPermissions None = new();

    /// <summary>
    ///     获取一个包含所有可以为文字频道设置的权限的 <see cref="ChannelPermissions"/>。
    /// </summary>
    public static readonly ChannelPermissions Text = new(0b00_0000_0000_0110_0111_1100_0010_1000);

    /// <summary>
    ///     获取一个包含所有可以为语音频道设置的权限的 <see cref="ChannelPermissions"/>。
    /// </summary>
    public static readonly ChannelPermissions Voice = new(0b01_1011_1101_0111_1111_1100_0010_1000);

    /// <summary>
    ///     获取一个包含所有可以为分组频道设置的权限的 <see cref="ChannelPermissions"/>。
    /// </summary>
    public static readonly ChannelPermissions Category = new(0b11_1011_1101_0111_1111_1100_0010_1000);

    /// <summary>
    ///     获取一个包含所有可以为私聊频道设置的权限的 <see cref="ChannelPermissions"/>。
    /// </summary>
    public static readonly ChannelPermissions DM = new(0b00_0000_0000_0100_0101_1000_0000_0000);

    /// <summary>
    ///     获取一个包含所有可以为帖子频道设权限的 <see cref="ChannelPermissions"/>。
    /// </summary>
    public static readonly ChannelPermissions Thread = new(0b10_0000_0000_0110_0111_1100_0010_1000);

    /// <summary>
    ///     为指定的频道根据其类型获取一个包含所有权限的 <see cref="ChannelPermissions"/>。
    /// </summary>
    /// <param name="channel"> 要获取其包含所有权限的频道。 </param>
    /// <returns> 一个包含所有该频道可以拥有的权限的 <see cref="ChannelPermissions"/>。 </returns>
    /// <exception cref="ArgumentException"> 未知的频道类型。 </exception>
    public static ChannelPermissions All(IChannel channel) =>
        channel switch
        {
            IVoiceChannel => Voice,
            ITextChannel => Text,
            IThreadChannel => Thread,
            ICategoryChannel => Category,
            IDMChannel => DM,
            _ => throw new ArgumentException("Unknown channel type.", nameof(channel))
        };

    /// <summary>
    ///     获取此权限集的原始值。
    /// </summary>
    public ulong RawValue { get; }

    /// <inheritdoc cref="Kook.GuildPermissions.CreateInvites" />
    public bool CreateInvites => Permissions.GetValue(RawValue, ChannelPermission.CreateInvites);

    /// <inheritdoc cref="Kook.GuildPermissions.ManageChannels" />
    public bool ManageChannels => Permissions.GetValue(RawValue, ChannelPermission.ManageChannels);

    /// <inheritdoc cref="Kook.GuildPermissions.ManageRoles" />
    public bool ManageRoles => Permissions.GetValue(RawValue, ChannelPermission.ManageRoles);

    /// <inheritdoc cref="Kook.GuildPermissions.ViewChannel" />
    public bool ViewChannel => Permissions.GetValue(RawValue, ChannelPermission.ViewChannel);

    /// <inheritdoc cref="Kook.GuildPermissions.SendMessages" />
    public bool SendMessages => Permissions.GetValue(RawValue, ChannelPermission.SendMessages);

    /// <inheritdoc cref="Kook.GuildPermissions.ManageMessages" />
    public bool ManageMessages => Permissions.GetValue(RawValue, ChannelPermission.ManageMessages);

    /// <inheritdoc cref="Kook.GuildPermissions.AttachFiles" />
    public bool AttachFiles => Permissions.GetValue(RawValue, ChannelPermission.AttachFiles);

    /// <inheritdoc cref="Kook.GuildPermissions.Connect" />
    public bool Connect => Permissions.GetValue(RawValue, ChannelPermission.Connect);

    /// <inheritdoc cref="Kook.GuildPermissions.ManageVoice" />
    public bool ManageVoice => Permissions.GetValue(RawValue, ChannelPermission.ManageVoice);

    /// <inheritdoc cref="Kook.GuildPermissions.MentionEveryone" />
    public bool MentionEveryone => Permissions.GetValue(RawValue, ChannelPermission.MentionEveryone);

    /// <inheritdoc cref="Kook.GuildPermissions.AddReactions" />
    public bool AddReactions => Permissions.GetValue(RawValue, ChannelPermission.AddReactions);

    /// <inheritdoc cref="Kook.GuildPermissions.PassiveConnect" />
    public bool PassiveConnect => Permissions.GetValue(RawValue, ChannelPermission.PassiveConnect);

    /// <inheritdoc cref="Kook.GuildPermissions.UseVoiceActivity" />
    public bool UseVoiceActivity => Permissions.GetValue(RawValue, ChannelPermission.UseVoiceActivity);

    /// <inheritdoc cref="Kook.GuildPermissions.Speak" />
    public bool Speak => Permissions.GetValue(RawValue, ChannelPermission.Speak);

    /// <inheritdoc cref="Kook.GuildPermissions.DeafenMembers" />
    public bool DeafenMembers => Permissions.GetValue(RawValue, ChannelPermission.DeafenMembers);

    /// <inheritdoc cref="Kook.GuildPermissions.MuteMembers" />
    public bool MuteMembers => Permissions.GetValue(RawValue, ChannelPermission.MuteMembers);

    /// <inheritdoc cref="Kook.GuildPermissions.PlaySoundtrack" />
    public bool PlaySoundtrack => Permissions.GetValue(RawValue, ChannelPermission.PlaySoundtrack);

    /// <inheritdoc cref="Kook.GuildPermissions.ShareScreen" />
    public bool ShareScreen => Permissions.GetValue(RawValue, ChannelPermission.ShareScreen);

    /// <inheritdoc cref="Kook.GuildPermissions.ReplyToPost" />
    public bool ReplyToPost => Permissions.GetValue(RawValue, ChannelPermission.ReplyToPost);

    /// <summary>
    ///     使用指定的权限原始值创建一个 <see cref="ChannelPermissions"/> 结构的新实例。
    /// </summary>
    /// <param name="rawValue"> 权限原始值。 </param>
    public ChannelPermissions(ulong rawValue)
    {
        RawValue = rawValue;
    }

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
        bool? shareScreen = null,
        bool? replyToPost = null)
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
        Permissions.SetValue(ref value, replyToPost, ChannelPermission.ReplyToPost);

        RawValue = value;
    }

    /// <summary>
    ///     使用指定的权限位信息创建一个 <see cref="ChannelPermissions"/> 结构的新实例。
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
        bool? shareScreen = false,
        bool? replyToPost = false)
        : this(0, createInvites, manageChannels, manageRoles, viewChannel, sendMessages, manageMessages, attachFiles,
            connect, manageVoice, mentionEveryone, addReactions, passiveConnect, useVoiceActivity, speak, deafenMembers,
            muteMembers, playSoundtrack, shareScreen, replyToPost)
    {
    }

    /// <summary>
    ///     以当前权限集为基础，更改指定的权限，返回一个 <see cref="ChannelPermissions"/> 结构的新实例。
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
        bool? shareScreen = null,
        bool? replyToPost = null) =>
        new(RawValue,
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
            shareScreen,
            replyToPost);

    /// <summary>
    ///     获取当前权限集是否包含指定的权限。
    /// </summary>
    /// <param name="permission"> 要检查的权限。 </param>
    /// <returns> 如果当前权限集包含了所有指定的权限信息，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public bool Has(ChannelPermission permission) => Permissions.GetValue(RawValue, permission);

    /// <summary>
    ///     获取一个包含当前权限集所包含的所有已设置的 <see cref="ChannelPermission"/> 独立位标志枚举值的集合。
    /// </summary>
    /// <returns> 一个包含当前权限集所包含的所有已设置的 <see cref="ChannelPermission"/> 独立位标志枚举值的集合；如果当前权限集未包含任何已设置的权限位，则会返回一个空集合。 </returns>
    public List<ChannelPermission> ToList()
    {
        List<ChannelPermission> perms = [];

        // bitwise operations on raw value
        // each of the ChannelPermissions increments by 2^i from 0 to MaxBits
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = (ulong)1 << i;
            if ((RawValue & flag) != 0)
                perms.Add((ChannelPermission)flag);
        }

        return perms;
    }

    /// <summary>
    ///     获取此权限集原始值的字符串表示。
    /// </summary>
    /// <returns> 此权限集原始值的字符串表示。 </returns>
    public override string ToString() => RawValue.ToString();

    private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
}

namespace KaiHeiLa;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class GuildPermissions
{
    public static readonly GuildPermissions None = new GuildPermissions();
    public static readonly GuildPermissions All = new GuildPermissions(0b1_11111_1111111_1111111_1111111111111_111111);

    public ulong RawValue { get; }

    public bool Administrator => Permissions.GetValue(RawValue, GuildPermission.Administrator);
    public bool ManageGuild => Permissions.GetValue(RawValue, GuildPermission.ManageGuild);
    public bool ViewAuditLog => Permissions.GetValue(RawValue, GuildPermission.ViewAuditLog);
    public bool CreateInvites => Permissions.GetValue(RawValue, GuildPermission.CreateInvites);
    public bool ManageInvites => Permissions.GetValue(RawValue, GuildPermission.ManageInvites);
    public bool ManageChannels => Permissions.GetValue(RawValue, GuildPermission.ManageChannels);
    public bool KickMembers => Permissions.GetValue(RawValue, GuildPermission.KickMembers);
    public bool BanMembers => Permissions.GetValue(RawValue, GuildPermission.BanMembers);
    public bool ManageEmojis => Permissions.GetValue(RawValue, GuildPermission.ManageEmojis);
    public bool ChangeNickname => Permissions.GetValue(RawValue, GuildPermission.ChangeNickname);
    public bool ManageRoles => Permissions.GetValue(RawValue, GuildPermission.ManageRoles);
    public bool ViewChannels => Permissions.GetValue(RawValue, GuildPermission.ViewChannels);
    public bool SendMessages => Permissions.GetValue(RawValue, GuildPermission.SendMessages);
    public bool ManageMessages => Permissions.GetValue(RawValue, GuildPermission.ManageMessages);
    public bool AttachFiles => Permissions.GetValue(RawValue, GuildPermission.AttachFiles);
    public bool Connect => Permissions.GetValue(RawValue, GuildPermission.Connect);
    public bool ManageVoice => Permissions.GetValue(RawValue, GuildPermission.ManageVoice);
    public bool MentionEveryone => Permissions.GetValue(RawValue, GuildPermission.MentionEveryone);
    public bool AddReactions => Permissions.GetValue(RawValue, GuildPermission.AddReactions);
    public bool FollowReactions => Permissions.GetValue(RawValue, GuildPermission.FollowReactions);
    public bool PassiveConnect => Permissions.GetValue(RawValue, GuildPermission.PassiveConnect);
    public bool OnlyPressToTalk => Permissions.GetValue(RawValue, GuildPermission.OnlyPressToTalk);
    public bool UseVoiceActivity => Permissions.GetValue(RawValue, GuildPermission.UseVoiceActivity);
    public bool Speak => Permissions.GetValue(RawValue, GuildPermission.Speak);
    public bool DeafenMembers => Permissions.GetValue(RawValue, GuildPermission.DeafenMembers);
    public bool MuteMembers => Permissions.GetValue(RawValue, GuildPermission.MuteMembers);
    public bool ManageNicknames => Permissions.GetValue(RawValue, GuildPermission.ManageNicknames);
    public bool PlaySoundtrack => Permissions.GetValue(RawValue, GuildPermission.PlaySoundtrack);

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
        bool? viewChannels = null,
        bool? sendMessages = null,
        bool? manageMessages = null,
        bool? attachFiles = null,
        bool? connect = null,
        bool? manageVoice = null,
        bool? mentionEveryone = null,
        bool? addReactions = null,
        bool? followReactions = null,
        bool? passiveConnect = null,
        bool? onlyPressToTalk = null,
        bool? useVoiceActivity = null,
        bool? speak = null,
        bool? deafenMembers = null,
        bool? muteMembers = null,
        bool? manageNicknames = null,
        bool? playSoundtrack = null
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
        Permissions.SetValue(ref value, viewChannels, GuildPermission.ViewChannels);
        Permissions.SetValue(ref value, sendMessages, GuildPermission.SendMessages);
        Permissions.SetValue(ref value, manageMessages, GuildPermission.ManageMessages);
        Permissions.SetValue(ref value, attachFiles, GuildPermission.AttachFiles);
        Permissions.SetValue(ref value, connect, GuildPermission.Connect);
        Permissions.SetValue(ref value, manageVoice, GuildPermission.ManageVoice);
        Permissions.SetValue(ref value, mentionEveryone, GuildPermission.MentionEveryone);
        Permissions.SetValue(ref value, addReactions, GuildPermission.AddReactions);
        Permissions.SetValue(ref value, followReactions, GuildPermission.FollowReactions);
        Permissions.SetValue(ref value, passiveConnect, GuildPermission.PassiveConnect);
        Permissions.SetValue(ref value, onlyPressToTalk, GuildPermission.OnlyPressToTalk);
        Permissions.SetValue(ref value, useVoiceActivity, GuildPermission.UseVoiceActivity);
        Permissions.SetValue(ref value, speak, GuildPermission.Speak);
        Permissions.SetValue(ref value, deafenMembers, GuildPermission.DeafenMembers);
        Permissions.SetValue(ref value, muteMembers, GuildPermission.MuteMembers);
        Permissions.SetValue(ref value, manageNicknames, GuildPermission.ManageNicknames);
        Permissions.SetValue(ref value, playSoundtrack, GuildPermission.PlaySoundtrack);

        RawValue = value;
    }

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
        bool viewChannels = false,
        bool sendMessages = false,
        bool manageMessages = false,
        bool attachFiles = false,
        bool connect = false,
        bool manageVoice = false,
        bool mentionEveryone = false,
        bool addReactions = false,
        bool followReactions = false,
        bool passiveConnect = false,
        bool onlyPressToTalk = false,
        bool useVoiceActivity = false,
        bool speak = false,
        bool deafenMembers = false,
        bool muteMembers = false,
        bool manageNicknames = false,
        bool playSoundtrack = false)
        : this(0, administrator, manageGuild, viewAuditLog, createInvites, manageInvites, manageChannels, kickMembers,
            banMembers, manageEmojis, changeNickname, manageRoles, viewChannels, sendMessages, manageMessages,
            attachFiles, connect, manageVoice, mentionEveryone, addReactions, followReactions, passiveConnect,
            onlyPressToTalk, useVoiceActivity, speak, deafenMembers, muteMembers, manageNicknames, playSoundtrack)
    {
    }

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
        bool? viewChannels = null,
        bool? sendMessages = null,
        bool? manageMessages = null,
        bool? attachFiles = null,
        bool? connect = null,
        bool? manageVoice = null,
        bool? mentionEveryone = null,
        bool? addReactions = null,
        bool? followReactions = null,
        bool? passiveConnect = null,
        bool? onlyPressToTalk = null,
        bool? useVoiceActivity = null,
        bool? speak = null,
        bool? deafenMembers = null,
        bool? muteMembers = null,
        bool? manageNicknames = null,
        bool? playSoundtrack = null)
        => new GuildPermissions(RawValue, administrator, manageGuild, viewAuditLog, createInvites, manageInvites,
            manageChannels, kickMembers, banMembers, manageEmojis, changeNickname, manageRoles, viewChannels,
            sendMessages, manageMessages, attachFiles, connect, manageVoice, mentionEveryone, addReactions,
            followReactions, passiveConnect, onlyPressToTalk, useVoiceActivity, speak, deafenMembers, muteMembers,
            manageNicknames, playSoundtrack);

    public bool Has(GuildPermission permission) => Permissions.GetValue(RawValue, permission);

    public List<GuildPermission> ToList()
    {
        List<GuildPermission> perms = new();

        // bitwise operations on raw value
        // each of the GuildPermissions increments by 2^i from 0 to MaxBits
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = ((ulong) 1 << i);
            if ((RawValue & flag) != 0)
                perms.Add((GuildPermission) flag);
        }

        return perms;
    }

    internal void Ensure(GuildPermission permissions)
    {
        if (!Has(permissions))
        {
            var vals = Enum.GetValues(typeof(GuildPermission)).Cast<GuildPermission>();
            var currentValues = RawValue;
            var missingValues = vals.Where(x => permissions.HasFlag(x) && !Permissions.GetValue(currentValues, x));

            throw new InvalidOperationException(
                $"Missing required guild permission{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
        }
    }

    public override string ToString() => RawValue.ToString();
    private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
}
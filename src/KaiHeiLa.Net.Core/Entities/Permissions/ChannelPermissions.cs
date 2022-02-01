using System.Diagnostics;

namespace KaiHeiLa;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public struct ChannelPermissions
{
    public static readonly ChannelPermissions None = new ChannelPermissions();
    public static readonly ChannelPermissions Text = new ChannelPermissions(0b0_11111_0101100_0000000_1111111110001_010001);
    public static readonly ChannelPermissions Voice = new ChannelPermissions(0b1_00000_0000100_1111110_0000000011100_010001);
    public static readonly ChannelPermissions Category = new ChannelPermissions(0b01100_1111110_1111111110001_010001);

    public static ChannelPermissions All(IChannel channel)
    {
        return channel switch
        {
            ITextChannel _ => Text,
            IVoiceChannel _ => Voice,
            ICategoryChannel _ => Category,
            _ => throw new ArgumentException("Unknown channel type.", nameof(channel)),
        };
    }
    
    public ulong RawValue { get; }
    
    public bool ManageGuild => Permissions.GetValue(RawValue, ChannelPermission.ManageGuild);
    public bool ManageInvites => Permissions.GetValue(RawValue, ChannelPermission.ManageInvites);
    public bool ManageRoles => Permissions.GetValue(RawValue, ChannelPermission.ManageRoles);
    public bool ViewChannels => Permissions.GetValue(RawValue, ChannelPermission.ViewChannels);
    public bool SendMessages => Permissions.GetValue(RawValue, ChannelPermission.SendMessages);
    public bool ManageMessages => Permissions.GetValue(RawValue, ChannelPermission.ManageMessages);
    public bool AttachFiles => Permissions.GetValue(RawValue, ChannelPermission.AttachFiles);
    public bool Connect => Permissions.GetValue(RawValue, ChannelPermission.Connect);
    public bool ManageVoice => Permissions.GetValue(RawValue, ChannelPermission.ManageVoice);
    public bool MentionEveryone => Permissions.GetValue(RawValue, ChannelPermission.MentionEveryone);
    public bool AddReactions => Permissions.GetValue(RawValue, ChannelPermission.AddReactions);
    public bool PassiveConnect => Permissions.GetValue(RawValue, ChannelPermission.PassiveConnect);
    public bool UseVoiceActivity => Permissions.GetValue(RawValue, ChannelPermission.UseVoiceActivity);
    public bool Speak => Permissions.GetValue(RawValue, ChannelPermission.Speak);
    public bool DeafenMembers => Permissions.GetValue(RawValue, ChannelPermission.DeafenMembers);
    public bool MuteMembers => Permissions.GetValue(RawValue, ChannelPermission.MuteMembers);
    public bool PlaySoundtrack => Permissions.GetValue(RawValue, ChannelPermission.PlaySoundtrack);
    
    public ChannelPermissions(ulong rawValue) { RawValue = rawValue; }

    private ChannelPermissions(ulong initialValue,
        bool? manageGuild = null,
        bool? manageInvites = null,
        bool? manageRoles = null,
        bool? viewChannels = null,
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
        bool? playSoundtrack = null)
    {
        ulong value = initialValue;

        Permissions.SetValue(ref value, manageGuild, ChannelPermission.ManageGuild);
        Permissions.SetValue(ref value, manageInvites, ChannelPermission.ManageInvites);
        Permissions.SetValue(ref value, manageRoles, ChannelPermission.ManageRoles);
        Permissions.SetValue(ref value, viewChannels, ChannelPermission.ViewChannels);
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
        
        RawValue = value;
    }

    public ChannelPermissions(
        bool? manageGuild = false,
        bool? manageInvites = false,
        bool? manageRoles = false,
        bool? viewChannels = false,
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
        bool? playSoundtrack = false)
        : this(0, manageGuild, manageInvites, manageRoles, viewChannels, sendMessages, manageMessages, attachFiles,
            connect, manageVoice, mentionEveryone, addReactions, passiveConnect, useVoiceActivity, speak, deafenMembers,
            muteMembers, playSoundtrack)
    { }

    public ChannelPermissions Modify(
        bool? manageGuild = null,
        bool? manageInvites = null,
        bool? manageRoles = null,
        bool? viewChannels = null,
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
        bool? playSoundtrack = null)
        => new ChannelPermissions(RawValue,
            manageGuild,
            manageInvites,
            manageRoles,
            viewChannels,
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
            playSoundtrack);

    public bool Has(ChannelPermission permission) => Permissions.GetValue(RawValue, permission);

    public List<ChannelPermission> ToList()
    {
        List<ChannelPermission> perms = new();
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = ((ulong)1 << i);
            if ((RawValue & flag) != 0)
                perms.Add((ChannelPermission)flag);
        }
        return perms;
    }

    public override string ToString() => RawValue.ToString();
    private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
}
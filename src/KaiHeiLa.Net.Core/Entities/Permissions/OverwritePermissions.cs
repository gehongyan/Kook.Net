using System.Diagnostics;

namespace KaiHeiLa;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public struct OverwritePermissions
{
    public static OverwritePermissions InheritAll { get; } = new OverwritePermissions();

    public static OverwritePermissions AllowAll(IChannel channel)
        => new OverwritePermissions(ChannelPermissions.All(channel).RawValue, 0);

    public static OverwritePermissions DenyAll(IChannel channel)
        => new OverwritePermissions(0, ChannelPermissions.All(channel).RawValue);

    public ulong AllowValue { get; }
    public ulong DenyValue { get; }

    public PermValue CreateInvites => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.CreateInvites);
    public PermValue ManageChannels => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageChannels);
    public PermValue ManageRoles => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageRoles);
    public PermValue ViewChannels => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ViewChannels);
    public PermValue SendMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.SendMessages);
    public PermValue ManageMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageMessages);
    public PermValue AttachFiles => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.AttachFiles);
    public PermValue Connect => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.Connect);
    public PermValue ManageVoice => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ManageVoice);
    public PermValue MentionEveryone => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MentionEveryone);
    public PermValue AddReactions => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.AddReactions);
    public PermValue PassiveConnect => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.PassiveConnect);
    public PermValue UseVoiceActivity => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.UseVoiceActivity);
    public PermValue Speak => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.Speak);
    public PermValue DeafenMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.DeafenMembers);
    public PermValue MuteMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MuteMembers);
    public PermValue PlaySoundtrack => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.PlaySoundtrack);

    public OverwritePermissions(ulong allowValue, ulong denyValue)
    {
        AllowValue = allowValue;
        DenyValue = denyValue;
    }

    public OverwritePermissions(string allowValue, string denyValue)
    {
        AllowValue = uint.Parse(allowValue);
        DenyValue = uint.Parse(denyValue);
    }

    private OverwritePermissions(ulong allowValue, ulong denyValue,
        PermValue? createInvites = null,
        PermValue? manageChannels = null,
        PermValue? manageRoles = null,
        PermValue? viewChannels = null,
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
        PermValue? playSoundtrack = null)
    {
        Permissions.SetValue(ref allowValue, ref denyValue, createInvites, ChannelPermission.CreateInvites);
        Permissions.SetValue(ref allowValue, ref denyValue, manageChannels, ChannelPermission.ManageChannels);
        Permissions.SetValue(ref allowValue, ref denyValue, manageRoles, ChannelPermission.ManageRoles);
        Permissions.SetValue(ref allowValue, ref denyValue, viewChannels, ChannelPermission.ViewChannels);
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

        AllowValue = allowValue;
        DenyValue = denyValue;
    }

    public OverwritePermissions(
        PermValue createInvites = PermValue.Inherit,
        PermValue manageChannels = PermValue.Inherit,
        PermValue manageRoles = PermValue.Inherit,
        PermValue viewChannels = PermValue.Inherit,
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
        PermValue playSoundtrack = PermValue.Inherit)
        : this(0, 0, createInvites, manageChannels, manageRoles, viewChannels, sendMessages, manageMessages,
            attachFiles, connect, manageVoice, mentionEveryone, addReactions, passiveConnect, useVoiceActivity, speak,
            deafenMembers, muteMembers, playSoundtrack)
    {
    }

    public OverwritePermissions Modify(
        PermValue? createInvites = null,
        PermValue? manageChannels = null,
        PermValue? manageRoles = null,
        PermValue? viewChannels = null,
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
        PermValue? playSoundtrack = null)
        => new OverwritePermissions(AllowValue, DenyValue, createInvites, manageChannels, manageRoles,
            viewChannels, sendMessages, manageMessages, attachFiles, connect, manageVoice, mentionEveryone,
            addReactions, passiveConnect, useVoiceActivity, speak, deafenMembers, muteMembers, playSoundtrack);

    public List<ChannelPermission> ToAllowList()
    {
        List<ChannelPermission> perms = new();
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            // first operand must be long or ulong to shift >31 bits
            ulong flag = ((ulong) 1 << i);
            if ((AllowValue & flag) != 0)
                perms.Add((ChannelPermission) flag);
        }

        return perms;
    }

    public List<ChannelPermission> ToDenyList()
    {
        List<ChannelPermission> perms = new();
        for (byte i = 0; i < Permissions.MaxBits; i++)
        {
            ulong flag = ((ulong) 1 << i);
            if ((DenyValue & flag) != 0)
                perms.Add((ChannelPermission) flag);
        }

        return perms;
    }

    public override string ToString() => $"Allow {AllowValue}, Deny {DenyValue}";

    private string DebuggerDisplay =>
        $"Allow {string.Join(", ", ToAllowList())}, " +
        $"Deny {string.Join(", ", ToDenyList())}";
}
using System;
using System.Collections.Generic;
using Xunit;

namespace Kook;

/// <summary>
///     Tests the behavior of the <see cref="Kook.GuildPermissions"/> type and related functions.
/// </summary>
[Trait("Category", "Unit")]
public class GuildPermissionsTests
{
    /// <summary>
    ///     Tests the default value of the <see cref="Kook.GuildPermissions"/> constructor.
    /// </summary>
    [Fact]
    public void DefaultConstructor()
    {
        var p = new GuildPermissions();
        Assert.Equal((ulong)0, p.RawValue);
        Assert.Equal(GuildPermissions.None.RawValue, p.RawValue);
    }

    /// <summary>
    ///     Tests the behavior of the <see cref="Kook.GuildPermissions"/> raw value constructor.
    /// </summary>
    [Fact]
    public void RawValueConstructor()
    {
        // returns all of the values that will be tested
        // a Theory cannot be used here, because these values are not all constants
        IEnumerable<ulong> GetTestValues()
        {
            yield return 0;
            yield return GuildPermissions.None.RawValue;
            yield return GuildPermissions.All.RawValue;
        }

        foreach (var rawValue in GetTestValues())
        {
            var p = new GuildPermissions(rawValue);
            Assert.Equal(rawValue, p.RawValue);
        }
    }

    /// <summary>
    ///     Tests the behavior of the <see cref="Kook.GuildPermissions"/> constructor for each
    ///     of it's flags.
    /// </summary>
    [Fact]
    public void FlagsConstructor()
    {
        // util method for asserting that the constructor sets the given flag
        void AssertFlag(Func<GuildPermissions> cstr, GuildPermission flag)
        {
            var p = cstr();
            // ensure flag set to true
            Assert.True(p.Has(flag));
            // ensure only this flag is set
            Assert.Equal((ulong)flag, p.RawValue);
        }

        AssertFlag(() => new GuildPermissions(administrator: true), GuildPermission.Administrator);
        AssertFlag(() => new GuildPermissions(manageGuild: true), GuildPermission.ManageGuild);
        AssertFlag(() => new GuildPermissions(viewAuditLog: true), GuildPermission.ViewAuditLog);
        AssertFlag(() => new GuildPermissions(createInvites: true), GuildPermission.CreateInvites);
        AssertFlag(() => new GuildPermissions(manageInvites: true), GuildPermission.ManageInvites);
        AssertFlag(() => new GuildPermissions(manageChannels: true), GuildPermission.ManageChannels);
        AssertFlag(() => new GuildPermissions(kickMembers: true), GuildPermission.KickMembers);
        AssertFlag(() => new GuildPermissions(banMembers: true), GuildPermission.BanMembers);
        AssertFlag(() => new GuildPermissions(manageEmojis: true), GuildPermission.ManageEmojis);
        AssertFlag(() => new GuildPermissions(changeNickname: true), GuildPermission.ChangeNickname);
        AssertFlag(() => new GuildPermissions(manageRoles: true), GuildPermission.ManageRoles);
        AssertFlag(() => new GuildPermissions(viewChannel: true), GuildPermission.ViewChannel);
        AssertFlag(() => new GuildPermissions(sendMessages: true), GuildPermission.SendMessages);
        AssertFlag(() => new GuildPermissions(manageMessages: true), GuildPermission.ManageMessages);
        AssertFlag(() => new GuildPermissions(attachFiles: true), GuildPermission.AttachFiles);
        AssertFlag(() => new GuildPermissions(connect: true), GuildPermission.Connect);
        AssertFlag(() => new GuildPermissions(manageVoice: true), GuildPermission.ManageVoice);
        AssertFlag(() => new GuildPermissions(mentionEveryone: true), GuildPermission.MentionEveryone);
        AssertFlag(() => new GuildPermissions(addReactions: true), GuildPermission.AddReactions);
        AssertFlag(() => new GuildPermissions(followReactions: true), GuildPermission.FollowReactions);
        AssertFlag(() => new GuildPermissions(passiveConnect: true), GuildPermission.PassiveConnect);
        AssertFlag(() => new GuildPermissions(onlyPushToTalk: true), GuildPermission.OnlyPushToTalk);
        AssertFlag(() => new GuildPermissions(useVoiceActivity: true), GuildPermission.UseVoiceActivity);
        AssertFlag(() => new GuildPermissions(speak: true), GuildPermission.Speak);
        AssertFlag(() => new GuildPermissions(deafenMembers: true), GuildPermission.DeafenMembers);
        AssertFlag(() => new GuildPermissions(muteMembers: true), GuildPermission.MuteMembers);
        AssertFlag(() => new GuildPermissions(manageNicknames: true), GuildPermission.ManageNicknames);
        AssertFlag(() => new GuildPermissions(playSoundtrack: true), GuildPermission.PlaySoundtrack);
    }

    /// <summary>
    ///     Tests the behavior of <see cref="Kook.GuildPermissions.Modify(bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?)"/>
    ///     with each of the parameters.
    /// </summary>
    [Fact]
    public void Modify()
    {
        // asserts that flag values can be checked
        // and that flag values can be toggled on and off
        // and that the behavior of ToList works as expected
        void AssertUtil(GuildPermission permission,
            Func<GuildPermissions, bool> has,
            Func<GuildPermissions, bool, GuildPermissions> modify)
        {
            var perm = new GuildPermissions();
            // ensure permission initially false
            // use both the function and Has to ensure that the GetPermission
            // function is working
            Assert.False(has(perm));
            Assert.False(perm.Has(permission));

            // enable it, and ensure that it gets set
            perm = modify(perm, true);
            Assert.True(has(perm));
            Assert.True(perm.Has(permission));

            // check ToList behavior
            var list = perm.ToList();
            Assert.Contains(permission, list);
            Assert.Single(list);

            // set it false again
            perm = modify(perm, false);
            Assert.False(has(perm));
            Assert.False(perm.Has(permission));

            // ensure that no perms are set now
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);
        }

        AssertUtil(GuildPermission.Administrator, x => x.Administrator, (p, enable) => p.Modify(administrator: enable));
        AssertUtil(GuildPermission.ManageGuild, x => x.ManageGuild, (p, enable) => p.Modify(manageGuild: enable));
        AssertUtil(GuildPermission.ViewAuditLog, x => x.ViewAuditLog, (p, enable) => p.Modify(viewAuditLog: enable));
        AssertUtil(GuildPermission.CreateInvites, x => x.CreateInvites, (p, enable) => p.Modify(createInvites: enable));
        AssertUtil(GuildPermission.ManageInvites, x => x.ManageInvites, (p, enable) => p.Modify(manageInvites: enable));
        AssertUtil(GuildPermission.ManageChannels, x => x.ManageChannels, (p, enable) => p.Modify(manageChannels: enable));
        AssertUtil(GuildPermission.KickMembers, x => x.KickMembers, (p, enable) => p.Modify(kickMembers: enable));
        AssertUtil(GuildPermission.BanMembers, x => x.BanMembers, (p, enable) => p.Modify(banMembers: enable));
        AssertUtil(GuildPermission.ManageEmojis, x => x.ManageEmojis, (p, enable) => p.Modify(manageEmojis: enable));
        AssertUtil(GuildPermission.ChangeNickname, x => x.ChangeNickname, (p, enable) => p.Modify(changeNickname: enable));
        AssertUtil(GuildPermission.ManageRoles, x => x.ManageRoles, (p, enable) => p.Modify(manageRoles: enable));
        AssertUtil(GuildPermission.ViewChannel, x => x.ViewChannel, (p, enable) => p.Modify(viewChannel: enable));
        AssertUtil(GuildPermission.SendMessages, x => x.SendMessages, (p, enable) => p.Modify(sendMessages: enable));
        AssertUtil(GuildPermission.ManageMessages, x => x.ManageMessages, (p, enable) => p.Modify(manageMessages: enable));
        AssertUtil(GuildPermission.AttachFiles, x => x.AttachFiles, (p, enable) => p.Modify(attachFiles: enable));
        AssertUtil(GuildPermission.Connect, x => x.Connect, (p, enable) => p.Modify(connect: enable));
        AssertUtil(GuildPermission.ManageVoice, x => x.ManageVoice, (p, enable) => p.Modify(manageVoice: enable));
        AssertUtil(GuildPermission.MentionEveryone, x => x.MentionEveryone, (p, enable) => p.Modify(mentionEveryone: enable));
        AssertUtil(GuildPermission.AddReactions, x => x.AddReactions, (p, enable) => p.Modify(addReactions: enable));
        AssertUtil(GuildPermission.FollowReactions, x => x.FollowReactions, (p, enable) => p.Modify(followReactions: enable));
        AssertUtil(GuildPermission.PassiveConnect, x => x.PassiveConnect, (p, enable) => p.Modify(passiveConnect: enable));
        AssertUtil(GuildPermission.OnlyPushToTalk, x => x.OnlyPushToTalk, (p, enable) => p.Modify(onlyPushToTalk: enable));
        AssertUtil(GuildPermission.UseVoiceActivity, x => x.UseVoiceActivity, (p, enable) => p.Modify(useVoiceActivity: enable));
        AssertUtil(GuildPermission.Speak, x => x.Speak, (p, enable) => p.Modify(speak: enable));
        AssertUtil(GuildPermission.DeafenMembers, x => x.DeafenMembers, (p, enable) => p.Modify(deafenMembers: enable));
        AssertUtil(GuildPermission.MuteMembers, x => x.MuteMembers, (p, enable) => p.Modify(muteMembers: enable));
        AssertUtil(GuildPermission.ManageNicknames, x => x.ManageNicknames, (p, enable) => p.Modify(manageNicknames: enable));
        AssertUtil(GuildPermission.PlaySoundtrack, x => x.PlaySoundtrack, (p, enable) => p.Modify(playSoundtrack: enable));

    }
}

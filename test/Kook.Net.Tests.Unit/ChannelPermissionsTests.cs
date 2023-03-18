using System;
using System.Collections.Generic;
using Xunit;

namespace Kook;

/// <summary>
///     Tests the behavior of the <see cref="Kook.ChannelPermissions"/> type and related functions.
/// </summary>
[Trait("Category", "Unit")]
public class ChannelPermissionsTests
{
    /// <summary>
    ///     Tests the default value of the <see cref="Kook.ChannelPermissions"/> constructor.
    /// </summary>
    [Fact]
    public void DefaultConstructor()
    {
        ChannelPermissions permission = new();
        Assert.Equal((ulong)0, permission.RawValue);
        Assert.Equal(ChannelPermissions.None.RawValue, permission.RawValue);
    }

    /// <summary>
    ///     Tests the behavior of the <see cref="Kook.ChannelPermission"/> raw value constructor.
    /// </summary>
    [Fact]
    public void RawValueConstructor()
    {
        // returns all of the values that will be tested
        // a Theory cannot be used here, because these values are not all constants
        IEnumerable<ulong> GetTestValues()
        {
            yield return 0;
            yield return ChannelPermissions.None.RawValue;
            yield return ChannelPermissions.Category.RawValue;
            yield return ChannelPermissions.DM.RawValue;
            yield return ChannelPermissions.Text.RawValue;
            yield return ChannelPermissions.Voice.RawValue;
        }

        ;

        foreach (ulong rawValue in GetTestValues())
        {
            ChannelPermissions p = new(rawValue);
            Assert.Equal(rawValue, p.RawValue);
        }
    }

    /// <summary>
    ///     Tests the behavior of the <see cref="Kook.ChannelPermissions"/> constructor for each
    ///     of it's flags.
    /// </summary>
    [Fact]
    public void FlagsConstructor()
    {
        // util method for asserting that the constructor sets the given flag
        void AssertFlag(Func<ChannelPermissions> cstr, ChannelPermission flag)
        {
            ChannelPermissions p = cstr();
            // ensure that this flag is set to true
            Assert.True(p.Has(flag));
            // ensure that only this flag is set
            Assert.Equal((ulong)flag, p.RawValue);
        }

        AssertFlag(() => new ChannelPermissions(true), ChannelPermission.CreateInvites);
        AssertFlag(() => new ChannelPermissions(manageChannels: true), ChannelPermission.ManageChannels);
        AssertFlag(() => new ChannelPermissions(manageRoles: true), ChannelPermission.ManageRoles);
        AssertFlag(() => new ChannelPermissions(viewChannel: true), ChannelPermission.ViewChannel);
        AssertFlag(() => new ChannelPermissions(sendMessages: true), ChannelPermission.SendMessages);
        AssertFlag(() => new ChannelPermissions(manageMessages: true), ChannelPermission.ManageMessages);
        AssertFlag(() => new ChannelPermissions(attachFiles: true), ChannelPermission.AttachFiles);
        AssertFlag(() => new ChannelPermissions(connect: true), ChannelPermission.Connect);
        AssertFlag(() => new ChannelPermissions(manageVoice: true), ChannelPermission.ManageVoice);
        AssertFlag(() => new ChannelPermissions(mentionEveryone: true), ChannelPermission.MentionEveryone);
        AssertFlag(() => new ChannelPermissions(addReactions: true), ChannelPermission.AddReactions);
        AssertFlag(() => new ChannelPermissions(passiveConnect: true), ChannelPermission.PassiveConnect);
        AssertFlag(() => new ChannelPermissions(useVoiceActivity: true), ChannelPermission.UseVoiceActivity);
        AssertFlag(() => new ChannelPermissions(speak: true), ChannelPermission.Speak);
        AssertFlag(() => new ChannelPermissions(deafenMembers: true), ChannelPermission.DeafenMembers);
        AssertFlag(() => new ChannelPermissions(muteMembers: true), ChannelPermission.MuteMembers);
        AssertFlag(() => new ChannelPermissions(playSoundtrack: true), ChannelPermission.PlaySoundtrack);
    }

    /// <summary>
    ///     Tests the behavior of <see cref="Kook.ChannelPermissions.Modify(bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?)"/>
    ///     with each of the parameters.
    /// </summary>
    [Fact]
    public void Modify()
    {
        // asserts that a channel permission flag value can be checked
        // and that modify can set and unset each flag
        // and that ToList performs as expected
        void AssertUtil(ChannelPermission permission,
            Func<ChannelPermissions, bool> has,
            Func<ChannelPermissions, bool, ChannelPermissions> modify)
        {
            ChannelPermissions perm = new();
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
            List<ChannelPermission> list = perm.ToList();
            Assert.Contains(permission, list);
            Assert.Single(list);

            // set it false again
            perm = modify(perm, false);
            Assert.False(has(perm));
            Assert.False(perm.Has(permission));

            // ensure that no perms are set now
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);
        }

        AssertUtil(ChannelPermission.CreateInvites, x => x.CreateInvites, (p, enable) => p.Modify(enable));
        AssertUtil(ChannelPermission.ManageChannels, x => x.ManageChannels, (p, enable) => p.Modify(manageChannels: enable));
        AssertUtil(ChannelPermission.ManageRoles, x => x.ManageRoles, (p, enable) => p.Modify(manageRoles: enable));
        AssertUtil(ChannelPermission.ViewChannel, x => x.ViewChannel, (p, enable) => p.Modify(viewChannel: enable));
        AssertUtil(ChannelPermission.SendMessages, x => x.SendMessages, (p, enable) => p.Modify(sendMessages: enable));
        AssertUtil(ChannelPermission.ManageMessages, x => x.ManageMessages, (p, enable) => p.Modify(manageMessages: enable));
        AssertUtil(ChannelPermission.AttachFiles, x => x.AttachFiles, (p, enable) => p.Modify(attachFiles: enable));
        AssertUtil(ChannelPermission.Connect, x => x.Connect, (p, enable) => p.Modify(connect: enable));
        AssertUtil(ChannelPermission.ManageVoice, x => x.ManageVoice, (p, enable) => p.Modify(manageVoice: enable));
        AssertUtil(ChannelPermission.MentionEveryone, x => x.MentionEveryone, (p, enable) => p.Modify(mentionEveryone: enable));
        AssertUtil(ChannelPermission.AddReactions, x => x.AddReactions, (p, enable) => p.Modify(addReactions: enable));
        AssertUtil(ChannelPermission.PassiveConnect, x => x.PassiveConnect, (p, enable) => p.Modify(passiveConnect: enable));
        AssertUtil(ChannelPermission.UseVoiceActivity, x => x.UseVoiceActivity, (p, enable) => p.Modify(useVoiceActivity: enable));
        AssertUtil(ChannelPermission.Speak, x => x.Speak, (p, enable) => p.Modify(speak: enable));
        AssertUtil(ChannelPermission.DeafenMembers, x => x.DeafenMembers, (p, enable) => p.Modify(deafenMembers: enable));
        AssertUtil(ChannelPermission.MuteMembers, x => x.MuteMembers, (p, enable) => p.Modify(muteMembers: enable));
        AssertUtil(ChannelPermission.PlaySoundtrack, x => x.PlaySoundtrack, (p, enable) => p.Modify(playSoundtrack: enable));
    }

    /// <summary>
    ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for a null channel will throw an <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public void ChannelTypeResolution_Null() =>
        Assert.Throws<ArgumentException>(() => { ChannelPermissions.All(null); });

    /// <summary>
    ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="ITextChannel"/> will return a value
    ///     equivalent to <see cref="ChannelPermissions.Text"/>.
    /// </summary>
    [Fact]
    public void ChannelTypeResolution_Text() =>
        Assert.Equal(ChannelPermissions.Text.RawValue, ChannelPermissions.All(new MockedTextChannel()).RawValue);

    /// <summary>
    ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="IVoiceChannel"/> will return a value
    ///     equivalent to <see cref="ChannelPermissions.Voice"/>.
    /// </summary>
    [Fact]
    public void ChannelTypeResolution_Voice() =>
        Assert.Equal(ChannelPermissions.Voice.RawValue, ChannelPermissions.All(new MockedVoiceChannel()).RawValue);

    /// <summary>
    ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="ICategoryChannel"/> will return a value
    ///     equivalent to <see cref="ChannelPermissions.Category"/>.
    /// </summary>
    [Fact]
    public void ChannelTypeResolution_Category() =>
        Assert.Equal(ChannelPermissions.Category.RawValue, ChannelPermissions.All(new MockedCategoryChannel()).RawValue);

    /// <summary>
    ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="IDMChannel"/> will return a value
    ///     equivalent to <see cref="ChannelPermissions.DM"/>.
    /// </summary>
    [Fact]
    public void ChannelTypeResolution_DM() => Assert.Equal(ChannelPermissions.DM.RawValue, ChannelPermissions.All(new MockedDMChannel()).RawValue);

    /// <summary>
    ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an invalid channel will throw an <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public void ChannelTypeResolution_Invalid() => Assert.Throws<ArgumentException>(() => ChannelPermissions.All(new MockedInvalidChannel()));
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kook.Rest;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

/// <summary>
///     Tests that channels can be created and modified.
/// </summary>
[CollectionDefinition(nameof(ChannelTests), DisableParallelization = true)]
[Trait("Category", "Integration")]
public class ChannelTests : IClassFixture<RestGuildFixture>
{
    private readonly IGuild _guild;
    private readonly ITestOutputHelper _output;

    public ChannelTests(RestGuildFixture guildFixture, ITestOutputHelper output)
    {
        _guild = guildFixture.Guild;
        _output = output;
        _output.WriteLine($"RestGuildFixture using guild: {_guild.Id}");
        // capture all console output
        guildFixture.Client.Log += LogAsync;
    }
    private Task LogAsync(LogMessage message)
    {
        _output.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Checks that a text channel can be created and modified.
    /// </summary>
    [Fact]
    public async Task ModifyTextChannel()
    {
        // create a text channel to modify
        ITextChannel channel = await _guild.CreateTextChannelAsync("TEXT");
        try
        {
            Assert.NotNull(channel);
            // check that it can be modified
            await channel.ModifyAsync(x =>
            {
                x.Name = "UPDATED TEXT";
                x.SlowModeInterval = SlowModeInterval._5;
                x.Topic = "TOPIC";
                x.CategoryId = null;
            });
            // check the results of modifying this channel
            Assert.Equal("UPDATED TEXT", channel.Name);
            Assert.Equal(5, channel.SlowModeInterval);
            Assert.Equal("TOPIC", channel.Topic);
            Assert.Null(channel.CategoryId);
        }
        finally
        {
            await channel.DeleteAsync();
        }
    }

    /// <summary>
    ///     Checks that a voice channel can be created, modified, and deleted.
    /// </summary>
    [Fact]
    public async Task ModifyVoiceChannel()
    {
        IVoiceChannel channel = await _guild.CreateVoiceChannelAsync("VOICE");
        try
        {
            Assert.NotNull(channel);
            // try to modify it
            await channel.ModifyAsync(x =>
            {
                x.Name = "UPDATED VOICE";
                x.UserLimit = 5;
                x.VoiceQuality = VoiceQuality._96kbps;
            });
            // check that these were updated
            Assert.Equal("UPDATED VOICE", channel.Name);
            Assert.Equal(5, channel.UserLimit);
            Assert.Equal(VoiceQuality._96kbps, channel.VoiceQuality);
        }
        finally
        {
            await channel.DeleteAsync();
        }
    }

    /// <summary>
    ///     Creates a category channel, a voice channel, and a text channel, then tries to assign them under that category.
    /// </summary>
    [Fact]
    public async Task ModifyChannelCategories()
    {
        // util method for checking if a category is set
        async Task CheckAsync(INestedChannel channel, ICategoryChannel cat)
        {
            // check that the category is not set
            if (cat == null)
            {
                Assert.Null(channel.CategoryId);
                Assert.Null(await channel.GetCategoryAsync());
            }
            else
            {
                Assert.NotNull(channel.CategoryId);
                Assert.Equal(cat.Id, channel.CategoryId);
                ICategoryChannel getCat = await channel.GetCategoryAsync();
                Assert.NotNull(getCat);
                Assert.Equal(cat.Id, getCat.Id);
            }
        }
        // initially create these not under the category
        ICategoryChannel category = await _guild.CreateCategoryChannelAsync("CATEGORY");
        ITextChannel text = await _guild.CreateTextChannelAsync("TEXT");
        IVoiceChannel voice = await _guild.CreateVoiceChannelAsync("VOICE");

        try
        {
            Assert.NotNull(category);
            Assert.NotNull(text);
            Assert.NotNull(voice);
            // check that the category is not set for either
            await CheckAsync(text, null);
            await CheckAsync(voice, null);

            // set the category
            await text.ModifyAsync(x => x.CategoryId = category.Id);
            await voice.ModifyAsync(x => x.CategoryId = category.Id);

            // check that this is set, and that it's the category that was created earlier
            await CheckAsync(text, category);
            await CheckAsync(voice, category);

            // create one more channel immediately under this category
            ITextChannel newText = await _guild.CreateTextChannelAsync("NEW TEXT", x => x.CategoryId = category.Id);
            try
            {
                Assert.NotNull(newText);
                await CheckAsync(newText, category);
            }
            finally
            {
                await newText.DeleteAsync();
            }
        }
        finally
        {
            // clean up
            await category.DeleteAsync();
            await text.DeleteAsync();
            await voice.DeleteAsync();
        }
    }

    [Fact]
    public async Task MiscAsync()
    {
        ITextChannel channel = await _guild.CreateTextChannelAsync("TEXT");
        try
        {
            IGuildUser selfUser = await _guild.GetCurrentUserAsync();
            IRole role = await _guild.CreateRoleAsync("TEST ROLE");
            Assert.NotNull(channel);
            Assert.NotNull(selfUser);
            Assert.NotNull(role);
            
            // check that the creator is myself
            Assert.Equal(selfUser.Id, channel.CreatorId);
            IUser creator = await channel.GetCreatorAsync();
            Assert.Equal(selfUser.Id, creator.Id);
            
            // check permission overwrites
            await channel.AddPermissionOverwriteAsync(selfUser);
            Assert.Single(channel.UserPermissionOverwrites);
            await channel.ModifyPermissionOverwriteAsync(selfUser, permissions => permissions
                .Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny, attachFiles: PermValue.Inherit));
            UserPermissionOverwrite userPermissionOverwrite = channel.UserPermissionOverwrites.First();
            Assert.Equal(selfUser.Id, userPermissionOverwrite.Target.Id);
            Assert.Equal(PermValue.Allow, userPermissionOverwrite.Permissions.ViewChannel);
            Assert.Equal(PermValue.Deny, userPermissionOverwrite.Permissions.SendMessages);
            Assert.Equal(PermValue.Inherit, userPermissionOverwrite.Permissions.AttachFiles);
            await channel.RemovePermissionOverwriteAsync(selfUser);
            Assert.Empty(channel.UserPermissionOverwrites);
            await channel.AddPermissionOverwriteAsync(role);
            Assert.Single(channel.RolePermissionOverwrites.Where(overwrite => overwrite.Target > 0));
            await channel.ModifyPermissionOverwriteAsync(role, permissions => permissions
                .Modify(viewChannel: PermValue.Allow, sendMessages: PermValue.Deny, attachFiles: PermValue.Inherit));
            RolePermissionOverwrite rolePermissionOverwrite = channel.RolePermissionOverwrites.First(overwrite => overwrite.Target > 0);
            Assert.Equal(role.Id, rolePermissionOverwrite.Target);
            Assert.Equal(PermValue.Allow, rolePermissionOverwrite.Permissions.ViewChannel);
            Assert.Equal(PermValue.Deny, rolePermissionOverwrite.Permissions.SendMessages);
            Assert.Equal(PermValue.Inherit, rolePermissionOverwrite.Permissions.AttachFiles);
            await channel.RemovePermissionOverwriteAsync(role);
            Assert.Empty(channel.RolePermissionOverwrites.Where(overwrite => overwrite.Target > 0));
            
            // check invites
            IInvite invite = await channel.CreateInviteAsync(InviteMaxAge._86400, InviteMaxUses._50);
            Assert.NotNull(invite);
            Assert.Equal(TimeSpan.FromSeconds(86400), invite.MaxAge);
            Assert.Equal(50, invite.MaxUses);
            invite.Code.Should().NotBeNullOrWhiteSpace();
        }
        finally
        {
            await channel.DeleteAsync();
        }
    }
}
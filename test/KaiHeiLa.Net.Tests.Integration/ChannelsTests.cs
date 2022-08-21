using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace KaiHeiLa
{
    /// <summary>
    ///     Tests that channels can be created and modified.
    /// </summary>
    [CollectionDefinition(nameof(ChannelsTests), DisableParallelization = true)]
    [Trait("Category", "Integration")]
    public class ChannelsTests : IClassFixture<RestGuildFixture>
    {
        private readonly IGuild _guild;
        private readonly ITestOutputHelper _output;

        public ChannelsTests(RestGuildFixture guildFixture, ITestOutputHelper output)
        {
            _guild = guildFixture.Guild;
            _output = output;
            output.WriteLine($"RestGuildFixture using guild: {_guild.Id}");
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
    }
}

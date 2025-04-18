using Kook.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

/// <summary>
///     Tests that channels can be created and modified.
/// </summary>
[CollectionDefinition(nameof(MessageInTextTests), DisableParallelization = true)]
[Trait("Category", "Integration.Rest")]
public class MessageInTextTests : IClassFixture<RestChannelFixture>
{
    private readonly IGuild _guild;
    private readonly ITextChannel _channel;
    private readonly ITestOutputHelper _output;

    public MessageInTextTests(RestChannelFixture channelFixture, ITestOutputHelper output)
    {
        _guild = channelFixture.Guild;
        _channel = channelFixture.TextChannel;
        _output = output;
        _output.WriteLine($"RestGuildFixture using guild: {_guild.Id}");
        // capture all console output
        channelFixture.Client.Log += LogAsync;
    }

    private Task LogAsync(LogMessage message)
    {
        _output.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SendTextAsync()
    {
        IGuildUser? selfUser = await _guild.GetCurrentUserAsync();
        Assert.NotNull(selfUser);
        string kMarkdownSourceContent = $"""
            *TEST* **KMARKDOWN** ~~MESSAGE~~
            > NOTHING
            [INLINE LINK](https://kooknet.dev)
            (ins)UNDERLINE(ins)(spl)SPOLIER(spl)
            :maple_leaf:
            {_channel.KMarkdownMention}
            {selfUser.KMarkdownMention}
            {_guild.EveryoneRole.KMarkdownMention}
            (met)here(met)
            `INLINE CODE`
            ```csharp
            CODE BLOCK
            ```
            """;
        //  Emoji will be replaced with unicode emoji instead of the name.
        string kMarkdownParsedContent = $"""
            *TEST* **KMARKDOWN** ~~MESSAGE~~
            > NOTHING
            [INLINE LINK](https://kooknet.dev)
            (ins)UNDERLINE(ins)(spl)SPOLIER(spl)
            🍁
            {_channel.KMarkdownMention}
            {selfUser.KMarkdownMention}
            {_guild.EveryoneRole.KMarkdownMention}
            (met)here(met)
            `INLINE CODE`
            ```csharp
            CODE BLOCK
            ```
            """;
        // Due to the client is REST, the channel name will not be parsed.
        // Hence, the channel name will be displayed as empty string.
        string kMarkdownCleanContent = """
            TEST KMARKDOWN MESSAGE
             NOTHING
            [INLINE LINK](https://kooknet.dev)
            UNDERLINESPOLIER
            🍁

            @Kook.Net Test#0721
            @全体成员
            @在线成员
            INLINE CODE
            csharp
            CODE BLOCK

            """;
        Cacheable<IUserMessage, Guid> cacheable = await _channel.SendTextAsync(kMarkdownSourceContent);
        IUserMessage? message = await cacheable.GetOrDownloadAsync();
        try
        {
            selfUser = await _guild.GetCurrentUserAsync();
            Assert.NotNull(selfUser);
            Assert.NotEqual(Guid.Empty, cacheable.Id);
            Assert.False(cacheable.HasValue);
            Assert.NotNull(message);
            Assert.Equal(MessageType.KMarkdown, message.Type);
            Assert.Equal(kMarkdownParsedContent, message.Content);
            Assert.Equal(kMarkdownCleanContent, message.CleanContent);
            Assert.Equal(selfUser.Id, message.Author.Id);
            Assert.Equal(MessageSource.Bot, message.Source);
            Assert.Equal(4, message.Tags.Count);
            List<ITag> tags = message.Tags.ToList();
            Assert.Equal(_channel.Id, tags.Single(tag => tag.Type == TagType.ChannelMention).Key);
            Assert.Equal(selfUser.Id, tags.Single(tag => tag.Type == TagType.UserMention).Key);
            Assert.Equal(0U, tags.Single(tag => tag.Type == TagType.EveryoneMention).Key);
            Assert.Equal(0U, tags.Single(tag => tag.Type == TagType.HereMention).Key);
        }
        finally
        {
            if (message != null)
                await message.DeleteAsync();
        }
    }

    [Fact]
    public async Task ReactionsAsync()
    {
        IGuildUser? currentUser = await _guild.GetCurrentUserAsync();
        Cacheable<IUserMessage, Guid> cacheable = await _channel.SendTextAsync("TEST MESSAGE");
        RestMessage? message = await cacheable.GetOrDownloadAsync() as RestMessage;
        try
        {
            Assert.NotNull(message);
            Assert.NotNull(currentUser);

            Assert.Empty(message.Reactions);

            Emoji emoji = Emoji.Parse(":+1:");
            await message.AddReactionAsync(emoji);
            await message.UpdateAsync();
            Assert.Single(message.Reactions);
            Assert.Equal(emoji, message.Reactions.First().Key);
            IReadOnlyCollection<IUser> reactionUsers = await message.GetReactionUsersAsync(emoji);
            Assert.Single(reactionUsers);
            Assert.Equal(currentUser.Id, reactionUsers.First().Id);

            await message.RemoveReactionAsync(emoji, currentUser);
            await message.UpdateAsync();
            Assert.Empty(message.Reactions);
            reactionUsers = await message.GetReactionUsersAsync(emoji);
            Assert.Empty(reactionUsers);
        }
        finally
        {
            if (message != null)
                await message.DeleteAsync();
        }
    }
}

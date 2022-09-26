using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

/// <summary>
///     Tests that channels can be created and modified.
/// </summary>
[CollectionDefinition(nameof(MessageTests), DisableParallelization = true)]
[Trait("Category", "Integration")]
public class MessageTests : IClassFixture<RestChannelFixture>
{
    
    private readonly IGuild _guild;
    private readonly ITextChannel _channel;
    private readonly IGuildUser _selfUser;
    private readonly ITestOutputHelper _output;

    public MessageTests(RestChannelFixture channelFixture, ITestOutputHelper output)
    {
        _guild = channelFixture.Guild;
        _channel = channelFixture.TextChannel;
        _selfUser = _guild.GetCurrentUserAsync().GetAwaiter().GetResult();
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
        string kMarkdownSourceContent = @$"*TEST* **KMARKDOWN** ~~MESSAGE~~
> NOTHING
[INLINE LINK](https://kooknet.dev)
(ins)UNDERLINE(ins)(spl)SPOLIER(spl)
:maple_leaf:
{_channel.KMarkdownMention}
{_selfUser.KMarkdownMention}
{_guild.EveryoneRole.KMarkdownMention}
(met)here(met)
`INLINE CODE`
```csharp
CODE BLOCK
```";
        //  Emoji will be replaced with unicode emoji instead of the name.
        string kMarkdownParsedContent = @$"*TEST* **KMARKDOWN** ~~MESSAGE~~
> NOTHING
[INLINE LINK](https://kooknet.dev)
(ins)UNDERLINE(ins)(spl)SPOLIER(spl)
🍁
{_channel.KMarkdownMention}
{_selfUser.KMarkdownMention}
{_guild.EveryoneRole.KMarkdownMention}
(met)here(met)
`INLINE CODE`
```csharp
CODE BLOCK
```";
        // Due to the client is REST, the channel name will not be parsed.
        // Hence, the channel name will be displayed as empty string.
        string kMarkDownCleanContent = @"TEST KMARKDOWN MESSAGE
 NOTHING
[INLINE LINK](https://kooknet.dev)
UNDERLINESPOLIER
🍁

@Kook.Net Test#0721
全体成员
在线成员
INLINE CODE
csharp
CODE BLOCK
";
        Cacheable<IUserMessage, Guid> cacheable = await _channel.SendTextAsync(kMarkdownSourceContent);
        IUserMessage message = await cacheable.GetOrDownloadAsync();
        try
        {
            IGuildUser selfUser = await _guild.GetCurrentUserAsync();
            Assert.NotNull(selfUser);
            Assert.NotEqual(Guid.Empty, cacheable.Id);
            Assert.False(cacheable.HasValue);
            Assert.NotNull(message);
            Assert.Equal(MessageType.KMarkdown, message.Type);
            Assert.Equal(kMarkdownParsedContent, message.Content);
            Assert.Equal(kMarkDownCleanContent, message.CleanContent);
            Assert.Equal(selfUser.Id, message.Author.Id);
            Assert.Equal(MessageSource.Bot, message.Source);
            Assert.Equal(4, message.Tags.Count);
            List<ITag> tags = message.Tags.ToList();
            Assert.Equal(tags.Single(tag => tag.Type == TagType.ChannelMention).Key, _channel.Id);
            Assert.Equal(tags.Single(tag => tag.Type == TagType.UserMention).Key, _selfUser.Id);
            Assert.Equal(tags.Single(tag => tag.Type == TagType.EveryoneMention).Key, 0);
            Assert.Equal(tags.Single(tag => tag.Type == TagType.HereMention).Key, 0);
        }
        finally
        {
            await message.DeleteAsync();
        }
    }

}
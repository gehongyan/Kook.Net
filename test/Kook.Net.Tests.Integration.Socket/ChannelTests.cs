using System;
using System.Threading.Tasks;
using Kook.Rest;
using Kook.WebSocket;
using Xunit;
using Xunit.Abstractions;

namespace Kook;

[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
[CollectionDefinition(nameof(MessageTests), DisableParallelization = true)]
[Trait("Category", "Integration.Socket")]
public class ChannelTests : IClassFixture<SocketGuildFixture>, IClassFixture<ChannelTestFixture>
{
    private readonly ChannelTestFixture _channelFixture;
    private readonly ITestOutputHelper _output;
    private readonly KookSocketClient _client;
    private readonly SocketGuild _guild;

    public ChannelTests(SocketGuildFixture guildFixture, ChannelTestFixture channelFixture, ITestOutputHelper output)
    {
        _channelFixture = channelFixture;
        _output = output;
        _guild = guildFixture.Guild;
        _client = guildFixture.Client;
        channelFixture.EnsureLogger(_client, LogAsync);
    }

    private Task LogAsync(LogMessage message)
    {
        _output.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    [Fact]
    [TestPriority(1)]
    public async Task AddCategoryAsync()
    {
        TaskCompletionSource<SocketCategoryChannel> promise = new();
        _client.ChannelCreated += OnChannelCreated;
        const string categoryName = "CREATED CATEGORY";
        RestCategoryChannel restCategory = await _guild.CreateCategoryChannelAsync(categoryName);
        ulong categoryId = restCategory.Id;
        SocketCategoryChannel categoryChannel = await promise.Task.WithTimeout();
        Assert.Equal(categoryId, categoryChannel.Id);
        Assert.Equal(categoryName, categoryChannel.Name);

        // Prepare another category
        promise = new TaskCompletionSource<SocketCategoryChannel>();
        const string anotherCategoryName = "ANOTHER CATEGORY";
        await _guild.CreateCategoryChannelAsync(anotherCategoryName);
        SocketCategoryChannel anotherCategory = await promise.Task.WithTimeout();

        // Clean up
        _channelFixture.Category = categoryChannel;
        _channelFixture.AnotherCategory = anotherCategory;
        _client.ChannelCreated -= OnChannelCreated;
        return;

        Task OnChannelCreated(SocketChannel argChannel)
        {
            if (argChannel is SocketCategoryChannel category)
                promise.SetResult(category);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(2)]
    public async Task ModifyCategoryAsync()
    {
        Assert.NotNull(_channelFixture.Category);
        SocketCategoryChannel categoryChannel = _channelFixture.Category;
        TaskCompletionSource<SocketCategoryChannel> categoryBefore = new();
        TaskCompletionSource<SocketCategoryChannel> categoryAfter = new();
        string previousName = categoryChannel.Name;
        int? previousPosition = categoryChannel.Position;
        _client.ChannelUpdated += OnChannelUpdated;
        const string newCategoryName = "MODIFIED CATEGORY";
        const int newPosition = 500;
        await categoryChannel.ModifyAsync(x =>
        {
            x.Name = newCategoryName;
            x.Position = newPosition;
        });
        SocketCategoryChannel before = await categoryBefore.Task.WithTimeout();
        SocketCategoryChannel after = await categoryAfter.Task.WithTimeout();
        Assert.Same(categoryChannel, after);
        Assert.Equal(categoryChannel.Id, before.Id);
        Assert.Equal(categoryChannel.Id, after.Id);
        Assert.Equal(previousName, before.Name);
        Assert.Equal(newCategoryName, after.Name);
        Assert.Equal(previousPosition, before.Position);
        Assert.Equal(newPosition, after.Position);

        // Clean up
        _client.ChannelUpdated -= OnChannelUpdated;
        return;

        Task OnChannelUpdated(SocketChannel argBefore, SocketChannel argAfter)
        {
            if (argBefore is SocketCategoryChannel argCategoryBefore)
                categoryBefore.SetResult(argCategoryBefore);
            if (argAfter is SocketCategoryChannel argCategoryAfter)
                categoryAfter.SetResult(argCategoryAfter);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(3)]
    public async Task AddTextChannelAsync()
    {
        Assert.NotNull(_channelFixture.Category);
        SocketCategoryChannel categoryChannel = _channelFixture.Category;
        TaskCompletionSource<SocketTextChannel> promise = new();
        _client.ChannelCreated += OnChannelCreated;
        const string channelName = "CREATED TEXT CHANNEL";
        RestTextChannel restChannel = await _guild.CreateTextChannelAsync(channelName,
            x => x.CategoryId = categoryChannel.Id);
        ulong channelId = restChannel.Id;
        SocketTextChannel textChannel = await promise.Task.WithTimeout();
        Assert.Equal(channelId, textChannel.Id);
        Assert.Equal(channelName, textChannel.Name);
        Assert.Equal(categoryChannel.Id, textChannel.CategoryId);

        // Clean up
        _channelFixture.TextChannel = textChannel;
        _client.ChannelCreated -= OnChannelCreated;
        return;

        Task OnChannelCreated(SocketChannel argChannel)
        {
            if (argChannel is SocketTextChannel text)
                promise.SetResult(text);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(4)]
    public async Task ModifyTextChannelAsync()
    {
        Assert.NotNull(_channelFixture.Category);
        Assert.NotNull(_channelFixture.AnotherCategory);
        Assert.NotNull(_channelFixture.TextChannel);
        SocketTextChannel textChannel = _channelFixture.TextChannel;
        TaskCompletionSource<SocketTextChannel> channelBefore = new();
        TaskCompletionSource<SocketTextChannel> channelAfter = new();
        string previousName = textChannel.Name;
        int? previousPosition = textChannel.Position;
        string topic = textChannel.Topic;
        int slowModeInterval = textChannel.SlowModeInterval;
        _client.ChannelUpdated += OnChannelUpdated;
        const string newTextChannelName = "MODIFIED TEXT CHANNEL";
        const int newPosition = 500;
        const string newTopic = "MODIFIED TOPIC";
        const SlowModeInterval newSlowModeInterval = SlowModeInterval._1800;
        await textChannel.ModifyAsync(x =>
        {
            x.Name = newTextChannelName;
            x.Position = newPosition;
            x.CategoryId = _channelFixture.AnotherCategory.Id;
            x.Topic = newTopic;
            x.SlowModeInterval = newSlowModeInterval;
        });
        SocketTextChannel before = await channelBefore.Task.WithTimeout();
        SocketTextChannel after = await channelAfter.Task.WithTimeout();
        Assert.Same(textChannel, after);
        Assert.Equal(textChannel.Id, before.Id);
        Assert.Equal(textChannel.Id, after.Id);
        Assert.Equal(previousName, before.Name);
        Assert.Equal(newTextChannelName, after.Name);
        Assert.Equal(previousPosition, before.Position);
        Assert.Equal(newPosition, after.Position);
        Assert.Equal(_channelFixture.Category.Id, before.CategoryId);
        Assert.Equal(_channelFixture.AnotherCategory.Id, after.CategoryId);
        Assert.Equal(topic, before.Topic);
        Assert.Equal(newTopic, after.Topic);
        Assert.Equal(slowModeInterval, before.SlowModeInterval);
        Assert.Equal((int)newSlowModeInterval, after.SlowModeInterval);

        // Clean up
        _client.ChannelUpdated -= OnChannelUpdated;
        return;

        Task OnChannelUpdated(SocketChannel argBefore, SocketChannel argAfter)
        {
            if (argBefore is SocketTextChannel argChannelBefore)
                channelBefore.SetResult(argChannelBefore);
            if (argAfter is SocketTextChannel argChannelAfter)
                channelAfter.SetResult(argChannelAfter);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(5)]
    public async Task DeleteTextChannelAsync()
    {
        Assert.NotNull(_channelFixture.TextChannel);
        SocketTextChannel textChannel = _channelFixture.TextChannel;
        TaskCompletionSource<SocketTextChannel> promise = new();
        _client.ChannelDestroyed += OnChannelDestroyed;
        await textChannel.DeleteAsync();
        SocketTextChannel deletedChannel = await promise.Task.WithTimeout();
        Assert.Equal(textChannel.Id, deletedChannel.Id);
        Assert.Same(textChannel, deletedChannel);
        Assert.Null(_guild.GetChannel(deletedChannel.Id));

        // Clean up
        _channelFixture.TextChannel = null;
        _client.ChannelDestroyed -= OnChannelDestroyed;
        return;

        Task OnChannelDestroyed(SocketChannel argChannel)
        {
            if (argChannel is SocketTextChannel text)
                promise.SetResult(text);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(6)]
    public async Task AddVoiceChannelAsync()
    {
        Assert.NotNull(_channelFixture.Category);
        SocketCategoryChannel categoryChannel = _channelFixture.Category;
        TaskCompletionSource<SocketVoiceChannel> promise = new();
        _client.ChannelCreated += OnChannelCreated;
        const string channelName = "CREATED VOICE CHANNEL";
        RestVoiceChannel restChannel = await _guild.CreateVoiceChannelAsync(channelName,
            x => x.CategoryId = categoryChannel.Id);
        ulong channelId = restChannel.Id;
        SocketVoiceChannel voiceChannel = await promise.Task.WithTimeout();
        Assert.Equal(channelId, voiceChannel.Id);
        Assert.Equal(channelName, voiceChannel.Name);
        Assert.Equal(categoryChannel.Id, voiceChannel.CategoryId);

        // Clean up
        _channelFixture.VoiceChannel = voiceChannel;
        _client.ChannelCreated -= OnChannelCreated;
        return;

        Task OnChannelCreated(SocketChannel argChannel)
        {
            if (argChannel is SocketVoiceChannel voice)
                promise.SetResult(voice);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(7)]
    public async Task ModifyVoiceChannelAsync()
    {
        Assert.NotNull(_channelFixture.Category);
        Assert.NotNull(_channelFixture.AnotherCategory);
        Assert.NotNull(_channelFixture.VoiceChannel);
        SocketVoiceChannel voiceChannel = _channelFixture.VoiceChannel;
        TaskCompletionSource<SocketVoiceChannel> channelBefore = new();
        TaskCompletionSource<SocketVoiceChannel> channelAfter = new();
        string name = voiceChannel.Name;
        // int? position = voiceChannel.Position;
        string topic = voiceChannel.Topic;
        int slowModeInterval = voiceChannel.SlowModeInterval;
        VoiceQuality? voiceQuality = voiceChannel.VoiceQuality;
        int userLimit = voiceChannel.UserLimit;
        bool hasPassword = voiceChannel.HasPassword;
        bool? overwriteVoiceRegion = voiceChannel.IsVoiceRegionOverwritten;
        string? voiceRegion = voiceChannel.VoiceRegion;
        _client.ChannelUpdated += OnChannelUpdated;
        const string newVoiceChannelName = "MODIFIED VOICE CHANNEL";
        // const int newPosition = 500;
        const string newTopic = "MODIFIED TOPIC";
        const SlowModeInterval newSlowModeInterval = SlowModeInterval._1800;
        const VoiceQuality newVoiceQuality = VoiceQuality._96kbps;
        const int newUserLimit = 10;
        const string newPassword = "12345678";
        const bool newOverwriteVoiceRegion = true;
        const string newVoiceRegion = "shenzhen";
        await voiceChannel.ModifyAsync(x =>
        {
            x.Name = newVoiceChannelName;
            // x.Position = newPosition;
            x.CategoryId = _channelFixture.AnotherCategory.Id;
            x.Topic = newTopic;
            x.SlowModeInterval = newSlowModeInterval;
            x.VoiceQuality = newVoiceQuality;
            x.UserLimit = newUserLimit;
            x.Password = newPassword;
            x.OverwriteVoiceRegion = newOverwriteVoiceRegion;
            x.VoiceRegion = newVoiceRegion;
        });
        SocketVoiceChannel before = await channelBefore.Task.WithTimeout();
        SocketVoiceChannel after = await channelAfter.Task.WithTimeout();
        Assert.Same(voiceChannel, after);
        Assert.Equal(voiceChannel.Id, before.Id);
        Assert.Equal(voiceChannel.Id, after.Id);
        Assert.Equal(name, before.Name);
        Assert.Equal(newVoiceChannelName, after.Name);
        // Assert.Equal(position, before.Position);
        // Assert.Equal(newPosition, after.Position);
        Assert.Equal(_channelFixture.Category.Id, before.CategoryId);
        Assert.Equal(_channelFixture.AnotherCategory.Id, after.CategoryId);
        Assert.Equal(topic, before.Topic);
        Assert.Equal(newTopic, after.Topic);
        Assert.Equal(slowModeInterval, before.SlowModeInterval);
        Assert.Equal((int)newSlowModeInterval, after.SlowModeInterval);
        Assert.Equal(voiceQuality, before.VoiceQuality);
        // TODO: Gateway event does not have voice quality
        // Assert.Equal(newVoiceQuality, after.VoiceQuality);
        Assert.Equal(userLimit, before.UserLimit);
        Assert.Equal(newUserLimit, after.UserLimit);
        Assert.Equal(hasPassword, before.HasPassword);
        Assert.True(after.HasPassword);
        Assert.Equal(overwriteVoiceRegion, before.IsVoiceRegionOverwritten);
        Assert.True(after.IsVoiceRegionOverwritten);
        Assert.Equal(voiceRegion, before.VoiceRegion);
        Assert.Equal(newVoiceRegion, after.VoiceRegion);

        // Clean up
        _client.ChannelUpdated -= OnChannelUpdated;
        return;

        Task OnChannelUpdated(SocketChannel argBefore, SocketChannel argAfter)
        {
            if (argBefore is SocketVoiceChannel argChannelBefore)
                channelBefore.SetResult(argChannelBefore);
            if (argAfter is SocketVoiceChannel argChannelAfter)
                channelAfter.SetResult(argChannelAfter);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(8)]
    public async Task DeleteVoiceChannelAsync()
    {
        Assert.NotNull(_channelFixture.VoiceChannel);
        SocketVoiceChannel voiceChannel = _channelFixture.VoiceChannel;
        TaskCompletionSource<SocketVoiceChannel> promise = new();
        _client.ChannelDestroyed += OnChannelDestroyed;
        await voiceChannel.DeleteAsync();
        SocketVoiceChannel deletedChannel = await promise.Task.WithTimeout();
        Assert.Equal(voiceChannel.Id, deletedChannel.Id);
        Assert.Same(voiceChannel, deletedChannel);
        Assert.Null(_guild.GetChannel(deletedChannel.Id));

        // Clean up
        _channelFixture.VoiceChannel = null;
        _client.ChannelDestroyed -= OnChannelDestroyed;
        return;

        Task OnChannelDestroyed(SocketChannel argChannel)
        {
            if (argChannel is SocketVoiceChannel voice)
                promise.SetResult(voice);
            return Task.CompletedTask;
        }
    }

    [Fact]
    [TestPriority(9)]
    public async Task DeleteCategoryChannelAsync()
    {
        Assert.NotNull(_channelFixture.Category);
        SocketCategoryChannel categoryChannel = _channelFixture.Category;
        TaskCompletionSource<SocketCategoryChannel> promise = new();
        _client.ChannelDestroyed += OnChannelDestroyed;
        await categoryChannel.DeleteAsync();
        SocketCategoryChannel deletedChannel = await promise.Task.WithTimeout();
        Assert.Equal(categoryChannel.Id, deletedChannel.Id);
        Assert.Same(categoryChannel, deletedChannel);
        Assert.Null(_guild.GetChannel(deletedChannel.Id));

        // Clean up
        _channelFixture.Category = null;
        _client.ChannelDestroyed -= OnChannelDestroyed;
        return;

        Task OnChannelDestroyed(SocketChannel argChannel)
        {
            if (argChannel is SocketCategoryChannel category)
                promise.SetResult(category);
            return Task.CompletedTask;
        }
    }

}

public class ChannelTestFixture : IAsyncDisposable
{
    private Action? _loggerDisposer;

    private bool _loggerSubscribed;
    public SocketCategoryChannel? Category { get; set; }
    public SocketCategoryChannel? AnotherCategory { get; set; }
    public SocketTextChannel? TextChannel { get; set; }
    public SocketVoiceChannel? VoiceChannel { get; set; }

    public void EnsureLogger(KookSocketClient client, Func<LogMessage, Task> logAction)
    {
        if (_loggerSubscribed) return;
        client.Log += logAction;
        _loggerDisposer += () => client.Log -= logAction;
        _loggerSubscribed = true;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (TextChannel is not null)
            await TextChannel.DeleteAsync();
        if (VoiceChannel is not null)
            await VoiceChannel.DeleteAsync();
        if (Category is not null)
            await Category.DeleteAsync();
        _loggerDisposer?.Invoke();
    }
}

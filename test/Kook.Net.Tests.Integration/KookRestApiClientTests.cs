using Kook.API;
using Kook.API.Rest;
using Kook.Net;
using Kook.Rest;
using FluentAssertions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

[CollectionDefinition(nameof(KookRestApiClientTests), DisableParallelization = true)]
[Trait("Category", "Integration")]
public class KookRestApiClientTests : IClassFixture<RestGuildFixture>, IAsyncDisposable
{
    private readonly KookRestApiClient _apiClient;
    // private readonly IGuild _guild;
    // private readonly ITextChannel _channel;

    public KookRestApiClientTests(RestGuildFixture guildFixture)
    {
        // _guild = guildFixture.Guild;
        _apiClient = guildFixture.Client.ApiClient;
        // _channel = _guild.CreateTextChannelAsync("TEST TEXT CHANNEL").GetAwaiter().GetResult();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
        // await _channel.DeleteAsync();
    }

    [Fact]
    public async Task CreateAsset_WithMaximumSize_DontThrowsException()
    {
        ulong fileSize = (ulong) (30 * Math.Pow(2, 20));
        using MemoryStream stream = new(new byte[fileSize]);
        CreateAssetResponse response =
            await _apiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = "test.file"});
        response.Url.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateAsset_WithOverSize_ThrowsException()
    {

        Func<Task> upload = async () =>
        {
            ulong fileSize = (ulong) (30 * Math.Pow(2, 20)) + 1;
            using MemoryStream stream = new(new byte[fileSize]);
            await _apiClient.CreateAssetAsync(new CreateAssetParams {File = stream, FileName = "test.file"});
        };

        await upload.Should().ThrowExactlyAsync<HttpException>()
            .Where(e => e.KookCode == KookErrorCode.RequestEntityTooLarge);
    }
}

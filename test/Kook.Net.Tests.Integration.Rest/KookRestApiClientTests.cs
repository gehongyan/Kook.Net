using Kook.API;
using Kook.API.Rest;
using Kook.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Kook;

[CollectionDefinition(nameof(KookRestApiClientTests), DisableParallelization = true)]
[Trait("Category", "Integration.Rest")]
public class KookRestApiClientTests : IClassFixture<KookRestClientFixture>, IAsyncDisposable
{
    private readonly KookRestApiClient _apiClient;

    public KookRestApiClientTests(KookRestClientFixture clientFixture)
    {
        _apiClient = clientFixture.Client.ApiClient;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task CreateAsset_WithMaximumSize_DontThrowsException()
    {
        ulong fileSize = (ulong)(30 * Math.Pow(2, 20));
        using MemoryStream stream = new(new byte[fileSize]);
        CreateAssetResponse response =
            await _apiClient.CreateAssetAsync(new CreateAssetParams
            {
                File = stream,
                FileName = "test.file"
            });
        Assert.False(string.IsNullOrWhiteSpace(response.Url));
    }

    [Fact]
    public async Task CreateAsset_WithOverSize_ThrowsException()
    {
        HttpException exception = await Assert.ThrowsAsync<HttpException>(async () =>
        {
            ulong fileSize = (ulong)(30 * Math.Pow(2, 20)) + 1;
            using MemoryStream stream = new(new byte[fileSize]);
            await _apiClient.CreateAssetAsync(new CreateAssetParams
            {
                File = stream,
                FileName = "test.file"
            });
        });
        Assert.Equal(KookErrorCode.RequestEntityTooLarge, exception.KookCode);
    }
}

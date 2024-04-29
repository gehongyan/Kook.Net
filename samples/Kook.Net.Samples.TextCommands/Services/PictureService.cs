namespace Kook.Net.Samples.TextCommands.Services;

public class PictureService(IHttpClientFactory httpClientFactory)
{
    public async Task<Stream> GetCatPictureAsync()
    {
        HttpClient httpClient = httpClientFactory.CreateClient("Pictures");
        HttpResponseMessage resp = await httpClient.GetAsync("https://cataas.com/cat");
        return await resp.Content.ReadAsStreamAsync();
    }
}

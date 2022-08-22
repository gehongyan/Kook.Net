using System.Net.Http.Headers;
using HtmlAgilityPack;

namespace Kook.Net.Samples.ApiHelper.Extensions;

public static class ApiSearchHelper
{
    public static async Task<IEnumerable<(string Name, string Link)>> GetResult(string name)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://kaiheila.net/");
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/toc.html");
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var responseString = response.Content.ReadAsStringAsync().Result;
        var html = new HtmlDocument();
        html.LoadHtml(responseString);
        var nodes = html.DocumentNode.SelectNodes("//a[@class='sidebar-item']");
        var href = nodes.Select(x => (Name: x.InnerText, Link: x.Attributes["href"]?.Value));
        var result = href.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                                     || x.Link.Contains(name, StringComparison.OrdinalIgnoreCase)).AsEnumerable();  
        return result;
    }
}
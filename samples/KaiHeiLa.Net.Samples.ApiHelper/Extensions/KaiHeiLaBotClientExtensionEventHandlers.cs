using KaiHeiLa.WebSocket;

namespace KaiHeiLa.Net.Samples.ApiHelper.Extensions;

public partial class KaiHeiLaBotClientExtension
{
    private async Task ProcessApiHelperCommand(SocketMessage message)
    {
        if (message.Author.IsBot != false) return;
        if (!message.CleanContent.StartsWith("/api")) return;
        
        var api = message.CleanContent.Substring(5);
        var result = await ApiSearchHelper.GetResult(api);
    }
}
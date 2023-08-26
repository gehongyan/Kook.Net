public class ClientHandler
{
    private readonly KookSocketClient _client;

    public ClientHandler(KookSocketClient client)
    {
        _client = client;
    }

    public async Task ConfigureAsync()
    {
        //...
    }
}

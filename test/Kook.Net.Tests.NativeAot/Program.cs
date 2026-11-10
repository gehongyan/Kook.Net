using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook;
using Kook.API;
using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Net.Contexts;
using Kook.Net.Converters;
using Kook.Net.Rest;
using Kook.Net.Webhooks;
using Kook.Net.WebSockets;
using Kook.Rest;

Check(!JsonSerializer.IsReflectionEnabledByDefault, "JSON reflection must be disabled");

// Exercise public card APIs and the nested card/module/element converter graph.
const string card = """
{"type":"card","theme":"primary","size":"lg","modules":[
 {"type":"header","text":{"type":"plain-text","content":"AOT <测试>"}},
 {"type":"section","text":{"type":"paragraph","cols":2,"fields":[{"type":"kmarkdown","content":"**hello**"},{"type":"plain-text","content":"world"}]},"accessory":{"type":"button","theme":"primary","text":{"type":"plain-text","content":"OK"},"value":"yes"},"mode":"right"},
 {"type":"context","elements":[{"type":"image","src":"https://example.org/a.png"}]},
 {"type":"divider"}]}
""";
ICardBuilder parsed = CardJsonExtension.ParseSingle(card);
string roundTrip = parsed.Build().ToJsonString(false);
Check(roundTrip.Contains("AOT <测试>"), "card encoder and nested round trip");
Check(CardJsonExtension.ParseMany("[" + roundTrip + "]").Count() == 1, "card enumerable root");
Check(!CardJsonExtension.TryParseSingle("{\"type\":\"unsupported\"}", out _), "unknown card rejection");
Console.WriteLine("PASS cards and nested converters");

JsonSerializerOptions protocol = new(KookRestJsonSerializerContext.Default.Options)
{
    Converters = { CardConverterFactory.Instance }
};
protocol.TypeInfoResolverChain.Add(KookWebSocketJsonSerializerContext.Default);
var embed = JsonSerializer.Deserialize("{\"type\":\"image\",\"origin_url\":\"source\",\"url\":\"image\"}", protocol.GetTypedTypeInfo<Kook.API.EmbedBase>());
Check(embed is Kook.API.ImageEmbed { Url: "image" }, "embed nested metadata");
var unknown = JsonSerializer.Deserialize("{\"type\":\"future\",\"Value\":\"custom\"}", protocol.GetTypedTypeInfo<Kook.API.EmbedBase>());
Check(unknown is Kook.API.NotImplementedEmbed, "unknown embed preservation");
var custom = new Kook.NotImplementedEmbed("future", JsonNode.Parse("{\"Value\":\"custom\"}")!);
Check(custom.Resolve(CustomContext.Default.CustomEmbed)?.Value == "custom", "typed user embed metadata");
var poke = new Kook.NotImplementedPokeResource("future", JsonNode.Parse("{\"Value\":\"custom\"}")!);
Check(poke.Resolve(CustomContext.Default.CustomPoke)?.Value == "custom", "typed user poke metadata");
var userOptions = JsonSerializerOptionsExtensions.GetUserSerializerOptions(new() { TypeInfoResolver = CustomContext.Default });
Check(JsonSerializer.Serialize(new Template { Value = "template" }, userOptions.GetTypedTypeInfo<Template>()).Contains("template"), "user template options");
try
{
    JsonSerializerOptionsExtensions.GetUserSerializerOptions(null);
    throw new Exception("Missing custom metadata was accepted");
}
catch (NotSupportedException e) when (e.Message.Contains("TypeInfoResolver")) { }
// Closed generic and array roots used by REST pagination and gateway batch dispatch.
Check(protocol.GetTypeInfo(typeof(PagedResponseBase<Kook.API.Guild>)) is not null, "paged guild metadata");
Check(protocol.GetTypeInfo(typeof(Kook.API.Channel[])) is not null, "channel batch metadata");
Check(protocol.GetTypeInfo(typeof(ChannelBatchDeleteEventItem[])) is not null, "delete batch metadata");
var page = JsonSerializer.Deserialize("{\"items\":[],\"meta\":{\"page\":1,\"page_total\":1,\"page_size\":50,\"total\":0},\"sort\":{}}", protocol.GetTypedTypeInfo<PagedResponseBase<Kook.API.Guild>>());
Check(page?.Items.Length == 0, "paged response converters");
var direct = JsonSerializer.Deserialize("{\"channel_type\":\"PERSON\",\"content\":\"hi\",\"extra\":{\"type\":1,\"author\":null,\"quote\":\"\",\"channel\":[]}}", protocol.GetTypedTypeInfo<GatewayEvent<GatewayPersonMessageExtraData>>());
Check(direct?.Content == "hi", "direct gateway quote and nullable channel");
var batchEmpty = JsonSerializer.Deserialize("{\"added_channel\":[],\"updated_channel\":[]}", protocol.GetTypedTypeInfo<ChannelBatchUpdateEvent>());
Check(batchEmpty?.AddedChannel is null, "nullable channel empty array");
var batch = JsonSerializer.Deserialize("{\"added_channel\":{\"name\":\"channel\",\"permission_overwrites\":[],\"permission_users\":[]},\"updated_channel\":[]}", protocol.GetTypedTypeInfo<ChannelBatchUpdateEvent>());
Check(batch?.AddedChannel?.Name == "channel", "nullable channel object nested metadata");
var pokeModel = JsonSerializer.Deserialize("{\"type\":\"ImageAnimation\",\"preview_expired\":\"\",\"webp\":\"\",\"pag\":\"\",\"gif\":\"\",\"time\":1,\"width\":1,\"height\":1,\"percent\":100}", protocol.GetTypedTypeInfo<Kook.API.PokeResourceBase>());
Check(pokeModel is Kook.API.ImageAnimationPokeResource, "poke converter metadata");
Console.WriteLine("PASS protocol metadata and application-owned types");

using var handler = new StubHttpHandler();
using var http = new HttpClient(handler);
RestClientProvider rest = url => new DefaultRestClient(url, httpClientFactory: () => http);
using (var api = new KookRestApiClient(rest, "aot-smoke", "en-US"))
{
    await api.LoginAsync(TokenType.Bot, "offline-smoke-token");
    handler.Data = "{\"url\":\"wss://example.org/gateway\"}";
    Check((await api.GetBotGatewayAsync()).Url == "wss://example.org/gateway", "REST response and bucket expression");
    handler.Data = "{\"msg_id\":\"11111111-1111-1111-1111-111111111111\",\"msg_timestamp\":1700000000000}";
    var sent = await api.CreateMessageAsync(new CreateMessageParams { Type = MessageType.Text, ChannelId = 42, Content = "AOT <测试>" });
    Check(sent.MessageId != Guid.Empty && handler.LastBody!.Contains("AOT <测试>"), "REST request metadata and encoder");
    Check(!handler.LastBody!.Contains("temp_target_id"), "optional request fields omitted");
    handler.Status = HttpStatusCode.BadRequest;
    handler.Code = 40000;
    try
    {
        await api.GetBotGatewayAsync();
        throw new Exception("REST error was accepted");
    }
    catch (Kook.Net.HttpException) { }
}
handler.Status = HttpStatusCode.OK;
handler.Code = 0;
using (var client = new KookRestClient(new KookRestConfig { RestClientProvider = rest }))
{
    await client.ApiClient.LoginAsync(TokenType.Bot, "offline-smoke-token");
    handler.Data = "{\"msg_id\":\"11111111-1111-1111-1111-111111111111\",\"msg_timestamp\":1700000000000}";
    var guild = new RestGuild(client, 1);
    var channel = new RestTextChannel(client, guild, 42);
    JsonSerializerOptions templateOptions = new() { TypeInfoResolver = CustomContext.Default };
    await channel.SendTextAsync(1, new Template { Value = "public-template" }, jsonSerializerOptions: templateOptions);
    using (var body = JsonDocument.Parse(handler.LastBody!))
        Check(body.RootElement.GetProperty("content").GetString() == "{\"Value\":\"public-template\"}", "public text template metadata");
    await channel.SendCardsAsync(1, new Template { Value = "public-card-template" }, jsonSerializerOptions: templateOptions);
    Check(handler.LastBody!.Contains("public-card-template"), "public card template metadata");
}
Console.WriteLine("PASS REST transport, requests, responses, errors and public templates");

var socket = new StubSocket();
using (var api = new KookSocketApiClient(rest, () => socket, "aot-smoke", "en-US", "wss://example.org/offline"))
{
    int received = 0;
    api.ReceivedGatewayEvent += (kind, sequence, payload, extra) => { Check(kind == GatewaySocketFrameType.Hello, "gateway kind"); received++; return Task.CompletedTask; };
    const string frame = "{\"s\":1,\"d\":{\"code\":0,\"session_id\":\"11111111-1111-1111-1111-111111111111\"}}";
    await socket.EmitText(frame);
    await socket.EmitBinary(Compress(frame));
    Check(received == 2, "text and compressed gateway events");
    await api.LoginAsync(TokenType.Bot, "offline-smoke-token");
    await api.ConnectAsync(null, 0);
    await api.SendGatewayAsync(GatewaySocketFrameType.Ping, null, 7, new RequestOptions());
    Check(socket.LastSent?.Contains("\"s\":2") == true, "outbound gateway metadata");
}
Console.WriteLine("PASS WebSocket receive, compression and send");

var webhook = new StubWebhook();
using (var api = new KookWebhookApiClient(rest, () => new StubSocket(), () => webhook, "test-key", "verify", "aot-smoke", "en-US"))
{
    const string challenge = "{\"s\":0,\"d\":{\"type\":255,\"channel_type\":\"WEBHOOK_CHALLENGE\",\"verify_token\":\"verify\",\"challenge\":\"aot-challenge\"}}";
    string encrypted = Encrypt(challenge);
    Check(await webhook.HandleTextMessageAsync(encrypted) == "{\"challenge\":\"aot-challenge\"}", "encrypted text challenge");
    byte[] compressed = Compress(encrypted);
    Check(await webhook.HandleBinaryMessageAsync(compressed, 0, compressed.Length) == "{\"challenge\":\"aot-challenge\"}", "encrypted compressed challenge");
    try
    {
        await webhook.HandleTextMessageAsync(Encrypt(challenge.Replace("\"verify\"", "\"wrong\"")));
        throw new Exception("Invalid verify token accepted");
    }
    catch (InvalidOperationException e) when (e.Message.Contains("verify token")) { }
    int events = 0;
    api.ReceivedGatewayEvent += (_, _, _, _) => { events++; return Task.CompletedTask; };
    await webhook.HandleTextMessageAsync(Encrypt("{\"s\":0,\"sn\":1,\"d\":{\"type\":1,\"content\":\"hi\"}}"));
    Check(events == 1, "webhook event forwarding");
}
Console.WriteLine("PASS Webhook AES, compression, challenge, token rejection and events");
Console.WriteLine("ALL NATIVEAOT SMOKE CHECKS PASSED");

static void Check(bool condition, string name) { if (!condition) throw new Exception("FAILED: " + name); }
static byte[] Compress(string value)
{
    using var output = new MemoryStream();
    using (var zip = new ZLibStream(output, CompressionLevel.Fastest, true)) zip.Write(Encoding.UTF8.GetBytes(value));
    return output.ToArray();
}
static string Encrypt(string value)
{
    const string iv = "0123456789abcdef";
    using var aes = Aes.Create();
    aes.Key = Encoding.UTF8.GetBytes("test-key".PadRight(32, '\0'));
    aes.IV = Encoding.UTF8.GetBytes(iv);
    using var transform = aes.CreateEncryptor();
    byte[] plain = Encoding.UTF8.GetBytes(value);
    string cipher = Convert.ToBase64String(transform.TransformFinalBlock(plain, 0, plain.Length));
    string encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(iv + cipher));
    return "{\"encrypt\":\"" + encrypted + "\"}";
}

internal class Template { public string? Value { get; set; } }
internal class CustomEmbed : Kook.IEmbed { public EmbedType Type => EmbedType.NotImplemented; public string? Value { get; set; } }
internal class CustomPoke : Kook.IPokeResource { public PokeResourceType Type => PokeResourceType.NotImplemented; public string? Value { get; set; } }
[JsonSerializable(typeof(Template))]
[JsonSerializable(typeof(CustomEmbed))]
[JsonSerializable(typeof(CustomPoke))]
internal partial class CustomContext : JsonSerializerContext;

internal sealed class StubHttpHandler : HttpMessageHandler
{
    public string Data = "{}";
    public int Code;
    public HttpStatusCode Status = HttpStatusCode.OK;
    public string? LastBody;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        return new HttpResponseMessage(Status) { Content = new StringContent("{\"code\":" + Code + ",\"message\":\"offline\",\"data\":" + Data + "}", Encoding.UTF8, "application/json") };
    }
}
internal sealed class StubSocket : IWebSocketClient
{
    public event Func<byte[], int, int, Task>? BinaryMessage;
    public event Func<string, Task>? TextMessage;
    public event Func<Exception, Task>? Closed { add { } remove { } }
    public string? LastSent;
    public Task EmitText(string text) => TextMessage!(text);
    public Task EmitBinary(byte[] bytes) => BinaryMessage!(bytes, 0, bytes.Length);
    public void SetHeader(string key, string value) { }
    public void SetKeepAliveInterval(TimeSpan interval) { }
    public void SetCancellationToken(CancellationToken token) { }
    public Task ConnectAsync(string host) => Task.CompletedTask;
    public Task DisconnectAsync(int closeCode = 1000) => Task.CompletedTask;
    public Task SendAsync(byte[] bytes, int index, int count, bool isText) { LastSent = Encoding.UTF8.GetString(bytes, index, count); return Task.CompletedTask; }
    public void Dispose() { }
}
internal sealed class StubWebhook : IWebhookClient
{
    public event Func<byte[], int, int, Task<string?>>? BinaryMessage;
    public event Func<string, Task<string?>>? TextMessage;
    public event Func<Exception, Task>? Closed { add { } remove { } }
    public Task<string?> HandleTextMessageAsync(string text) => TextMessage!(text);
    public Task<string?> HandleBinaryMessageAsync(byte[] bytes, int index, int count) => BinaryMessage!(bytes, index, count);
    public void Dispose() { }
}

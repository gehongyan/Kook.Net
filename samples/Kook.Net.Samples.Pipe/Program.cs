using System.Text.Encodings.Web;
using System.Text.Json;
using Kook.Pipe;

KookPipeClient client = new("");
client.SentRequest += (method, endpoint, millis) =>
{
    Console.WriteLine($"[{DateTimeOffset.Now}] [{method}] {endpoint} ({millis}ms)");
    return Task.CompletedTask;
};
await client.SendTextAsync("ss");
await client.SendCardsAsync([]);
await client.SendTemplateAsync(new
{
    guildname = "KOOK开发者中心",
    username = "开发者",
    roles = new[] { "role1", "role2", "role3" }
}, new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
});

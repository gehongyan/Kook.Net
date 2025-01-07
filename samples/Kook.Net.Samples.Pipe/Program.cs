using Kook;
using Kook.Pipe;

KookPipeClient client = new("");
client.SentRequest += (method, endpoint, millis) =>
{
    Console.WriteLine($"[{DateTimeOffset.Now}] [{method}] {endpoint} ({millis}ms)");
    return Task.CompletedTask;
};
await client.SendCardAsync(new CardBuilder()
    .AddModule(new HeaderModuleBuilder("Hello, World!"))
    .WithTheme(CardTheme.Invisible)
    .Build());

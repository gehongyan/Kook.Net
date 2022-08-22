// Program.cs

var listener = services.GetRequiredService<KookEventListener>();
await listener.StartAsync();
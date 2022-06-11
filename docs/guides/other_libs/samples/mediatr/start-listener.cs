// Program.cs

var listener = services.GetRequiredService<KaiHeiLaEventListener>();
await listener.StartAsync();
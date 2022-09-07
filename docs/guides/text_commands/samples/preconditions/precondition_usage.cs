[RequireOwner]
[Command("echo")]
public Task EchoAsync(string input) => ReplyTextAsync(input);
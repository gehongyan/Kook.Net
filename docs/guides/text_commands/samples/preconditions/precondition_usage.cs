[RequireOwner]
[Command("echo")]
public Task EchoAsync(string input) => ReplyKMarkdownAsync(input);
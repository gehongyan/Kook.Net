[RequireBotPermission(ChannelPermission.SendMessages)]
[Command("echo")]
public Task EchoAsync(string input) => ReplyTextAsync(input);

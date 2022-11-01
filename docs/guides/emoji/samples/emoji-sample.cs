public async Task ReactAsync(SocketUserMessage userMsg)
{
    // 使用表情符号本身
    await userMsg.AddReactionAsync(new Emoji("👌"));
    // 使用 Unicode
    await userMsg.AddReactionAsync(new Emoji("\uD83D\uDC4C"));
    // 使用短代码
    await userMsg.AddReactionAsync(Emoji.Parse(":ok_hand:"));
}
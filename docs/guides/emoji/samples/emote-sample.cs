public async Task ReactAsync(SocketUserMessage userMsg)
{
    if (Emote.TryParse(emoteString, out var emote))
    {
        await userMsg.AddReactionAsync(emote);
    }
}
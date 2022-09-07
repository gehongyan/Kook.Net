[Group("admin")]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    [Group("ban")]
    public class CleanModule : ModuleBase<SocketCommandContext>
    {
        // !admin ban @戈小荷
        [Command]
        public Task BanAsync(IGuildUser user) => 
            Context.Guild.AddBanAsync(user);
        
        // !admin ban clean @戈小荷 7
        [Command("clean")]
        public Task BanAsync(IGuildUser user, int pruneDays) => 
            Context.Guild.AddBanAsync(user, pruneDays);
        
        // !admin ban list
        [Command("list")]
        public async Task ListBansAsync()
        {
            var bans = await Context.Guild.GetBansAsync();
            var banInfo = bans.Select(ban => $"> {ban.User.Username}#{ban.User.IdentifyNumber} " +
                                             $"({ban.CreatedAt:yyyy'/'M'/'d HH':'mm}, {ban.Reason})")
                .Aggregate((a, b) => $"{a}\n{b}");
            await ReplyTextAsync($"{bans.Count} 条封禁：\n{banInfo}");
        }
    }
    // !admin kick @戈小荷
    [Command("kick")]
    public Task KickAsync(IGuildUser user) =>
        user.KickAsync();
}
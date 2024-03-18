using Kook.Commands;
using Kook.Net.Samples.CardMarkup.Extensions;
using Kook.Net.Samples.CardMarkup.Models;
using Kook.Net.Samples.CardMarkup.Models.Template;

namespace Kook.Net.Samples.CardMarkup.Modules;

public class CardCommandModule : ModuleBase<SocketCommandContext>
{
    [Command("vote")]
    public async Task SendTestVoteCard()
    {
        var allUsers = new List<User>
        {
            new() { Avatar = "https://img.kaiheila.cn/avatars/2020-09/ov2wQ8r2qZ0dc07i.gif/icon" },
            new() { Avatar = "https://img.kaiheila.cn/avatars/2020-11/LjtEMkmH3U0hs0hs.jpg/icon" },
            new() { Avatar = "https://img.kaiheila.cn/avatars/2021-01/YaJKS70ClV04g04g.png/icon" },
            new() { Avatar = "https://img.kaiheila.cn/avatars/2020-11/reWIJpyTQt05k05k.png/icon" },
            new() { Avatar = "https://img.kaiheila.cn/assets/2021-01/qcU601U2IH0xc0pn.png/icon" },
            new() { Avatar = "https://img.kaiheila.cn/avatars/2020-12/MKUoDHdTVK0u00u0.jpg/icon" },
            new() { Avatar = "https://img.kaiheila.cn/avatars/2020-06/q4hNJU6KhU02s02s.jpg/icon" },
            new() { Avatar = "https://img.kaiheila.cn/avatars/2020-11/acXV4jRb4A08c08c.jpg/icon" },
            new() { Avatar = "https://img.kaiheila.cn/avatars/2020-06/SbFPjoBb5202s02s.jpg/icon" }
        };

        var model = new Vote
        {
            Title = "朋友们，今晚开黑玩什么游戏？",
            Games =
            [
                new Game
                {
                    Name = "英雄联盟",
                    Slogan = "艾欧尼亚，昂扬不灭。",
                    Voters = allUsers.Take(1).ToList()
                },
                new Game
                {
                    Name = "Warframe",
                    Slogan = "<<网络连接无响应>>",
                    Voters = allUsers.Skip(1).Take(3).ToList()
                },
                new Game
                {
                    Name = "CSGO",
                    Slogan = "不听不听，无甲沙鹰。",
                    Voters = allUsers.Skip(4).ToList()
                }
            ]
        };

        var cards = await model.RenderVoteAsync();

        if (cards is null)
        {
            await ReplyTextAsync("Failed to render cards, check the logs.");
        }

        await ReplyCardsAsync(cards);
    }

    [Command("big-card")]
    public async Task SendBigCard()
    {
        var cards = await TemplateExtensions.RenderBigCard();
        await ReplyCardsAsync(cards);
    }

    [Command("multiple")]
    public async Task SendMultipleCards()
    {
        var cards = await TemplateExtensions.RenderMultipleCards();
        await ReplyCardsAsync(cards);
    }
}

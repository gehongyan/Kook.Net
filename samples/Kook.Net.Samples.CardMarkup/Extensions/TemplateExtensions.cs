using System.Text.Encodings.Web;
using Fluid;
using Kook.CardMarkup;
using Kook.Net.Samples.CardMarkup.Models;
using Kook.Net.Samples.CardMarkup.Models.Template;

namespace Kook.Net.Samples.CardMarkup.Extensions;

public static class TemplateExtensions
{
    private static IFluidTemplate? _voteTemplate;

    public static async Task<IEnumerable<ICard>?> RenderVoteAsync(this Vote vote)
    {
        if (!await LoadVoteTemplateAsync()) return null;

        TemplateOptions templateOptions = new();
        templateOptions.MemberAccessStrategy.Register<Vote>();
        templateOptions.MemberAccessStrategy.Register<Game>();
        templateOptions.MemberAccessStrategy.Register<User>();

        TemplateContext context = new(vote, templateOptions);
        string result = await _voteTemplate.RenderAsync(context, HtmlEncoder.Default);
        if (string.IsNullOrWhiteSpace(result)) return null;

        return await CardMarkupSerializer.DeserializeAsync(result);
    }

    public static async Task<IEnumerable<ICard>> RenderBigCard()
    {
        string source = await File.ReadAllTextAsync("Cards/big-card.xml");
        IEnumerable<ICard> cards = await CardMarkupSerializer.DeserializeAsync(source);
        return cards;
    }

    public static async Task<IEnumerable<ICard>> RenderMultipleCards()
    {
        string source = await File.ReadAllTextAsync("Cards/multiple-cards.xml");
        IEnumerable<ICard> cards = await CardMarkupSerializer.DeserializeAsync(source);
        return cards;
    }

    private static async Task<bool> LoadVoteTemplateAsync()
    {
        if (_voteTemplate is not null) return true;

        string source = await File.ReadAllTextAsync("Cards/vote.xml.liquid");
        FluidParser parser = new();
        bool result = parser.TryParse(source, out IFluidTemplate? template);

        if (!result) return false;

        _voteTemplate = template;
        return true;
    }
}

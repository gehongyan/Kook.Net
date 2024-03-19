using System.Text.Encodings.Web;
using Fluid;
using Kook.CardMarkup;
using Kook.Net.Samples.CardMarkup.Models;
using Kook.Net.Samples.CardMarkup.Models.Template;

// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace Kook.Net.Samples.CardMarkup.Extensions;

public static class TemplateExtensions
{
    private static IFluidTemplate? _voteTemplate;

    public static async Task<IEnumerable<ICard>?> RenderVoteAsync(this Vote vote)
    {
        if (await LoadVoteTemplateAsync() is false)
        {
            return null;
        }

        var templateOptions = new TemplateOptions();
        templateOptions.MemberAccessStrategy.Register<Vote>();
        templateOptions.MemberAccessStrategy.Register<Game>();
        templateOptions.MemberAccessStrategy.Register<User>();

        var context = new TemplateContext(vote, templateOptions);
        var result = await _voteTemplate!.RenderAsync(context, HtmlEncoder.Default);
        if (result is null)
        {
            return null;
        }

        return await CardMarkupSerializer.DeserializeAsync(result);
    }

    public static async Task<IEnumerable<ICard>?> RenderBigCard()
    {
        var source = await File.ReadAllTextAsync("Cards/big-card.xml");
        var cards = await CardMarkupSerializer.DeserializeAsync(source);
        return cards;
    }

    public static async Task<IEnumerable<ICard>?> RenderMultipleCards()
    {
        var source = await File.ReadAllTextAsync("Cards/multiple-cards.xml");
        var cards = await CardMarkupSerializer.DeserializeAsync(source);
        return cards;
    }

    private static async Task<bool> LoadVoteTemplateAsync()
    {
        if (_voteTemplate is not null)
        {
            return true;
        }

        var source = await File.ReadAllTextAsync("Cards/vote.xml.liquid");
        var parser = new FluidParser();
        var result = parser.TryParse(source, out var template);

        if (result is false)
        {
            return false;
        }

        _voteTemplate = template;
        return true;
    }
}

using System.Collections.Immutable;

namespace Kook.Rest;

internal static class EntityExtensions
{
    #region Emotes

    public static GuildEmote ToEntity(this API.Emoji model, ulong guildId)
        => new(model.Id,
            model.Name,
            model.Type == EmojiType.Animated,
            guildId,
            model.UploadedBy?.Id);

    #endregion

    #region Elements

    public static IElement ToEntity(this API.ElementBase model)
    {
        if (model is null) return null;

        return model.Type switch
        {
            ElementType.PlainText => (model as API.PlainTextElement).ToEntity(),
            ElementType.KMarkdown => (model as API.KMarkdownElement).ToEntity(),
            ElementType.Image => (model as API.ImageElement).ToEntity(),
            ElementType.Button => (model as API.ButtonElement).ToEntity(),
            ElementType.Paragraph => (model as API.ParagraphStruct).ToEntity(),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
    }

    public static PlainTextElement ToEntity(this API.PlainTextElement model)
    {
        if (model is null) return null;

        return new PlainTextElement(model.Content, model.Emoji);
    }

    public static KMarkdownElement ToEntity(this API.KMarkdownElement model)
    {
        if (model is null) return null;

        return new KMarkdownElement(model.Content);
    }

    public static ImageElement ToEntity(this API.ImageElement model)
    {
        if (model is null) return null;

        return new ImageElement(model.Source, model.Alternative, model.Size, model.Circle);
    }

    public static ButtonElement ToEntity(this API.ButtonElement model)
    {
        if (model is null) return null;

        return new ButtonElement(model.Theme, model.Value, model.Click, model.Text.ToEntity());
    }

    public static ParagraphStruct ToEntity(this API.ParagraphStruct model)
    {
        if (model is null) return null;

        return new ParagraphStruct(model.ColumnCount, model.Fields.Select(e => e.ToEntity()).ToImmutableArray());
    }

    public static API.ElementBase ToModel(this IElement entity)
    {
        if (entity is null) return null;

        return entity.Type switch
        {
            ElementType.PlainText => (entity as PlainTextElement).ToModel(),
            ElementType.KMarkdown => (entity as KMarkdownElement).ToModel(),
            ElementType.Image => (entity as ImageElement).ToModel(),
            ElementType.Button => (entity as ButtonElement).ToModel(),
            ElementType.Paragraph => (entity as ParagraphStruct).ToModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    public static API.PlainTextElement ToModel(this PlainTextElement entity)
    {
        if (entity is null) return null;

        return new API.PlainTextElement { Content = entity.Content, Emoji = entity.Emoji, Type = entity.Type };
    }

    public static API.KMarkdownElement ToModel(this KMarkdownElement entity)
    {
        if (entity is null) return null;

        return new API.KMarkdownElement() { Content = entity.Content, Type = entity.Type };
    }

    public static API.ImageElement ToModel(this ImageElement entity)
    {
        if (entity is null) return null;

        return new API.ImageElement()
        {
            Alternative = entity.Alternative,
            Circle = entity.Circle,
            Size = entity.Size,
            Source = entity.Source,
            Type = entity.Type
        };
    }

    public static API.ButtonElement ToModel(this ButtonElement entity)
    {
        if (entity is null) return null;

        return new API.ButtonElement()
        {
            Click = entity.Click,
            Text = entity.Text.ToModel(),
            Theme = entity.Theme,
            Type = entity.Type,
            Value = entity.Value
        };
    }

    public static API.ParagraphStruct ToModel(this ParagraphStruct entity)
    {
        if (entity is null) return null;

        return new API.ParagraphStruct()
        {
            ColumnCount = entity.ColumnCount, Fields = entity.Fields.Select(e => e.ToModel()).ToArray(), Type = entity.Type
        };
    }

    #endregion

    #region Modules

    public static IModule ToEntity(this API.ModuleBase model)
    {
        if (model is null) return null;

        return model.Type switch
        {
            ModuleType.Header => (model as API.HeaderModule).ToEntity(),
            ModuleType.Section => (model as API.SectionModule).ToEntity(),
            ModuleType.ImageGroup => (model as API.ImageGroupModule).ToEntity(),
            ModuleType.Container => (model as API.ContainerModule).ToEntity(),
            ModuleType.ActionGroup => (model as API.ActionGroupModule).ToEntity(),
            ModuleType.Context => (model as API.ContextModule).ToEntity(),
            ModuleType.Divider => (model as API.DividerModule).ToEntity(),
            ModuleType.File => (model as API.FileModule).ToEntity(),
            ModuleType.Audio => (model as API.AudioModule).ToEntity(),
            ModuleType.Video => (model as API.VideoModule).ToEntity(),
            ModuleType.Countdown => (model as API.CountdownModule).ToEntity(),
            ModuleType.Invite => (model as API.InviteModule).ToEntity(),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
    }

    public static HeaderModule ToEntity(this API.HeaderModule model)
    {
        if (model is null) return null;

        return new HeaderModule(model.Text.ToEntity());
    }

    public static SectionModule ToEntity(this API.SectionModule model)
    {
        if (model is null) return null;

        return new SectionModule(model.Mode, model.Text.ToEntity(), model.Accessory.ToEntity());
    }

    public static ImageGroupModule ToEntity(this API.ImageGroupModule model)
    {
        if (model is null) return null;

        return new ImageGroupModule(model.Elements.Select(e => e.ToEntity()).ToImmutableArray());
    }

    public static ContainerModule ToEntity(this API.ContainerModule model)
    {
        if (model is null) return null;

        return new ContainerModule(model.Elements.Select(e => e.ToEntity()).ToImmutableArray());
    }

    public static ActionGroupModule ToEntity(this API.ActionGroupModule model)
    {
        if (model is null) return null;

        return new ActionGroupModule(model.Elements.Select(e => e.ToEntity()).ToImmutableArray());
    }

    public static ContextModule ToEntity(this API.ContextModule model)
    {
        if (model is null) return null;

        return new ContextModule(model.Elements.Select(e => e.ToEntity()).ToImmutableArray());
    }

    public static DividerModule ToEntity(this API.DividerModule model)
    {
        if (model is null) return null;

        return new DividerModule();
    }

    public static FileModule ToEntity(this API.FileModule model)
    {
        if (model is null) return null;

        return new FileModule(model.Source, model.Title);
    }

    public static AudioModule ToEntity(this API.AudioModule model)
    {
        if (model is null) return null;

        return new AudioModule(model.Source, model.Title, model.Cover);
    }

    public static VideoModule ToEntity(this API.VideoModule model)
    {
        if (model is null) return null;

        return new VideoModule(model.Source, model.Title);
    }

    public static CountdownModule ToEntity(this API.CountdownModule model)
    {
        if (model is null) return null;

        return new CountdownModule(model.Mode, model.EndTime, model.StartTime);
    }

    public static InviteModule ToEntity(this API.InviteModule model)
    {
        if (model is null) return null;

        return new InviteModule(model.Code);
    }


    public static API.ModuleBase ToModel(this IModule entity)
    {
        if (entity is null) return null;

        return entity.Type switch
        {
            ModuleType.Header => (entity as HeaderModule).ToModel(),
            ModuleType.Section => (entity as SectionModule).ToModel(),
            ModuleType.ImageGroup => (entity as ImageGroupModule).ToModel(),
            ModuleType.Container => (entity as ContainerModule).ToModel(),
            ModuleType.ActionGroup => (entity as ActionGroupModule).ToModel(),
            ModuleType.Context => (entity as ContextModule).ToModel(),
            ModuleType.Divider => (entity as DividerModule).ToModel(),
            ModuleType.File => (entity as FileModule).ToModel(),
            ModuleType.Audio => (entity as AudioModule).ToModel(),
            ModuleType.Video => (entity as VideoModule).ToModel(),
            ModuleType.Countdown => (entity as CountdownModule).ToModel(),
            ModuleType.Invite => (entity as InviteModule).ToModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    public static API.HeaderModule ToModel(this HeaderModule entity)
    {
        if (entity is null) return null;

        return new API.HeaderModule() { Text = entity.Text.ToModel(), Type = entity.Type };
    }

    public static API.SectionModule ToModel(this SectionModule entity)
    {
        if (entity is null) return null;

        return new API.SectionModule()
        {
            Accessory = entity.Accessory.ToModel(), Mode = entity.Mode, Text = entity.Text.ToModel(), Type = entity.Type
        };
    }

    public static API.ImageGroupModule ToModel(this ImageGroupModule entity)
    {
        if (entity is null) return null;

        return new API.ImageGroupModule() { Elements = entity.Elements.Select(e => e.ToModel()).ToArray(), Type = entity.Type };
    }

    public static API.ContainerModule ToModel(this ContainerModule entity)
    {
        if (entity is null) return null;

        return new API.ContainerModule() { Elements = entity.Elements.Select(e => e.ToModel()).ToArray(), Type = entity.Type };
    }

    public static API.ActionGroupModule ToModel(this ActionGroupModule entity)
    {
        if (entity is null) return null;

        return new API.ActionGroupModule() { Elements = entity.Elements.Select(e => e.ToModel()).ToArray(), Type = entity.Type };
    }

    public static API.ContextModule ToModel(this ContextModule entity)
    {
        if (entity is null) return null;

        return new API.ContextModule() { Elements = entity.Elements.Select(e => e.ToModel()).ToArray(), Type = entity.Type };
    }

    public static API.DividerModule ToModel(this DividerModule entity)
    {
        if (entity is null) return null;

        return new API.DividerModule() { Type = entity.Type };
    }

    public static API.FileModule ToModel(this FileModule entity)
    {
        if (entity is null) return null;

        return new API.FileModule() { Source = entity.Source, Title = entity.Title, Type = entity.Type };
    }

    public static API.AudioModule ToModel(this AudioModule entity)
    {
        if (entity is null) return null;

        return new API.AudioModule() { Cover = entity.Cover, Source = entity.Source, Title = entity.Title, Type = entity.Type };
    }

    public static API.VideoModule ToModel(this VideoModule entity)
    {
        if (entity is null) return null;

        return new API.VideoModule() { Source = entity.Source, Title = entity.Title, Type = entity.Type };
    }

    public static API.CountdownModule ToModel(this CountdownModule entity)
    {
        if (entity is null) return null;

        return new API.CountdownModule() { Mode = entity.Mode, EndTime = entity.EndTime, StartTime = entity.StartTime, Type = entity.Type };
    }

    public static API.InviteModule ToModel(this InviteModule entity)
    {
        if (entity is null) return null;

        return new API.InviteModule() { Code = entity.Code, Type = entity.Type };
    }

    #endregion

    #region Cards

    public static ICard ToEntity(this API.CardBase model)
    {
        if (model is null) return null;

        return model.Type switch
        {
            CardType.Card => (model as API.Card).ToEntity(),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
    }

    public static Card ToEntity(this API.Card model)
    {
        if (model is null) return null;

        return new Card(model.Theme, model.Size, model.Color, model.Modules.Select(m => m.ToEntity()).ToImmutableArray());
    }

    public static API.CardBase ToModel(this ICard entity)
    {
        if (entity is null) return null;

        return entity.Type switch
        {
            CardType.Card => (entity as Card).ToModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    public static API.Card ToModel(this Card entity)
    {
        if (entity is null) return null;

        return new API.Card()
        {
            Theme = entity.Theme,
            Color = entity.Color,
            Size = entity.Size,
            Modules = entity.Modules.Select(m => m.ToModel()).ToArray(),
            Type = entity.Type
        };
    }

    #endregion

    #region Embeds

    public static IEmbed ToEntity(this API.EmbedBase model)
    {
        if (model is null)
            return null;

        return model.Type switch
        {
            EmbedType.Link => (model as API.LinkEmbed).ToEntity(),
            EmbedType.Image => (model as API.ImageEmbed).ToEntity(),
            EmbedType.BilibiliVideo => (model as API.BilibiliVideoEmbed).ToEntity(),
            EmbedType.Card => (model as API.CardEmbed).ToEntity(),
            _ => (model as API.NotImplementedEmbed).ToNotImplementedEntity()
        };
    }

    public static LinkEmbed ToEntity(this API.LinkEmbed model) =>
        new(model.Url, model.Title, model.Description, model.SiteName, model.Color, model.Image);

    public static ImageEmbed ToEntity(this API.ImageEmbed model) => new(model.Url, model.OriginUrl);

    public static BilibiliVideoEmbed ToEntity(this API.BilibiliVideoEmbed model) =>
        new(model.Url, model.OriginUrl, model.BvNumber, model.IframePath,
            TimeSpan.FromSeconds(model.Duration), model.Title, model.Cover);

    public static CardEmbed ToEntity(this API.CardEmbed model)
    {
        return new CardEmbed(new Card(model.Theme, model.Size, model.Color,
            model.Modules.Select(m => m.ToEntity()).ToImmutableArray()));
    }


    public static NotImplementedEmbed ToNotImplementedEntity(this API.NotImplementedEmbed model) =>
        new(model.RawType, model.RawJsonNode);

    #endregion

    #region Pokes

    public static IPokeResource ToEntity(this API.PokeResourceBase model)
    {
        if (model is null) return null;

        return model.Type switch
        {
            PokeResourceType.ImageAnimation => (model as API.ImageAnimationPokeResource).ToEntity(),
            _ => (model as API.NotImplementedPokeResource).ToNotImplementedEntity()
        };
    }

    public static NotImplementedPokeResource ToNotImplementedEntity(this API.NotImplementedPokeResource model) =>
        new(model.RawType, model.RawJsonNode);

    public static ImageAnimationPokeResource ToEntity(this API.ImageAnimationPokeResource model) =>
        new(new Dictionary<string, string>() { ["webp"] = model.WebP, ["pag"] = model.PAG, ["gif"] = model.GIF },
            TimeSpan.FromMilliseconds(model.Duration), model.Width, model.Height, model.Percent / 100D);

    #endregion

    #region Overwrites

    public static UserPermissionOverwrite ToEntity(this API.UserPermissionOverwrite model) =>
        new(RestUser.Create(null, model.User), new OverwritePermissions(model.Allow, model.Deny));

    public static RolePermissionOverwrite ToEntity(this API.RolePermissionOverwrite model) =>
        new(model.RoleId, new OverwritePermissions(model.Allow, model.Deny));

    #endregion

    #region Recommendation Infos

    public static RecommendInfo ToEntity(this API.RecommendInfo model)
    {
        if (model is null) return null;

        return RecommendInfo.Create(model);
    }

    #endregion

    #region User Tags

    public static UserTag ToEntity(this API.UserTag model)
    {
        if (model is null) return null;

        return UserTag.Create(model.Color, model.BackgroundColor, model.Text);
    }

    #endregion

    #region Nameplates

    public static Nameplate ToEntity(this API.Nameplate model)
    {
        if (model is null) return null;

        return Nameplate.Create(model.Name, model.Type, model.Icon, model.Tips);
    }

    #endregion
}

using Kook.API;

namespace Kook.Rest;

internal static class EntityExtensions
{
    #region Guilds

    public static GuildJoinRestrictionTypes ToEntity(this API.Rest.JoinRestrictions model)
    {
        GuildJoinRestrictionTypes entity = GuildJoinRestrictionTypes.None;
        if (model.DisableUnverified)
            entity |= GuildJoinRestrictionTypes.DisableUnverified;
        if (model.DisableViolation)
            entity |= GuildJoinRestrictionTypes.DisableViolation;
        if (model.DisableUnverifiedAndViolation)
            entity |= GuildJoinRestrictionTypes.DisableUnverifiedAndViolation;
        return entity;
    }

    #endregion

    #region Emotes

    public static GuildEmote ToEntity(this API.Emoji model, ulong guildId) =>
        new(model.Id, model.Name, model.Type, guildId, model.UploadedBy?.Id);

    public static IReadOnlyCollection<InteractiveEmoteRollResult>? ToResults(this InteractionResource resource)
    {
        return resource switch
        {
            {
                EmojiId: InteractiveEmoteType.SingleDie,
                Result: [var value],
                ResultImg: [var image]
            } => [new InteractiveEmoteDiceResult(value, image)],
            {
                EmojiId: InteractiveEmoteType.DualDice,
                Result: [var value1, var value2],
                ResultImg: [var image1, var image2]
            } => [new InteractiveEmoteDiceResult(value1, image1), new InteractiveEmoteDiceResult(value2, image2)],
            {
                EmojiId: InteractiveEmoteType.RockPaperScissors,
                Result: [var value],
                ResultImg: [var image]
            } => [new InteractiveEmoteRockPaperScissorsResult(value, image)],
            _ => null
        };
    }

    #endregion

    #region Elements

    public static IElement ToEntity(this API.ElementBase model)
    {
        return model switch
        {
            API.PlainTextElement { Type: ElementType.PlainText } x => x.ToEntity(),
            API.KMarkdownElement { Type: ElementType.KMarkdown } x => x.ToEntity(),
            API.ImageElement { Type: ElementType.Image } x => x.ToEntity(),
            API.ButtonElement { Type: ElementType.Button } x => x.ToEntity(),
            API.ParagraphStruct { Type: ElementType.Paragraph } x => x.ToEntity(),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
    }

    public static PlainTextElement ToEntity(this API.PlainTextElement model) =>
        new(model.Content, model.Emoji);

    public static KMarkdownElement ToEntity(this API.KMarkdownElement model) =>
        new(model.Content);

    public static ImageElement ToEntity(this API.ImageElement model) =>
        new(model.Source, model.Alternative, model.Size, model.Circle, model.FallbackUrl);

    public static ButtonElement ToEntity(this API.ButtonElement model) =>
        new(model.Theme, model.Value, model.Click, model.Text.ToEntity());

    public static ParagraphStruct ToEntity(this API.ParagraphStruct model) =>
        new(model.ColumnCount, [..model.Fields?.Select(e => e.ToEntity()) ?? []]);

    public static API.ElementBase ToModel(this IElement entity)
    {
        return entity switch
        {
            PlainTextElement { Type: ElementType.PlainText } x => x.ToModel(),
            KMarkdownElement { Type: ElementType.KMarkdown } x => x.ToModel(),
            ImageElement { Type: ElementType.Image } x => x.ToModel(),
            ButtonElement { Type: ElementType.Button } x => x.ToModel(),
            ParagraphStruct { Type: ElementType.Paragraph } x => x.ToModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    public static API.PlainTextElement ToModel(this PlainTextElement entity) => new()
    {
        Content = entity.Content,
        Emoji = entity.Emoji,
        Type = entity.Type
    };

    public static API.KMarkdownElement ToModel(this KMarkdownElement entity) => new()
    {
        Content = entity.Content,
        Type = entity.Type
    };

    public static API.ImageElement ToModel(this ImageElement entity) => new()
    {
        Alternative = entity.Alternative,
        Circle = entity.Circle,
        Size = entity.Size,
        Source = entity.Source,
        Type = entity.Type,
        FallbackUrl = entity.FallbackUrl
    };

    public static API.ButtonElement ToModel(this ButtonElement entity) => new()
    {
        Click = entity.Click,
        Text = entity.Text.ToModel(),
        Theme = entity.Theme,
        Type = entity.Type,
        Value = entity.Value
    };

    public static API.ParagraphStruct ToModel(this ParagraphStruct entity) =>
        new()
        {
            ColumnCount = entity.ColumnCount,
            Fields = entity.Fields.Select(e => e.ToModel()).ToArray(),
            Type = entity.Type
        };

    #endregion

    #region Modules

    public static IModule ToEntity(this API.ModuleBase model)
    {
        return model switch
        {
            API.HeaderModule { Type: ModuleType.Header } x => x.ToEntity(),
            API.SectionModule { Type: ModuleType.Section } x => x.ToEntity(),
            API.ImageGroupModule { Type: ModuleType.ImageGroup } x => x.ToEntity(),
            API.ContainerModule { Type: ModuleType.Container } x => x.ToEntity(),
            API.ActionGroupModule { Type: ModuleType.ActionGroup } x => x.ToEntity(),
            API.ContextModule { Type: ModuleType.Context } x => x.ToEntity(),
            API.DividerModule { Type: ModuleType.Divider } x => x.ToEntity(),
            API.FileModule { Type: ModuleType.File } x => x.ToEntity(),
            API.AudioModule { Type: ModuleType.Audio } x => x.ToEntity(),
            API.VideoModule { Type: ModuleType.Video } x => x.ToEntity(),
            API.CountdownModule { Type: ModuleType.Countdown } x => x.ToEntity(),
            API.InviteModule { Type: ModuleType.Invite } x => x.ToEntity(),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
    }

    public static HeaderModule ToEntity(this API.HeaderModule model) =>
        new(model.Text?.ToEntity());

    public static SectionModule ToEntity(this API.SectionModule model) =>
        new(model.Mode, model.Text?.ToEntity(), model.Accessory?.ToEntity());

    public static ImageGroupModule ToEntity(this API.ImageGroupModule model) =>
        new([..model.Elements.Select(e => e.ToEntity())]);

    public static ContainerModule ToEntity(this API.ContainerModule model) =>
        new([..model.Elements.Select(e => e.ToEntity())]);

    public static ActionGroupModule ToEntity(this API.ActionGroupModule model) =>
        new([..model.Elements.Select(e => e.ToEntity())]);

    public static ContextModule ToEntity(this API.ContextModule model) =>
        new([..model.Elements?.Select(e => e.ToEntity()) ?? []]);

    public static DividerModule ToEntity(this API.DividerModule model) =>
        new();

    public static FileModule ToEntity(this API.FileModule model) =>
        new(model.Source, model.Title, model.Size);

    public static AudioModule ToEntity(this API.AudioModule model) =>
        new(model.Source, model.Title, model.Cover);

    public static VideoModule ToEntity(this API.VideoModule model) =>
        new(model.Source, model.Title, model.Cover, model.Size,
            model.Duration.HasValue ? TimeSpan.FromSeconds(model.Duration.Value) : null,
            model.Width, model.Height);

    public static CountdownModule ToEntity(this API.CountdownModule model) =>
        new(model.Mode, model.EndTime, model.StartTime);

    public static InviteModule ToEntity(this API.InviteModule model) =>
        new(model.Code);

    public static API.ModuleBase ToModel(this IModule entity)
    {
        return entity switch
        {
            HeaderModule { Type: ModuleType.Header } x => x.ToModel(),
            SectionModule { Type: ModuleType.Section } x => x.ToModel(),
            ImageGroupModule { Type: ModuleType.ImageGroup } x => x.ToModel(),
            ContainerModule { Type: ModuleType.Container } x => x.ToModel(),
            ActionGroupModule { Type: ModuleType.ActionGroup } x => x.ToModel(),
            ContextModule { Type: ModuleType.Context } x => x.ToModel(),
            DividerModule { Type: ModuleType.Divider } x => x.ToModel(),
            FileModule { Type: ModuleType.File } x => x.ToModel(),
            AudioModule { Type: ModuleType.Audio } x => x.ToModel(),
            VideoModule { Type: ModuleType.Video } x => x.ToModel(),
            CountdownModule { Type: ModuleType.Countdown } x => x.ToModel(),
            InviteModule { Type: ModuleType.Invite } x => x.ToModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity))
        };
    }

    public static API.HeaderModule ToModel(this HeaderModule entity) => new()
    {
        Text = entity.Text?.ToModel(),
        Type = entity.Type
    };

    public static API.SectionModule ToModel(this SectionModule entity) => new()
    {
        Accessory = entity.Accessory?.ToModel(),
        Mode = entity.Mode,
        Text = entity.Text?.ToModel(),
        Type = entity.Type
    };

    public static API.ImageGroupModule ToModel(this ImageGroupModule entity) => new()
    {
        Elements = entity.Elements.Select(e => e.ToModel()).ToArray(),
        Type = entity.Type
    };

    public static API.ContainerModule ToModel(this ContainerModule entity) => new()
    {
        Elements = entity.Elements.Select(e => e.ToModel()).ToArray(),
        Type = entity.Type
    };

    public static API.ActionGroupModule ToModel(this ActionGroupModule entity) => new()
    {
        Elements = entity.Elements.Select(e => e.ToModel()).ToArray(),
        Type = entity.Type
    };

    public static API.ContextModule ToModel(this ContextModule entity) => new()
    {
        Elements = entity.Elements.Select(e => e.ToModel()).ToArray(),
        Type = entity.Type
    };

    public static API.DividerModule ToModel(this DividerModule entity) => new()
    {
        Type = entity.Type
    };

    public static API.FileModule ToModel(this FileModule entity) => new()
    {
        Source = entity.Source,
        Title = entity.Title,
        Type = entity.Type
    };

    public static API.AudioModule ToModel(this AudioModule entity) => new()
    {
        Cover = entity.Cover,
        Source = entity.Source,
        Title = entity.Title,
        Type = entity.Type
    };

    public static API.VideoModule ToModel(this VideoModule entity) => new()
    {
        Source = entity.Source,
        Title = entity.Title,
        Type = entity.Type
    };

    public static API.CountdownModule ToModel(this CountdownModule entity) => new()
    {
        Mode = entity.Mode,
        EndTime = entity.EndTime,
        StartTime = entity.StartTime,
        Type = entity.Type
    };

    public static API.InviteModule ToModel(this InviteModule entity) => new()
    {
        Code = entity.Code,
        Type = entity.Type
    };

    #endregion

    #region Cards

    public static ICard ToEntity(this API.CardBase model) => model switch
    {
        API.Card { Type: CardType.Card } x => x.ToEntity(),
        _ => throw new ArgumentOutOfRangeException(nameof(model))
    };

    public static Card ToEntity(this API.Card model) =>
        new(model.Theme ?? CardTheme.Primary, model.Size ?? CardSize.Large, model.Color, [..model.Modules.Select(m => m.ToEntity())]);

    public static API.CardBase ToModel(this ICard entity) => entity switch
    {
        Card { Type: CardType.Card } x => x.ToModel(),
        _ => throw new ArgumentOutOfRangeException(nameof(entity))
    };

    public static API.Card ToModel(this Card entity) => new()
    {
        Theme = entity.Theme,
        Color = entity.Color,
        Size = entity.Size,
        Modules = entity.Modules.Select(m => m.ToModel()).ToArray(),
        Type = entity.Type
    };

    #endregion

    #region Embeds

    public static IEmbed ToEntity(this API.EmbedBase model)
    {
        return model switch
        {
            API.LinkEmbed { Type: EmbedType.Link } x => x.ToEntity(),
            API.ImageEmbed { Type: EmbedType.Image } x => x.ToEntity(),
            API.BilibiliVideoEmbed { Type: EmbedType.BilibiliVideo } x => x.ToEntity(),
            API.CardEmbed { Type: EmbedType.Card } x => x.ToEntity(),
            API.NotImplementedEmbed { Type: EmbedType.NotImplemented } x => x.ToNotImplementedEntity(),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
    }

    public static LinkEmbed ToEntity(this API.LinkEmbed model) =>
        new(model.Url, model.Title, model.Description, model.SiteName, model.Color, model.Image);

    public static ImageEmbed ToEntity(this API.ImageEmbed model) => new(model.Url, model.OriginUrl);

    public static BilibiliVideoEmbed ToEntity(this API.BilibiliVideoEmbed model) =>
        new(model.Url, model.OriginUrl, model.BvNumber, model.IframePath,
            TimeSpan.FromSeconds(model.Duration), model.Title, model.Cover);

    public static CardEmbed ToEntity(this API.CardEmbed model) =>
        new(new Card(model.Theme ?? CardTheme.Primary, model.Size ?? CardSize.Large, model.Color, [..model.Modules.Select(m => m.ToEntity())]));

    public static NotImplementedEmbed ToNotImplementedEntity(this API.NotImplementedEmbed model) =>
        new(model.RawType, model.RawJsonNode);

    #endregion

    #region Pokes

    public static IPokeResource ToEntity(this API.PokeResourceBase model)
    {
        return model switch
        {
            API.ImageAnimationPokeResource { Type: PokeResourceType.ImageAnimation } x => x.ToEntity(),
            API.NotImplementedPokeResource { Type: PokeResourceType.NotImplemented } x => x.ToNotImplementedEntity(),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
    }

    public static NotImplementedPokeResource ToNotImplementedEntity(this API.NotImplementedPokeResource model) =>
        new(model.RawType, model.RawJsonNode);

    public static ImageAnimationPokeResource ToEntity(this API.ImageAnimationPokeResource model) =>
        new(new Dictionary<string, string> { ["webp"] = model.WebP, ["pag"] = model.PAG, ["gif"] = model.GIF },
            TimeSpan.FromMilliseconds(model.Duration), model.Width, model.Height, model.Percent / 100M);

    #endregion

    #region Overwrites

    public static UserPermissionOverwrite ToEntity(this API.UserPermissionOverwrite model, IUser? entity) =>
        new(model.User.Id, entity, new OverwritePermissions(model.Allow, model.Deny));

    public static RolePermissionOverwrite ToEntity(this API.RolePermissionOverwrite model, IRole? entity) =>
        new(model.RoleId, entity, new OverwritePermissions(model.Allow, model.Deny));

    #endregion

    #region Recommendation Infos

    public static RecommendInfo ToEntity(this API.RecommendInfo model)
    {
        return RecommendInfo.Create(model);
    }

    public static GuildCertification ToEntity(this API.GuildCertification model)
    {
        return GuildCertification.Create(model.Type, model.Title, model.Picture, model.Description);
    }

    public static RareGuildResources ToEntity(this RareGuildSettings model)
    {
        return new RareGuildResources
        {
            FrameImages = model.Frame,
            FrameColor = model.FrameColor,
            NameplateImages = model.RareNameplate,
            CoverImage = model.CoverImage,
            Summary = model.BriefText,
            Description = model.Description,
            IdIcon = model.IdIcon
        };
    }

    #endregion

    #region User Tags

    public static UserTag ToEntity(this API.UserTag model)
    {
        return UserTag.Create(model.Color, model.BackgroundColor, model.Text);
    }

    #endregion

    #region Nameplates

    public static Nameplate ToEntity(this API.Nameplate model)
    {
        return Nameplate.Create(model.Name, model.Type, model.Icon, model.Tips);
    }

    #endregion

    #region KPM VIP

    public static KpmVipInfo ToEntity(this API.KpmVipInfo model) =>
        KpmVipInfo.Create(model.Level, model.Exp, model.Discount / 100M, model.Icon, model.Text);

    #endregion
}

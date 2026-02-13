namespace Kook.Rest;

internal static class ExperimentalEntityExtensions
{
    #region Guild Restrictions

    public static IBehaviorRestrictionCondition? ToEntity(this API.Rest.GuildSecurityCondition model)
    {
        return model.Type switch
        {
            "reg" when model.Duration.HasValue =>
                new RegistrationDurationBehaviorRestrictionCondition
                    { MinimumDuration = TimeSpan.FromMinutes(model.Duration.Value) },
            "overseas" => new OverseasBehaviorRestrictionCondition(),
            "banned" => new ViolationBehaviorRestrictionCondition(),
            "real_auth" => new UnverifiedBehaviorRestrictionCondition(),
            _ => null
        };
    }

    public static API.Rest.GuildSecurityCondition ToModel(this IBehaviorRestrictionCondition entity)
    {
        return entity switch
        {
            RegistrationDurationBehaviorRestrictionCondition regCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "reg",
                Duration = (int?)regCondition.MinimumDuration.TotalMinutes
            },
            OverseasBehaviorRestrictionCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "overseas"
            },
            ViolationBehaviorRestrictionCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "banned"
            },
            UnverifiedBehaviorRestrictionCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "real_auth"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(entity), "Unknown restriction condition type.")
        };
    }

    #endregion

    #region Guild Content Filters

    public static IContentFilterTarget ToEntity(this API.Rest.ContentFilterTarget model, ContentFilterType type)
    {
        return type switch
        {
            ContentFilterType.Word => new WordFilterTarget
            {
                Words = model.StringItems ?? []
            },
            ContentFilterType.Invite => new InviteFilterTarget
            {
                Mode = model.Enabled,
                GuildIds = [..model.TargetItems?.Select(x => x.Id) ?? []]
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Unknown content filter type.")
        };
    }

    public static API.Rest.ContentFilterTarget ToModel(this IContentFilterTarget entity)
    {
        return entity switch
        {
            WordFilterTarget wordTarget => new API.Rest.ContentFilterTarget
            {
                Enabled = wordTarget.Mode,
                StringItems = [..wordTarget.Words]
            },
            InviteFilterTarget inviteTarget => new API.Rest.ContentFilterTarget
            {
                Enabled = inviteTarget.Mode,
                TargetItems = [..inviteTarget.GuildIds.Select(id => new API.Rest.ContentFilterTargetItem { Id = id })]
            },
            _ => throw new ArgumentOutOfRangeException(nameof(entity), "Unknown content filter target type.")
        };
    }

    public static ContentFilterExemption ToEntity(this API.Rest.ContentFilterExemption model) =>
        new(model.Type, model.Id);

    public static API.Rest.ContentFilterExemption ToModel(this ContentFilterExemption entity) => new()
    {
        Type = entity.Type,
        Id = entity.Id
    };

    public static IContentFilterHandler ToEntity(this API.Rest.ContentFilterHandler x)
    {
        return x.Type switch
        {
            ContentFilterHandlerType.Intercept => new ContentFilterInterceptHandler
            {
                Name = x.Name,
                Enabled = x.Enabled,
                CustomErrorMessage = string.IsNullOrWhiteSpace(x.CustomErrorMessage) ? null : x.CustomErrorMessage,
            },
            ContentFilterHandlerType.LogToChannel => new ContentFilterLogToChannelHandler
            {
                Name = x.Name,
                Enabled = x.Enabled,
                ChannelId = x.AlertChannelId,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(x), "Unknown content filter handler type.")
        };
    }

    public static API.Rest.ContentFilterHandler ToModel(this IContentFilterHandler entity)
    {
        return entity switch
        {
            ContentFilterInterceptHandler interceptHandler => new API.Rest.ContentFilterHandler
            {
                Type = ContentFilterHandlerType.Intercept,
                Name = interceptHandler.Name,
                Enabled = interceptHandler.Enabled,
                CustomErrorMessage = interceptHandler.CustomErrorMessage,
            },
            ContentFilterLogToChannelHandler logHandler => new API.Rest.ContentFilterHandler
            {
                Type = ContentFilterHandlerType.LogToChannel,
                Name = logHandler.Name,
                Enabled = logHandler.Enabled,
                AlertChannelId = logHandler.ChannelId,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(entity), "Unknown content filter handler type.")
        };
    }

    #endregion
}

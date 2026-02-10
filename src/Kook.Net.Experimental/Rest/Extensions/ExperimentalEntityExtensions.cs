namespace Kook.Rest;

internal static class ExperimentalEntityExtensions
{
    #region Guild Restrictions

    public static IGuildBehaviorRestrictionCondition? ToEntity(this API.Rest.GuildSecurityCondition model)
    {
        return model.Type switch
        {
            "reg" when model.Duration.HasValue =>
                new GuildRegistrationDurationBehaviorRestrictionCondition
                    { MinimumDuration = TimeSpan.FromMinutes(model.Duration.Value) },
            "overseas" => new GuildOverseasBehaviorRestrictionCondition(),
            "banned" => new GuildViolationBehaviorRestrictionCondition(),
            "real_auth" => new GuildUnverifiedBehaviorRestrictionCondition(),
            _ => null
        };
    }

    public static API.Rest.GuildSecurityCondition ToModel(this IGuildBehaviorRestrictionCondition entity)
    {
        return entity switch
        {
            GuildRegistrationDurationBehaviorRestrictionCondition regCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "reg",
                Duration = (int?)regCondition.MinimumDuration.TotalMinutes
            },
            GuildOverseasBehaviorRestrictionCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "overseas"
            },
            GuildViolationBehaviorRestrictionCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "banned"
            },
            GuildUnverifiedBehaviorRestrictionCondition => new API.Rest.GuildSecurityCondition
            {
                Type = "real_auth"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(entity), "Unknown restriction condition type.")
        };
    }

    #endregion
}

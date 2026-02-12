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
}

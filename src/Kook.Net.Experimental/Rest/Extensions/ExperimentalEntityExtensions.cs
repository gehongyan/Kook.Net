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

    #endregion
}

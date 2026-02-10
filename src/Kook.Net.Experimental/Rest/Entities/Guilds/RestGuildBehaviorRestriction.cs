using System.Diagnostics;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的服务器行为限制。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestGuildBehaviorRestriction : RestEntity<string>, IGuildBehaviorRestriction
{
    /// <inheritdoc />
    public ulong GuildId { get; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<IGuildBehaviorRestrictionCondition> Conditions { get; private set; }

    /// <inheritdoc />
    public TimeSpan Duration { get; private set; }

    /// <inheritdoc />
    public GuildBehaviorRestrictionType RestrictionType { get; private set; }

    /// <inheritdoc />
    public bool IsEnabled { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <inheritdoc />
    private RestGuildBehaviorRestriction(BaseKookClient kook, ulong guildId, string id)
        : base(kook, id)
    {
        GuildId = guildId;
        Name = string.Empty;
        Conditions = [];
    }

    internal static RestGuildBehaviorRestriction Create(BaseKookClient kook, ulong guildId, API.Rest.GuildSecurityItem model)
    {
        RestGuildBehaviorRestriction entity = new(kook, guildId, model.Id);
        entity.Update(model);
        return entity;
    }

    private void Update(API.Rest.GuildSecurityItem model)
    {
        Name = model.Name;
        IEnumerable<IGuildBehaviorRestrictionCondition> conditions = model.Conditions
            .Select(x => x.ToEntity())
            .OfType<IGuildBehaviorRestrictionCondition>();
        Conditions = [..conditions];
        Duration = TimeSpan.FromMinutes(model.LimitTime);
        RestrictionType = model.Action;
        IsEnabled = model.Switch;
        CreatedAt = model.CreatedAt;
        UpdatedAt = model.UpdatedAt;
    }

    private string DebuggerDisplay => $"BehaviorRestriction: {Name} ({Id}, if {string.Join(" and ", Conditions.Select(x => x.Type))}, disallow {RestrictionType} for {Duration}, {(IsEnabled ? "Enabled" : "Disabled")})";

    /// <inheritdoc />
    public async Task ModifyAsync(Action<ModifyGuildBehaviorRestrictionProperties> func, RequestOptions? options = null) =>
        await ExperimentalGuildHelper.ModifyBehaviorRestrictionAsync(Kook, this, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task EnableAsync(RequestOptions? options = null) =>
        await ExperimentalGuildHelper.EnableBehaviorRestrictionAsync(Kook, this, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DisableAsync(RequestOptions? options = null) =>
        await ExperimentalGuildHelper.DisableBehaviorRestrictionAsync(Kook, this, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteAsync(RequestOptions? options = null) =>
        await ExperimentalGuildHelper.DeleteGuildBehaviorRestrictionAsync(Kook, this, options).ConfigureAwait(false);
}


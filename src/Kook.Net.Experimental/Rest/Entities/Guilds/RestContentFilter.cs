using System.Diagnostics;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的内容过滤器实体。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestContentFilter : RestEntity<string>, IContentFilter
{
    /// <inheritdoc />
    public ulong GuildId { get; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public IContentFilterTarget Target { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ContentFilterExemption> Exemptions { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<IContentFilterHandler> Handlers { get; private set; }

    /// <inheritdoc />
    public bool IsEnabled { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <inheritdoc />
    private RestContentFilter(BaseKookClient kook, ulong guildId, string id)
        : base(kook, id)
    {
        GuildId = guildId;
        Name = string.Empty;
        Target = null!;
        Exemptions = [];
        Handlers = [];
    }

    internal static RestContentFilter Create(BaseKookClient kook, ulong guildId, API.Rest.GuildSecurityWordfilterItem model)
    {
        RestContentFilter entity = new(kook, guildId, model.Id);
        entity.Update(model);
        return entity;
    }

    private void Update(API.Rest.GuildSecurityWordfilterItem model)
    {
        Name = model.Name;
        Target = model.Type switch
        {
            ContentFilterType.Word => new WordFilterTarget
            {
                Words = model.Targets.StringItems ?? []
            },
            ContentFilterType.Invite => new InviteFilterTarget
            {
                Mode = model.Targets.Enabled,
                GuildIds = [..model.Targets.TargetItems?.Select(x => x.Id) ?? []]
            },
            _ => new UnknownFilterTarget(),
        };
        Exemptions = [..model.Exemptions.Select(x => new ContentFilterExemption(x.Type, x.Id))];
        Handlers = [..model.Handlers.Select(IContentFilterHandler (x) => x.Type switch
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
                ChannelId = x.AlertChannelId
                    ?? throw new InvalidOperationException("Log channel ID cannot be null for LogToChannel handler."),
            },
            _ => new ContentFilterUnknownHandler
            {
                Name = x.Name,
                Enabled = x.Enabled,
            }
        })];
        IsEnabled = model.Switch;
        CreatedAt = model.CreatedAt;
        UpdatedAt = model.UpdatedAt;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(RequestOptions? options = null) =>
        await ExperimentalGuildHelper.DeleteContentFilterAsync(Kook, this, options).ConfigureAwait(false);

    private string DebuggerDisplay => $"ContentFilter: {Name} ({Id})";
}

using System.Collections.Immutable;
using Model = Kook.API.Intimacy;

namespace Kook.Rest;

/// <summary>
///     Gets the intimacy information associated with the specified user.
/// </summary>
public class RestIntimacy : RestEntity<ulong>, IIntimacy
{
    private ImmutableArray<IntimacyImage> _images;

    /// <inheritdoc />
    public IUser User { get; }

    /// <inheritdoc />
    public string SocialInfo { get; internal set; }

    /// <inheritdoc />
    public DateTimeOffset LastReadAt { get; internal set; }

    /// <inheritdoc />
    public DateTimeOffset LastModifyAt { get; internal set; }

    /// <inheritdoc />
    public int Score { get; internal set; }

    /// <inheritdoc />
    public IReadOnlyCollection<IntimacyImage> Images => _images;

    /// <inheritdoc />
    public async Task UpdateAsync(Action<IntimacyProperties> func, RequestOptions options = null) =>
        await IntimacyHelper.UpdateAsync(this, Kook, func, options).ConfigureAwait(false);

    internal RestIntimacy(BaseKookClient kook, IUser user, ulong id)
        : base(kook, id) =>
        User = user;

    internal static RestIntimacy Create(BaseKookClient kook, IUser user, Model model)
    {
        RestIntimacy entity = new(kook, user, user.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        // update properties from model
        SocialInfo = model.SocialInfo;
        LastReadAt = model.LastReadAt;
        LastModifyAt = model.LastModifyAt;
        Score = model.Score;
        _images = model.Images.Select(i => new IntimacyImage(i.Id, i.Url)).ToImmutableArray();
    }
}

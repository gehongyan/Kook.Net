using System.Collections.Immutable;
using Model = Kook.API.Intimacy;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的亲密度。
/// </summary>
/// <remarks>
///     由于亲密度的使用率较低，官方已隐藏亲密度的管理入口。如要管理亲密度，请访问
///     https://developer.kookapp.cn/bot/cohesion，在左上角切换至要管理其亲密度的应用。
/// </remarks>
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
    public DateTimeOffset? LastModifyAt { get; internal set; }

    /// <inheritdoc />
    public int Score { get; internal set; }

    /// <inheritdoc />
    public IReadOnlyCollection<IntimacyImage> Images => _images;

    /// <inheritdoc />
    public async Task UpdateAsync(Action<IntimacyProperties> func, RequestOptions? options = null) =>
        await IntimacyHelper.UpdateAsync(this, Kook, func, options).ConfigureAwait(false);

    internal RestIntimacy(BaseKookClient kook, IUser user, ulong id)
        : base(kook, id)
    {
        _images = [];
        User = user;
        SocialInfo = string.Empty;
    }

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
        _images = [..model.Images.Select(i => new IntimacyImage(i.Id, i.Url))];
    }
}

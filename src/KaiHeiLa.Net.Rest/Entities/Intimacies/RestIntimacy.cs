using System.Collections.Immutable;
using Model = KaiHeiLa.API.Intimacy;

namespace KaiHeiLa.Rest;

// The identifier for intimacy is the associated user's id.
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
    public async Task UpdateAsync(Action<IntimacyProperties> func, RequestOptions options = null)
    {
        await IntimacyHelper.UpdateAsync(this, KaiHeiLa, func, options).ConfigureAwait(false);
    }

    internal RestIntimacy(BaseKaiHeiLaClient kaiHeiLa, IUser user, ulong id)
        : base(kaiHeiLa, id)
    {
        User = user;
    }
    
    internal static RestIntimacy Create(BaseKaiHeiLaClient kaiHeiLa, IUser user, Model model)
    {
        var entity = new RestIntimacy(kaiHeiLa, user, user.Id);
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
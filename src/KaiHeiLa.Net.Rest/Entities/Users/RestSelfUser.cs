using System.Diagnostics;
using Model = KaiHeiLa.API.Rest.SelfUser;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents the logged-in REST-based user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestSelfUser : RestUser, ISelfUser
{
    /// <inheritdoc />
    public string MobilePrefix { get; private set; }
    /// <inheritdoc />
    public string Mobile { get; private set; }
    /// <inheritdoc />
    public int InvitedCount { get; private set; }
    /// <inheritdoc />
    public bool IsMobileVerified { get; private set; }
    
    internal RestSelfUser(BaseKaiHeiLaClient discord, ulong id)
        : base(discord, id)
    {
    }
    internal static RestSelfUser Create(BaseKaiHeiLaClient discord, Model model)
    {
        var entity = new RestSelfUser(discord, model.Id);
        entity.Update(model);
        return entity;
    }
    internal void Update(Model model)
    {
        base.Update(model);
        
        MobilePrefix = model.MobilePrefix;
        Mobile = model.Mobile;
        InvitedCount = model.InvitedCount ?? 0;
        IsMobileVerified = model.MobileVerified;
    }

    #region ISelfUser
    
    /// <inheritdoc />
    public async Task StartPlayingAsync(IGame game, RequestOptions options = null)
    {
        await UserHelper.StartPlayingAsync(this, KaiHeiLa, game, options).ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task StopPlayingAsync(RequestOptions options = null)
    {
        await UserHelper.StopPlayingAsync(this, KaiHeiLa, options).ConfigureAwait(false);
    }

    #endregion
}
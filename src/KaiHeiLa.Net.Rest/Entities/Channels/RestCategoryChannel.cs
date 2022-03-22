using Model = KaiHeiLa.API.Channel;

using System.Diagnostics;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based category channel.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestCategoryChannel : RestGuildChannel, ICategoryChannel
{
    #region RestCategoryChannel
    internal RestCategoryChannel(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, ulong id)
        : base(kaiHeiLa, guild, id, ChannelType.Category)
    {
    }
    internal new static RestCategoryChannel Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, Model model)
    {
        var entity = new RestCategoryChannel(kaiHeiLa, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    
    #endregion
    
    private string DebuggerDisplay => $"{Name} ({Id}, Category)";
    
    #region IChannel
    
    /// <inheritdoc />
    /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => throw new NotSupportedException();
    /// <inheritdoc />
    /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => throw new NotSupportedException();
    
    #endregion
}
using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based category channel.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestCategoryChannel : RestGuildChannel, ICategoryChannel
{
    #region RestCategoryChannel

    internal RestCategoryChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, guild, id)
    {
        Type = ChannelType.Category;
    }

    internal static new RestCategoryChannel Create(BaseKookClient kook, IGuild guild, Model model)
    {
        RestCategoryChannel entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Category)";

    #region IChannel

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options = null)
        => throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options = null)
        => throw new NotSupportedException();

    #endregion
}

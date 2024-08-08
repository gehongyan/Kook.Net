using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的分组频道。
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
    /// <exception cref="NotSupportedException"> 分组频道不支持此操作。 </exception>
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 分组频道不支持此操作。 </exception>
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        throw new NotSupportedException();

    #endregion
}

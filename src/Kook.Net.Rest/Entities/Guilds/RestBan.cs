using System.Diagnostics;
using Model = Kook.API.Ban;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based ban object.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestBan : IBan
{
    #region RestBan

    /// <summary>
    ///     Gets the banned user.
    /// </summary>
    /// <returns>
    ///     A generic <see cref="RestUser"/> object that was banned.
    /// </returns>
    public RestUser User { get; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; }

    /// <inheritdoc />
    public string Reason { get; }

    internal RestBan(RestUser user, string reason, DateTimeOffset createdAt)
    {
        User = user;
        Reason = reason;
        CreatedAt = createdAt;
    }

    internal static RestBan Create(BaseKookClient client, Model model) => new(RestUser.Create(client, model.User), model.Reason, model.CreatedAt);

    /// <summary>
    ///     Gets the name of the banned user.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the user that was banned.
    /// </returns>
    public override string ToString() => User.ToString();

    private string DebuggerDisplay => $"{User}: {Reason}";

    #endregion

    #region IBan

    /// <inheritdoc />
    IUser IBan.User => User;

    #endregion
}

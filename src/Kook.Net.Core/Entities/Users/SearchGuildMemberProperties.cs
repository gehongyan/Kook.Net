namespace Kook;

/// <summary>
///     Properties that are used to search for a <see cref="IGuildUser" /> in a <see cref="IGuild" />.
/// </summary>
/// <seealso cref="IGuild.SearchUsersAsync"/>
public class SearchGuildMemberProperties
{
    /// <summary>
    ///     Gets or sets the name of the user to be searched for;
    ///     <c>null</c> to not search via a name.
    /// </summary>
    public string SearchName { get; set; }
    /// <summary>
    ///     Gets or sets the ID of the role the user must have to be searched for;
    ///     <c>null</c> to not search via a role.
    /// </summary>
    public uint? RoleId { get; set; }
    /// <summary>
    ///     Gets or sets whether the user must have his/her mobile verified to be searched for;
    ///     <c>null</c> to not search via a verified status.
    /// </summary>
    public bool? IsMobileVerified { get; set; }
    /// <summary>
    ///     Gets or sets whether and how the searching results should be sorted by the activating time of the user;
    ///     <c>null</c> to not sort by active time.
    /// </summary>
    public SortMode? SortedByActiveTime { get; set; }
    /// <summary>
    ///     Gets or sets whether and how the searching results should be sorted by the joining time of the user;
    ///     <c>null</c> to not sort by joining time.
    /// </summary>
    public SortMode? SortedByJoinTime { get; set; }
}

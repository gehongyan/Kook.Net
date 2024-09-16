namespace Kook;

/// <summary>
///     提供用于搜索服务器用户的属性。
/// </summary>
/// <seealso cref="Kook.IGuild.SearchUsersAsync(System.Action{Kook.SearchGuildMemberProperties},System.Int32,Kook.CacheMode,Kook.RequestOptions)"/>
public class SearchGuildMemberProperties
{
    /// <summary>
    ///     获取或设置要用于搜索用户的用户名关键字。
    /// </summary>
    /// <remarks>
    ///     如果此属性不为 <see langword="null"/>，则搜索结果中的用户的用户名必须包含此属性的值；否则不使用用户名关键字作为搜索条件。
    /// </remarks>
    public string? SearchName { get; set; }

    /// <summary>
    ///     获取或设置要用于搜索用户的角色 ID。
    /// </summary>
    /// <remarks>
    ///     如果此属性不为 <see langword="null"/>，则搜索结果中的用户必须拥有此属性的值对应的角色；否则不使用角色 ID 作为搜索条件。
    /// </remarks>
    public uint? RoleId { get; set; }

    /// <summary>
    ///     获取或设置搜索用户的结果中所有用户是否都必须已经验证了手机号码。
    /// </summary>
    /// <remarks>
    ///     如果此属性为 <see langword="true"/>，则搜索结果中的所有用户必须已经验证了手机号码；如果此属性为
    ///     <see langword="false"/>，则搜索结果中的所有用户必须没有验证了手机号码；如果此属性为
    ///     <see langword="null"/>，则不使用手机号码验证状态作为搜索条件。
    /// </remarks>
    public bool? IsMobileVerified { get; set; }

    /// <summary>
    ///     获取或设置搜索用户的结果是否应该按用户在此服务器内的最近活跃时间排序。
    /// </summary>
    /// <remarks>
    ///     如果此属性不为 <see langword="null"/>，则搜索结果中的用户将按照其在此服务器内的最近活跃时间排序；否则不使用最近活跃时间作为排序条件。
    /// </remarks>
    public SortMode? SortedByActiveTime { get; set; }

    /// <summary>
    ///     获取或设置搜索用户的结果是否应该按用户加入此服务器的时间排序。
    /// </summary>
    /// <remarks>
    ///     如果此属性不为 <see langword="null"/>，则搜索结果中的用户将按照其加入此服务器的时间排序；否则不使用加入时间作为排序条件。
    /// </remarks>
    public SortMode? SortedByJoinTime { get; set; }
}

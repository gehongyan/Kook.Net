namespace Kook;

/// <summary>
///     表示一个可授予服务器用户的通用的角色。
/// </summary>
public interface IRole : IEntity<uint>, IDeletable, IMentionable, IComparable<IRole>
{
    /// <summary>
    ///     获取拥有此角色的服务器。
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     获取此角色的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取此角色的类型。
    /// </summary>
    RoleType Type { get; }

    /// <summary>
    ///     获取此角色的颜色。
    /// </summary>
    /// <remarks>
    ///     如果此用户所拥有的最高角色的颜色类型为渐变色，则此属性返回的颜色是渐变色权益失效后的回退颜色。
    /// </remarks>
    Color Color { get; }

    /// <summary>
    ///     获取此角色的颜色类型。
    /// </summary>
    ColorType ColorType { get; }

    /// <summary>
    ///     获取此角色的渐变色。
    /// </summary>
    /// <remarks>
    ///     如果此角色的颜色类型 <see cref="P:Kook.IRole.ColorType"/> 不为
    ///     <see cref="F:Kook.ColorType.Gradient"/>，则此属性会返回 <see langword="null"/>。
    /// </remarks>
    GradientColor? GradientColor { get; }

    /// <summary>
    ///     获取此角色在服务器角色列表中的位置。
    /// </summary>
    /// <remarks>
    ///     更小的数值表示更靠近列表顶部的位置。
    /// </remarks>
    int Position { get; }

    /// <summary>
    ///     获取拥有此角色的用户是否在用户列表中与普通在线成员分开显示。
    /// </summary>
    bool IsHoisted { get; }

    /// <summary>
    ///     获取是否允许任何人提及此角色。
    /// </summary>
    bool IsMentionable { get; }

    /// <summary>
    ///     获取此角色拥有的权限。
    /// </summary>
    GuildPermissions Permissions { get; }

    /// <summary>
    ///     修改此角色。
    /// </summary>
    /// <remarks>
    ///     此方法使用指定的属性修改当前角色信息。要查看可用的属性，请参考 <see cref="T:Kook.RoleProperties"/>。
    /// </remarks>
    /// <param name="func"> 一个包含修改角色属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    Task ModifyAsync(Action<RoleProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     获取拥有此角色的用户的集合。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取拥有此角色的所有服务器用户。此方法会根据 <see cref="F:Kook.KookConfig.MaxUsersPerBatch"/>
    ///     将请求拆分。换句话说，如果存在 500 个用户拥有此角色，而 <see cref="F:Kook.KookConfig.MaxUsersPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的用户集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);
}

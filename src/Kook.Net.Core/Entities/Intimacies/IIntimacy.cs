namespace Kook;

/// <summary>
///     表示一个通用的亲密度。
/// </summary>
public interface IIntimacy : IEntity<ulong>
{
    /// <summary>
    ///     获取与此亲密度相关的用户。
    /// </summary>
    IUser User { get; }

    /// <summary>
    ///     获取与此亲密度关联的社交信息。
    /// </summary>
    string SocialInfo { get; }

    /// <summary>
    ///     获取用户最近一次查看此亲密度的时间。
    /// </summary>
    DateTimeOffset LastReadAt { get; }

    /// <summary>
    ///     获取此亲密度最近一次修改的时间。
    /// </summary>
    DateTimeOffset? LastModifyAt { get; }

    /// <summary>
    ///     获取此亲密度的分数。
    /// </summary>
    /// <remarks>
    ///     亲密度的分数是一个整数，表示用户与此亲密度的亲密程度，最小值为 0，最大值为 2200。<br />
    ///     亲密度以 10 颗颜色与样式不同的心形图案组成。<br />
    ///     当亲密度分数不小于 1000 时，亲密度将显示为红色，其中，实心图案的数量为 <c>(Score - 1000) / 100</c> 的四舍五入值，最大值为 10，其余为空心图案。<br />
    ///     当亲密度分数小于 1000 时，亲密度将显示为灰色，其中，心碎图案的数量为 <c>10 - Score / 100</c> 的四舍五入值，其余为空心图案。
    /// </remarks>
    int Score { get; }

    /// <summary>
    ///     获取此亲密度的所有形象图像。
    /// </summary>
    IReadOnlyCollection<IntimacyImage> Images { get; }

    /// <summary>
    ///     修改此用户的亲密度信息。
    /// </summary>
    /// <param name="func"> 一个包含修改此亲密度信息的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous operation for updating the intimacy information.
    ///     一个表示异步修改的任务。
    /// </returns>
    Task UpdateAsync(Action<IntimacyProperties> func, RequestOptions? options = null);
}

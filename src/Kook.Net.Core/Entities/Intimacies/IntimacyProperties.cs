namespace Kook;

/// <summary>
///     提供用于修改 <see cref="Kook.IIntimacy" /> 的属性。
/// </summary>
/// <seealso cref="Kook.IIntimacy.UpdateAsync(System.Action{Kook.IntimacyProperties},Kook.RequestOptions)"/>
public class IntimacyProperties
{
    internal IntimacyProperties(string socialInfo, int score)
    {
        SocialInfo = socialInfo;
        Score = score;
    }

    /// <summary>
    ///     获取或设置要设置到此亲密度的社交信息。
    /// </summary>
    /// <remarks>
    ///     社交信息是展示给用户的文本块。<br />
    ///     如果此值为 <c>null</c>，则不会修改此亲密度的社交信息。
    /// </remarks>
    public string? SocialInfo { get; set; }

    /// <summary>
    ///     获取或设置要设置到此亲密度的分数。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>null</c>，则不会修改此亲密度的分数。
    /// </remarks>
    /// <seealso cref="Kook.IIntimacy.Score"/>
    public int? Score { get; set; }

    /// <summary>
    ///     获取或设置要设置到此亲密度的形象图像的 ID。
    /// </summary>
    /// <remarks>
    ///     如果此值为 <c>null</c>，则不会修改此亲密度的形象图像。
    /// </remarks>
    public uint? ImageId { get; set; }
}

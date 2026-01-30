namespace Kook;

/// <summary>
///     表示一个 KPM 官方陪玩服务器中的 VIP 信息。
/// </summary>
public record KpmVipInfo
{
    /// <summary>
    ///     获取 VIP 等级。
    /// </summary>
    public int Level { get; init; }

    /// <summary>
    ///     获取经验值。
    /// </summary>
    public int ExperiencePoints { get; init; }

    /// <summary>
    ///     获取价格倍率。
    /// </summary>
    /// <remarks>
    ///     示折扣后的金额与原价的比例，当值为 1.0 时表示无折扣。
    /// </remarks>
    public decimal PriceRate { get; init; }

    /// <summary>
    ///     获取 VIP 图标的 URL。
    /// </summary>
    public string Icon { get; init; }

    /// <summary>
    ///     获取 VIP 文本描述。
    /// </summary>
    public string Text { get; init; }

    private KpmVipInfo(int level, int experiencePoints, decimal priceRate, string icon, string text)
    {
        Level = level;
        ExperiencePoints = experiencePoints;
        PriceRate = priceRate;
        Icon = icon;
        Text = text;
    }

    internal static KpmVipInfo Create(int level, int experiencePoints, decimal priceRate, string icon, string text) =>
        new(level, experiencePoints, priceRate, icon, text);
}

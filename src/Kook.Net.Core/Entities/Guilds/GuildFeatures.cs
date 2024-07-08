using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     Represents a collection of features of a guild.
///     表示一个服务器的所有特性。
/// </summary>
public readonly struct GuildFeatures
{
    /// <summary>
    ///     获取此服务器的已识别特性。
    /// </summary>
    public GuildFeature Value { get; }

    /// <summary>
    ///     获取此服务器的所有特性的原始值。
    /// </summary>
    /// <remarks>
    ///     若特性未包含在 <see cref="GuildFeature"/> 中，则可以在此找到。
    /// </remarks>
    public IReadOnlyCollection<string> RawValues { get; }

    /// <summary>
    ///     获取此服务器是否为官方服务器。
    /// </summary>
    public bool IsOfficial => HasFeature(GuildFeature.Official);

    /// <summary>
    ///     获取此服务器是否为合作伙伴服务器。
    /// </summary>
    public bool IsPartner => HasFeature(GuildFeature.Partner);

    /// <summary>
    ///     获取此服务器是否为重点客户服务器。
    /// </summary>
    public bool IsKeyAccount => HasFeature(GuildFeature.KeyAccount);

    internal GuildFeatures(GuildFeature value, IEnumerable<string> rawValues)
    {
        Value = value;
        RawValues = rawValues.ToImmutableArray();
    }

    /// <summary>
    ///     获取此服务器是否具有指定的特性。
    /// </summary>
    /// <param name="feature"> 要进行检查的服务器特性。 </param>
    /// <returns> 如果此服务器具有指定的特性，则为 <c>true</c>，否则为 <c>false</c>。 </returns>
    public bool HasFeature(GuildFeature feature) =>
        Value.HasFlag(feature);

    /// <summary>
    ///     获取此服务器是否具有指定的特性。
    /// </summary>
    /// <param name="feature"> 要进行检查的服务器特性。 </param>
    /// <returns> 如果此服务器具有指定的特性，则为 <c>true</c>，否则为 <c>false</c>。 </returns>
    public bool HasFeature(string feature) =>
        RawValues.Contains(feature);

    internal void EnsureFeature(GuildFeature feature)
    {
        if (HasFeature(feature)) return;
        GuildFeatures features = this;
        IEnumerable<GuildFeature> values = Enum.GetValues(typeof(GuildFeature)).Cast<GuildFeature>();
        IEnumerable<GuildFeature> missingValues = values.Where(x => feature.HasFlag(x) && !features.Value.HasFlag(x)).ToList();
        throw new InvalidOperationException($"Missing required guild feature{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
    }
}

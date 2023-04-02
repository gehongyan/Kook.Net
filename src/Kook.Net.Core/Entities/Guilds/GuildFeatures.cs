using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     Represents a collection of features of a guild.
/// </summary>
public class GuildFeatures
{
    /// <summary>
    ///     Gets the flags of recognized features for this guild.
    /// </summary>
    public GuildFeature Value { get; }

    /// <summary>
    ///     Gets a collection of <see langword="string"/>s representing the raw values of the features.
    /// </summary>
    /// <remarks>
    ///     Features that are not contained in <see cref="GuildFeature"/> can be found here.
    /// </remarks>
    public IReadOnlyCollection<string> RawValues { get; }

    /// <summary>
    ///     Gets whether or not the guild is an official KOOK guild.
    /// </summary>
    public bool IsOfficial => HasFeature(GuildFeature.Official);

    /// <summary>
    ///     // TODO: To be investigated.
    /// </summary>
    public bool IsKa => HasFeature(GuildFeature.Ka);

    internal GuildFeatures(GuildFeature value, IEnumerable<string> rawValues)
    {
        Value = value;
        RawValues = rawValues.ToImmutableArray();
    }

    /// <summary>
    ///     Returns whether or not this guild has a feature.
    /// </summary>
    /// <param name="feature">The feature(s) to check for.</param>
    /// <returns><see langword="true"/> if this guild has the provided feature(s), otherwise <see langword="false"/>.</returns>
    public bool HasFeature(GuildFeature feature)
        => Value.HasFlag(feature);

    /// <summary>
    ///     Returns whether or not this guild has a feature.
    /// </summary>
    /// <param name="feature">The feature to check for.</param>
    /// <returns><see langword="true"/> if this guild has the provided feature, otherwise <see langword="false"/>.</returns>
    public bool HasFeature(string feature)
        => RawValues.Contains(feature);

    internal void EnsureFeature(GuildFeature feature)
    {
        if (!HasFeature(feature))
        {
            IEnumerable<GuildFeature> values = Enum.GetValues(typeof(GuildFeature)).Cast<GuildFeature>();

            IEnumerable<GuildFeature> missingValues = values.Where(x => feature.HasFlag(x) && !Value.HasFlag(x)).ToList();

            throw new InvalidOperationException($"Missing required guild feature{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
        }
    }
}

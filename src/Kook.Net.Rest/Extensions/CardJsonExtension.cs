using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.Net.Converters;

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Kook.Rest;

/// <summary>
///     Provides extension methods for <see cref="Card"/> and <see cref="CardBuilder"/>.
/// </summary>
public static class CardJsonExtension
{
    private static readonly Lazy<JsonSerializerOptions> _options = new(() => new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new CardConverter(), new ModuleConverter(), new ElementConverter() }
    });

    /// <summary>
    ///     Tries to parse a string into an <see cref="ICardBuilder"/>.
    /// </summary>
    /// <param name="json">The json string to parse.</param>
    /// <param name="builder">The <see cref="ICardBuilder"/> with populated values. An empty instance if method returns <c>false</c>.</param>
    /// <returns><c>true</c> if <paramref name="json"/> was successfully parsed. <c>false</c> if not.</returns>
    public static bool TryParseSingle(string json,
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
        [NotNullWhen(true)]
#endif
        out ICardBuilder builder)
    {
        try
        {
            CardBase model = JsonSerializer.Deserialize<CardBase>(json, _options.Value);

            if (model is not null)
            {
                builder = model.ToEntity().ToBuilder();
                return true;
            }

            builder = new CardBuilder();
            return false;
        }
        catch
        {
            builder = new CardBuilder();
            return false;
        }
    }

    /// <summary>
    ///     Tries to parse a string into an <see cref="ICardBuilder"/>.
    /// </summary>
    /// <param name="json">The json string to parse.</param>
    /// <param name="builders">A collection of <see cref="ICardBuilder"/> with populated values. An empty instance if method returns <c>false</c>.</param>
    /// <returns><c>true</c> if <paramref name="json"/> was successfully parsed. <c>false</c> if not.</returns>
    public static bool TryParseMany(string json,
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
        [NotNullWhen(true)]
#endif
        out IEnumerable<ICardBuilder> builders)
    {
        try
        {
            IEnumerable<CardBase> models = JsonSerializer.Deserialize<IEnumerable<CardBase>>(json, _options.Value);

            if (models is not null)
            {
                builders = models.Select(x => x.ToEntity().ToBuilder());
                return true;
            }

            builders = null;
            return false;
        }
        catch
        {
            builders = null;
            return false;
        }
    }

    /// <summary>
    ///     Parses a string into an <see cref="ICardBuilder"/>.
    /// </summary>
    /// <param name="json">The json string to parse.</param>
    /// <returns>An <see cref="ICardBuilder"/> with populated values from the passed <paramref name="json"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the string passed is not valid json.</exception>
    public static ICardBuilder ParseSingle(string json)
    {
        CardBase model = JsonSerializer.Deserialize<CardBase>(json, _options.Value);

        if (model is not null)
            return model.ToEntity().ToBuilder();

        return new CardBuilder();
    }

    /// <summary>
    ///     Parses a string into a collection of <see cref="ICardBuilder"/>s.
    /// </summary>
    /// <param name="json">The json string to parse.</param>
    /// <returns>A collection of <see cref="ICardBuilder"/>s with populated values from the passed <paramref name="json"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the string passed is not valid json.</exception>
    public static IEnumerable<ICardBuilder> ParseMany(string json)
    {
        IEnumerable<CardBase> models = JsonSerializer.Deserialize<IEnumerable<CardBase>>(json, _options.Value);

        if (models is not null)
            return models.Select(x => x.ToEntity().ToBuilder());

        return Enumerable.Empty<ICardBuilder>();
    }

    /// <summary>
    ///     Gets a Json formatted <c>string</c> from an <see cref="CardBuilder"/>.
    /// </summary>
    /// <remarks>
    ///     See <see cref="TryParseSingle"/> to parse Json back into card.
    /// </remarks>
    /// <param name="builder">The builder to format as Json <c>string</c>.</param>
    /// <param name="writeIndented">Whether to write the json with indents.</param>
    /// <returns>A Json <c>string</c> containing the data from the <paramref name="builder"/>.</returns>
    public static string ToJsonString(this ICardBuilder builder, bool writeIndented = true)
        => ToJsonString(builder.Build(), writeIndented);

    /// <summary>
    ///     Gets a Json formatted <c>string</c> from an <see cref="Card"/>.
    /// </summary>
    /// <remarks>
    ///     See <see cref="TryParseSingle"/> to parse Json back into card.
    /// </remarks>
    /// <param name="card">The card to format as Json <c>string</c>.</param>
    /// <param name="writeIndented">Whether to write the json with indents.</param>
    /// <returns>A Json <c>string</c> containing the data from the <paramref name="card"/>.</returns>
    public static string ToJsonString(this ICard card, bool writeIndented = true)
    {
        JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = writeIndented,
            Converters = { new CardConverter(), new ModuleConverter(), new ElementConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(card.ToModel(), options);
    }
}

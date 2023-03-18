using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Kook;

/// <summary>
///     Represents a unimplemented embed.
/// </summary>
public struct NotImplementedEmbed : IEmbed
{
    internal NotImplementedEmbed(string rawType, JsonNode jsonNode)
    {
        RawType = rawType;
        Url = null;
        JsonNode = jsonNode;
    }

    /// <inheritdoc />
    public EmbedType Type => EmbedType.NotImplemented;

    /// <summary>
    ///     Gets the type of the embed.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the type of the embed.
    /// </returns>
    /// <remarks>
    ///     This value originally came from the <c>type</c> field of the <see cref="JsonNode"/>.
    /// </remarks>
    public string RawType { get; internal set; }

    /// <inheritdoc />
    public string Url { get; internal set; }

    /// <summary>
    ///     Gets the raw JSON of the embed.
    /// </summary>
    /// <returns>
    ///     A JsonNode representing the raw JSON of the embed.
    /// </returns>
    public JsonNode JsonNode { get; internal set; }

    /// <summary>
    ///     Resolves the embed to a concrete type via JSON deserialization.
    /// </summary>
    /// <param name="options">
    ///     The options to use when deserializing the embed.
    /// </param>
    /// <typeparam name="T">
    ///     The concrete type to deserialize the embed to.
    /// </typeparam>
    /// <returns>
    ///     A <typeparamref name="T"/> representing the resolved embed.
    /// </returns>
    public T Resolve<T>(JsonSerializerOptions options = null)
        where T : IEmbed
    {
        options ??= new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        T embed = JsonNode.Deserialize<T>(options);
        return embed;
    }

    /// <summary>
    ///     Resolves the embed to a concrete type via delegate.
    /// </summary>
    /// <param name="resolvingFunc">
    ///     The resolving function to use when resolves the embed.
    /// </param>
    /// <typeparam name="T">
    ///     The concrete type to deserialize the embed to.
    /// </typeparam>
    /// <returns>
    ///     A <typeparamref name="T"/> representing the resolved embed.
    /// </returns>
    public T Resolve<T>(Func<NotImplementedEmbed, T> resolvingFunc)
        where T : IEmbed
    {
        T embed = resolvingFunc(this);
        return embed;
    }
}

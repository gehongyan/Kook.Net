using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Kook;

/// <summary>
///     Represents a poke resource that is not implemented to be resolved yet.
/// </summary>
public struct NotImplementedPokeResource : IPokeResource
{
    internal NotImplementedPokeResource(string rawType, JsonNode jsonNode)
    {
        RawType = rawType;
        JsonNode = jsonNode;
    }

    /// <inheritdoc />
    public PokeResourceType Type => PokeResourceType.NotImplemented;

    /// <summary>
    ///     Gets the type of the poke resource.
    /// </summary>
    /// <returns>
    ///     A <see langword="string"/> representing the type of the poke resource.
    /// </returns>
    /// <remarks>
    ///     This value originally came from the <c>type</c> field of the <see cref="JsonNode"/>.
    /// </remarks>
    public string RawType { get; internal set; }

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
        where T : IPokeResource
    {
        options ??= new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        T pokeResource = JsonNode.Deserialize<T>(options);
        return pokeResource;
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
    public T Resolve<T>(Func<NotImplementedPokeResource, T> resolvingFunc)
        where T : IPokeResource
    {
        T pokeResource = resolvingFunc(this);
        return pokeResource;
    }
}

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Kook;

/// <summary>
///     表示一个未能被解析为已知的强类型的 POKE 资源。
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
    ///     获取此 POKE 资源的类型的原始值。
    /// </summary>
    public string RawType { get; }

    /// <summary>
    ///     获取此 POKE 资源的原始 JSON。
    /// </summary>
    public JsonNode JsonNode { get; }

    /// <summary>
    ///     通过 JSON 反序列化将 POKE 资源解析为具体类型。
    /// </summary>
    /// <param name="options"> 用于反序列化操作的选项。 </param>
    /// <typeparam name="T"> 要解析为的具体类型。 </typeparam>
    /// <returns> 解析后的 POKE 资源。 </returns>
    public T? Resolve<T>(JsonSerializerOptions? options = null)
        where T : IPokeResource
    {
        options ??= new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        T? pokeResource = JsonNode.Deserialize<T>(options);
        return pokeResource;
    }

    /// <summary>
    ///     通过指定的解析函数将 POKE 资源 解析为具体类型。
    /// </summary>
    /// <param name="resolvingFunc"> 用于解析的函数。 </param>
    /// <typeparam name="T"> 要解析为的具体类型。 </typeparam>
    /// <returns> 解析后的 POKE 资源 。 </returns>
    public T Resolve<T>(Func<NotImplementedPokeResource, T> resolvingFunc)
        where T : IPokeResource
    {
        T pokeResource = resolvingFunc(this);
        return pokeResource;
    }
}

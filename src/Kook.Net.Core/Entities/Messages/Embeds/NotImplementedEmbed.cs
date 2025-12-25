using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Kook;

/// <summary>
///     表示一个消息中未能被解析为已知的强类型的嵌入式内容。
/// </summary>
public struct NotImplementedEmbed : IEmbed
{
    internal NotImplementedEmbed(string rawType, JsonNode jsonNode)
    {
        RawType = rawType;
        JsonNode = jsonNode;
    }

    /// <inheritdoc />
    public EmbedType Type => EmbedType.NotImplemented;

    /// <summary>
    ///     获取嵌入式内容的类型的原始值。
    /// </summary>
    public string RawType { get; }

    /// <summary>
    ///     获取嵌入式内容的原始 JSON。
    /// </summary>
    public JsonNode JsonNode { get; }

    /// <summary>
    ///     通过 JSON 反序列化将嵌入式内容解析为具体类型。
    /// </summary>
    /// <param name="options"> 用于反序列化操作的选项。 </param>
    /// <typeparam name="T"> 要解析为的具体类型。 </typeparam>
    /// <returns> 解析后的嵌入式内容。 </returns>
#if NET5_0_OR_GREATER
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a resolving function for AOT applications.")]
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use the overload that takes a resolving function for AOT applications.")]
#endif
    public T? Resolve<T>(JsonSerializerOptions? options = null)
        where T : IEmbed
    {
        options ??= new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        T? embed = JsonNode.Deserialize<T>(options);
        return embed;
    }

    /// <summary>
    ///     通过 JSON 反序列化将嵌入式内容解析为具体类型。
    /// </summary>
    /// <param name="jsonTypeInfo"> 用于反序列化操作的类型信息。 </param>
    /// <typeparam name="T"> 要解析为的具体类型。 </typeparam>
    /// <returns> 解析后的嵌入式内容。 </returns>
    public T? Resolve<T>(JsonTypeInfo jsonTypeInfo)
        where T : IEmbed
    {
        if (jsonTypeInfo is not JsonTypeInfo<T> typedJsonTypeInfo)
            throw new ArgumentException($"The provided JsonTypeInfo is not of the expected type {typeof(T).FullName}.", nameof(jsonTypeInfo));
        T? embed = JsonNode.Deserialize(typedJsonTypeInfo);
        return embed;
    }

    /// <summary>
    ///     通过 JSON 反序列化将嵌入式内容解析为具体类型。
    /// </summary>
    /// <param name="jsonTypeInfo"> 用于反序列化操作的类型信息。 </param>
    /// <typeparam name="T"> 要解析为的具体类型。 </typeparam>
    /// <returns> 解析后的嵌入式内容。 </returns>
    public T? Resolve<T>(JsonTypeInfo<T> jsonTypeInfo)
        where T : IEmbed
    {
        T? embed = JsonNode.Deserialize(jsonTypeInfo);
        return embed;
    }

    /// <summary>
    ///     通过指定的解析函数将嵌入式内容解析为具体类型。
    /// </summary>
    /// <param name="resolvingFunc"> 用于解析的函数。 </param>
    /// <typeparam name="T"> 要解析为的具体类型。 </typeparam>
    /// <returns> 解析后的嵌入式内容。 </returns>
    public T Resolve<T>(Func<NotImplementedEmbed, T> resolvingFunc)
        where T : IEmbed
    {
        T embed = resolvingFunc(this);
        return embed;
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Kook.API;
using Kook.Net.Converters;

namespace Kook.Rest;

/// <summary>
///     提供一系列用于 <see cref="Card"/> 和 <see cref="CardBuilder"/> 的扩展方法。
/// </summary>
public static class CardJsonExtension
{
    private static readonly Lazy<JsonSerializerOptions> _options = new(() => new JsonSerializerOptions(KookJsonSerializerContext.Default.Options)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { CardConverterFactory.Instance }
    });

    /// <summary>
    ///     尝试将字符串解析为单个卡片构造器 <see cref="ICardBuilder"/>。
    /// </summary>
    /// <param name="json"> 要解析的 JSON 字符串。 </param>
    /// <param name="builder"> 如果所提供的 JSON 字符串可以解析为单个卡片构造器实例，则返回该实例；否则返回 <c>null</c>。 </param>
    /// <returns> 如果成功解析 <paramref name="json"/>，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryParseSingle(string json, [NotNullWhen(true)] out ICardBuilder? builder)
    {
        try
        {
            CardBase? model = JsonSerializer.Deserialize<CardBase>(json, _options.Value);

            if (model is not null)
            {
                builder = model.ToEntity().ToBuilder();
                return true;
            }

            builder = null;
            return false;
        }
        catch
        {
            builder = null;
            return false;
        }
    }

    /// <summary>
    ///     尝试将字符串解析为多个卡片构造器 <see cref="ICardBuilder"/>。
    /// </summary>
    /// <param name="json"> 要解析的 JSON 字符串。 </param>
    /// <param name="builders"> 如果所提供的 JSON 字符串可以解析为多个卡片构造器实例，则返回该实例；否则返回 <c>null</c>。 </param>
    /// <returns> 如果成功解析 <paramref name="json"/>，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool TryParseMany(string json, [NotNullWhen(true)] out IEnumerable<ICardBuilder>? builders)
    {
        try
        {
            IEnumerable<CardBase>? models = JsonSerializer.Deserialize<IEnumerable<CardBase>>(json, _options.Value);

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
    ///     将字符串解析为单个卡片构造器 <see cref="ICardBuilder"/>。
    /// </summary>
    /// <param name="json"> 要解析的 JSON 字符串。 </param>
    /// <returns> 单个卡片构造器实例。 </returns>
    /// <exception cref="InvalidOperationException"> 如果无法将 JSON 解析为单个卡片构造器。 </exception>
    public static ICardBuilder ParseSingle(string json)
    {
        CardBase model = JsonSerializer.Deserialize<CardBase>(json, _options.Value)
            ?? throw new JsonException("Unable to parse json into card.");
        return model.ToEntity().ToBuilder();
    }

    /// <summary>
    ///     将字符串解析为多个卡片构造器 <see cref="ICardBuilder"/>。
    /// </summary>
    /// <param name="json"> 要解析的 JSON 字符串。 </param>
    /// <returns> 多个卡片构造器实例。 </returns>
    /// <exception cref="InvalidOperationException"> 如果无法将 JSON 解析为多个卡片构造器。 </exception>
    public static IEnumerable<ICardBuilder> ParseMany(string json)
    {
        JsonTypeInfo<IEnumerable<CardBase>> typeInfo = _options.Value.GetTypeInfo<IEnumerable<CardBase>>();
        IEnumerable<CardBase> models = JsonSerializer.Deserialize(json, typeInfo)
            ?? throw new JsonException("Unable to parse json into cards.");
        return models.Select(x => x.ToEntity().ToBuilder());
    }

    /// <summary>
    ///     将卡片构造器 <see cref="ICardBuilder"/> 序列化为 JSON 格式的字符串。
    /// </summary>
    /// <param name="builder"> 要序列化的卡片构造器。 </param>
    /// <param name="writeIndented"> 是否使用缩进写入 JSON。 </param>
    /// <returns> 包含来自 <paramref name="builder"/> 的数据的 JSON 字符串。 </returns>
    public static string ToJsonString(this ICardBuilder builder, bool writeIndented = true) =>
        ToJsonString(builder.Build(), writeIndented);

    /// <summary>
    ///     将卡片 <see cref="ICard"/> 序列化为 JSON 格式的字符串。
    /// </summary>
    /// <param name="card"> 要序列化的卡片构造器。 </param>
    /// <param name="writeIndented"> 是否使用缩进写入 JSON。 </param>
    /// <returns> 包含来自 <paramref name="card"/> 的数据的 JSON 字符串。 </returns>
    public static string ToJsonString(this ICard card, bool writeIndented = true)
    {
        JsonSerializerOptions options = new(KookJsonSerializerContext.Default.Options)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = writeIndented,
            Converters = { CardConverterFactory.Instance }
        };
        CardBase model = card.ToModel();
        JsonTypeInfo typeInfo = options.GetTypeInfo(model.GetType());
        return JsonSerializer.Serialize(model, typeInfo);
    }
}

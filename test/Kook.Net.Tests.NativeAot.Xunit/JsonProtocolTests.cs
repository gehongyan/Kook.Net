using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Net.Contexts;
using Kook.Net.Converters;
using Kook.Rest;
using Xunit;

namespace Kook.NativeAot.Xunit;

public class JsonProtocolTests
{
    private static JsonSerializerOptions ProtocolOptions()
    {
        JsonSerializerOptions options = new(KookRestJsonSerializerContext.Default.Options)
        {
            Converters = { CardConverterFactory.Instance }
        };
        options.TypeInfoResolverChain.Add(KookWebSocketJsonSerializerContext.Default);
        options.TypeInfoResolverChain.Add(KookWebhookJsonSerializerContext.Default);
        return options;
    }

    [Fact]
    public void RuntimeUsesExpectedExecutionMode()
    {
        Assert.False(JsonSerializer.IsReflectionEnabledByDefault);
#if EXPECT_NATIVE_AOT
        Assert.False(RuntimeFeature.IsDynamicCodeSupported);
        Assert.False(RuntimeFeature.IsDynamicCodeCompiled);
#else
        Assert.True(RuntimeFeature.IsDynamicCodeSupported);
#endif
    }

    [Fact]
    public void PublicCardsRoundTripNestedModulesAndElements()
    {
        CardBuilder source = new CardBuilder()
            .WithSize(CardSize.Large)
            .AddModule(new HeaderModuleBuilder().WithText("AOT <测试>"))
            .AddModule(new SectionModuleBuilder().WithText("**markdown**", true))
            .AddModule(new DividerModuleBuilder());
        string json = source.Build().ToJsonString(false);
        Assert.Contains("AOT <测试>", json);
        ICard actual = CardJsonExtension.ParseSingle(json).Build();
        Assert.Equal(json, actual.ToJsonString(false));
        Assert.Single(CardJsonExtension.ParseMany("[" + json + "]"));
    }

    [Fact]
    public void UnknownCardsAreRejected()
    {
        Assert.False(CardJsonExtension.TryParseSingle("{\"type\":\"future\"}", out _));
    }

    [Fact]
    public void KnownEmbedUsesNestedMetadata()
    {
        JsonSerializerOptions options = ProtocolOptions();
        API.EmbedBase? result = JsonSerializer.Deserialize("{\"type\":\"image\",\"origin_url\":\"source\",\"url\":\"image\"}", options.GetTypedTypeInfo<API.EmbedBase>());
        Assert.Equal("image", Assert.IsType<API.ImageEmbed>(result).Url);
    }

    [Fact]
    public void UnknownEmbedPreservesPayloadAndResolvesUserMetadata()
    {
        JsonSerializerOptions options = ProtocolOptions();
        API.EmbedBase? result = JsonSerializer.Deserialize("{\"type\":\"future\",\"Value\":\"custom\"}", options.GetTypedTypeInfo<API.EmbedBase>());
        API.NotImplementedEmbed unknown = Assert.IsType<API.NotImplementedEmbed>(result);
        Assert.Equal("custom", unknown.RawJsonNode["Value"]!.GetValue<string>());
        NotImplementedEmbed publicEmbed = new("future", unknown.RawJsonNode);
        Assert.Equal("custom", publicEmbed.Resolve(ApplicationJsonContext.Default.CustomEmbed)!.Value);
    }

    [Fact]
    public void UnknownPokeResolvesUserMetadata()
    {
        NotImplementedPokeResource poke = new("future", JsonNode.Parse("{\"Value\":\"custom\"}")!);
        Assert.Equal("custom", poke.Resolve(ApplicationJsonContext.Default.CustomPoke)!.Value);
    }

    [Fact]
    public void CustomTemplateOptionsRemainUnchanged()
    {
        JsonSerializerOptions original = new() { TypeInfoResolver = ApplicationJsonContext.Default, WriteIndented = true };
        original.MakeReadOnly();
        JsonSerializerOptions copy = JsonSerializerOptionsExtensions.GetUserSerializerOptions(original);
        string json = JsonSerializer.Serialize(new Template { Value = "hello" }, copy.GetTypedTypeInfo<Template>());
        Assert.Contains("hello", json);
        Assert.True(original.IsReadOnly);
        Assert.Single(original.TypeInfoResolverChain);
        Assert.NotSame(original, copy);
    }

    [Fact]
    public void CustomTemplateRequiresMetadataWhenReflectionIsDisabled()
    {
        NotSupportedException exception = Assert.Throws<NotSupportedException>(() => JsonSerializerOptionsExtensions.GetUserSerializerOptions(null));
        Assert.Contains("TypeInfoResolver", exception.Message);
    }

    [Fact]
    public void RestRequestOmitsOptionalFieldsAndKeepsProtocolNames()
    {
        JsonSerializerOptions options = ProtocolOptions();
        CreateMessageParams request = new() { Type = MessageType.Text, ChannelId = 42, Content = "hello" };
        string json = JsonSerializer.Serialize(request, options.GetTypedTypeInfo<CreateMessageParams>());
        using JsonDocument document = JsonDocument.Parse(json);
        Assert.Equal(42UL, document.RootElement.GetProperty("target_id").GetUInt64());
        Assert.Equal("hello", document.RootElement.GetProperty("content").GetString());
        Assert.False(document.RootElement.TryGetProperty("temp_target_id", out _));
    }

    [Fact]
    public void RestPaginationUsesClosedGenericMetadataAndConverters()
    {
        JsonSerializerOptions options = ProtocolOptions();
        PagedResponseBase<API.Guild>? page = JsonSerializer.Deserialize("{\"items\":[],\"meta\":{\"page\":2,\"page_total\":3,\"page_size\":50,\"total\":101},\"sort\":{\"id\":1}}", options.GetTypedTypeInfo<PagedResponseBase<API.Guild>>());
        Assert.NotNull(page);
        Assert.Empty(page.Items);
        Assert.Equal(2, page.Meta.Page);
        Assert.Equal(API.Rest.SortMode.Ascending, page.PageSortInfo.SortMode);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("null")]
    public void GatewayNullableChannelHandlesEmptyValues(string value)
    {
        JsonSerializerOptions options = ProtocolOptions();
        ChannelBatchUpdateEvent? batch = JsonSerializer.Deserialize("{\"added_channel\":" + value + ",\"updated_channel\":[]}", options.GetTypedTypeInfo<ChannelBatchUpdateEvent>());
        Assert.NotNull(batch);
        Assert.Null(batch.AddedChannel);
        Assert.Empty(batch.UpdatedChannels);
    }

    [Fact]
    public void GatewayNullableChannelUsesNestedObjectMetadata()
    {
        JsonSerializerOptions options = ProtocolOptions();
        ChannelBatchUpdateEvent? batch = JsonSerializer.Deserialize("{\"added_channel\":{\"name\":\"channel\",\"permission_overwrites\":[],\"permission_users\":[]},\"updated_channel\":[]}", options.GetTypedTypeInfo<ChannelBatchUpdateEvent>());
        Assert.Equal("channel", batch?.AddedChannel?.Name);
    }

    [Fact]
    public void GatewayFramePreservesSequenceAndPayload()
    {
        JsonSerializerOptions options = ProtocolOptions();
        GatewaySocketFrame? frame = JsonSerializer.Deserialize("{\"s\":0,\"sn\":12,\"d\":{\"type\":1,\"content\":\"hello\"}}", options.GetTypedTypeInfo<GatewaySocketFrame>());
        Assert.NotNull(frame);
        Assert.Equal(12, frame.Sequence);
        Assert.Equal("hello", frame.Payload!.Value.GetProperty("content").GetString());
        string json = JsonSerializer.Serialize(frame, options.GetTypedTypeInfo<GatewaySocketFrame>());
        Assert.Contains("hello", json);
    }

    [Fact]
    public void WebhookChallengeUsesGeneratedMetadata()
    {
        JsonSerializerOptions options = ProtocolOptions();
        API.Webhook.GatewayChallengeFrame frame = new() { Challenge = "native-xunit" };
        string json = JsonSerializer.Serialize(frame, options.GetTypedTypeInfo<API.Webhook.GatewayChallengeFrame>());
        Assert.Equal("{\"challenge\":\"native-xunit\"}", json);
    }
}

internal sealed class Template { public string? Value { get; set; } }
internal sealed class CustomEmbed : IEmbed { public EmbedType Type => EmbedType.NotImplemented; public string? Value { get; set; } }
internal sealed class CustomPoke : IPokeResource { public PokeResourceType Type => PokeResourceType.NotImplemented; public string? Value { get; set; } }
[JsonSerializable(typeof(Template))]
[JsonSerializable(typeof(CustomEmbed))]
[JsonSerializable(typeof(CustomPoke))]
internal partial class ApplicationJsonContext : JsonSerializerContext;

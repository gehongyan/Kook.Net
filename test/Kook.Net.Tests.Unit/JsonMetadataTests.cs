using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.Net.Contexts;
using Kook.Net.Rest;
using Kook.Rest;
using NSubstitute;
using Xunit;

namespace Kook;

[Trait("Category", "Unit")]
public class JsonMetadataTests
{
    [Fact]
    public void EveryConcreteRestAndGatewayProtocolTypeHasGeneratedMetadata()
    {
        JsonSerializerOptions options = new(KookRestJsonSerializerContext.Default.Options);
        options.TypeInfoResolverChain.Add(KookWebSocketJsonSerializerContext.Default);
        Type[] types = new[] { typeof(KookRestApiClient).Assembly, typeof(KookSocketApiClient).Assembly }
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.Namespace?.StartsWith("Kook.API", StringComparison.Ordinal) == true
                && !typeof(KookRestApiClient).IsAssignableFrom(type) && !type.ContainsGenericParameters && !type.IsAbstract && !type.IsInterface && !type.IsNested)
            .ToArray();
        Assert.NotEmpty(types);
        foreach (Type type in types)
            Assert.NotNull(options.GetTypeInfo(type));
    }

    [Fact]
    public void UserOptionsPreserveReflectionDefaultsAndReadOnlyOptionsOnJit()
    {
        Parameters parameters = new() { Content = "<test>" };
        JsonSerializerOptions defaults = JsonSerializerOptionsExtensions.GetUserSerializerOptions(null);
        Assert.Equal(JsonSerializer.Serialize(parameters), JsonSerializer.Serialize(parameters, defaults.GetTypedTypeInfo<Parameters>()));
        JsonSerializerOptions original = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        string expected = JsonSerializer.Serialize(parameters, original);
        Assert.True(original.IsReadOnly);
        JsonSerializerOptions copy = JsonSerializerOptionsExtensions.GetUserSerializerOptions(original);
        Assert.Equal(expected, JsonSerializer.Serialize(parameters, copy.GetTypedTypeInfo<Parameters>()));
    }

    [Fact]
    public void OptionalPackageModelsRetainJitReflectionFallback()
    {
        using TestApiClient client = new();
        Assert.Contains("extension", client.Serialize(new Parameters { Content = "extension" }));
    }

    private sealed class TestApiClient() : KookRestApiClient(_ => Substitute.For<IRestClient>(), "test", "en-US")
    {
        public string Serialize(object value) => SerializeJson(value);
    }

    private sealed class Parameters
    {
        public string? Content { get; set; }
    }
}

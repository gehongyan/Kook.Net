using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Net.Contexts;

/// <summary>
///     Provides JSON serialization context for Native AOT compatibility.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowReadingFromString)]
[JsonSerializable(typeof(IReadOnlyCollection<Kook.API.Rest.ThreadTag>))]
[JsonSerializable(typeof(ValidateCardsParams))]
internal partial class KookExperimentalJsonSerializerContexts : JsonSerializerContext;

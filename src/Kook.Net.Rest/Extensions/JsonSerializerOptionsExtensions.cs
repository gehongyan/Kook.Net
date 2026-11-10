using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Kook.Net.Contexts;

namespace Kook.Rest;

/// <summary>
///     Provides extension methods for <see cref="JsonSerializerOptions"/>.
/// </summary>
internal static class JsonSerializerOptionsExtensions
{
    // Preserve JIT extensibility for application-owned and optional-package protocol types.
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "The reflection resolver is created only when the feature switch JsonSerializer.IsReflectionEnabledByDefault is enabled.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "The reflection resolver branch is removed when JSON reflection is disabled by NativeAOT.")]
#endif
    internal static IJsonTypeInfoResolver? CreateReflectionResolver() =>
        JsonSerializer.IsReflectionEnabledByDefault ? new DefaultJsonTypeInfoResolver() : null;

    // Public template APIs accept application-owned types through their existing options parameter.
    internal static JsonSerializerOptions GetUserSerializerOptions(JsonSerializerOptions? options)
    {
        options = options is null
            ? new JsonSerializerOptions()
            : new JsonSerializerOptions(options);
        if (options.TypeInfoResolver is null)
        {
            options.TypeInfoResolver = CreateReflectionResolver()
                ?? throw new NotSupportedException("Supply JsonSerializerOptions with a source-generated TypeInfoResolver for custom template parameters when JSON reflection is disabled.");
        }
        options.TypeInfoResolverChain.Add(KookRestJsonSerializerContext.Default);
        return options;
    }

    /// <summary>
    ///     Gets the <see cref="JsonTypeInfo{T}"/> for the specified type from the <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">The type to get the type info for.</typeparam>
    /// <param name="options">The serializer options containing the type resolver.</param>
    /// <returns>The <see cref="JsonTypeInfo{T}"/> for the specified type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the type info cannot be resolved from the options.
    /// </exception>
    public static JsonTypeInfo<T> GetTypedTypeInfo<T>(this JsonSerializerOptions options)
    {
        if (options.GetTypeInfo(typeof(T)) is JsonTypeInfo<T> typeInfo)
            return typeInfo;

        throw new InvalidOperationException(
            $"Unable to resolve JsonTypeInfo for type {typeof(T).FullName}. " +
            "Ensure the type is registered in the JsonSerializerContext.");
    }
}

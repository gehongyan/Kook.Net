using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Kook.Net.Rest;

/// <summary>
///     Provides extension methods for <see cref="JsonSerializerOptions"/>.
/// </summary>
internal static class JsonSerializerOptionsExtensions
{
    /// <summary>
    ///     Gets the <see cref="JsonTypeInfo{T}"/> for the specified type from the <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <typeparam name="T">The type to get the type info for.</typeparam>
    /// <param name="options">The serializer options containing the type resolver.</param>
    /// <returns>The <see cref="JsonTypeInfo{T}"/> for the specified type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the type info cannot be resolved from the options.
    /// </exception>
    public static JsonTypeInfo<T> GetTypeInfo<T>(this JsonSerializerOptions options)
    {
        if (options.TypeInfoResolver?.GetTypeInfo(typeof(T), options) is JsonTypeInfo<T> typeInfo)
            return typeInfo;

        throw new InvalidOperationException(
            $"Unable to resolve JsonTypeInfo for type {typeof(T).FullName}. " +
            "Ensure the type is registered in the JsonSerializerContext.");
    }

    /// <summary>
    ///     Gets the <see cref="JsonTypeInfo"/> for the specified type from the <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <param name="options">The serializer options containing the type resolver.</param>
    /// <param name="type">The type to get the type info for.</param>
    /// <returns>The <see cref="JsonTypeInfo"/> for the specified type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the type info cannot be resolved from the options.
    /// </exception>
    public static JsonTypeInfo GetTypeInfo(this JsonSerializerOptions options, Type type)
    {
        if (options.TypeInfoResolver?.GetTypeInfo(type, options) is JsonTypeInfo typeInfo)
            return typeInfo;

        throw new InvalidOperationException(
            $"Unable to resolve JsonTypeInfo for type {type.FullName}. " +
            "Ensure the type is registered in the JsonSerializerContext.");
    }
}

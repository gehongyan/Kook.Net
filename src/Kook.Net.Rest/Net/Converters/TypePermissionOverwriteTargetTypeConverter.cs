using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Net.Converters;

internal class TypePermissionOverwriteTargetTypeConverter : JsonConverter<PermissionOverwriteTargetType>
{
    public override PermissionOverwriteTargetType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "role" => PermissionOverwriteTargetType.Role,
            "user" => PermissionOverwriteTargetType.User,
            _ => PermissionOverwriteTargetType.User
        };

    public override void Write(Utf8JsonWriter writer, PermissionOverwriteTargetType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            PermissionOverwriteTargetType.Role => "role",
            PermissionOverwriteTargetType.User => "user",
            _ => "user"
        });
}

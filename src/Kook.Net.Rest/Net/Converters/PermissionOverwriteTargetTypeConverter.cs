using Kook.API.Rest;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class PermissionOverwriteTargetTypeConverter : JsonConverter<PermissionOverwriteTargetType>
{
    public override PermissionOverwriteTargetType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "role_id" => PermissionOverwriteTargetType.Role,
            "user_id" => PermissionOverwriteTargetType.User,
            _ => PermissionOverwriteTargetType.User
        };
    }

    public override void Write(Utf8JsonWriter writer, PermissionOverwriteTargetType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            PermissionOverwriteTargetType.Role => "role_id",
            PermissionOverwriteTargetType.User => "user_id",
            _ => "user_id"
        });
    }
}

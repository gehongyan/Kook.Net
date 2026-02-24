using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class TypeIdPermissionOverwriteTargetTypeConverter : JsonConverter<PermissionOverwriteTarget>
{
    public override PermissionOverwriteTarget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "role_id" => PermissionOverwriteTarget.Role,
            "user_id" => PermissionOverwriteTarget.User,
            _ => PermissionOverwriteTarget.User
        };

    public override void Write(Utf8JsonWriter writer, PermissionOverwriteTarget value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            PermissionOverwriteTarget.Role => "role_id",
            PermissionOverwriteTarget.User => "user_id",
            _ => "user_id"
        });
}

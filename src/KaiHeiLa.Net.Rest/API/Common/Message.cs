using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class Message
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("type")]
    public MessageType Type { get; set; }
    
    [JsonPropertyName("content")]
    public string Content { get; set; }
    
    [JsonPropertyName("mention")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong[] Mention { get; set; }

    [JsonPropertyName("mention_all")]
    public bool MentionAll { get; set; }

    [JsonPropertyName("mention_roles")]
    public uint[] MentionRoles { get; set; }

    [JsonPropertyName("mention_here")]
    public bool MentionHere { get; set; }

    [JsonPropertyName("embeds")]
    public Embed[] Embeds { get; set; }

    [JsonPropertyName("attachment")]
    public Attachment Attachment { get; set; }

    [JsonPropertyName("create_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset CreateAt { get; set; }
    
    [JsonPropertyName("update_at")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset UpdateAt { get; set; }

    [JsonPropertyName("reactions")]
    public Reaction[] Reactions { get; set; }
    
    [JsonPropertyName("author")]
    public User Author { get; set; }

    [JsonPropertyName("image_name")]
    public string ImageName { get; set; }

    [JsonPropertyName("read_status")]
    public bool ReadStatus { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }

    [JsonPropertyName("mention_info")]
    public MentionInfo MentionInfo { get; set; }
}

internal class MentionInfo
{
    public MentionUser[] MentionUsers { get; set; }
    public MentionRole[] MentionRoles { get; set; }
}

internal class MentionUser
{
    [JsonPropertyName("id")] public ulong Id { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("full_name")] public string FullName { get; set; }
    [JsonPropertyName("avatar")] public string Avatar { get; set; }
}

internal class MentionRole
{
    [JsonPropertyName("role_id")] public uint RoleId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(ColorConverter))]
    public Color Color { get; set; }

    [JsonPropertyName("position")] public int Position { get; set; }
    [JsonPropertyName("hoist")] public int Hoist { get; set; }
    [JsonPropertyName("mentionable")] public int Mentionable { get; set; }
    [JsonPropertyName("permissions")] public ulong Permissions { get; set; }
}


namespace Kook.WebSocket;

internal static class SocketEntityExtensions
{
    public static GuildEmote ToEntity(this API.Gateway.GuildEmojiEvent model, ulong guildId) =>
        new(model.Id,
            model.Name,
            model.Type == EmojiType.Animated,
            guildId,
            null);

    public static RolePermissionOverwrite ToEntity(this API.RolePermissionOverwrite model) =>
        new(model.RoleId, new OverwritePermissions(model.Allow, model.Deny));

    public static UserPermissionOverwrite ToEntity(this API.UserPermissionOverwrite model, KookSocketClient kook, ClientState state) =>
        new(SocketGlobalUser.Create(kook, state, model.User), new OverwritePermissions(model.Allow, model.Deny));
}

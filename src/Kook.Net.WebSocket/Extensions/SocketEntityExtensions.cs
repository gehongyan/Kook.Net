namespace Kook.WebSocket;

internal static class SocketEntityExtensions
{
    public static GuildEmote ToEntity(this API.Gateway.GuildEmojiEvent model, ulong guildId)
        => new GuildEmote(model.Id,
            model.Name,
            model.Type == EmojiType.Animated,
            guildId,
            null);
    
    public static RolePermissionOverwrite ToEntity(this API.RolePermissionOverwrite model)
    {
        return new RolePermissionOverwrite(model.RoleId, new OverwritePermissions(model.Allow, model.Deny));
    }
    
    public static UserPermissionOverwrite ToEntity(this API.UserPermissionOverwrite model, KookSocketClient kook, ClientState state)
    {
        return new UserPermissionOverwrite(SocketGlobalUser.Create(kook, state, model.User), new OverwritePermissions(model.Allow, model.Deny));
    }
}
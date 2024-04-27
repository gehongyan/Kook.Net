namespace Kook.Commands;

/// <summary>
///     A <see cref="TypeReader"/> for parsing objects implementing <see cref="IMessage"/>.
/// </summary>
/// <typeparam name="T">The type to be checked; must implement <see cref="IMessage"/>.</typeparam>
public class MessageTypeReader<T> : TypeReader
    where T : class, IMessage
{
    /// <inheritdoc />
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        //By Id (1.0)
        if (Guid.TryParse(input, out Guid id)
            && await context.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) is T msg)
            return TypeReaderResult.FromSuccess(msg);
        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Message not found.");
    }
}

using System.Globalization;

namespace Kook.Commands;

/// <summary>
///     表示一个用于解析字符串到实现了 <see cref="Kook.IChannel"/> 的对象的类型读取器。
/// </summary>
/// <typeparam name="T"> 要解析为的频道类型。 </typeparam>
public class ChannelTypeReader<T> : TypeReader
    where T : class, IChannel
{
    /// <inheritdoc />
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (context.Guild != null)
        {
            Dictionary<ulong, TypeReaderValue> results = new();
            IReadOnlyCollection<IGuildChannel> channels = await context.Guild.GetChannelsAsync(CacheMode.CacheOnly).ConfigureAwait(false);
            TagMode tagMode = context.Message.Type switch
            {
                MessageType.Text => TagMode.PlainText,
                MessageType.KMarkdown => TagMode.KMarkdown,
                _ => throw new ArgumentOutOfRangeException(nameof(context.Message.Type))
            };
            //By Mention (1.0)
            if (MentionUtils.TryParseChannel(input, out ulong id, tagMode))
                AddResult(results, await context.Guild.GetChannelAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 1.00f);

            //By Id (0.9)
            if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                AddResult(results, await context.Guild.GetChannelAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 0.90f);

            //By Name (0.7-0.8)
            foreach (IGuildChannel channel in channels.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)))
                AddResult(results, channel as T, channel.Name == input ? 0.80f : 0.70f);

            if (results.Count > 0)
                return TypeReaderResult.FromSuccess(results.Values.ToReadOnlyCollection());
        }

        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Channel not found.");
    }

    private void AddResult(Dictionary<ulong, TypeReaderValue> results, T? channel, float score)
    {
        if (channel != null && !results.ContainsKey(channel.Id))
            results.Add(channel.Id, new TypeReaderValue(channel, score));
    }
}

using System.Collections.Immutable;
using System.Globalization;

namespace Kook.Commands;

/// <summary>
///     表示一个用于解析实现了 <see cref="T:Kook.IUser"/> 的对象的类型读取器。
/// </summary>
/// <typeparam name="T"> 要解析为的用户类型。 </typeparam>
public class UserTypeReader<T> : TypeReader
    where T : class, IUser
{
    /// <inheritdoc />
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        Dictionary<ulong, TypeReaderValue> results = new();
        List<IUser> channelUsers = await context.Channel
            .GetUsersAsync(CacheMode.CacheOnly)
            .Flatten()
            .ToListAsync()
            .ConfigureAwait(false); // it's better
        TagMode tagMode = context.Message.Type switch
        {
            MessageType.Text => TagMode.PlainText,
            MessageType.KMarkdown => TagMode.KMarkdown,
            _ => throw new ArgumentOutOfRangeException(nameof(context.Message.Type))
        };

        IReadOnlyCollection<IGuildUser> guildUsers = context.Guild != null
            ? await context.Guild.GetUsersAsync(CacheMode.CacheOnly).ConfigureAwait(false)
            : [];

        //By Mention (1.0)
        if (MentionUtils.TryParseUser(input, out ulong id, tagMode))
        {
            T? user = context.Guild != null
                ? await context.Guild.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T
                : await context.Channel.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
            AddResult(results, user, 1.00f);
        }

        //By Id (0.9)
        if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
        {
            T? user = context.Guild != null
                ? await context.Guild.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T
                : await context.Channel.GetUserAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T;
            AddResult(results, user, 0.90f);
        }

        //By Username + IdentifyNumber (0.7-0.85)
        int index = input.LastIndexOf('#');
        if (index >= 0)
        {
            string username = input[..index];
            if (ushort.TryParse(input[(index + 1)..], out ushort identifyNumber))
            {
                IUser? channelUser = channelUsers.Find(x =>
                        x.IdentifyNumberValue == identifyNumber
                        && string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase));
                AddResult(results, channelUser as T, channelUser?.Username == username ? 0.85f : 0.75f);

                IGuildUser? guildUser = guildUsers.FirstOrDefault(x =>
                    x.IdentifyNumberValue == identifyNumber
                    && string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase));
                AddResult(results, guildUser as T, guildUser?.Username == username ? 0.80f : 0.70f);
            }
        }

        //By Username (0.5-0.6)
        {
            IEnumerable<IUser> channelUserMatches = channelUsers
                .Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase));
            foreach (IUser x in channelUserMatches)
                AddResult(results, x as T, x.Username == input ? 0.65f : 0.55f);
            IEnumerable<IGuildUser> guildUserMatches = guildUsers
                .Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase));
            foreach (IGuildUser x in guildUserMatches)
                AddResult(results, x as T, x.Username == input ? 0.60f : 0.50f);
        }

        //By Nickname (0.5-0.6)
        {
            IEnumerable<IUser> channelUserMatches = channelUsers
                .Where(x => string.Equals(input, (x as IGuildUser)?.Nickname, StringComparison.OrdinalIgnoreCase));
            foreach (IUser x in channelUserMatches)
                AddResult(results, x as T, (x as IGuildUser)?.Nickname == input ? 0.65f : 0.55f);
            IEnumerable<IGuildUser> guildUserMatches = guildUsers
                .Where(x => string.Equals(input, x.Nickname, StringComparison.OrdinalIgnoreCase));
            foreach (IGuildUser x in guildUserMatches)
                AddResult(results, x as T, x.Nickname == input ? 0.60f : 0.50f);
        }

        if (results.Count > 0)
            return TypeReaderResult.FromSuccess(results.Values.ToImmutableArray());
        return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");
    }

    private void AddResult(Dictionary<ulong, TypeReaderValue> results, T? user, float score)
    {
        if (user != null && !results.ContainsKey(user.Id))
            results.Add(user.Id, new TypeReaderValue(user, score));
    }
}

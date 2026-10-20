using Kook.Commands;
using Kook.WebSocket;

namespace Kook.Net.Samples.WebRTC.Modules;

/// <summary>
///     帮助命令。命令列表与用法来自命令框架里已经注册的命令，新增命令只要写了 <see cref="SummaryAttribute"/> 就会出现在这里。
/// </summary>
public class HelpModule : ModuleBase<SocketCommandContext>
{
    private readonly CommandService _commands;
    private readonly WebRtcSampleOptions _options;

    public HelpModule(CommandService commands, WebRtcSampleOptions options)
    {
        _commands = commands;
        _options = options;
    }

    [Command("help")]
    [Summary("Show all commands, or the usage of one command, for example: help record")]
    public async Task HelpAsync(string? command = null)
    {
        if (Context.Message.Source is not MessageSource.User) return;

        if (string.IsNullOrWhiteSpace(command))
        {
            List<string> lines = ["Commands (use them in the text chat of the voice channel):"];
            // help 自己放在最后
            lines.AddRange(_commands.Commands.OrderBy(x => x.Name == "help").Select(x => $"{FormatUsage(x)} - {x.Summary}"));
            lines.Add($"Use {_options.CommandPrefix}help <command> for the usage of one command.");
            await ReplyTextAsync(string.Join(Environment.NewLine, lines));
            return;
        }

        string name = command.Trim().TrimStart(_options.CommandPrefix);
        CommandInfo? found = _commands.Commands.FirstOrDefault(x =>
            x.Aliases.Any(alias => string.Equals(alias, name, StringComparison.OrdinalIgnoreCase)));
        if (found is null)
        {
            await ReplyTextAsync($"Unknown command: {name}. Use {_options.CommandPrefix}help to see all commands.");
            return;
        }

        List<string> detail = [FormatUsage(found), found.Summary ?? string.Empty];
        if (found.Aliases.Count > 1)
            detail.Add($"Aliases: {string.Join(", ", found.Aliases.Skip(1).Select(x => _options.CommandPrefix + x))}");
        await ReplyTextAsync(string.Join(Environment.NewLine, detail));
    }

    /// <summary> 例如 <c>!record [user]</c>：尖括号是必填的参数，方括号是可选的参数。 </summary>
    private string FormatUsage(CommandInfo command)
    {
        IEnumerable<string> parameters = command.Parameters.Select(x => x.IsOptional ? $"[{x.Name}]" : $"<{x.Name}>");
        return string.Join(' ', new[] { _options.CommandPrefix + command.Name }.Concat(parameters));
    }
}

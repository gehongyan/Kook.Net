using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Kook.Commands;

internal class CommandMapNode
{
    private static readonly char[] WhitespaceChars = [' ', '\r', '\n'];

    private readonly ConcurrentDictionary<string, CommandMapNode> _nodes;
    private readonly string _name;
#if NET9_0_OR_GREATER
    private readonly Lock _lockObj = new();
#else
    private readonly object _lockObj = new();
#endif
    private ImmutableArray<CommandInfo> _commands;

    // ReSharper disable InconsistentlySynchronizedField
    public bool IsEmpty => _commands.Length == 0 && _nodes.Count == 0;
    // ReSharper restore InconsistentlySynchronizedField

    public CommandMapNode(string name)
    {
        _name = name;
        _nodes = [];
        _commands = [];
    }

    /// <exception cref="InvalidOperationException">Cannot add commands to the root node.</exception>
    public void AddCommand(CommandService service, string text, int index, CommandInfo command)
    {
        int nextSegment = NextSegment(text, index, service._separatorChar);

        lock (_lockObj)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (string.IsNullOrEmpty(_name))
                    throw new InvalidOperationException("Cannot add commands to the root node.");
                _commands = _commands.Add(command);
            }
            else
            {
                string name = nextSegment == -1
                    ? text[index..]
                    : text.Substring(index, nextSegment - index);
                string fullName = _name == string.Empty
                    ? name
                    : _name + service._separatorChar + name;
                CommandMapNode nextNode = _nodes.GetOrAdd(name, x => new CommandMapNode(fullName));
                nextNode.AddCommand(service, nextSegment == -1 ? string.Empty : text, nextSegment + 1, command);
            }
        }
    }

    public void RemoveCommand(CommandService service, string text, int index, CommandInfo command)
    {
        int nextSegment = NextSegment(text, index, service._separatorChar);

        lock (_lockObj)
        {
            if (string.IsNullOrEmpty(text))
                _commands = _commands.Remove(command);
            else
            {
                string name = nextSegment == -1
                    ? text[index..]
                    : text.Substring(index, nextSegment - index);
                if (_nodes.TryGetValue(name, out CommandMapNode? nextNode))
                {
                    nextNode.RemoveCommand(service, nextSegment == -1 ? "" : text, nextSegment + 1, command);
                    if (nextNode.IsEmpty) _nodes.TryRemove(name, out nextNode);
                }
            }
        }
    }

    public IEnumerable<CommandMatch> GetCommands(CommandService service, string text, int index, bool visitChildren = true)
    {
        ImmutableArray<CommandInfo> commands = _commands;
        for (int i = 0; i < commands.Length; i++) yield return new CommandMatch(_commands[i], _name);

        if (visitChildren)
        {
            //Search for next segment
            int nextSegment = NextSegment(text, index, service._separatorChar);
            string name = nextSegment == -1
                ? text[index..]
                : text.Substring(index, nextSegment - index);

            if (_nodes.TryGetValue(name, out CommandMapNode? nextNode))
            {
                IEnumerable<CommandMatch> matches = nextNode.GetCommands(service, nextSegment == -1 ? string.Empty : text, nextSegment + 1, true);
                foreach (CommandMatch cmd in matches)
                    yield return cmd;
            }

            //Check if this is the last command segment before args
            nextSegment = NextSegment(text, index, WhitespaceChars, service._separatorChar);
            if (nextSegment != -1)
            {
                name = text.Substring(index, nextSegment - index);
                if (_nodes.TryGetValue(name, out nextNode))
                {
                    foreach (CommandMatch cmd in nextNode.GetCommands(service, nextSegment == -1 ? string.Empty : text, nextSegment + 1, false))
                        yield return cmd;
                }
            }
        }
    }

    private static int NextSegment(string text, int startIndex, char separator) => text.IndexOf(separator, startIndex);

    private static int NextSegment(string text, int startIndex, char[] separators, char except)
    {
        int lowest = int.MaxValue;
        foreach (char x in separators.Where(x => x != except))
        {
            int index = text.IndexOf(x, startIndex);
            if (index != -1 && index < lowest)
                lowest = index;
        }

        return lowest != int.MaxValue ? lowest : -1;
    }
}

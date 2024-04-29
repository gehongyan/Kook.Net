namespace Kook.Commands;

/// <summary>
///     Sets priority of commands.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PriorityAttribute : Attribute
{
    /// <summary>
    ///     Gets the priority which has been set for the command.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    ///     Initializes a new <see cref="PriorityAttribute" /> attribute with the given priority.
    /// </summary>
    public PriorityAttribute(int priority)
    {
        Priority = priority;
    }
}

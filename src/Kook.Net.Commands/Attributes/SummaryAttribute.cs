namespace Kook.Commands;

// Cosmetic Summary, for Groups and Commands
/// <summary>
///     Attaches a summary to your command.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class SummaryAttribute : Attribute
{
    /// <summary>
    ///     Gets the summary of the command.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Initializes a new <see cref="SummaryAttribute" /> attribute with the specified summary.
    /// </summary>
    /// <param name="text"></param>
    public SummaryAttribute(string text) => Text = text;
}

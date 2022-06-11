public class MyCustomResult : RuntimeResult
{
    public string Hint { get; set; }
    public MyCustomResult(CommandError? error, string reason, string hint)
        : base(error, reason)
    {
        Hint = hint;
    }
    public static MyCustomResult FromError(string reason, string hint = null) =>
        new MyCustomResult(CommandError.Unsuccessful, reason, hint);
    public static MyCustomResult FromSuccess(string reason = null) =>
        new MyCustomResult(null, reason);
}
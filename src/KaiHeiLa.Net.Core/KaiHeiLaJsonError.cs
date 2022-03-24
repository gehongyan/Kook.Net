using System.Collections.Immutable;

namespace KaiHeiLa;

/// <summary>
///     Represents a generic parsed json error received from KaiHeiLa after performing a rest request.
/// </summary>
public struct KaiHeiLaJsonError
{
    /// <summary>
    ///     Gets the json path of the error.
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///     Gets a collection of errors associated with the specific property at the path.
    /// </summary>
    public IReadOnlyCollection<KaiHeiLaError> Errors { get; }

    internal KaiHeiLaJsonError(string path, KaiHeiLaError[] errors)
    {
        Path = path;
        Errors = errors.ToImmutableArray();
    }
}

/// <summary>
///     Represents an error with a property.
/// </summary>
public struct KaiHeiLaError
{
    /// <summary>
    ///     Gets the code of the error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    ///     Gets the message describing what went wrong.
    /// </summary>
    public string Message { get; }

    internal KaiHeiLaError(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
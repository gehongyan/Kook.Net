namespace Kook;

/// <summary>
///     Specifies the handling type the tag should use.
/// </summary>
/// <seealso cref="MentionUtils"/>
/// <seealso cref="IUserMessage.Resolve"/>
public enum TagHandling
{
    /// <summary>
    ///     Tag handling is ignored. (e.g. (met)2810246202(met) -&gt; (met)2810246202(met))
    /// </summary>
    Ignore = 0,

    /// <summary>
    ///     Removes the tag entirely.
    /// </summary>
    Remove,

    /// <summary>
    ///     Resolves to username (e.g. (met)2810246202(met) -&gt; @Someone).
    /// </summary>
    Name,

    /// <summary>
    ///     Resolves to username without mention prefix (e.g. (met)2810246202(met) -&gt; Someone).
    /// </summary>
    NameNoPrefix,

    /// <summary>
    ///     Resolves to username with identify number value (e.g. (met)2810246202(met) -&gt; @Someone#1234).
    /// </summary>
    FullName,

    /// <summary>
    ///     Resolves to username with identify number value without mention prefix (e.g. (met)2810246202(met) -&gt; Someone#1234).
    /// </summary>
    FullNameNoPrefix,

    /// <summary>
    ///     Sanitizes the tag. (e.g. (met)2810246202(met) -&gt; (met)2810246202(met) (an nbsp is inserted before the key)).
    /// </summary>
    Sanitize
}

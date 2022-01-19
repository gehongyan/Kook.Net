namespace KaiHeiLa;

public interface IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     唯一标识
    /// </summary>
    TId Id { get; }
}
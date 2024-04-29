using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.Game;

namespace Kook.Rest;

/// <summary>
///     Represents a game object.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestGame : RestEntity<int>, IGame
{
    private ImmutableArray<string> _productNames = [];
    private ImmutableArray<string> _processNames = [];

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public GameType GameType { get; private set; }

    /// <inheritdoc />
    public string? Options { get; private set; }

    /// <inheritdoc />
    public bool RequireAdminPrivilege { get; private set; }

    /// <inheritdoc />
    public string? Icon { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> ProductNames => _productNames.ToReadOnlyCollection();

    /// <inheritdoc />
    public IReadOnlyCollection<string> ProcessNames => _processNames.ToReadOnlyCollection();

    internal RestGame(BaseKookClient kook, int id)
        : base(kook, id)
    {
        Name = string.Empty;
    }

    internal static RestGame Create(BaseKookClient kook, Model model)
    {
        RestGame entity = new(kook, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Name = model.Name;
        GameType = model.Type;
        Options = model.Options;
        RequireAdminPrivilege = model.KmHookAdmin;
        Icon = model.Icon;
        _productNames = [..model.ProductNames];
        _processNames = [..model.ProcessNames];
    }

    /// <inheritdoc cref="IGame.ModifyAsync(Action{GameProperties},RequestOptions)" />
    public Task<RestGame> ModifyAsync(Action<GameProperties> func, RequestOptions? options = null) =>
        GameHelper.ModifyAsync(this, Kook, func, options);

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) => GameHelper.DeleteAsync(this, Kook, options);

    /// <inheritdoc />
    async Task<IGame> IGame.ModifyAsync(Action<GameProperties> func, RequestOptions? options) =>
        await ModifyAsync(func, options);

    private string DebuggerDisplay => $"{Name} ({Id}, {GameType.ToString()})";
}

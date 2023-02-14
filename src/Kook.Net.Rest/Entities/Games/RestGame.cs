using System.Collections.Immutable;
using Model = Kook.API.Game;

namespace Kook.Rest;

/// <summary>
///     Represents a game object.
/// </summary>
public class RestGame : RestEntity<int>, IGame
{
    private ImmutableArray<string> _productNames;
    private ImmutableArray<string> _processNames;

    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public GameType GameType { get; private set; }
    /// <inheritdoc />
    public string Options { get; private set; }
    /// <inheritdoc />
    public bool RequireAdminPrivilege { get; private set; }
    /// <inheritdoc />
    public string Icon { get; private set; }
    /// <inheritdoc />
    public IReadOnlyCollection<string> ProductNames => _productNames.ToReadOnlyCollection();
    /// <inheritdoc />
    public IReadOnlyCollection<string> ProcessNames => _processNames.ToReadOnlyCollection();

    internal RestGame(BaseKookClient kook, int id)
        : base(kook, id)
    {
    }

    internal static RestGame Create(BaseKookClient kook, Model model)
    {
        var entity = new RestGame(kook, model.Id);
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
        _productNames = model.ProductNames.ToImmutableArray();
        _processNames = model.ProcessNames.ToImmutableArray();
    }

    /// <inheritdoc cref="IGame.ModifyAsync(Action{GameProperties},RequestOptions)" />
    public async Task<RestGame> ModifyAsync(Action<GameProperties> func, RequestOptions options = null)
    {
        return await GameHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task DeleteAsync(RequestOptions options = null)
    {
        await GameHelper.DeleteAsync(this, Kook, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    async Task<IGame> IGame.ModifyAsync(Action<GameProperties> func, RequestOptions options)
        => await ModifyAsync(func, options);
}

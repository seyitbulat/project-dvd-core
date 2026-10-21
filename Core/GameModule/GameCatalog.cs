

using System.Diagnostics.CodeAnalysis;

namespace Core.GameModule;



public class GameCatalog
{
    private Dictionary<int, Game> _games;
    public IReadOnlyDictionary<int, Game> Games => _games;

    public GameCatalog()
    {
        _games = new();
    }


    public bool TryGet(int id, [NotNullWhen(true)] out Game? game)
    {
      return _games.TryGetValue(id, out game);
    }

    public bool Add(Game game)
    {
        return _games.TryAdd(game.Id, game);
    }


}
using Core.ComputerModule.Models;


namespace Core.GameModule;



public class GameCatalog
{
    private Dictionary<int, Game> _games;
    public IReadOnlyDictionary<int, Game> Games => _games;

    public GameCatalog()
    {
        _games = new();
    }

    
}
namespace Core.ComputerModule.Models;

using Core.GameModule;
using Sizes = (double TotalSize, double GameSize, double EmptySize);

public class HardDisk
{
    private double Size { get; set; } // kbyte
    private List<Game> _games; // downloaded games 

    private List<Game> _reservedGames;

    public IReadOnlyList<Game> InstalledGames => _games;
    public IReadOnlyList<Game> ReservedGames => _reservedGames;


    public HardDisk(double size)
    {
        Size = size;
        _games = new();
        _reservedGames = new();
    }


    public Sizes GetSizes()
    {
        double totalSize = Size;
        double emptySize = totalSize;
        double gameSize = 0;

        double reservedSize = GetReservedSize();


        foreach (var game in _games)
        {
            gameSize += game.Size;
        }

        emptySize = totalSize - gameSize - reservedSize;

        return (totalSize, gameSize, emptySize);
    }

    private bool IsCapacityEnough(Game game)
    {
        var sizes = GetSizes();

        return game.Size <= sizes.EmptySize;

    }

    private double GetReservedSize()
    {
        double size = 0;

        foreach (var game in _reservedGames)
        {
            size += game.Size;
        }
        return size;
    }

    private bool IsGameExists(Game game)
    {
        return _games.Contains(game);
    }

    private bool IsGameReserved(Game game)
    {
        return _reservedGames.Contains(game);
    }

    public bool AddGame(Game game)
    {
        UnreserveGame(game);
        if (!IsCapacityEnough(game))
        {
            return false;
        }
        if (IsGameExists(game))
        {
            return false;
        }
        _games.Add(game);

        return true;
    }


    public bool ReserveGame(Game game)
    {
        if (IsGameReserved(game))
        {
            return false;
        }

        if (IsGameExists(game))
        {
            return false;
        }

        if (!IsCapacityEnough(game))
        {
            return false;
        }

        _reservedGames.Add(game);

        return true;
    }

    public bool UnreserveGame(Game game)
    {
        return _reservedGames.Remove(game);
    }


}
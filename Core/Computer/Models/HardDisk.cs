namespace Core.Computer.Models;


using Sizes = (double TotalSize, double GameSize, double EmptySize);

public class HardDisk
{
    private double Size { get; set; } // kbyte
    private List<Game> _games; // downloaded games 

    private List<Game> _reservedGames;

    public IReadOnlyList<Game> InstalledGames {get => _games;}
    public IReadOnlyList<Game> ReservedGames {get => _reservedGames;}


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

        gameSize = _games.Select(x => x.Size).Sum();

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
        return ReservedGames.Select(x => x.Size).Sum();
    }

    private bool IsGameExists(Game game)
    {
        return _games.Any(x => x.Name == game.Name);
    }

    private bool IsGameReserved(Game game)
    {
        return ReservedGames.Any(x => x.Name == game.Name);
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
        if (IsGameReserved(game))
        {
            _reservedGames.RemoveAll(g => g.Name == game.Name);
            return true;
        }
        return false;
    }





}
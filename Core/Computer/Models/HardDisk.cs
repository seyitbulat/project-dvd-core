namespace Core.Computer.Models;


using Sizes = (double TotalSize, double GameSize, double EmptySize);

public class HardDisk
{
    private double Size { get; set; } // kbyte
    private List<Game> Games = new(); // downloaded games 

    private List<Game> ReservedGames = new();
    private double AllocatedSize = 0;

    public HardDisk(double size)
    {
        Size = size;
    }


    public Sizes GetSizes()
    {
        double totalSize = Size;
        double emptySize = totalSize;
        double gameSize = 0;

        double reservedSize = GetReservedSize();

        if (Games.Count > 0)
        {
            gameSize = Games.Select(x => x.Size).Sum();
        }

        emptySize = totalSize - gameSize - reservedSize;

        return (totalSize, gameSize, emptySize);
    }

    private bool IsCapacityEnough(Game game)
    {
        (double totalSize, double gameSize, double emptySize) = GetSizes();

        if (game.Size > emptySize)
        {
            return false;
        }

        return true;
    }

    private double GetReservedSize()
    {
        return ReservedGames.Count > 0 ? ReservedGames.Select(x => x.Size).Sum() : 0;
    }

    private bool IsGameExists(Game game)
    {
        return Games.Any(x => x.Name == game.Name);
    }

    private bool IsGameReserved(Game game)
    {
        return ReservedGames.Any(x => x.Name == game.Name);
    }

    public bool AddGame(Game game)
    {
        UnReserveGame(game);
        if (!IsCapacityEnough(game))
        {
            return false;
        }
        if (IsGameExists(game))
        {
            return false;
        }
        Games.Add(game);

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

        ReservedGames.Add(game);

        return true;
    }

    public bool UnReserveGame(Game game)
    {
        if (IsGameReserved(game))
        {
            ReservedGames.Remove(game);
            return true;
        }
        return false;
    }

    public bool AllocateSize(Game game)
    {
        if (!IsCapacityEnough(game))
        {
            return false;
        }

        if (IsGameExists(game))
        {
            return false;
        }

        AllocatedSize += game.Size;

        return true;
    }

    private bool FreeSize(Game game)
    {
        AllocatedSize -= game.Size;

        return true;
    }

}
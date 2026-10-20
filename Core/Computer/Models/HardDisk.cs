namespace Core.Computer.Models;


using Sizes = (double TotalSize, double GameSize, double EmptySize);

public class HardDisk
{
    private double Size { get; set; } // kbyte
    private List<Game> Games = new(); // downloaded games 

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
        if (Games.Count > 0)
        {
            gameSize = Games.Select(x => x.Size).Sum();
        }
        emptySize = totalSize - gameSize - AllocatedSize;

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

    private bool IsGameExists(Game game)
    {
        return Games.Any(x => x.Name == game.Name);
    }

    public bool AddGame(Game game)
    {
        FreeSize(game);

        if (!IsCapacityEnough(game))
        {
            return false;
        }
        
        Games.Add(game);

        return true;
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
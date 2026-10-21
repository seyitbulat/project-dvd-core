

using Core.GameModule;

namespace Core.DiscModule;

using GameDisc = (int Piece, double LastDiscSize);


public class DiscType
{
    private string _name;
    public string Name => _name;

    private long _price; // kurus or cent
    public long Price => _price;

    private double _capacity;
    public double Capacity => _capacity;


    public static readonly DiscType CD = new("CD", 100, 700 * 1024);
    public static readonly DiscType DVD = new("DVD", 300, 5 * 1024 * 1024);

    private DiscType(string name, long price, double capacity)
    {
        _name = name;
        _price = price;
        _capacity = capacity;

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
    }


    public GameDisc GetCountAndSize(Game game)
    {
        int count = (int)Math.Ceiling(game.Size / _capacity);

        var remainSize = game.Size - (_capacity * (count - 1));

        return (count, remainSize);
    }

   
}
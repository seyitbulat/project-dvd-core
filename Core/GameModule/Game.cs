namespace Core.GameModule;



public class Game
{
    private int _id;
    public int Id => _id;

    private string _name;
    public string Name { get => _name; }


    private double _size;
    public double Size { get => _size; } // kbyte

    public Game(int id, string name, double size)
    {
        _id = id;
        _name = name;
        _size = size;

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
    }
}
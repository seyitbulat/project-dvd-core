namespace Core.ComputerModule.Models;



public class Game
{
    private string _name;
    public string Name { get => _name; }


    private double _size;
    public double Size { get => _size; } // kbyte


    public Game(string name, double size)
    {
        _name = name;
        _size = size;
    }
}
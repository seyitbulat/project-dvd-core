namespace Core.Computer.Models;



public class Game
{
    public string Name { get; set; }
    public double Size { get; set; } // kbyte


    public Game(string name, double size)
    {
        Name = name;
        Size = size;
    }
}
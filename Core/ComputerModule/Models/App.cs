
namespace Core.ComputerModule.Models;


public enum AppStatus
{
    Closed,
    Open,
}

public abstract class App
{
    private string _name;
    public string Name { get => _name; }

    private AppStatus _status;
    public AppStatus Status
    {
        get => _status;
    }

    public App(string name)
    {
        _name = name;
        _status = AppStatus.Closed;
    }

    public bool OpenApp()
    {
        if (Status == AppStatus.Open)
        {
            return false;
        }

        _status = AppStatus.Open;

        return true;
    }

    public bool CloseApp()
    {
        if (_status == AppStatus.Closed)
        {
            return false;
        }

        _status = AppStatus.Closed;
        return true;
    }

   
}
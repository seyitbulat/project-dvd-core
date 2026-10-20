
namespace Core.Computer.Models;


public enum AppStatus
{
    CLOESD,
    OPEN,
}

public abstract class App
{
    public string Name { get; set; }
    private AppStatus Status { get; set; }



    public bool OpenApp()
    {
        if (Status == AppStatus.OPEN)
        {
            return false;
        }

        Status = AppStatus.OPEN;

        return true;
    }

    public bool CloseApp()
    {
        if (Status == AppStatus.CLOESD)
        {
            return false;
        }

        Status = AppStatus.CLOESD;
        return true;
    }

    public AppStatus GetStatus()
    {
        return Status;
    }
}

using Core.Computer.Apps;
using Core.Computer.Models;

namespace Core.Computer;



public class Computer
{
    // To Upgradables
    private HardDisk _hardDisk;
    public HardDisk HardDisk { get => _hardDisk; }

    private double _internetSpeed;
    public double InternetSpeed { get => _internetSpeed;} // kbyte/s

    // Apps
    private DownloadManager _downloadManager;
    public DownloadManager DownloadManager { get => _downloadManager; }


    public Computer(double diskSize = 30 * 1024 * 1024, double internetSpeed = 100)
    {
        _hardDisk = new(diskSize);
        _downloadManager = new(HardDisk);
        _internetSpeed = internetSpeed;
    }

    public bool OpenApp(App app)
    {
        return app.OpenApp();
    }

    public bool CloseApp(App app)
    {
        return app.CloseApp();
    }


    public void Update(double delta)
    {
        _downloadManager.Update(delta, InternetSpeed);
    }


}
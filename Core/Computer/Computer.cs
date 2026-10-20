
using Core.Computer.Apps;
using Core.Computer.Models;

namespace Core.Computer;



public class Computer
{
    // To Upgradables
    public HardDisk HardDisk { get; set; }
    public double InternetSpeed { get; set; } // kbyte/s

    // Apps
    public DownloadManager DownloadManager { get; set; }


    public Computer(double size = 30 * 1024 * 1024)
    {
        HardDisk = new(size);
        DownloadManager = new(HardDisk);

        DownloadManager.DownloadCompleted += OnDownloadeCompleted;
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
        DownloadManager.Update(delta, InternetSpeed);
    }

    private void OnDownloadeCompleted(DownloadJob downloadJob)
    {
        HardDisk.AddGame(downloadJob.Game);
    }
}

using Core.Computer.Models;

namespace Core.Computer.Apps;


public class DownloadManager : App
{
    public List<DownloadJob> DownloadList = new();
    public event Action<DownloadJob>? DownloadCompleted;


    private HardDisk HardDisk; // reference

    public DownloadManager(HardDisk hardDisk)
    {
        HardDisk = hardDisk;
    }
    public bool AddGameToDownloadQueue(Game game)
    {

        if (!DownloadList.Any(x => x.Game.Name == game.Name))
        {

            if (!HardDisk.AllocateSize(game))
            {
                return false;
            }
            DownloadJob downloadJob = new(game);
            DownloadList.Add(downloadJob);

            downloadJob.DownloadCompleted += OnDownloaded;
            return true;
        }

        return false;
    }


    public void Update(double delta, double internetSpeed)
    {
        List<DownloadJob> downloading = DownloadList.Where(x => x.DownloadJobStatus == DownloadJobStatusEnum.Downloading).ToList();

        if (downloading.Count > 0)
        {
            double tempSpeed = internetSpeed / downloading.Count;

            downloading.ForEach(c =>
            {
                c.Update(delta, tempSpeed);
            });
        }


    }

    private void OnDownloaded(DownloadJob downloadJob)
    {
        DownloadCompleted?.Invoke(downloadJob);
    }

}

using Core.Computer.Models;

namespace Core.Computer.Apps;


public class DownloadManager : App
{

    private List<DownloadJob> _downloadList;

    public IReadOnlyList<DownloadJob> DownloadList => _downloadList;
   

    public event Action<DownloadJob>? DownloadCompleted;


    private HardDisk HardDisk; // reference

    public DownloadManager(HardDisk hardDisk) : base("Download Manager")
    {
        HardDisk = hardDisk;
        _downloadList = new();
    }
    public bool AddGameToDownload(Game game)
    {

        if (!_downloadList.Any(x => x.Game.Name == game.Name))
        {

            if (!HardDisk.ReserveGame(game))
            {
                return false;
            }
            DownloadJob downloadJob = new(game);
            _downloadList.Add(downloadJob);

            downloadJob.StartDownload();

            downloadJob.DownloadCompleted += OnDownloaded;
            return true;
        }

        return false;
    }


    public void Update(double delta, double internetSpeed)
    {
        int downloadingCount = 0;
        foreach (var job in _downloadList)
        {
            if (job.Status == DownloadJobStatus.Downloading)
            {
                downloadingCount++;
            }
        }

        if (downloadingCount == 0)
            return;

        double tempSpeed = internetSpeed / downloadingCount;

        foreach (var job in _downloadList)
        {
            if (job.Status == DownloadJobStatus.Downloading)
            {
                job.Update(delta, tempSpeed);
            }
        }

    }

    private void OnDownloaded(DownloadJob downloadJob)
    {
        HardDisk.AddGame(downloadJob.Game);

        DownloadCompleted?.Invoke(downloadJob);

    }


    public bool PauseDownload(Game game)
    {
        var downloadJob = _downloadList.FirstOrDefault(x => x.Game.Name == game.Name);
        if (downloadJob != null)
        {
            return downloadJob.PauseDownload();
        }

        return false;
    }

    public bool ResumeDownload(Game game)
    {
        var downloadJob = _downloadList.FirstOrDefault(x => x.Game.Name == game.Name);
        if (downloadJob != null)
        {
            return downloadJob.StartDownload();
        }

        return false;
    }

}
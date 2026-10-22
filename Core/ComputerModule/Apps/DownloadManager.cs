
using System.Diagnostics.CodeAnalysis;
using Core.ComputerModule.Models;
using Core.GameModule;

namespace Core.ComputerModule.Apps;


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

        if (TryGetDownloadJobByGame(game, out _))
        {
            return false;
        }

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


        _downloadList.RemoveAll(di => di.Status == DownloadJobStatus.Completed || di.Status == DownloadJobStatus.Queued);

    }

    private void OnDownloaded(DownloadJob downloadJob)
    {
        HardDisk.AddGame(downloadJob.Game);

        DownloadCompleted?.Invoke(downloadJob);

    }


    public bool PauseDownload(Game game)
    {

        if (TryGetDownloadJobByGame(game, out var job))
        {
            return job.PauseDownload();
        }

        return false;
    }

    public bool ResumeDownload(Game game)
    {
        if (TryGetDownloadJobByGame(game, out var job))
        {
            return job.StartDownload();
        }

        return false;
    }


    public bool CancelDownload(Game game)
    {
        if (TryGetDownloadJobByGame(game, out var job))
        {
            return job.CancelDownload();
        }

        return false;
    }

    private bool TryGetDownloadJobByGame(Game game, [NotNullWhen(true)] out DownloadJob? outJob)
    {
        foreach (var job in _downloadList)
        {
            if (ReferenceEquals(game, job.Game))
            {
                outJob = job;

                return true;
            }
        }

        outJob = null;
        return false;
    }
}
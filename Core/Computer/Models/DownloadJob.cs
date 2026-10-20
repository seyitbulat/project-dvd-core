namespace Core.Computer.Models;

public enum DownloadJobStatusEnum
{
    Queued,
    Downloading,
    Paused,
    Completed
}

public class DownloadJob
{
    public Game Game { get; private set; }
    public DownloadJobStatusEnum DownloadJobStatus { get; private set; }
    public double CurrentSize { get; private set; } // kbyte

    //events
    public event Action<DownloadJob>? DownloadCompleted;
    public event Action<DownloadJob>? DownloadProgressChanged;


    public DownloadJob(Game game)
    {
        Game = game;
        CurrentSize = 0;
        DownloadJobStatus = DownloadJobStatusEnum.Queued;
    }

    internal bool StartDownload()
    {

        switch (DownloadJobStatus)
        {
            case (DownloadJobStatusEnum.Completed):
                return false;
            case DownloadJobStatusEnum.Downloading:
                return false;
            default:
                DownloadJobStatus = DownloadJobStatusEnum.Downloading;
                return true;
        }
    }

    internal bool PauseDownload()
    {
        switch (DownloadJobStatus)
        {
            case DownloadJobStatusEnum.Completed:
                return false;

            case DownloadJobStatusEnum.Queued:
                return false;

            case DownloadJobStatusEnum.Paused:
                return false;

            default:
                DownloadJobStatus = DownloadJobStatusEnum.Paused;
                return true;
        }
    }

    internal void Update(double delta, double speed)
    {
        if (DownloadJobStatus == DownloadJobStatusEnum.Downloading)
        {
            CurrentSize += (delta * speed);


            if (CurrentSize >= Game.Size)
            {
                DownloadJobStatus = DownloadJobStatusEnum.Completed;
                CurrentSize = Game.Size;
                DownloadProgressChanged?.Invoke(this);

                DownloadCompleted?.Invoke(this);
            }
            else
            {
                DownloadProgressChanged?.Invoke(this);

            }

        }
    }


}
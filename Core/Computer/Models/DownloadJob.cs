namespace Core.ComputerModule.Models;

public enum DownloadJobStatus
{
    Queued,
    Downloading,
    Paused,
    Completed
}

public class DownloadJob
{
    private Game _game;
    public Game Game { get => _game; }

    private DownloadJobStatus _status;
    public DownloadJobStatus Status { get => _status; }

    private double _currentSize;
    public double CurrentSize { get => _currentSize;  } // kbyte

    //events
    public event Action<DownloadJob>? DownloadCompleted;
    public event Action<DownloadJob>? DownloadProgressChanged;


    internal DownloadJob(Game game)
    {
        _game = game;
        _currentSize = 0;
        _status = DownloadJobStatus.Queued;
    }

    internal bool StartDownload()
    {

        switch (Status)
        {
            case (DownloadJobStatus.Completed):
                return false;
            case DownloadJobStatus.Downloading:
                return false;
            default:
                _status = DownloadJobStatus.Downloading;
                return true;
        }
    }

    internal bool PauseDownload()
    {
        switch (Status)
        {
            case DownloadJobStatus.Completed:
                return false;

            case DownloadJobStatus.Queued:
                return false;

            case DownloadJobStatus.Paused:
                return false;

            default:
                _status = DownloadJobStatus.Paused;
                return true;
        }
    }

    internal void Update(double delta, double speed)
    {
        if (Status == DownloadJobStatus.Downloading)
        {
            _currentSize += (delta * speed);


            if (CurrentSize >= Game.Size)
            {
                _status = DownloadJobStatus.Completed;
                _currentSize = Game.Size;
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
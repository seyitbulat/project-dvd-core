
namespace Core.Tests.ComputerModule;

using Core.ComputerModule;
using Core.ComputerModule.Models;
using Xunit.Abstractions;

public class ComputerTests
{
    private readonly ITestOutputHelper output;

    public ComputerTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    [Fact]
    public void Update_WithActiveDownloads_DoesNotAllocate()
    {
        double delta = 0.016;
        Computer cp = new(256 * 1024 * 1024, 100);

        var game1 = new Game("Cyberpunk", 39 * 1024 * 1024);
        var game2 = new Game("FF7", 68 * 1024 * 1024);

        cp.DownloadManager.AddGameToDownload(game1);
        cp.DownloadManager.AddGameToDownload(game2);

        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);

        var bf = GC.GetAllocatedBytesForCurrentThread();

        for (int i = 0; i < 1000; i++)
        {
            cp.Update(delta);
        }

        var af = GC.GetAllocatedBytesForCurrentThread();

        var result = af - bf;
        output.WriteLine($"Result: {result}");

        Assert.Equal(0, result);
    }

    [Fact]
    public void DownloadList_WithActiveDownloads_DoesNotAllocate()
    {
        Computer cp = new(256 * 1024 * 1024, 100);

        var game1 = new Game("Cyberpunk", 39 * 1024 * 1024);
        var game2 = new Game("FF7", 68 * 1024 * 1024);

        cp.DownloadManager.AddGameToDownload(game1);
        cp.DownloadManager.AddGameToDownload(game2);

        var test = cp.DownloadManager.DownloadList;
        test = cp.DownloadManager.DownloadList;
        test = cp.DownloadManager.DownloadList;
        test = cp.DownloadManager.DownloadList;
        test = cp.DownloadManager.DownloadList;
        test = cp.DownloadManager.DownloadList;
        test = cp.DownloadManager.DownloadList;

        var bf = GC.GetAllocatedBytesForCurrentThread();

        for (int i = 0; i < 1000; i++)
        {
            test = cp.DownloadManager.DownloadList;
        }

        var af = GC.GetAllocatedBytesForCurrentThread();

        var result = af - bf;
        output.WriteLine($"Result: {result}");

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetSizes_WithActiveDownloads_DoesNotAllocate()
    {
        Computer cp = new(256 * 1024 * 1024, 100);

        var game1 = new Game("Cyberpunk", 39 * 1024 * 1024);
        var game2 = new Game("FF7", 68 * 1024 * 1024);
        
        cp.DownloadManager.AddGameToDownload(game1);
        cp.DownloadManager.AddGameToDownload(game2);
        var test = cp.HardDisk.GetSizes();
        test = cp.HardDisk.GetSizes();
        test = cp.HardDisk.GetSizes();
        test = cp.HardDisk.GetSizes();
        test = cp.HardDisk.GetSizes();
        test = cp.HardDisk.GetSizes();


        var bf = GC.GetAllocatedBytesForCurrentThread();

        for (int i = 0; i < 1000; i++)
        {
            test = cp.HardDisk.GetSizes();
        }

        var af = GC.GetAllocatedBytesForCurrentThread();

        var result = af - bf;
        output.WriteLine($"Result: {result}");

        Assert.Equal(0, result);
    }


    [Fact]
    public void Update_DownloadFinishes_GameIsInstalledOnDisk()
    {
       Computer cp = new(100 * 1024, 100);

       var game = new Game("CyberPunk", 400);
       cp.DownloadManager.AddGameToDownload(game);
       cp.Update(1);
       cp.Update(1);
       cp.Update(1);
       cp.Update(1);

       var iGame = cp.HardDisk.InstalledGames.Any(c => c.Name == game.Name);

       var sizes = cp.HardDisk.GetSizes();
    

       Assert.True(iGame);
       Assert.Equal((100 * 1024) - 400 , sizes.EmptySize);
    }
}
using Core.ComputerModule.Apps;
using Core.ComputerModule.Models;
using Core.GameModule;

namespace Core.Tests.ComputerModule;



public class DownloadManagerTests
{
    // Yeni oyunu ekle → true
    [Fact]
    public void AddGameToDownload_NewGame_ReturnsTrue()
    {
        var hd = new HardDisk(4096 * 1024);
        var dm = new DownloadManager(hd);

        var game = new Game(1,"game", 600 * 1024);
        var res = dm.AddGameToDownload(game);

        Assert.True(res);
    }

    // Aynı oyunu iki kez ekle → ikincisi false
    [Fact]
    public void AddGameToDownload_SameGameTwice_SecondReturnsFalse()
    {

        var hd = new HardDisk(4096 * 1024);
        var dm = new DownloadManager(hd);

        var game = new Game(1,"game", 600 * 1024);
        var res1 = dm.AddGameToDownload(game);
        var res2 = dm.AddGameToDownload(game);

        Assert.True(res1);
        Assert.False(res2);
    }

    // 900'lük disk, 600'lük iki oyunu ekle → ikincisi false
    [Fact]
    public void AddGameToDownload_ExceedsDiskCapacity_ReturnsFalse()
    {

        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);

        var game = new Game(1,"game", 600 * 1024);
        var game1 = new Game(2,"game1", 600 * 1024);
        var res1 = dm.AddGameToDownload(game);
        var res2 = dm.AddGameToDownload(game1);

        Assert.True(res1);
        Assert.False(res2);
    }

    // Hiç indirme yokken Update → çökmemeli
    [Fact]
    public void Update_NoActiveDownloads_DoesNotThrow()
    {

        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        dm.Update(100, 100 * 1024);


    }

    // İki indirme aktif, hız 100, delta 1 → her birinin CurrentSize'ı 50
    [Fact]
    public void Update_TwoActiveDownloads_SplitsSpeedEqually()
    {

        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);

        var game1 = new Game(1,"game1", 400 * 1024);
        var game2 = new Game(2,"game2", 400 * 1024);

        dm.AddGameToDownload(game1);
        dm.AddGameToDownload(game2);

        dm.Update(1, 100 * 1024);

        var gm1 = dm.DownloadList.First(x => x.Game == game1);
        var gm2 = dm.DownloadList.First(x => x.Game == game2);


        Assert.Equal(50 * 1024, gm1.CurrentSize);
        Assert.Equal(50 * 1024, gm2.CurrentSize);
    }

    [Fact]
    public void Update_DownloadFinishes_GameIsOnDisk()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);

        var game1 = new Game(1,"game1", 400 * 1024);

        dm.AddGameToDownload(game1);

        dm.Update(1, 100 * 1024);
        dm.Update(1, 100 * 1024);
        dm.Update(1, 100 * 1024);
        dm.Update(1, 100 * 1024);

        var res = hd.GetSizes();
        Assert.Equal(400 * 1024, res.GameSize);
    }

    [Fact]
    public void Update_ResumeDownload_Game()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);

        var game1 = new Game(1,"game1", 400 * 1024);

        dm.AddGameToDownload(game1);

        dm.Update(1, 100 * 1024);

        dm.PauseDownload(game1);

        dm.Update(1, 100 * 1024);

        dm.ResumeDownload(game1);

        dm.Update(1, 100 * 1024);

        var gm1 = dm.DownloadList.First(x => x.Game == game1);

        var res = gm1.CurrentSize;

        Assert.Equal(200 * 1024, res);
    }


    [Fact]
    public void Update_OneOfTwoPaused_OtherGetsFullSpeed()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);

        var game1 = new Game(1,"game1", 400 * 1024);
        var game2 = new Game(1,"game2", 400 * 1024);

        dm.AddGameToDownload(game1);
        dm.AddGameToDownload(game2);

        var gm1 = dm.DownloadList.First(x => x.Game == game1);
        var gm2 = dm.DownloadList.First(x => x.Game == game2);


        dm.Update(1, 100 * 1024);

        dm.PauseDownload(game2);

        dm.Update(1, 100 * 1024);


        var game1Res = gm1.CurrentSize; // should 150
        var game2Res = gm2.CurrentSize; // should 50


        Assert.Equal(150 * 1024, game1Res);
        Assert.Equal(50 * 1024, game2Res);
    }
}
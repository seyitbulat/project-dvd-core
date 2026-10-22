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

    // --- Tamamlanan işler ---

    [Fact]
    public void Update_DownloadCompletes_RemovesJobFromList()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 100 * 1024);
        dm.AddGameToDownload(game1);

        dm.Update(1, 100 * 1024);

        Assert.Empty(dm.DownloadList);
    }

    [Fact]
    public void Update_DownloadCompletes_GameInstalledAndEventFiredOnce()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 100 * 1024);
        int completedCount = 0;
        dm.DownloadCompleted += _ => completedCount++;
        dm.AddGameToDownload(game1);

        dm.Update(1, 100 * 1024);
        dm.Update(1, 100 * 1024);

        Assert.Contains(game1, hd.InstalledGames);
        Assert.Equal(1, completedCount);
    }

    // Biri biterken diğeri aynı karede indirmeye devam etmeli
    [Fact]
    public void Update_OneOfTwoCompletes_OtherStaysAndKeepsProgress()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var small = new Game(1, "small", 50 * 1024);
        var big = new Game(2, "big", 400 * 1024);
        dm.AddGameToDownload(small);
        dm.AddGameToDownload(big);
        var bigJob = dm.DownloadList.First(x => x.Game == big);

        dm.Update(1, 100 * 1024); // ikisi de 50 alır, small biter

        Assert.Single(dm.DownloadList);
        Assert.Same(bigJob, dm.DownloadList[0]);
        Assert.Equal(50 * 1024, bigJob.CurrentSize);
    }

    // Sırası listede ilk olmayan iş bittiğinde de çalışmalı
    [Fact]
    public void Update_SecondJobCompletes_FirstStays()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var big = new Game(1, "big", 400 * 1024);
        var small = new Game(2, "small", 50 * 1024);
        dm.AddGameToDownload(big);
        dm.AddGameToDownload(small);

        dm.Update(1, 100 * 1024);

        Assert.Single(dm.DownloadList);
        Assert.Same(big, dm.DownloadList[0].Game);
    }

    // Kalan iş, tek başına kaldığı için sonraki karede tüm hızı almalı
    [Fact]
    public void Update_AfterOneCompletes_RemainingGetsFullSpeed()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var small = new Game(1, "small", 50 * 1024);
        var big = new Game(2, "big", 400 * 1024);
        dm.AddGameToDownload(small);
        dm.AddGameToDownload(big);
        var bigJob = dm.DownloadList.First(x => x.Game == big);

        dm.Update(1, 100 * 1024); // big: 50
        dm.Update(1, 100 * 1024); // big: 150

        Assert.Equal(150 * 1024, bigJob.CurrentSize);
    }

    // --- CancelDownload ---

    [Fact]
    public void CancelDownload_DownloadingGame_ReturnsTrue()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        dm.AddGameToDownload(game1);

        Assert.True(dm.CancelDownload(game1));
    }

    [Fact]
    public void CancelDownload_PausedGame_ReturnsTrue()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        dm.AddGameToDownload(game1);
        dm.PauseDownload(game1);

        Assert.True(dm.CancelDownload(game1));
    }

    [Fact]
    public void CancelDownload_UnknownGame_ReturnsFalse()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);

        Assert.False(dm.CancelDownload(new Game(1, "game1", 400 * 1024)));
    }

    [Fact]
    public void CancelDownload_Twice_SecondReturnsFalse()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        dm.AddGameToDownload(game1);

        dm.CancelDownload(game1);

        Assert.False(dm.CancelDownload(game1));
    }

    [Fact]
    public void CancelDownload_CompletedGame_ReturnsFalse()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 100 * 1024);
        dm.AddGameToDownload(game1);
        dm.Update(1, 100 * 1024);

        Assert.False(dm.CancelDownload(game1));
    }

    // İptal edilen iş, başka indirme varken sonraki Update'te listeden çıkmalı
    [Fact]
    public void CancelDownload_WithOtherActiveDownload_RemovedAfterUpdate()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        var game2 = new Game(2, "game2", 400 * 1024);
        dm.AddGameToDownload(game1);
        dm.AddGameToDownload(game2);

        dm.CancelDownload(game1);
        dm.Update(1, 100 * 1024);

        Assert.Single(dm.DownloadList);
        Assert.Same(game2, dm.DownloadList[0].Game);
    }

    // Tek indirme iptal edilince de listeden çıkmalı
    [Fact]
    public void CancelDownload_OnlyDownload_RemovedAfterUpdate()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        dm.AddGameToDownload(game1);

        dm.CancelDownload(game1);
        dm.Update(1, 100 * 1024);

        Assert.Empty(dm.DownloadList);
    }

    // İptal edilen iş hızdan pay almamalı
    [Fact]
    public void CancelDownload_OneOfTwo_OtherGetsFullSpeed()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        var game2 = new Game(2, "game2", 400 * 1024);
        dm.AddGameToDownload(game1);
        dm.AddGameToDownload(game2);
        var job2 = dm.DownloadList.First(x => x.Game == game2);

        dm.CancelDownload(game1);
        dm.Update(1, 100 * 1024);

        Assert.Equal(100 * 1024, job2.CurrentSize);
    }

    [Fact]
    public void CancelDownload_ThenUpdate_GameNotInstalled()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 100 * 1024);
        dm.AddGameToDownload(game1);

        dm.CancelDownload(game1);
        dm.Update(1, 100 * 1024);

        Assert.DoesNotContain(game1, hd.InstalledGames);
    }

    // İptal edilmiş iş devam ettirilememeli
    [Fact]
    public void CancelDownload_ThenResume_ReturnsFalse()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        dm.AddGameToDownload(game1);

        dm.CancelDownload(game1);

        Assert.False(dm.ResumeDownload(game1));
    }

    // İptal, diskteki ayrılmış yeri serbest bırakmalı
    [Fact]
    public void CancelDownload_ReleasesReservedDiskSpace()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        dm.AddGameToDownload(game1);

        dm.CancelDownload(game1);

        Assert.DoesNotContain(game1, hd.ReservedGames);
        Assert.Equal(900 * 1024, hd.GetSizes().EmptySize);
    }

    // İptal edilen oyun tekrar indirilebilmeli
    [Fact]
    public void CancelDownload_ThenUpdate_SameGameCanBeAddedAgain()
    {
        var hd = new HardDisk(900 * 1024);
        var dm = new DownloadManager(hd);
        var game1 = new Game(1, "game1", 400 * 1024);
        dm.AddGameToDownload(game1);

        dm.CancelDownload(game1);
        dm.Update(1, 100 * 1024);

        Assert.True(dm.AddGameToDownload(game1));
    }
}

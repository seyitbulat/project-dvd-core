namespace Core.Tests.ComputerModule;

using Core.ComputerModule.Models;
using Core.GameModule;

public class HardDiskTests
{
    // --- GetSizes ---

    // Boş 900'lük disk → EmptySize 900, GameSize 0
    [Fact]
    public void GetSizes_EmptyDisk_EmptySizeEqualsTotal()
    {
        var hd = new HardDisk(900 * 1024);

        var res = hd.GetSizes();

        Assert.Equal(900 * 1024, res.EmptySize);
    }

    // --- ReserveGame ---

    // 900'lük disk, 600'lük oyun → true
    [Fact]
    public void ReserveGame_EnoughSpace_ReturnsTrue()
    {
        var hd = new HardDisk(900 * 1024);

        var game = new Game(1,"game", 600 * 1024);

        var res = hd.ReserveGame(game);

        Assert.True(res);
    }

    // 500'lük disk, 600'lük oyun → false
    [Fact]
    public void ReserveGame_NotEnoughSpace_ReturnsFalse()
    {
        var hd = new HardDisk(500 * 1024);

        var game = new Game(1,"game", 600 * 1024);

        var res = hd.ReserveGame(game);

        Assert.False(res);

    }

    // 900'lük diskte 600 ayır → EmptySize 300
    [Fact]
    public void ReserveGame_Success_ReducesEmptySize()
    {

        var hd = new HardDisk(900 * 1024);

        var game = new Game(1,"game", 600 * 1024);

        hd.ReserveGame(game);

        var res = hd.GetSizes();

        Assert.Equal(300 * 1024, res.EmptySize);

    }

    //  900'lük disk, iki tane 600'lük oyun, hiçbiri diske eklenmeden → ikinci rezervasyon false
    [Fact]
    public void ReserveGame_TwoGamesExceedCapacity_SecondReturnsFalse()
    {
        var hd = new HardDisk(900 * 1024);

        var game = new Game(1,"game", 600 * 1024);
        var game1 = new Game(2,"game 1", 600 * 1024);
        hd.ReserveGame(game);

        var res = hd.ReserveGame(game1);

        Assert.False(res);
    }

    // Aynı oyunu iki kez ayır → ikincisi false  
    [Fact]
    public void ReserveGame_AlreadyReserved_ReturnsFalse()
    {
        var hd = new HardDisk(1800 * 1024);

        var game = new Game(1,"game", 600 * 1024);
        hd.ReserveGame(game);
        var res = hd.ReserveGame(game);

        Assert.False(res);
    }

    // Ayır + ekle, sonra tekrar ayır → false   
    [Fact]
    public void ReserveGame_AlreadyOnDisk_ReturnsFalse()
    {
        var hd = new HardDisk(1800 * 1024);

        var game = new Game(1,"game", 600 * 1024);
        hd.ReserveGame(game);
        hd.AddGame(game);

        var res = hd.ReserveGame(game);

        Assert.False(res);
    }

    // Aynı isimde ama farklı iki Game nesnesi → ikincisi false
    [Fact]
    public void ReserveGame_DifferentObjectSameName_ReturnsFalse()
    {

        var hd = new HardDisk(1800 * 1024);

        var game = new Game(1,"game", 600 * 1024);

        var game2 = new Game(2,"game", 600 * 1024);

        hd.ReserveGame(game);
        var res = hd.ReserveGame(game2);

        Assert.False(res);
    }

    // --- AddGame ---

    // 900'lük disk: 600 ayır, sonra ekle → true, EmptySize 300 (300 değil 0'sa rezervasyon serbest bırakılmamış demektir)
    [Fact]
    public void AddGame_ReservedGame_ReleasesReservation()
    {
        var hd = new HardDisk(900 * 1024);

        var game = new Game(1,"game", 600 * 1024);
        hd.ReserveGame(game);
        hd.AddGame(game);

        var res = hd.GetSizes();

        Assert.Equal(300 * 1024, res.EmptySize);
    }

    //  DVD senaryosu: rezervasyon yapmadan 600 ekle → true, EmptySize 300
    [Fact]
    public void AddGame_UnreservedWithSpace_ReturnsTrue()
    {
        var hd = new HardDisk(900 * 1024);

        var game = new Game(1,"game", 600 * 1024);

        hd.AddGame(game);

        var res = hd.GetSizes();

        Assert.Equal(300 * 1024, res.EmptySize);
    }

    // Rezervasyon yapmadan 500'lük diske 600 ekle → false
    [Fact]
    public void AddGame_UnreservedWithoutSpace_ReturnsFalse()
    {
        
        var hd = new HardDisk(500 * 1024);

        var game = new Game(1,"game", 600 * 1024);

        var res = hd.AddGame(game);

        Assert.False(res);
    }

    // Aynı oyunu iki kez ekle → ilki true, ikincisi false
    [Fact]
    public void AddGame_SameGameTwice_SecondReturnsFalse()
    {
        var hd = new HardDisk(1200 * 1024);

        var game = new Game(1,"game", 300 * 1024);

        var res1 = hd.AddGame(game);

        var res2 = hd.AddGame(game);

        Assert.True(res1);
        Assert.False(res2);

    }
}

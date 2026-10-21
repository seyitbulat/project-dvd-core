using Core.DiscModule;
using Core.GameModule;

namespace Core.Tests.DiscModule;

public class DiscTypeTests
{
    // CD kapasitesi 700 * 1024 = 716800 KB (tam sayı → sınır testleri kesin)
    private const double CdCapacity = 700 * 1024;

    // --- Statik örnekler ---

    // Her erişimde aynı nesne dönmeli; envanter anahtarı buna güveniyor
    [Fact]
    public void StaticInstances_AccessedTwice_ReturnSameInstance()
    {
        Assert.Same(DiscType.CD, DiscType.CD);
        Assert.Same(DiscType.DVD, DiscType.DVD);
    }

    [Fact]
    public void StaticInstances_CdAndDvd_AreDifferent()
    {
        Assert.NotSame(DiscType.CD, DiscType.DVD);
    }

    // --- GetCountAndSize (CD, kesin sınır değerleri) ---

    // gameSize | beklenen parça sayısı | beklenen son parça boyutu
    [Theory]
    [InlineData(512000, 1, 512000)]      // kapasiteden küçük
    [InlineData(716800, 1, 716800)]      // tam kapasite (sınır)
    [InlineData(716801, 2, 1)]           // kapasite + 1 KB (sınır)
    [InlineData(1433600, 2, 716800)]     // tam 2 kat (sınır)
    [InlineData(1792000, 3, 358400)]     // 2,5 kat
    public void GetCountAndSize_CdGameSizes_ReturnsCountAndLastPartSize(
        double gameSize, long expectedCount, double expectedLastSize)
    {
        var game = new Game(1, "Game", gameSize);

        var result = DiscType.CD.GetCountAndSize(game);

        Assert.Equal(expectedCount, result.Piece);
        Assert.Equal(expectedLastSize, result.LastDiscSize);
    }

    // --- GetCountAndSize (DVD) ---

    // DVD kapasitesi 5 GB; 6 GB'lık oyun → 2 parça, son parça 1 GB
    [Fact]
    public void GetCountAndSize_DvdMultiPartGame_ReturnsCountAndLastPartSize()
    {
        var game = new Game(1, "Spiderman", 6 * 1024 * 1024);

        var result = DiscType.DVD.GetCountAndSize(game);

        Assert.Equal(2, result.Piece);
        Assert.Equal(1 * 1024 * 1024, result.LastDiscSize);
    }

    // Parçaların toplamı her zaman oyun boyutuna eşit olmalı
    [Fact]
    public void GetCountAndSize_DvdMultiPartGame_PartsAddUpToGameSize()
    {
        var game = new Game(1, "Batman", 12.5 * 1024 * 1024);

        var result = DiscType.DVD.GetCountAndSize(game);
        double total = DiscType.DVD.Capacity * (result.Piece - 1) + result.LastDiscSize;

        Assert.Equal(game.Size, total, precision: 6);
    }

    // Aynı oyun CD'de daha çok parçaya bölünür
    [Fact]
    public void GetCountAndSize_SameGameDifferentTypes_ReturnsDifferentCounts()
    {
        var game = new Game(1, "Ori", 1_000_000);

        var onCd = DiscType.CD.GetCountAndSize(game);
        var onDvd = DiscType.DVD.GetCountAndSize(game);

        Assert.Equal(2, onCd.Piece);
        Assert.Equal(1, onDvd.Piece);
    }

    // Son parça hiçbir zaman kapasiteyi aşmamalı ve boş olmamalı
    [Theory]
    [InlineData(1)]
    [InlineData(716799)]
    [InlineData(716800)]
    [InlineData(716801)]
    [InlineData(5_000_000)]
    public void GetCountAndSize_AnyPositiveSize_LastPartWithinCapacity(double gameSize)
    {
        var game = new Game(1, "Game", gameSize);

        var result = DiscType.CD.GetCountAndSize(game);

        Assert.InRange(result.LastDiscSize, double.Epsilon, CdCapacity);
    }
}

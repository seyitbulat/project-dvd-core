using Core.GameModule;

namespace Core.Tests.GameModule;

public class GameTests
{
    [Fact]
    public void Constructor_ValidValues_SetsProperties()
    {
        var game = new Game(7, "Ori", 820 * 1024);

        Assert.Equal(7, game.Id);
        Assert.Equal("Ori", game.Name);
        Assert.Equal(820 * 1024, game.Size);
    }

    // En küçük geçerli boyut (sınır değeri)
    [Fact]
    public void Constructor_SmallestPositiveSize_DoesNotThrow()
    {
        var game = new Game(1, "Tiny", 1);

        Assert.Equal(1, game.Size);
    }

    // Boyutu 0 ya da negatif olan oyun anlamsız; disk/parça hesaplarını bozar
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Constructor_ZeroOrNegativeSize_Throws(double size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Game(1, "Broken", size));
    }
}

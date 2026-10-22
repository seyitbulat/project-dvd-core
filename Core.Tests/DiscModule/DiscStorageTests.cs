using Core.DiscModule;
using Core.GameModule;

namespace Core.Tests.DiscModule;

public class DiscStorageTests
{
    // CD (716800 KB) üzerinde 2,5 kat → 3 parça
    private readonly Game _threePartCdGame = new(1, "GTA", 1792000);

    // --- Count ---

    [Fact]
    public void Count_NewStorage_ReturnsZeroForBlankDisc()
    {
        var storage = new DiscStorage();


        Assert.Equal(0, storage.Count(DiscKey.Blank(DiscType.CD)));
    }

    [Fact]
    public void Count_NewStorage_ReturnsZeroForBurnedDisc()
    {
        var storage = new DiscStorage();

        Assert.Equal(0, storage.Count(DiscKey.Burned(DiscType.CD, _threePartCdGame, 1)));
    }

    // --- Add ---

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Add_NewKey_CountEqualsAmount(int amount)
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);

        storage.Add(key, amount);

        Assert.Equal(amount, storage.Count(key));
    }

    [Fact]
    public void Add_ExistingKey_AccumulatesAmount()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);

        storage.Add(key, 3);
        storage.Add(key, 2);

        Assert.Equal(5, storage.Count(key));
    }

    [Fact]
    public void Add_ExistingKeyOneByOne_AccumulatesAmount()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);

        storage.Add(key, 1);
        storage.Add(key, 1);

        Assert.Equal(2, storage.Count(key));
    }

    // Geçersiz sınırlar: 0 ve negatif
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Add_ZeroOrNegativeAmount_Throws(int amount)
    {
        var storage = new DiscStorage();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => storage.Add(DiscKey.Blank(DiscType.CD), amount));
    }

    [Fact]
    public void Add_InvalidAmount_DoesNotChangeCount()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);
        storage.Add(key, 2);

        Assert.ThrowsAny<ArgumentException>(() => storage.Add(key, 0));

        Assert.Equal(2, storage.Count(key));
    }

    [Fact]
    public void Add_BlankCd_DoesNotAffectBlankDvd()
    {
        var storage = new DiscStorage();

        storage.Add(DiscKey.Blank(DiscType.CD), 4);

        Assert.Equal(0, storage.Count(DiscKey.Blank(DiscType.DVD)));
    }

    [Fact]
    public void Add_BurnedDisc_DoesNotAffectBlankOfSameType()
    {
        var storage = new DiscStorage();

        storage.Add(DiscKey.Burned(DiscType.CD, _threePartCdGame, 1), 2);

        Assert.Equal(0, storage.Count(DiscKey.Blank(DiscType.CD)));
    }

    [Fact]
    public void Add_DifferentParts_CountedSeparately()
    {
        var storage = new DiscStorage();
        var part1 = DiscKey.Burned(DiscType.CD, _threePartCdGame, 1);
        var part2 = DiscKey.Burned(DiscType.CD, _threePartCdGame, 2);

        storage.Add(part1, 1);
        storage.Add(part2, 3);

        Assert.Equal(1, storage.Count(part1));
        Assert.Equal(3, storage.Count(part2));
    }

    // Aynı değerlerle ayrı ayrı oluşturulan anahtarlar aynı yığına düşmeli
    [Fact]
    public void Add_EqualKeysCreatedSeparately_ShareCount()
    {
        var storage = new DiscStorage();

        storage.Add(DiscKey.Burned(DiscType.CD, _threePartCdGame, 2), 1);

        Assert.Equal(1, storage.Count(DiscKey.Burned(DiscType.CD, _threePartCdGame, 2)));
    }

    // --- Remove ---

    [Fact]
    public void Remove_PartialAmount_DecreasesCount()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);
        storage.Add(key, 5);

        storage.Remove(key, 2);

        Assert.Equal(3, storage.Count(key));
    }

    // Sınır: elindekinin tamamı
    [Fact]
    public void Remove_ExactAmount_CountBecomesZero()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);
        storage.Add(key, 3);

        storage.Remove(key, 3);

        Assert.Equal(0, storage.Count(key));
    }

    // Sınır: elindekinden bir fazla
    [Fact]
    public void Remove_MoreThanAvailable_Throws()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);
        storage.Add(key, 3);

        Assert.Throws<ArgumentOutOfRangeException>(() => storage.Remove(key, 4));
    }

    [Fact]
    public void Remove_MoreThanAvailable_DoesNotChangeCount()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);
        storage.Add(key, 3);

        Assert.ThrowsAny<Exception>(() => storage.Remove(key, 4));

        Assert.Equal(3, storage.Count(key));
    }

    // Hiç eklenmemiş anahtar = elinde 0 disk var demek, "fazla" durumuyla aynı hata beklenir
    [Fact]
    public void Remove_MissingKey_ThrowsArgumentOutOfRange()
    {
        var storage = new DiscStorage();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => storage.Remove(DiscKey.Blank(DiscType.DVD), 1));
    }

    [Fact]
    public void Remove_MissingKey_CountStaysZero()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.DVD);

        Assert.ThrowsAny<Exception>(() => storage.Remove(key, 1));

        Assert.Equal(0, storage.Count(key));
    }

    // Geçersiz sınırlar: 0 ve negatif
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Remove_ZeroOrNegativeAmount_Throws(int amount)
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);
        storage.Add(key, 2);

        Assert.Throws<ArgumentOutOfRangeException>(() => storage.Remove(key, amount));
        Assert.Equal(2, storage.Count(key));
    }

    [Fact]
    public void Remove_AllThenAddAgain_CountsFromZero()
    {
        var storage = new DiscStorage();
        var key = DiscKey.Blank(DiscType.CD);
        storage.Add(key, 2);
        storage.Remove(key, 2);

        storage.Add(key, 1);

        Assert.Equal(1, storage.Count(key));
    }

    [Fact]
    public void Remove_OneKey_DoesNotAffectOtherKey()
    {
        var storage = new DiscStorage();
        var cd = DiscKey.Blank(DiscType.CD);
        var dvd = DiscKey.Blank(DiscType.DVD);
        storage.Add(cd, 2);
        storage.Add(dvd, 2);

        storage.Remove(cd, 1);

        Assert.Equal(1, storage.Count(cd));
        Assert.Equal(2, storage.Count(dvd));
    }
}

using Core.DiscModule;
using Core.GameModule;

namespace Core.Tests.DiscModule;

public class DiscKeyTests
{
    // CD (716800 KB) üzerinde 2,5 kat → 3 parça
    private readonly Game _threePartCdGame = new(1, "GTA", 1792000);
    private readonly Game _otherGame = new(2, "Ori", 1792000);

    // --- Blank ---

    [Fact]
    public void Blank_ValidType_HasNoGameAndPartZero()
    {
        var key = DiscKey.Blank(DiscType.DVD);

        Assert.Same(DiscType.DVD, key.Type);
        Assert.Null(key.Game);
        Assert.Equal(0, key.Part);
        Assert.True(key.IsBlank);
    }

    [Fact]
    public void Blank_NullType_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DiscKey.Blank(null!));
    }

    // --- Burned ---

    [Fact]
    public void Burned_MiddlePart_SetsTypeGameAndPart()
    {
        var key = DiscKey.Burned(DiscType.CD, _threePartCdGame, 2);

        Assert.Same(DiscType.CD, key.Type);
        Assert.Same(_threePartCdGame, key.Game);
        Assert.Equal(2, key.Part);
        Assert.False(key.IsBlank);
    }

    // Geçerli sınırlar: ilk ve son parça
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Burned_FirstOrLastPart_KeepsPartNumber(int part)
    {
        var key = DiscKey.Burned(DiscType.CD, _threePartCdGame, part);

        Assert.Equal(part, key.Part);
    }

    // Geçersiz sınırlar: 0, negatif, son parça + 1
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4)]
    public void Burned_PartOutOfRange_Throws(int part)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => DiscKey.Burned(DiscType.CD, _threePartCdGame, part));
    }

    [Fact]
    public void Burned_NullType_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => DiscKey.Burned(null!, _threePartCdGame, 1));
    }

    [Fact]
    public void Burned_NullGame_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => DiscKey.Burned(DiscType.CD, null!, 1));
    }

    // --- Eşitlik (envanterde aynı yığına düşüp düşmeme) ---

    [Fact]
    public void Equals_SameBlankType_AreEqualWithSameHash()
    {
        var a = DiscKey.Blank(DiscType.CD);
        var b = DiscKey.Blank(DiscType.CD);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentBlankTypes_AreNotEqual()
    {
        Assert.NotEqual(DiscKey.Blank(DiscType.CD), DiscKey.Blank(DiscType.DVD));
    }

    [Fact]
    public void Equals_SameGameSamePart_AreEqualWithSameHash()
    {
        var a = DiscKey.Burned(DiscType.CD, _threePartCdGame, 1);
        var b = DiscKey.Burned(DiscType.CD, _threePartCdGame, 1);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_SameGameDifferentPart_AreNotEqual()
    {
        var part1 = DiscKey.Burned(DiscType.CD, _threePartCdGame, 1);
        var part2 = DiscKey.Burned(DiscType.CD, _threePartCdGame, 2);

        Assert.NotEqual(part1, part2);
    }

    [Fact]
    public void Equals_DifferentGameSamePart_AreNotEqual()
    {
        var a = DiscKey.Burned(DiscType.CD, _threePartCdGame, 1);
        var b = DiscKey.Burned(DiscType.CD, _otherGame, 1);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_BlankAndBurned_AreNotEqual()
    {
        Assert.NotEqual(
            DiscKey.Blank(DiscType.CD),
            DiscKey.Burned(DiscType.CD, _threePartCdGame, 1));
    }

    // --- Dictionary içinde kullanım ---

    // Ayrı ayrı oluşturulan aynı anahtarlar aynı satıra düşmeli
    [Fact]
    public void Dictionary_KeysCreatedSeparately_UpdateSameEntry()
    {
        var counts = new Dictionary<DiscKey, int>();

        counts[DiscKey.Blank(DiscType.CD)] = 5;
        counts[DiscKey.Blank(DiscType.CD)] += 1;
        counts[DiscKey.Burned(DiscType.CD, _threePartCdGame, 1)] = 2;
        counts[DiscKey.Burned(DiscType.CD, _threePartCdGame, 2)] = 3;

        Assert.Equal(3, counts.Count);
        Assert.Equal(6, counts[DiscKey.Blank(DiscType.CD)]);
        Assert.Equal(2, counts[DiscKey.Burned(DiscType.CD, _threePartCdGame, 1)]);
    }

    // record struct → IEquatable; Dictionary aramasında kutulama (boxing) olmamalı
    [Fact]
    public void Dictionary_LookupWithKey_DoesNotAllocate()
    {
        var counts = new Dictionary<DiscKey, int>
        {
            [DiscKey.Blank(DiscType.CD)] = 5,
            [DiscKey.Blank(DiscType.DVD)] = 7,
        };
        var key = DiscKey.Blank(DiscType.DVD);
        long total = 0;

        for (int i = 0; i < 10; i++)
            total += counts[key];

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++)
            total += counts[key];
        var after = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(0, after - before);
        Assert.Equal(7 * 1010, total);
    }
}

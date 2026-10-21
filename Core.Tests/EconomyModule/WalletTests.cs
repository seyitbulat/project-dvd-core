using Core.EconomyModule;

namespace Core.Tests.EconomyModule;

public class WalletTests
{
    // --- Constructor ---

    [Fact]
    public void Constructor_PositiveBalance_SetsBalance()
    {
        var wallet = new Wallet(2000);

        Assert.Equal(2000, wallet.Balance);
    }

    // Borç paraya dönüşmesin: negatif başlangıç bakiyesi reddedilir
    [Fact]
    public void Constructor_NegativeBalance_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Wallet(-1));
    }

    // --- Pay ---

    // 2000 bakiye, 1999 öde → 1 kalır
    [Fact]
    public void Pay_LessThanBalance_ReturnsTrueAndDecreasesBalance()
    {
        var wallet = new Wallet(2000);

        var result = wallet.Pay(1999);

        Assert.True(result);
        Assert.Equal(1, wallet.Balance);
    }

    // 2000 bakiye, 2000 öde → sınır değeri, cüzdan boşalır
    [Fact]
    public void Pay_ExactBalance_ReturnsTrueAndEmptiesWallet()
    {
        var wallet = new Wallet(2000);

        var result = wallet.Pay(2000);

        Assert.True(result);
        Assert.Equal(0, wallet.Balance);
    }

    // 2000 bakiye, 2001 öde → reddedilir, bakiye değişmez
    [Fact]
    public void Pay_MoreThanBalance_ReturnsFalseAndKeepsBalance()
    {
        var wallet = new Wallet(2000);

        var result = wallet.Pay(2001);

        Assert.False(result);
        Assert.Equal(2000, wallet.Balance);
    }

    [Fact]
    public void Pay_NegativeAmount_ReturnsFalseAndKeepsBalance()
    {
        var wallet = new Wallet(2000);

        var result = wallet.Pay(-3000);

        Assert.False(result);
        Assert.Equal(2000, wallet.Balance);
    }

    [Fact]
    public void Pay_ZeroAmount_ReturnsTrueAndKeepsBalance()
    {
        var wallet = new Wallet(2000);

        var result = wallet.Pay(0);

        Assert.True(result);
        Assert.Equal(2000, wallet.Balance);
    }

    [Fact]
    public void Pay_Success_RaisesBalanceChangedWithNewBalance()
    {
        var wallet = new Wallet(2000);
        int raisedCount = 0;
        long raisedBalance = -1;
        wallet.BalanceChanged += balance =>
        {
            raisedCount++;
            raisedBalance = balance;
        };

        wallet.Pay(500);

        Assert.Equal(1, raisedCount);
        Assert.Equal(1500, raisedBalance);
    }

    [Fact]
    public void Pay_MoreThanBalance_DoesNotRaiseBalanceChanged()
    {
        var wallet = new Wallet(2000);
        int raisedCount = 0;
        wallet.BalanceChanged += _ => raisedCount++;

        wallet.Pay(2001);

        Assert.Equal(0, raisedCount);
    }

    // Bakiye değişmedi → "bakiye değişti" haberi verilmemeli
    [Fact]
    public void Pay_ZeroAmount_DoesNotRaiseBalanceChanged()
    {
        var wallet = new Wallet(2000);
        int raisedCount = 0;
        wallet.BalanceChanged += _ => raisedCount++;

        wallet.Pay(0);

        Assert.Equal(0, raisedCount);
    }

    // --- Receive ---

    [Fact]
    public void Receive_PositiveAmount_ReturnsTrueAndIncreasesBalance()
    {
        var wallet = new Wallet(2000);

        var result = wallet.Receive(2000);

        Assert.True(result);
        Assert.Equal(4000, wallet.Balance);
    }

    [Fact]
    public void Receive_NegativeAmount_ReturnsFalseAndKeepsBalance()
    {
        var wallet = new Wallet(2000);

        var result = wallet.Receive(-3000);

        Assert.False(result);
        Assert.Equal(2000, wallet.Balance);
    }

    [Fact]
    public void Receive_Success_RaisesBalanceChangedWithNewBalance()
    {
        var wallet = new Wallet(2000);
        int raisedCount = 0;
        long raisedBalance = -1;
        wallet.BalanceChanged += balance =>
        {
            raisedCount++;
            raisedBalance = balance;
        };

        wallet.Receive(500);

        Assert.Equal(1, raisedCount);
        Assert.Equal(2500, raisedBalance);
    }

    [Fact]
    public void Receive_NegativeAmount_DoesNotRaiseBalanceChanged()
    {
        var wallet = new Wallet(2000);
        int raisedCount = 0;
        wallet.BalanceChanged += _ => raisedCount++;

        wallet.Receive(-500);

        Assert.Equal(0, raisedCount);
    }

    [Fact]
    public void Receive_ZeroAmount_DoesNotRaiseBalanceChanged()
    {
        var wallet = new Wallet(2000);
        int raisedCount = 0;
        wallet.BalanceChanged += _ => raisedCount++;

        wallet.Receive(0);

        Assert.Equal(0, raisedCount);
    }

    // --- HasEnough ---

    // Aynı test, üç farklı girdiyle: bakiyeden az / tam eşit / fazla
    [Theory]
    [InlineData(1999, true)]
    [InlineData(2000, true)]
    [InlineData(2001, false)]
    public void HasEnough_AroundBalance_ReturnsExpected(long amount, bool expected)
    {
        var wallet = new Wallet(2000);

        var result = wallet.HasEnough(amount);

        Assert.Equal(expected, result);
    }
}

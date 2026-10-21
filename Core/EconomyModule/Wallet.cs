

namespace Core.EconomyModule;

public class Wallet
{
    // kurus ya da cent 12,50 1250
    private long _balance;
    public long Balance => _balance;


    public event Action<long> BalanceChanged;

    public Wallet(long balance)
    {   
        _balance = balance < 0 ? -balance : balance;
    }


    public bool Pay(long amount)
    {
        if (amount < 0) return false;

        if (!HasEnough(amount))
        {
            return false;
        }

        _balance -= amount;

        BalanceChanged?.Invoke(_balance);

        return true;
    }

    public bool Receive(long amount)
    {
        if (amount < 0) return false;

        _balance += amount;

        BalanceChanged?.Invoke(_balance);
        return true;
    }


    public bool HasEnough(long amount)
    {
        return _balance - amount >= 0;
    }

}
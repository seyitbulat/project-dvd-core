

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Core.DiscModule;



public class DiscStorage
{
    private readonly Dictionary<DiscKey, int> _discCount;

    public DiscStorage()
    {
        _discCount = new();
    }

    public int Count(DiscKey key)
    {
        return _discCount.GetValueOrDefault(key);
    }


    public void Add(DiscKey key, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(_discCount, key, out _);
        count += amount;

    }


    public void Remove(DiscKey key, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        ref int count = ref CollectionsMarshal.GetValueRefOrNullRef(_discCount, key);


        if(Unsafe.IsNullRef(ref count))
        {
            throw new ArgumentOutOfRangeException("Key not found");
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, count);

        count -= amount;


        if(count == 0)
        {
            _discCount.Remove(key);
        }
    }
}
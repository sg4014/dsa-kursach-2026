using AirlineTicketing.Models;

namespace AirlineTicketing.DataStructures;

public class QuadraticHashTable
{
    private class HashEntry
    {
        public string Key { get; set; }
        public Passenger Value { get; set; }
        public bool IsDeleted { get; set; }

        public HashEntry(string key, Passenger value)
        {
            Key = key;
            Value = value;
            IsDeleted = false;
        }
    }

    private HashEntry[] _table;
    private int _count;
    private int _capacity;

    public QuadraticHashTable(int capacity = 101)
    {
        _capacity = capacity;
        _table = new HashEntry[_capacity];
        _count = 0;
    }

    private int GetHash(string key)
    {
        int hash = 0;
        foreach (char c in key)
        {
            hash = (hash * 31 + c) % _capacity;
        }
        return hash;
    }

    public void Insert(string key, Passenger value)
    {
        if (_count >= _capacity / 2)
            Resize();

        int hash = GetHash(key);
        int i = 0;
        int index = hash;

        while (_table[index] != null && !_table[index].IsDeleted && _table[index].Key != key)
        {
            ++i;
            index = (hash + i * i) % _capacity;
        }

        if (_table[index] == null || _table[index].IsDeleted)
            ++_count;

        _table[index] = new HashEntry(key, value);
    }

    public Passenger? Search(string key)
    {
        int hash = GetHash(key);
        int i = 0;
        int index = hash;

        while (_table[index] != null)
        {
            if (!_table[index].IsDeleted && _table[index].Key == key)
                return _table[index].Value;

            ++i;
            index = (hash + i * i) % _capacity;
        }
        return null;
    }

    public bool Remove(string key)
    {
        int hash = GetHash(key);
        int i = 0;
        int index = hash;

        while (_table[index] != null)
        {
            if (!_table[index].IsDeleted && _table[index].Key == key)
            {
                _table[index].IsDeleted = true;
                _count--;
                return true;
            }

            ++i;
            index = (hash + i * i) % _capacity;
        }
        return false;
    }

    public void Clear()
    {
        _table = new HashEntry[_capacity];
        _count = 0;
    }

    public IEnumerable<Passenger> GetAll()
    {
        for (int i = 0; i < _capacity; i++)
        {
            if (_table[i] != null && !_table[i].IsDeleted)
            {
                yield return _table[i].Value;
            }
        }
    }

    private void Resize()
    {
        int oldCapacity = _capacity;
        HashEntry[] oldTable = _table;

        _capacity = _capacity * 2 + 1; // Keeping it odd
        _table = new HashEntry[_capacity];
        _count = 0;

        for (int i = 0; i < oldCapacity; i++)
        {
            if (oldTable[i] != null && !oldTable[i].IsDeleted)
            {
                Insert(oldTable[i].Key, oldTable[i].Value);
            }
        }
    }
}

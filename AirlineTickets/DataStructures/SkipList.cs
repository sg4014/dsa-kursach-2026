using AirlineTicketing.Models;

namespace AirlineTicketing.DataStructures;

public class SkipList
{
    private class SkipListNode
    {
        public Ticket? Value { get; set; }
        public SkipListNode[] Forward { get; set; }

        public SkipListNode(Ticket? value, int level)
        {
            Value = value;
            Forward = new SkipListNode[level + 1];
        }
    }

    private const int MaxLevel = 16;
    private readonly SkipListNode _header = new SkipListNode(null, MaxLevel);
    private int _level = 0;
    private readonly Random _random = new Random();

    private int RandomLevel()
    {
        int lvl = 0;
        while (_random.NextDouble() < 0.5 && lvl < MaxLevel)
            ++lvl;
        return lvl;
    }

    public void Insert(Ticket value)
    {
        SkipListNode[] update = new SkipListNode[MaxLevel + 1];
        SkipListNode current = _header;

        for (int i = _level; i >= 0; i--)
        {
            while (current.Forward[i] != null && string.Compare(current.Forward[i].Value!.TicketNumber, value.TicketNumber, StringComparison.Ordinal) < 0)
            {
                current = current.Forward[i];
            }
            update[i] = current;
        }

        current = current.Forward[0];

        if (current == null || current.Value!.TicketNumber != value.TicketNumber)
        {
            int newLevel = RandomLevel();

            if (newLevel > _level)
            {
                for (int i = _level + 1; i <= newLevel; i++)
                    update[i] = _header;
                _level = newLevel;
            }

            SkipListNode newNode = new SkipListNode(value, newLevel);

            for (int i = 0; i <= newLevel; i++)
            {
                newNode.Forward[i] = update[i].Forward[i];
                update[i].Forward[i] = newNode;
            }
        }
    }

    public bool Remove(string ticketNumber)
    {
        SkipListNode[] update = new SkipListNode[MaxLevel + 1];
        SkipListNode current = _header;

        for (int i = _level; i >= 0; i--)
        {
            while (current.Forward[i] != null && string.Compare(current.Forward[i].Value!.TicketNumber, ticketNumber, StringComparison.Ordinal) < 0)
            {
                current = current.Forward[i];
            }
            update[i] = current;
        }

        current = current.Forward[0];

        if (current != null && current.Value!.TicketNumber == ticketNumber)
        {
            for (int i = 0; i <= _level; i++)
            {
                if (update[i].Forward[i] != current) break;
                update[i].Forward[i] = current.Forward[i];
            }

            while (_level > 0 && _header.Forward[_level] == null)
            {
                _level--;
            }
            return true;
        }
        return false;
    }

    public bool Contains(string ticketNumber)
    {
        SkipListNode current = _header;

        for (int i = _level; i >= 0; i--)
        {
            while (current.Forward[i] != null && string.Compare(current.Forward[i].Value!.TicketNumber, ticketNumber, StringComparison.Ordinal) < 0)
            {
                current = current.Forward[i];
            }
        }

        current = current.Forward[0];
        return current != null && current.Value!.TicketNumber == ticketNumber;
    }

    public IEnumerable<Ticket> GetAll()
    {
        SkipListNode current = _header.Forward[0];
        while (current != null)
        {
            yield return current.Value!;
            current = current.Forward[0];
        }
    }
    
    public void Clear()
    {
        for (int i = 0; i <= MaxLevel; i++)
        {
            _header.Forward[i] = null;
        }
        _level = 0;
    }
}

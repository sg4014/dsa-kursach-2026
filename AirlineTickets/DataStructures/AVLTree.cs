using AirlineTicketing.Models;

namespace AirlineTicketing.DataStructures;

public class AVLTree
{
    private class Node
    {
        public string Key { get; set; }
        public Flight Value { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }
        public int Height { get; set; }

        public Node(string key, Flight value)
        {
            Key = key;
            Value = value;
            Height = 1;
        }
    }

    private Node? _root;

    private int Height(Node? node) => node?.Height ?? 0;
    private int BalanceFactor(Node? node) => node != null ? Height(node.Left) - Height(node.Right) : 0;

    private void UpdateHeight(Node node)
    {
        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
    }

    private Node RotateRight(Node y)
    {
        Node x = y.Left!;
        Node T2 = x.Right;

        x.Right = y;
        y.Left = T2;

        UpdateHeight(y);
        UpdateHeight(x);

        return x;
    }

    private Node RotateLeft(Node x)
    {
        Node y = x.Right!;
        Node T2 = y.Left;

        y.Left = x;
        x.Right = T2;

        UpdateHeight(x);
        UpdateHeight(y);

        return y;
    }

    private Node Balance(Node node)
    {
        UpdateHeight(node);
        int balance = BalanceFactor(node);

        if (balance > 1)
        {
            if (BalanceFactor(node.Left) < 0)
                node.Left = RotateLeft(node.Left!);
            return RotateRight(node);
        }
        if (balance < -1)
        {
            if (BalanceFactor(node.Right) > 0)
                node.Right = RotateRight(node.Right!);
            return RotateLeft(node);
        }

        return node;
    }

    public void Insert(string key, Flight value)
    {
        _root = InsertRec(_root, key, value);
    }

    private Node InsertRec(Node? node, string key, Flight value)
    {
        if (node == null)
            return new Node(key, value);

        int cmp = string.Compare(key, node.Key, StringComparison.Ordinal);
        if (cmp < 0)
            node.Left = InsertRec(node.Left, key, value);
        else if (cmp > 0)
            node.Right = InsertRec(node.Right, key, value);
        else
            node.Value = value; // Update existing

        return Balance(node);
    }

    public Flight? Search(string key)
    {
        Node? current = _root;
        while (current != null)
        {
            int cmp = string.Compare(key, current.Key, StringComparison.Ordinal);
            if (cmp == 0) return current.Value;
            if (cmp < 0) current = current.Left;
            else current = current.Right;
        }
        return null;
    }

    public bool Remove(string key)
    {
        bool removed = false;
        _root = RemoveRec(_root, key, ref removed);
        return removed;
    }

    private Node? RemoveRec(Node? node, string key, ref bool removed)
    {
        if (node == null) return null;

        int cmp = string.Compare(key, node.Key, StringComparison.Ordinal);
        if (cmp < 0)
        {
            node.Left = RemoveRec(node.Left, key, ref removed);
        }
        else if (cmp > 0)
        {
            node.Right = RemoveRec(node.Right, key, ref removed);
        }
        else
        {
            removed = true;
            if (node.Left == null || node.Right == null)
            {
                Node? temp = node.Left ?? node.Right;
                if (temp == null) return null;
                else node = temp;
            }
            else
            {
                Node temp = GetMinValueNode(node.Right);
                node.Key = temp.Key;
                node.Value = temp.Value;
                node.Right = RemoveRec(node.Right, temp.Key, ref removed);
            }
        }

        if (node == null) return node;

        return Balance(node);
    }

    private Node GetMinValueNode(Node node)
    {
        Node current = node;
        while (current.Left != null)
            current = current.Left;
        return current;
    }

    public void Clear()
    {
        _root = null;
    }

    public IEnumerable<Flight> PostOrderTraversal()
    {
        var list = new List<Flight>();
        PostOrderRec(_root, list);
        return list;
    }

    private void PostOrderRec(Node? node, List<Flight> list)
    {
        if (node != null)
        {
            PostOrderRec(node.Left, list);
            PostOrderRec(node.Right, list);
            list.Add(node.Value);
        }
    }
    
    public IEnumerable<Flight> GetAll()
    {
        var list = new List<Flight>();
        InOrderRec(_root, list);
        return list;
    }

    private void InOrderRec(Node? node, List<Flight> list)
    {
        if (node != null)
        {
            InOrderRec(node.Left, list);
            list.Add(node.Value);
            InOrderRec(node.Right, list);
        }
    }
}

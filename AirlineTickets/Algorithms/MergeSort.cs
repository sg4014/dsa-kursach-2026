using AirlineTicketing.Models;

namespace AirlineTicketing.Algorithms;

public static class MergeSort
{
    public static void Sort(List<Ticket> list)
    {
        if (list.Count <= 1) return;
        
        int mid = list.Count / 2;
        List<Ticket> left = list.GetRange(0, mid);
        List<Ticket> right = list.GetRange(mid, list.Count - mid);

        Sort(left);
        Sort(right);

        Merge(list, left, right);
    }

    private static void Merge(List<Ticket> result, List<Ticket> left, List<Ticket> right)
    {
        int i = 0, j = 0, k = 0;

        while (i < left.Count && j < right.Count)
        {
            // Sorted by Ticket Number
            if (string.Compare(left[i].TicketNumber, right[j].TicketNumber, StringComparison.Ordinal) <= 0)
            {
                result[k++] = left[i++];
            }
            else
            {
                result[k++] = right[j++];
            }
        }

        while (i < left.Count)
        {
            result[k++] = left[i++];
        }

        while (j < right.Count)
        {
            result[k++] = right[j++];
        }
    }
}

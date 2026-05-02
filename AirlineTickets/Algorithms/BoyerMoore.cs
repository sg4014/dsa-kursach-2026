namespace AirlineTicketing.Algorithms;

public static class BoyerMoore
{
    public static bool Contains(string text, string pattern)
    {
        if (string.IsNullOrEmpty(pattern)) return true;
        if (string.IsNullOrEmpty(text)) return false;
        
        text = text.ToLowerInvariant();
        pattern = pattern.ToLowerInvariant();

        int n = text.Length;
        int m = pattern.Length;
        int[] badChar = new int[256];

        for (int i = 0; i < 256; i++)
            badChar[i] = -1;

        for (int i = 0; i < m; i++)
        {
            if (pattern[i] < 256)
                badChar[pattern[i]] = i;
        }

        int s = 0;
        while (s <= (n - m))
        {
            int j = m - 1;

            while (j >= 0 && pattern[j] == text[s + j])
                j--;

            if (j < 0)
            {
                return true; 
            }
            else
            {
                int matchedCharIdx = text[s + j] < 256 ? badChar[text[s + j]] : -1;
                s += Math.Max(1, j - matchedCharIdx);
            }
        }

        return false;
    }
}

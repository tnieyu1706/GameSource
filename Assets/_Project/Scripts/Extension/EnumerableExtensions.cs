using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static int NotNullCount<T>(this IEnumerable<T> enumerable)
    {
        int count = 0;
        foreach (var item in enumerable)
        {
            if (item != null)
                count++;
        }
        return count;
    }
}
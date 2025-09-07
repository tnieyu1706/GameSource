using System.Collections.Generic;
using UnityEngine;
using VFavorites.Libs;

public static class MyListExtensions
{

    public static void Resize<T>(this List<T> list, int newSize)
    {
        if (newSize < 0)
        {
            Debug.LogError($"New size cannot be less than 0");
            return;
        }

        int resizedCount = list.Count - newSize;
        while (resizedCount != 0)
        {
            if (resizedCount > 0)
            {
                list.RemoveLast();
                resizedCount--;
            }
            else
            {
                list.Add(default(T));
                resizedCount++;
            }
        }
    }
}
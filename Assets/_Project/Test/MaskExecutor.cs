using System;
using System.Collections.Generic;
using UnityEngine;

public static class MaskExecutor
{
    public static List<T> ExecuteMask<T>(Texture2D texture2D, float threshold, Func<float, float, T> action)
    {
        List<T> results = new List<T>();

        int width = texture2D.width;
        int height = texture2D.height;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                results.Add(action(i, j));
            }
        }

        return results;
    }

    public static List<Vector2> GetSatisfiedMaskPixels(Texture2D texture2D, float threshold)
    {
        List<Vector2> satisfiedPixels = new List<Vector2>();
        Color[] pixels = texture2D.GetPixels();
        int width = texture2D.width;
        int height = texture2D.height;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (pixels[y * width + x].r > threshold)
                {
                    satisfiedPixels.Add(new Vector2(x, y));
                }
            }
        }
        
        return satisfiedPixels;
    }
}
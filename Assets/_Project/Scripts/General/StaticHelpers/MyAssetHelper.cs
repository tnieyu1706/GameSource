#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;

namespace _Project.Scripts.General.StaticHelpers
{
    public static class MyAssetHelper
    {
        public static string GetAssetName(Object asset)
        {
            string filePath = AssetDatabase.GetAssetPath(asset);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            return fileName;
        }
    }
}


#endif
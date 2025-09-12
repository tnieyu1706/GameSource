using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CubeToLUTWindow : EditorWindow
{
    private string cubeFilePath = "";
    private int size = 32;          // LUT size (thường 16 hoặc 32)
    private string savePath = "Assets/LUT_Converted.png";

    [MenuItem("Tools/LUT/Convert .cube to Texture2D")]
    public static void ShowWindow()
    {
        GetWindow<CubeToLUTWindow>("Cube to LUT");
    }

    void OnGUI()
    {
        GUILayout.Label("Convert .cube LUT → Unity Texture2D", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        cubeFilePath = EditorGUILayout.TextField("Cube File", cubeFilePath);
        if (GUILayout.Button("Browse", GUILayout.MaxWidth(80)))
        {
            string path = EditorUtility.OpenFilePanel("Select .cube LUT file", "Assets", "cube");
            if (!string.IsNullOrEmpty(path))
            {
                // Chuyển sang đường dẫn relative để dùng trong project
                if (path.StartsWith(Application.dataPath))
                {
                    cubeFilePath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    cubeFilePath = path;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        size = EditorGUILayout.IntField("LUT Size", size);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Convert and Save"))
        {
            if (string.IsNullOrEmpty(cubeFilePath) || !File.Exists(cubeFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Please select a valid .cube file", "OK");
                return;
            }

            string text = File.ReadAllText(cubeFilePath);
            Texture2D tex = ConvertCubeToLUT(text, size);
            if (tex != null)
            {
                byte[] png = tex.EncodeToPNG();
                File.WriteAllBytes(savePath, png);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", "Saved LUT to " + savePath, "OK");
            }
        }
    }

    private Texture2D ConvertCubeToLUT(string cubeText, int size)
    {
        var colors = new List<Color>();

        using (StringReader reader = new StringReader(cubeText))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#") || char.IsLetter(line[0]))
                    continue;

                var parts = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    float r = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                    float g = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                    float b = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                    colors.Add(new Color(r, g, b, 1));
                }
            }
        }

        if (colors.Count != size * size * size)
        {
            Debug.LogError($"Cube file does not match expected LUT size ({size}), colors found: {colors.Count}");
            return null;
        }

        // Unity URP yêu cầu strip LUT 1024x32 (với size=32)
        Texture2D tex = new Texture2D(size * size, size, TextureFormat.RGBA32, false, true);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = x + y * size + z * size * size;
                    Color c = colors[index];
                    tex.SetPixel(x + z * size, y, c);
                }
            }
        }

        tex.Apply();
        return tex;
    }
}

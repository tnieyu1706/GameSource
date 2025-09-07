using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class TextureArrayCreator : EditorWindow
{
    private List<Texture2D> selectedTextures = new List<Texture2D>();
    private string saveFolder = "Assets";
    private string fileName = "MyTextureArray.asset";
    private bool generateMipMaps = true;
    private FilterMode filterMode = FilterMode.Bilinear;
    private TextureWrapMode wrapMode = TextureWrapMode.Repeat;

    [MenuItem("Tools/Texture Array Creator (Select Textures)")]
    public static void ShowWindow()
    {
        GetWindow<TextureArrayCreator>("Texture Array Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Textures to Combine", EditorStyles.boldLabel);

        // Danh sách texture đã chọn
        for (int i = 0; i < selectedTextures.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            selectedTextures[i] = (Texture2D)EditorGUILayout.ObjectField(selectedTextures[i], typeof(Texture2D), false);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                selectedTextures.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        // Nút thêm texture
        if (GUILayout.Button("Add Texture"))
        {
            selectedTextures.Add(null);
        }

        GUILayout.Space(10);

        // Chọn thư mục lưu
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save Folder", saveFolder);
        if (GUILayout.Button("Select Folder", GUILayout.Width(110)))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Save Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.StartsWith(Application.dataPath))
                {
                    saveFolder = "Assets" + selected.Substring(Application.dataPath.Length);
                }
                else
                {
                    Debug.LogError("Folder must be inside Assets!");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        // Tên file
        fileName = EditorGUILayout.TextField("File Name", fileName);

        generateMipMaps = EditorGUILayout.Toggle("Generate MipMaps", generateMipMaps);
        filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", filterMode);
        wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Wrap Mode", wrapMode);

        GUILayout.Space(10);
        if (GUILayout.Button("Create Texture Array", GUILayout.Height(30)))
        {
            CreateTextureArray();
        }
    }

    private void CreateTextureArray()
    {
        selectedTextures.RemoveAll(t => t == null);

        if (selectedTextures.Count == 0)
        {
            Debug.LogError("No textures selected!");
            return;
        }

        int width = selectedTextures[0].width;
        int height = selectedTextures[0].height;
        TextureFormat format = selectedTextures[0].format;

        Texture2DArray textureArray = new Texture2DArray(width, height, selectedTextures.Count, format, generateMipMaps, false);
        textureArray.filterMode = filterMode;
        textureArray.wrapMode = wrapMode;

        for (int i = 0; i < selectedTextures.Count; i++)
        {
            for (int mip = 0; mip < selectedTextures[i].mipmapCount; mip++)
            {
                Graphics.CopyTexture(selectedTextures[i], 0, mip, textureArray, i, mip);
            }
        }

        string savePath = Path.Combine(saveFolder, fileName);
        AssetDatabase.CreateAsset(textureArray, savePath);
        AssetDatabase.SaveAssets();

        Debug.Log($"✅ Texture2DArray created at: {savePath}");
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveMeshesWindow : EditorWindow
{
    private List<GameObject> selectedObjects = new List<GameObject>();
    private string saveFolder = "Assets";
    private Vector2 scrollPos;
    private string log = "";

    [MenuItem("Tools/Save Meshes Window")]
    public static void ShowWindow()
    {
        GetWindow<SaveMeshesWindow>("Save Meshes");
    }

    private void OnEnable()
    {
        RefreshSelection();
    }

    private void OnSelectionChange()
    {
        RefreshSelection();
        Repaint();
    }

    void RefreshSelection()
    {
        selectedObjects.Clear();
        foreach (var go in Selection.gameObjects)
        {
            if (go.GetComponent<MeshFilter>() != null)
                selectedObjects.Add(go);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Selected GameObjects with MeshFilter:", EditorStyles.boldLabel);

        if (selectedObjects.Count == 0)
        {
            EditorGUILayout.HelpBox("No GameObjects with MeshFilter selected.", MessageType.Warning);
        }
        else
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
            foreach (var go in selectedObjects)
            {
                EditorGUILayout.LabelField(go.name);
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Save folder (inside Assets):");
        EditorGUILayout.BeginHorizontal();
        saveFolder = EditorGUILayout.TextField(saveFolder);
        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(folder) && folder.StartsWith(Application.dataPath))
            {
                saveFolder = "Assets" + folder.Substring(Application.dataPath.Length);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid folder", "Please select a folder inside the Assets folder.", "OK");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUI.enabled = selectedObjects.Count > 0 && Directory.Exists(saveFolder);
        if (GUILayout.Button("Save Meshes & Materials"))
        {
            SaveMeshesAndMaterials();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Log:");
        EditorGUILayout.HelpBox(log, MessageType.Info);
    }

    void SaveMeshesAndMaterials()
    {
        int savedMeshes = 0;
        int savedMats = 0;
        log = "";

        foreach (var go in selectedObjects)
        {
            // Save Mesh
            var mf = go.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                Mesh meshCopy = Object.Instantiate(mf.sharedMesh);
                string meshPath = Path.Combine(saveFolder, go.name + ".asset");
                meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);
                AssetDatabase.CreateAsset(meshCopy, meshPath);
                savedMeshes++;
                log += $"Saved mesh '{go.name}' to '{meshPath}'\n";
            }
            else
            {
                log += $"Skipped mesh for '{go.name}' (no mesh)\n";
            }

            // Save Materials
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat == null) continue;

                    Material matCopy = new Material(mat);
                    string matPath = Path.Combine(saveFolder, mat.name + ".mat");
                    matPath = AssetDatabase.GenerateUniqueAssetPath(matPath);
                    AssetDatabase.CreateAsset(matCopy, matPath);
                    savedMats++;
                    log += $"Saved material '{mat.name}' to '{matPath}'\n";
                }
            }
        }

        if (savedMeshes > 0 || savedMats > 0)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            log += $"Done! {savedMeshes} meshes and {savedMats} materials saved.";
        }
        else
        {
            log += "No assets saved.";
        }
    }
}

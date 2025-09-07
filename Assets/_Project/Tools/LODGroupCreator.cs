using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LODGroupCreator : EditorWindow
{
    private List<float> lodScreenPercentages = new List<float> {0.8f, 0.6f, 0.4f, 0.2f, 0.1f };

    [MenuItem("Tools/Create LOD Group From Selection")]
    static void ShowWindow()
    {
        GetWindow<LODGroupCreator>("LOD Group Creator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("LOD Settings", EditorStyles.boldLabel);
        
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Slider($"LOD 0 Threshold", 1, 0f, 1f);
        EditorGUI.EndDisabledGroup();

        for (int i = 0; i < lodScreenPercentages.Count-1; i++)
            lodScreenPercentages[i] = EditorGUILayout.Slider($"LOD {i+1} Threshold", lodScreenPercentages[i], 0f, 1f);
        
        lodScreenPercentages[^1] = EditorGUILayout.Slider($"Culled Threshold", lodScreenPercentages.Last(), 0f, 1f);

        if (GUILayout.Button("Add LOD Level"))
        {
            lodScreenPercentages.Add(0.01f);
        }

        if (GUILayout.Button("Remove Last LOD Level") && lodScreenPercentages.Count > 0)
        {
            lodScreenPercentages.RemoveAt(lodScreenPercentages.Count - 1);
        }

        if (GUILayout.Button("Generate LOD Group from Selection"))
        {
            CreateLODGroup();
        }
    }

    private void CreateLODGroup()
    {
        GameObject[] selecteds = Selection.gameObjects;

        if (selecteds.Length == 0)
        {
            return;
        }

        foreach (var selected in selecteds)
        {
            var renderers = selected.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                Debug.LogError("No Renderers found in selected GameObject.");
                return;
            }

            Undo.RegisterCompleteObjectUndo(selected, "Create LOD Group");

            LODGroup lodGroup = selected.GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                lodGroup = selected.AddComponent<LODGroup>();
            }

            List<LOD> lods = new List<LOD>();
            int maxLODCount = Mathf.Min(lodScreenPercentages.Count, renderers.Length);

            for (int i = 0; i < maxLODCount; i++)
            {
                Renderer[] lodRenderers = { renderers[i] };
                LOD lod = new LOD(lodScreenPercentages[i], lodRenderers);
                lods.Add(lod);
            }

            lodGroup.SetLODs(lods.ToArray());
            lodGroup.RecalculateBounds();

            Debug.Log($"LOD Group created with {lods.Count} levels for gameObject {selected.name}.");
        }
    }
}
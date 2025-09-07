using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using _Project.Scripts.Item;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using Object = UnityEngine.Object;

public class IdentifySystemHelperWindow : EditorWindow
{
    private Vector2 scrollPos;
    private Dictionary<Type, bool> foldoutStates = new();

    // Area: Load Identifiers
    private string typeName = "";
    private string folderPath = "Assets/";

    // Addressable convert
    private AddressableAssetGroup addressableGroup;
    private string prefixName = "Item";
    private string typeNameAddress = "";

    [MenuItem("Window/Identify System")]
    public static void ShowWindow()
    {
        GetWindow<IdentifySystemHelperWindow>("Identify System");
    }

    private void OnGUI()
    {
        #region Default

        GUILayout.Label("Identify System", EditorStyles.boldLabel);
        GUILayout.Space(5);

        // ---------------- Reset Button ----------------
        if (GUILayout.Button("Reset Identify System", GUILayout.Height(25)))
        {
            IdentifySystemHelper.Registries.Clear();
            Debug.Log("IdentifySystem reset.");
        }

        GUILayout.Space(10);

        #endregion

        #region LoadIdentifiers

        // ---------------- Load Identifiers Area ----------------
        GUILayout.Label("Load Identifiers", EditorStyles.boldLabel);

        typeName = EditorGUILayout.TextField("Type (FullName)", typeName);

// Cho phép kéo thả folder
        Object folderObj = null;
        folderObj = EditorGUILayout.ObjectField("Folder", folderObj, typeof(DefaultAsset), false);

        if (folderObj != null)
        {
            string path = AssetDatabase.GetAssetPath(folderObj);
            if (AssetDatabase.IsValidFolder(path))
            {
                folderPath = path;
            }
            else
            {
                Debug.LogError("Please drop a valid folder from Assets/");
            }
        }

// Hiển thị lại path để tiện nhìn
        EditorGUILayout.LabelField("Folder Path:", folderPath);

        if (GUILayout.Button("Load Identifiers", GUILayout.Height(25)))
        {
            LoadIdentifiers(typeName, folderPath);
        }

        #endregion

        #region AddressConvert

        DrawAddressableAssignSection();

        #endregion

        #region ShowRegistries

        // ---------------- Show Registries ----------------
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var registry in IdentifySystemHelper.Registries)
        {
            var type = registry.Key;
            var manager = registry.Value;

            if (!foldoutStates.ContainsKey(type))
                foldoutStates[type] = false;

            foldoutStates[type] = EditorGUILayout.Foldout(
                foldoutStates[type],
                $"{type.Name} (CurrentId: {manager.CurrentId})",
                true,
                EditorStyles.foldoutHeader
            );

            if (foldoutStates[type])
            {
                EditorGUI.indentLevel++;
                foreach (var kv in manager.Identifiers)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField($"ID: {kv.Key}", GUILayout.Width(80));

                    if (kv.Value != null)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(kv.Value, kv.Value.GetType(), false);
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("Ping", GUILayout.Width(50)))
                        {
                            EditorGUIUtility.PingObject(kv.Value);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("null");
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            GUILayout.Space(3);
        }

        EditorGUILayout.EndScrollView();

        #endregion
    }

    private void LoadIdentifiers(string typeName, string folderPath)
    {
        if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("Type name or folder path is empty!");
            return;
        }

        var type = Type.GetType(typeName);
        if (type == null || !typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            Debug.LogError($"Invalid type: {typeName}. Must inherit UnityEngine.Object.");
            return;
        }

        if (!IdentifySystemHelper.Registries.ContainsKey(type))
            IdentifySystemHelper.Registries[type] = new IdentifyManager();

        var manager = IdentifySystemHelper.Registries[type];
        manager.Identifiers.Clear();
        manager.CurrentId = -1;

        // Lấy tất cả asset trong folder và subfolder
        string[] guids = AssetDatabase.FindAssets($"t:{type.Name}", new[] { folderPath });
        int id = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path)) // Bỏ qua folder
            {
                var obj = AssetDatabase.LoadAssetAtPath(path, type) as UnityEngine.Object;
                if (obj != null)
                {
                    manager.CurrentId = id;
                    manager.Identifiers[id] = obj;

                    if (obj is IBaseTypeData baseData)
                    {
                        baseData.TypeId = id;
                        EditorUtility.SetDirty(obj);
                    }

                    id++;
                }
            }
        }

        Debug.Log($"Loaded {id} identifiers for type {type.Name} from {folderPath} (including subfolders)");
    }


    private void DrawAddressableAssignSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Assign To Addressables", EditorStyles.boldLabel);

        prefixName = EditorGUILayout.TextField("Prefix Name", prefixName);
        addressableGroup = (AddressableAssetGroup)EditorGUILayout.ObjectField(
            "Addressable Group",
            addressableGroup,
            typeof(AddressableAssetGroup),
            false);
        typeNameAddress = EditorGUILayout.TextField("Type Name Address", typeNameAddress);

        if (GUILayout.Button("Assign Loaded Identifiers to Addressables", GUILayout.Height(25)))
        {
            AssignIdentifiersToAddressables(typeNameAddress, addressableGroup, prefixName);
        }
    }

    private void AssignIdentifiersToAddressables(string typeName, AddressableAssetGroup assetGroup, string prefixName)
    {
        var type = Type.GetType(typeName);
        if (type == null || !IdentifySystemHelper.Registries.TryGetValue(type, out var manager))
        {
            Debug.LogError("No identifiers found for type: " + typeName);
            return;
        }

        // Tìm group
        if (assetGroup == null)
        {
            Debug.Log($"AssetGroup is null!");
            return;
        }

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found!");
            return;
        }

        int count = 0;
        foreach (var kv in manager.Identifiers)
        {
            int id = kv.Key;
            var obj = kv.Value;
            if (obj == null) continue;

            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) continue;

            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid)) continue;

            // Tạo hoặc move entry vào group
            var entry = settings.CreateOrMoveEntry(guid, assetGroup);
            entry.address = $"{prefixName}_{id}";
            count++;
        }

        // Lưu thay đổi
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();

        Debug.Log(
            $"✅ Assigned {count} assets of type {type.Name} to group '{assetGroup.name}' with prefix '{prefixName}_'.");
    }
}
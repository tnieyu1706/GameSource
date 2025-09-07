// using UnityEditor;
// using UnityEngine;
//
// public class ReplaceAssetReferencesWindow : EditorWindow
// {
//     private Object dragSource; // cái đang dùng
//     private Object dropSource; // cái thay thế
//
//     [MenuItem("Tools/Replace Asset References")]
//     public static void ShowWindow()
//     {
//         GetWindow<ReplaceAssetReferencesWindow>("Replace References");
//     }
//
//     void OnGUI()
//     {
//         GUILayout.Label("Replace Asset References", EditorStyles.boldLabel);
//
//         dragSource = EditorGUILayout.ObjectField("Drag Source (From)", dragSource, typeof(Object), false);
//         dropSource = EditorGUILayout.ObjectField("Drop Source (To)", dropSource, typeof(Object), false);
//
//         // Kiểm tra type
//         if (dragSource != null && dropSource != null && dragSource.GetType() != dropSource.GetType())
//         {
//             EditorGUILayout.HelpBox("⚠ DragSource và DropSource phải cùng type!", MessageType.Error);
//             GUI.enabled = false;
//         }
//
//         if (GUILayout.Button("Execute Replace"))
//         {
//             ExecuteReplace();
//         }
//
//         GUI.enabled = true;
//     }
//
//     void ExecuteReplace()
//     {
//         if (dragSource == null || dropSource == null)
//         {
//             Debug.LogError("Chưa chọn đủ DragSource và DropSource!");
//             return;
//         }
//
//         string[] guids = AssetDatabase.FindAssets("t:Object"); // lấy toàn bộ asset
//         int count = 0;
//
//         foreach (string guid in guids)
//         {
//             string path = AssetDatabase.GUIDToAssetPath(guid);
//
//             // ❌ Bỏ qua scene (để khỏi bị lỗi)
//             if (path.EndsWith(".unity"))
//                 continue;
//
//             Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
//
//             foreach (Object obj in objs)
//             {
//                 SerializedObject so = new SerializedObject(obj);
//                 SerializedProperty sp = so.GetIterator();
//
//                 bool modified = false;
//
//                 while (sp.NextVisible(true))
//                 {
//                     if (sp.propertyType == SerializedPropertyType.ObjectReference &&
//                         sp.objectReferenceValue == dragSource)
//                     {
//                         sp.objectReferenceValue = dropSource;
//                         modified = true;
//                     }
//                 }
//
//                 if (modified)
//                 {
//                     so.ApplyModifiedProperties();
//                     EditorUtility.SetDirty(obj);
//                     count++;
//                 }
//             }
//         }
//
//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//         Debug.Log($"✅ Đã thay thế {count} references từ {dragSource.name} → {dropSource.name}");
//     }
// }
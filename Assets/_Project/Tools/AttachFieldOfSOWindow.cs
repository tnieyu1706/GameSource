using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class AttachFieldOfSOWindow : EditorWindow
{
    private string attachedTypeName;   // nhập text
    private Type attachedType;

    private ScriptableObject dragSO;
    private string dragFieldName;

    private ScriptableObject dropSO;
    private string dropFieldName;

    [MenuItem("Tools/Attach Field Of SO")]
    public static void ShowWindow()
    {
        GetWindow<AttachFieldOfSOWindow>("Attach Field Of SO");
    }

    private void OnGUI()
    {
        GUILayout.Label("Attach Field Tool", EditorStyles.boldLabel);

        // Text nhập TypeName
        attachedTypeName = EditorGUILayout.TextField("Attached Type (Name)", attachedTypeName);

        if (!string.IsNullOrEmpty(attachedTypeName))
        {
            attachedType = TypeIdentifySupport.ResolveType(attachedTypeName);
            if (attachedType == null)
                EditorGUILayout.HelpBox("⚠️ Type not found. Use full name: Namespace.TypeName", MessageType.Warning);
            else
                EditorGUILayout.LabelField("Resolved Type:", attachedType.FullName);
        }

        dragSO = (ScriptableObject)EditorGUILayout.ObjectField("Drag SO", dragSO, typeof(ScriptableObject), false);
        dragFieldName = EditorGUILayout.TextField("Drag Field Name", dragFieldName);

        dropSO = (ScriptableObject)EditorGUILayout.ObjectField("Drop SO", dropSO, typeof(ScriptableObject), false);
        dropFieldName = EditorGUILayout.TextField("Drop Field Name", dropFieldName);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Activate"))
        {
            ActivateAttach();
        }
    }

    private void ActivateAttach()
    {
        if (string.IsNullOrEmpty(attachedTypeName) || attachedType == null ||
            dragSO == null || dropSO == null ||
            string.IsNullOrEmpty(dragFieldName) || string.IsNullOrEmpty(dropFieldName))
        {
            Debug.LogError("❌ Missing information. Please fill in all fields.");
            return;
        }

        // Get Drag field
        var dragField = dragSO.GetType().GetField(dragFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (dragField == null)
        {
            Debug.LogError($"❌ Drag field '{dragFieldName}' not found in {dragSO.GetType().Name}");
            return;
        }

        if (!attachedType.IsAssignableFrom(dragField.FieldType))
        {
            Debug.LogError($"❌ Drag field type {dragField.FieldType} does not match AttachedType {attachedType}");
            return;
        }

        // Get Drop field
        var dropField = dropSO.GetType().GetField(dropFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (dropField == null)
        {
            Debug.LogError($"❌ Drop field '{dropFieldName}' not found in {dropSO.GetType().Name}");
            return;
        }

        if (!dropField.FieldType.IsAssignableFrom(attachedType))
        {
            Debug.LogError($"❌ Drop field type {dropField.FieldType} does not accept {attachedType}");
            return;
        }

        // Copy value
        object value = dragField.GetValue(dragSO);
        dropField.SetValue(dropSO, value);

        EditorUtility.SetDirty(dropSO);
        AssetDatabase.SaveAssets();

        Debug.Log($"✅ Successfully attached {dragSO.name}.{dragFieldName} → {dropSO.name}.{dropFieldName}");
    }
}
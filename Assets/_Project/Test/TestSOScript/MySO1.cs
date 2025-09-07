using _Project.Test.TestSOScript;
using UnityEditor;
using UnityEngine;

namespace _Project.Test.TesetSOScript
{
    [CreateAssetMenu(fileName = "New Test SO1", menuName = "TestSO/MySO1")]
    public class MySO1 : ScriptableObject
    {
        [SerializeField] private NatureSO nature;
    }
    
    
    // [CustomEditor(typeof(MySO1))]
    // public class MySO1Editor : Editor
    // {
    //     private SerializedProperty natureProp;
    //     private NatureSO droppedNatureSO;
    //
    //     private void OnEnable()
    //     {
    //         natureProp = serializedObject.FindProperty("nature");
    //     }
    //
    //     public override void OnInspectorGUI()
    //     {
    //         serializedObject.Update();
    //
    //         // Hiển thị field nature (interface)
    //         EditorGUILayout.PropertyField(natureProp, true);
    //
    //         EditorGUILayout.Space(10);
    //         EditorGUILayout.LabelField("Nature Drag", EditorStyles.boldLabel);
    //
    //         // Drag–drop field cho NatureSO
    //         droppedNatureSO = (NatureSO)EditorGUILayout.ObjectField(
    //             "Drop NatureSO",
    //             droppedNatureSO,
    //             typeof(NatureSO),
    //             false
    //         );
    //
    //         if (droppedNatureSO != null)
    //         {
    //             if (droppedNatureSO.nature != null)
    //             {
    //                 // Gán Nature từ SO vào SerializeReference
    //                 natureProp.managedReferenceValue = droppedNatureSO.nature;
    //
    //                 Debug.Log($"Assigned Nature '{droppedNatureSO.nature.NatureName}' to MySO1");
    //             }
    //             else
    //             {
    //                 Debug.LogWarning("Dropped NatureSO has no Nature data!");
    //             }
    //
    //             // Reset object field sau khi gán xong
    //             droppedNatureSO = null;
    //         }
    //
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
}
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Utils
{
    [CustomPropertyDrawer(typeof(IInterfaceShowInspector), false)]
    public class InterfaceValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            object target = property.managedReferenceValue;
            if (target == null)
            {
                EditorGUI.LabelField(position, label.text, "Null (no implement)");
                return;
            }

            // Header: hiển thị tên class
            EditorGUI.LabelField(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                $"{label.text} ({target.GetType().Name})"
            );
            position.y += EditorGUIUtility.singleLineHeight + 2;

            // 🔑 Duyệt tất cả SerializedProperty con thay vì reflection thủ công
            SerializedProperty iterator = property.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, end))
            {
                float height = EditorGUI.GetPropertyHeight(iterator, true);
                Rect fieldRect = new Rect(position.x, position.y, position.width, height);

                EditorGUI.PropertyField(fieldRect, iterator, true);

                position.y += height + 2;
                enterChildren = false;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 🔒 Nếu không phải managedReference thì height cơ bản thôi
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUIUtility.singleLineHeight;

            object target = property.managedReferenceValue;
            if (target == null) return EditorGUIUtility.singleLineHeight;

            var fields = target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            int visibleCount = 1 + fields.Length; // header + field
            return visibleCount * (EditorGUIUtility.singleLineHeight + 2);
        }
    }
}
#endif

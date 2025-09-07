using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Utils.MyAttribute.Editor
{
    public abstract class MyButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<MyButtonAttribute>();

                if (buttonAttribute != null)
                {
                    var label = string.IsNullOrEmpty(buttonAttribute.Label) ? method.Name : buttonAttribute.Label;

                    if (GUILayout.Button(label))
                    {
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }
}
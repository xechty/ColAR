using System;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RC_Extension
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class ShowAsLayerAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ShowAsLayerAttribute))]
    public class ShowAsLayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "Use ShowAsLayer only with int fields!", MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            {
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                {
                    property.intValue = EditorGUI.LayerField(position, label, property.intValue);
                }
                EditorGUI.showMixedValue = false;
            }
            EditorGUI.EndProperty();
        }
    }
#endif
}

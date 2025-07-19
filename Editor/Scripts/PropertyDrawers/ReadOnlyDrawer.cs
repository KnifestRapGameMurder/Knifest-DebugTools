using Knifest.DebugTools.PropertyAttributes;
using UnityEditor;
using UnityEngine;

namespace Knifest.DebugTools.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Adjust height for arrays/lists if needed
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Save previous GUI enabled state
            GUI.enabled = false;

            // Draw the property in a disabled state
            EditorGUI.PropertyField(position, property, label, true);

            // Restore GUI enabled state
            GUI.enabled = true;
        }
    }
}
using CommonUtils.Inspector;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
    [CustomPropertyDrawer(typeof(UnityLayerAttribute))]
    public class UnityLayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (property.propertyType == SerializedPropertyType.Integer)
                property.intValue = EditorGUI.LayerField(position, property.intValue);
            else
                EditorGUI.HelpBox(position, "Not an int.", MessageType.Error);

            EditorGUI.EndProperty( );
        }
    }
}

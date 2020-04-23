using CommonUtils.Inspector;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
	[CustomPropertyDrawer(typeof(UnityTagAttribute))]
	public class UnityTagAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

				//Debug.Log("TAGGGGG");
			if (property.propertyType == SerializedPropertyType.String)
				property.stringValue = EditorGUI.TagField(position, property.stringValue);
			else
				EditorGUI.HelpBox(position, "Not a string.", MessageType.Error);

			EditorGUI.EndProperty( );
		}
	}
}
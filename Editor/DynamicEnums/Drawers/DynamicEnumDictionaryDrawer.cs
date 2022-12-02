using CommonUtils.DynamicEnums.Dictionaries;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.DynamicEnums.Drawers {
	[CustomPropertyDrawer(typeof(DynamicEnumDictionary<>), true)]
	public class DynamicEnumDictionaryDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.PropertyField(position, property.FindPropertyRelative("innerList"), label);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var innerList = property.FindPropertyRelative("innerList");

			var isExpanded = innerList.isExpanded;
			if (!isExpanded) return EditorGUIUtility.singleLineHeight;

			var arraySize = innerList.arraySize;
			if (arraySize == 0) return EditorGUIUtility.singleLineHeight * 4;

			var lineCount = 3 + arraySize;
			return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount-1);
		}
	}
}
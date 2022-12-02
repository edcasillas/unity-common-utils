using CommonUtils.DynamicEnums.Dictionaries;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.DynamicEnums.Drawers {
	[CustomPropertyDrawer(typeof(DynamicEnumKeyValuePair<>), true)]
	public class DynamicEnumKeyValuePairDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			// Key takes 1/3 of space; Value takes 2/3; theres a 5 pixel separation between them.
			var keyRect = new Rect(position.x, position.y, position.width / 3, position.height);
			var valueRect = new Rect(position.x + (position.width / 3)+5, position.y, ((position.width / 3) * 2)-5, position.height);

			// Draw fields - pass GUIContent.none to each so they are drawn without labels
			DynamicEnumDrawer.DrawDynamicEnumField(
				keyRect,
				property.FindPropertyRelative("key"),
				GUIContent.none,
				property.FindPropertyRelative("enumName").stringValue);
			EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer(typeof(DynamicEnumDictionary), true)]
	public class DynamicEnumDictionaryDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.PropertyField(position, property.FindPropertyRelative("kvps"), label);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			bool isExpanded = property.FindPropertyRelative("kvps").isExpanded;
			if (!isExpanded) return EditorGUIUtility.singleLineHeight;

			int lineCount = 3 + property.FindPropertyRelative("kvps").arraySize;
			return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount-1);
		}
	}
}

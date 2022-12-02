using System.Linq;
using CommonUtils.DynamicEnums;
using CommonUtils.Editor.DynamicEnums.Drawers;
using CommonUtils.Inspector.ReorderableInspector;
using SubjectNerd.Utilities;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector.ReorderableInspector {
	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	public class ReorderableDrawer : PropertyDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var i = getIndexInArray(property);
			if (i < 0) return EditorGUIUtility.singleLineHeight;

			var parent = SerializedPropExtension.GetParentProp(property);
			if(i >= parent.arraySize) Debug.LogError("HERE!!!");

			try {
				return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
			} catch {
				Debug.LogError("Exception");
				return 0;
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var indexInArray = getIndexInArray(property);
			if (indexInArray < 0) {
				EditorGUI.HelpBox(position, $"{property.name} is not a list. Remove ReorderableAttribute.", MessageType.Error);
				return;
			}

			EditorGUI.BeginProperty(position, GUIContent.none, property);
			var buttonWidth = EditorGUIUtility.singleLineHeight;

			var parent = SerializedPropExtension.GetParentProp(property);

			position.width -= buttonWidth * 3;

			Rect rect = position;
			var targetElement = property;
			bool isExpanded = targetElement.isExpanded;
			if (isExpanded) rect.width += buttonWidth * 3;
			rect.height = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, isExpanded);
			if (targetElement.hasVisibleChildren)
				rect.xMin += 10;
			var propHeader = new GUIContent(targetElement.displayName);

			if (!tryDrawDynamicEnumField(targetElement, parent, rect, propHeader))
				EditorGUI.PropertyField(rect, targetElement, propHeader, isExpanded);

			var buttonRect = new Rect(position.x + position.width,
				position.y,
				buttonWidth,
				EditorGUIUtility.singleLineHeight);

			if (indexInArray > 0) {
				if (GUI.Button(buttonRect, new GUIContent("^", "Move up"), EditorStyles.miniButtonRight)) {
					parent.MoveArrayElement(indexInArray, indexInArray - 1);
					parent.serializedObject.ApplyModifiedProperties();
				}
			}

			buttonRect.x += buttonWidth;
			if (indexInArray < parent.arraySize - 1) {
				if (GUI.Button(buttonRect, new GUIContent("v", "Move down"), EditorStyles.miniButtonRight)) {
					parent.MoveArrayElement(indexInArray, indexInArray + 1);
					parent.serializedObject.ApplyModifiedProperties();
				}
			}

			buttonRect.x += buttonWidth;
			if (indexInArray < parent.arraySize - 1) {
				// Because removing the last item throws 2 exceptions in the console.
				if (GUI.Button(buttonRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight)) {
					property.DeleteCommand();
					parent.serializedObject.ApplyModifiedProperties();
				}
			}

			EditorGUI.EndProperty();
		}

		private static bool tryDrawDynamicEnumField(SerializedProperty property, SerializedProperty parent, Rect rect, GUIContent propHeader) {
			if ((property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.String)) {
				var parentParent = SerializedPropExtension.GetParentProp(parent);
				if (parentParent.GetAttributes<DynamicEnumAttribute>().FirstOrDefault() is DynamicEnumAttribute dynamicEnumAttribute) {
					DynamicEnumDrawer.DrawDynamicEnumField(rect, property, propHeader, dynamicEnumAttribute.EnumName);
					return true;
				}
			}

			return false;
		}

		private static int getIndexInArray(SerializedProperty property) {
			// TODO Handle ArgumentOutOfRangeException when this is not a collection.
			if (property == null || string.IsNullOrEmpty(property.propertyPath)) return -1;
			var lastIndexOfBracket = property.propertyPath.LastIndexOf("[");
			if (lastIndexOfBracket < 0) return -1;
			var indexInArrayAsString = property.propertyPath.Substring(lastIndexOfBracket)
				.Replace("[", "")
				.Replace("]", "");
			var indexInArray = int.Parse(indexInArrayAsString);
			return indexInArray;
		}
	}
}

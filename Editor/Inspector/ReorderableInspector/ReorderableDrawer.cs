using SubjectNerd.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CommonUtils.Editor.Inspector.ReorderableInspector {
	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	public class ReorderableDrawer : PropertyDrawer {
		private ReorderableList list;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			//initialize(property);
			//return list.GetHeight();
			return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
		}

		public static float ElementHeightCallback(SerializedProperty property, int index) {
			SerializedProperty arrayElement = property.GetArrayElementAtIndex(index);
			float calculatedHeight = EditorGUI.GetPropertyHeight(arrayElement, GUIContent.none, arrayElement.isExpanded);
			calculatedHeight += 3;
			return calculatedHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var buttonWidth = EditorGUIUtility.singleLineHeight;
			//initialize(property);
			//list.DoList(position);
			//EditorGUI.LabelField(position, "hola?");

			#region GetPropertyIndex
			var indexInArrayAsString = property.propertyPath.Substring(
				property.propertyPath.LastIndexOf("[")).Replace("[","").Replace("]","");
			var indexInArray = int.Parse(indexInArrayAsString);
			#endregion
			var parent = SerializedPropExtension.GetParentProp(property);

			position.width -= buttonWidth * 3;

			//EditorGUI.PropertyField(position, property);

			Rect rect = position;
			var targetElement = property;
			bool isExpanded = targetElement.isExpanded;
			if (isExpanded) rect.width += buttonWidth * 3;
			rect.height = EditorGUI.GetPropertyHeight(targetElement, GUIContent.none, isExpanded);
			if (targetElement.hasVisibleChildren)
				rect.xMin += 10;
			var propHeader = new GUIContent(targetElement.displayName);
			EditorGUI.PropertyField(rect, targetElement, propHeader, isExpanded);

			var buttonRect = new Rect(
				position.x + position.width,
				position.y,
				buttonWidth,
				EditorGUIUtility.singleLineHeight
			);

			if (indexInArray > 0) {
				if (GUI.Button(buttonRect, new GUIContent("^", "Move up"), EditorStyles.miniButtonRight)) {
					parent.MoveArrayElement(indexInArray, indexInArray - 1);
				}
			}

			buttonRect.x += buttonWidth;
			if (indexInArray < parent.arraySize - 1) {
				if (GUI.Button(buttonRect, new GUIContent("v", "Move down"), EditorStyles.miniButtonRight)) {
					parent.MoveArrayElement(indexInArray, indexInArray + 1);
				}
			}

			buttonRect.x += buttonWidth;
			if (GUI.Button(buttonRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight)) {
				parent.DeleteArrayElementAtIndex(indexInArray);
			}
		}

		private void initialize(SerializedProperty property) {
			if (list != null) return;

			//var target = SerializeableCollectionsPropertyHelper.GetParent(property);

			//property.ParentPath()
			//fieldInfo.GetValue(target);

			var parent = SerializedPropExtension.GetParentProp(property);
			//var grandParent = SerializedPropExtension.GetParentProp(parent);

			//Debug.Log($"{parent.name} - {property.name}");
			//Debug.Log($"{property.propertyPath}");

			list= new ReorderableList(parent.serializedObject, parent, true, false, true, true);
		}
	}
}

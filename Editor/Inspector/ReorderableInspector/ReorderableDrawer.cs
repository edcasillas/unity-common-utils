using SubjectNerd.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CommonUtils.Editor.Inspector.ReorderableInspector {
	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	public class ReorderableDrawer : PropertyDrawer {
		private ReorderableList list;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			initialize(property);
			return list.GetHeight();
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			initialize(property);
			//list.DoList(position);
			EditorGUI.LabelField(position, "hola?");
		}

		private void initialize(SerializedProperty property) {
			if (list != null) return;

			//var target = SerializeableCollectionsPropertyHelper.GetParent(property);

			//property.ParentPath()
			//fieldInfo.GetValue(target);

			var parent = SerializedPropExtension.GetParentProp(property);
			//var grandParent = SerializedPropExtension.GetParentProp(parent);


			list= new ReorderableList(parent.serializedObject, parent, true, false, true, true);
		}
	}
}

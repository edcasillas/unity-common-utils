using CommonUtils.DynamicEnums;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.DynamicEnums {
	[CustomEditor(typeof(DynamicEnumDefinitions))]
	public class DynamicEnumDefinitionsEditor : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			EditorGUILayout.HelpBox("Please make sure to press the Reload button when you're finished editing the Dynamic Enums.", MessageType.Info);
			if (GUILayout.Button("Reload")) {
				if (DynamicEnumManager.Reload()) {
					EditorUtility.DisplayDialog("Dynamic Enums", "Dynamic Enums have been reloaded.", "Ok");
				}
			}

			DrawDefaultInspector();
		}
	}
}
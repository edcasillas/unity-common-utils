using CommonUtils;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	[CustomEditor(typeof(GameProperties))]
	public class GamePropertiesEditor : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			EditorGUILayout.HelpBox("Game properties stored as key-value pairs.", MessageType.Info);
			
			if (GUILayout.Button("Reload")) {
				((GameProperties)target).Reload();
				EditorUtility.DisplayDialog("Game Properties", "Properties have been reloaded.", "Ok");
			}

			DrawDefaultInspector();
		}
	}
}
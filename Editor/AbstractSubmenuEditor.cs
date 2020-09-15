using CommonUtils.UI.Submenus;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	[CustomEditor(typeof(AbstractSubmenu), true)]
	public class AbstractSubmenuEditor : UnityEditor.Editor {
		private AbstractSubmenu submenu;

		private void OnEnable() => submenu = (AbstractSubmenu)target;

		public override void OnInspectorGUI() {
			if (Application.isPlaying) {
				EditorExtensions.BoxGroup(() => {
					EditorGUILayout.Toggle("Is Initialized", submenu.IsInitialized);
					EditorGUILayout.Toggle("Is Shown", submenu.IsShown);
					EditorGUILayout.Vector2Field("Hidden Value", submenu.HiddenValue);
					EditorGUILayout.Vector2Field("Shown Value", submenu.ShownValue);
				});

				EditorExtensions.BoxGroup(() => {
					if(GUILayout.Button("Init")) submenu.Init();
					if(GUILayout.Button("Show")) submenu.Show();
					if(GUILayout.Button("Hide")) submenu.Hide();
				});
			}

			DrawDefaultInspector();
		}
	}
}
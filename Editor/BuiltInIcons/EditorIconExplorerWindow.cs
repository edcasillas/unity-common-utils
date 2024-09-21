using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.BuiltInIcons
{
    public class EditorIconExplorerWindow : EditorWindow
    {
        #region Statics (To create the editor menu)
		private static EditorIconExplorerWindow instance;

		[MenuItem("Tools/Built-in Icon Explorer...")]
		private static void openActiveWindow() {
			if (!instance) {
				instance = GetWindow<EditorIconExplorerWindow>();
				instance.titleContent = new GUIContent("Built-in Icon Explorer");
				instance.maxSize = new Vector2(400f, 300f);
			}

			instance.Show();
		}
		#endregion

		private EditorIcon selectedIcon = EditorIcon.animationvisibilitytoggleoff;

		private void OnGUI() {
			selectedIcon = (EditorIcon)EditorGUILayout.EnumPopup("Icon", selectedIcon);
			EditorGUILayout.LabelField("Value", selectedIcon.GetIconName());
			selectedIcon.Draw(128,128);
		}
	}
}

using CommonUtils.Editor.BuiltInIcons;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.ScreenshotManager {
	public class ScreenshotManagerConfigWindow : EditorWindow {
		private static ScreenshotManagerConfigWindow instance;

		private string saveTo;
		private string prefix;
		private int currentCount;

		[MenuItem("Tools/Configure screenshots...")]
		private static void openConfigWindow() => openActiveWindow();

		private static void openActiveWindow() {
			if (!instance) {
				instance              = GetWindow<ScreenshotManagerConfigWindow>();
				instance.titleContent = new GUIContent("Screenshot Manager");
				instance.maxSize      = new Vector2(400f, 200f);
			}
			instance.saveTo = ScreenshotManager.SaveToFolder;
			instance.prefix = ScreenshotManager.FilePrefix;
			instance.currentCount = ScreenshotManager.CurrentCount;
			instance.Show();
		}

		private void OnEnable() {
			if(!instance) return;
			instance.saveTo = ScreenshotManager.SaveToFolder;
			instance.prefix = ScreenshotManager.FilePrefix;
			instance.currentCount = ScreenshotManager.CurrentCount;
			instance.Show();
		}

		private void OnGUI() {
			saveTo = EditorExtensions.FolderField("Save to", saveTo);

			prefix = EditorGUILayout.TextField("Prefix", prefix)?.Trim();

			EditorGUILayout.BeginHorizontal();
			EditorExtensions.ReadOnlyLabelField("Current count", currentCount);
			if (GUILayout.Button("Reset", EditorStyles.miniButtonRight)) {
				currentCount = 0;
			}
			EditorGUILayout.EndHorizontal();

			if (!Directory.Exists(saveTo)) {
				EditorGUILayout.HelpBox("The selected folder does not exist. Screenshots cannot be taken!", MessageType.Error);
			} else {
				EditorGUILayout.HelpBox($"Next screenshot will be saved as:\n{saveTo}/{prefix}{currentCount+1}.png", MessageType.None);
				if ((saveTo       != ScreenshotManager.SaveToFolder || prefix != ScreenshotManager.FilePrefix ||
					 currentCount != ScreenshotManager.CurrentCount) && GUILayout.Button("Save")) {
					ScreenshotManager.SaveToFolder.Value = saveTo;
					ScreenshotManager.FilePrefix.Value = prefix;
					ScreenshotManager.CurrentCount.Value = currentCount;
					if(instance) instance.Close();
				}
			}
		}
	}
}
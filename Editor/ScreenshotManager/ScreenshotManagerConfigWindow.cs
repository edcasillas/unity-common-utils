using System;
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
				instance.maxSize      = new Vector2(400f, 300f);
			}
			instance.Show();
		}

		private void OnEnable() {
			if(!instance) return;
			instance.saveTo = ScreenshotManager.SaveToFolder;
			instance.prefix = ScreenshotManager.FilePrefix;
			instance.currentCount = ScreenshotManager.CurrentCount;
		}

		private void OnGUI() {
			EditorExtensions.ReadOnlyLabelField("Save to", saveTo);
			if (GUILayout.Button("Change folder...")) {
				var selectedFolder =EditorUtility.SaveFolderPanel("Choose folder to save screenshots to", saveTo, "Screenshots");
				if (!string.IsNullOrWhiteSpace(selectedFolder)) saveTo = selectedFolder;
			}

			prefix = EditorGUILayout.TextField("Prefix", prefix)?.Trim();

			EditorExtensions.ReadOnlyLabelField("Current count", currentCount);
			if (GUILayout.Button("Reset count")) {
				currentCount = 0;
			}

			if (!Directory.Exists(saveTo)) {
				EditorGUILayout.HelpBox($"The selected folder does not exist. Screenshots cannot be taken!", MessageType.Error);
			} else {
				EditorGUILayout.HelpBox($"Next screenshot will be saved as:\n{saveTo}/{prefix}{currentCount}.png", MessageType.None);
				if ((saveTo       != ScreenshotManager.SaveToFolder || prefix != ScreenshotManager.FilePrefix ||
					 currentCount != ScreenshotManager.CurrentCount) && GUILayout.Button("Save")) {
					ScreenshotManager.SaveToFolder = saveTo;
					ScreenshotManager.FilePrefix = prefix;
					ScreenshotManager.CurrentCount = currentCount;
					if(instance) instance.Close();
				}
			}
		}
	}
}
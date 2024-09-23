using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.ScreenshotManager {
    public static class ScreenshotManager {
        #region Constants
        public const string EDITOR_PREF_KEY_SAVE_TO       = "ScreenshotManager.SaveTo";
        public const string EDITOR_PREF_KEY_PREFIX        = "ScreenshotManager.Prefix";
        public const string EDITOR_PREF_KEY_CURRENT_COUNT = "ScreenshotManager.CurrentCount";
        internal const string DEFAULT_SAVE_DIRECTORY        = "Assets/Screenshots";
        #endregion

        #region EditorPrefs
		internal static readonly EditorPrefsString SaveToFolder = new(EDITOR_PREF_KEY_SAVE_TO, DEFAULT_SAVE_DIRECTORY, true);
        internal static readonly EditorPrefsString FilePrefix = new(EDITOR_PREF_KEY_PREFIX, "screenshot", true);
		internal static readonly EditorPrefsInt CurrentCount = new(EDITOR_PREF_KEY_CURRENT_COUNT, 0, true);
        #endregion

        [MenuItem("Tools/Take Screenshot _F10")]
        private static void takeScreenshot() {
            var saveTo = SaveToFolder;
            if (!Directory.Exists(saveTo)) {
                Debug.LogError($"Could not save screenshot because the folder '{saveTo}' does not exist. Create this folder or change it in the menu Tools/Configure screenshots...");
                return;
            }

            int count = CurrentCount;
            var filename = $"{SaveToFolder}/{FilePrefix}{++count}.png";
            try {
                ScreenCapture.CaptureScreenshot(filename);
                Debug.Log($"Screenshot saved at \"{filename}\"");
                AssetDatabase.SaveAssets();
                CurrentCount.Value = count;
            } catch(Exception e) {
                Debug.LogError($"Could not save screenshot at {filename}: {e.Message}");
            }
        }
	}
}

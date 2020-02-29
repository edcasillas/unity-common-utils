using System;
using System.IO;
using CommonUtils.Extensions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.ScreenshotManager {
    public static class ScreenshotManager {
        #region Constants
        public const string EDITOR_PREF_KEY_SAVE_TO       = "ScreenshotManager.SaveTo";
        public const string EDITOR_PREF_KEY_PREFIX        = "ScreenshotManager.Prefix";
        public const string EDITOR_PREF_KEY_CURRENT_COUNT = "ScreenshotManager.CurrentCount";
        private const string defaultSaveDirectory        = "Assets/Screenshots";
        #endregion

        #region Properties (connected to EditorPrefs)
        internal static string SaveToFolder {
            get => EditorPrefs.GetString(EDITOR_PREF_KEY_SAVE_TO, defaultSaveDirectory);
            set => EditorPrefs.SetString(EDITOR_PREF_KEY_SAVE_TO, value);
        }
        
        internal static string FilePrefix {
            get => EditorPrefs.GetString(EDITOR_PREF_KEY_PREFIX, "screenshot");
            set => EditorPrefs.SetString(EDITOR_PREF_KEY_PREFIX, value);
        }
        
        internal static int CurrentCount {
            get => EditorPrefs.GetInt(EDITOR_PREF_KEY_CURRENT_COUNT, 0);
            set => EditorPrefs.SetInt(EDITOR_PREF_KEY_CURRENT_COUNT, value);
        }
        #endregion

        [MenuItem("Tools/Take Screenshot _F10")]
        private static void takeScreenshot() {
            var saveTo = SaveToFolder;
            if (!Directory.Exists(saveTo)) {
                Debug.LogError($"Could not save screenshot because the folder '{saveTo}' does not exist. Create this folder or change it in the menu Tools/Configure screenshots...");
                return;
            }

            var count = CurrentCount;
            var filename = $"{SaveToFolder}/{FilePrefix}{++count}.png"; 
            try {
                ScreenCapture.CaptureScreenshot(filename);
                Debug.Log($"Screenshot saved at \"{filename}\"");
                AssetDatabase.SaveAssets();
                CurrentCount = count;
            } catch(Exception e) {
                Debug.LogError($"Could not save screenshot at {filename}: {e.Message}");
            }
        }

        [MenuItem("Tools/Configure screenshots...")]
        private static void openConfigWindow() {
            ScreenshotManagerConfigWindow.OpenActiveWindow();
        }
    }
}

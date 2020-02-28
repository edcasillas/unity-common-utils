using System;
using System.IO;
using CommonUtils.Extensions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.ScreenshotManager {
    public static class ScreenshotManager {
        private const string defaultConfigDirectory = "Assets/ScreenshotManager/";
        private const string configFileName         = "ScreenshotManagerConfig.asset";
    
        [MenuItem("Tools/Take Screenshot #1")]
        private static void takeScreenshot() {
            var config          = AssetDatabase.LoadAssetAtPath<ScreenshotManagerConfig>($"{defaultConfigDirectory}{configFileName}");
            if (!config) config = createConfigAsset();

            var filename = $"{config.SaveToFolder}/{config.FilePrefix}{++config.CurrentCount}.png"; 
            try {
                ScreenCapture.CaptureScreenshot(filename);
                Debug.Log($"Screenshot saved at \"{filename}\"");
                AssetDatabase.SaveAssets();
            } catch(Exception e) {
                Debug.LogError($"Could not save screenshot at {filename}: {e.Message}");
            }
        
        }

        private static ScreenshotManagerConfig createConfigAsset() {
            var config = ScriptableObject.CreateInstance<ScreenshotManagerConfig>();

            config.SaveToFolder = EditorUtility.SaveFolderPanel("Choose folder to save screenshots", "", "Screenshots");
            if (config.SaveToFolder.IsNullOrEmpty()) return config;

            if (!Directory.Exists(defaultConfigDirectory)) {
                Directory.CreateDirectory(defaultConfigDirectory);
            }

            AssetDatabase.CreateAsset(config, $"{defaultConfigDirectory}{configFileName}");
            return config;
        }
    }
}

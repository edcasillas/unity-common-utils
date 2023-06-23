using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Android {
    public class MainActivityChecker : EditorWindow {
        private string filePath = string.Empty;
        private string mainActivityName = string.Empty;

        private const float verticalSpacing = 5f;

        private bool isFileSelected = false;

        [MenuItem("Tools/Android/Check main activity in AndroidManifest")]
        private static void OpenWindow() {
            var window = GetWindow<MainActivityChecker>();
            window.titleContent = new GUIContent("Check Main Activity");
            window.minSize = new Vector2(400f, EditorGUIUtility.singleLineHeight * 8 + verticalSpacing * 8);
            window.Show();
        }

        private void OnGUI() {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Select AndroidManifest.xml or APK", EditorStyles.boldLabel);
            filePath = EditorGUILayout.TextField("Path", filePath);

            if (GUILayout.Button("Browse")) {
                filePath = EditorUtility.OpenFilePanel("Select AndroidManifest.xml or APK", "", "xml,apk");
                isFileSelected = !string.IsNullOrEmpty(filePath);
            }

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isFileSelected);
            if (GUILayout.Button("Get Main Activity Name")) {
                if (File.Exists(filePath)) {
					if (filePath.EndsWith("xml")) {
						mainActivityName = AndroidManifestParser.GetMainActivityNameFromManifest(filePath);
					} else if (filePath.EndsWith("apk")) {
						mainActivityName = AndroidManifestParser.GetMainActivityNameFromApk(filePath);
					} else {
						EditorUtility.DisplayDialog("Check Main Activity", "Please select an AndroidManifest.xml or APK file.", "Ok");
						return;
					}
					if (string.IsNullOrEmpty(mainActivityName)) {
						EditorUtility.DisplayDialog("Check Main Activity", "Main activity not found!", "Ok");
					}
				} else {
                    mainActivityName = "AndroidManifest.xml not found";
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(mainActivityName));
            EditorGUILayout.LabelField("Main Activity Name", EditorStyles.boldLabel);
            EditorGUILayout.TextField(mainActivityName);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(mainActivityName));
            if (GUILayout.Button("Clear")) {
                mainActivityName = string.Empty;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

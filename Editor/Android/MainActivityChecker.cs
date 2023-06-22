using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Android {
    public class MainActivityChecker : EditorWindow {
        private string manifestPath = string.Empty;
        private string mainActivityName = string.Empty;

        private const float VerticalSpacing = 5f;

        private bool isFileSelected = false;

        [MenuItem("Tools/Android/Check main activity in AndroidManifest")]
        private static void OpenWindow() {
            MainActivityChecker window = GetWindow<MainActivityChecker>();
            window.titleContent = new GUIContent("Check Main Activity");
            window.minSize = new Vector2(400f, EditorGUIUtility.singleLineHeight * 8 + VerticalSpacing * 8);
            window.Show();
        }

        private void OnGUI() {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Select AndroidManifest.xml", EditorStyles.boldLabel);
            manifestPath = EditorGUILayout.TextField("Manifest Path", manifestPath);

            if (GUILayout.Button("Browse")) {
                manifestPath = EditorUtility.OpenFilePanel("Select AndroidManifest.xml", "", "xml");
                isFileSelected = !string.IsNullOrEmpty(manifestPath);
            }

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!isFileSelected);
            if (GUILayout.Button("Get Main Activity Name")) {
                if (File.Exists(manifestPath)) {
                    mainActivityName = AndroidManifestParser.GetMainActivityName(manifestPath);
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

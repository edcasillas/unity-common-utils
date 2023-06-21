using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Android {
	public class MainActivityChecker : EditorWindow {
		private string manifestPath = string.Empty;
		private string mainActivityName = string.Empty;

		[MenuItem("Tools/Android/Check main activity in AndroidManifest")]
		private static void OpenWindow() {
			MainActivityChecker window = EditorWindow.GetWindow<MainActivityChecker>();
			window.titleContent = new GUIContent("Check Main Activity");
			window.Show();
		}

		private void OnGUI() {
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Select AndroidManifest.xml", EditorStyles.boldLabel);
			manifestPath = EditorGUILayout.TextField("Manifest Path", manifestPath);

			if (GUILayout.Button("Browse")) {
				manifestPath = EditorUtility.OpenFilePanel("Select AndroidManifest.xml", "", "xml");
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Get Main Activity Name")) {
				if (File.Exists(manifestPath)) {
					Debug.Log($"Analyzing '{manifestPath}'");
					mainActivityName = AndroidManifestParser.GetMainActivityName(manifestPath);
					if (string.IsNullOrEmpty(mainActivityName)) {
						EditorUtility.DisplayDialog("Check Main Activity", "Main activity not found!", "Ok");
					}
				} else {
					mainActivityName = "AndroidManifest.xml not found";
				}
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Main Activity Name", EditorStyles.boldLabel);
			EditorGUILayout.TextField(mainActivityName);

			EditorGUILayout.Space();
		}
	}
}
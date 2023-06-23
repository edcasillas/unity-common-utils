using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public class APKManifestExtractor : EditorWindow {
	private string apkPath = string.Empty;
	private string manifestContent = string.Empty;

	[MenuItem("Tools/Android/Extract APK Manifest")]
	private static void OpenWindow() {
		APKManifestExtractor window = GetWindow<APKManifestExtractor>();
		window.titleContent = new GUIContent("APK Manifest Extractor");
		window.minSize = new Vector2(400f, 200f);
		window.Show();
	}

	private void OnGUI() {
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("APK Path", EditorStyles.boldLabel);
		apkPath = EditorGUILayout.TextField(apkPath);

		EditorGUILayout.Space();

		if (GUILayout.Button("Select APK")) {
			apkPath = EditorUtility.OpenFilePanel("Select APK", "", "apk");
		}

		EditorGUILayout.Space();

		EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(apkPath));
		if (GUILayout.Button("Extract Manifest")) {
			ExtractManifest();
		}

		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Manifest Content", EditorStyles.boldLabel);
		EditorGUILayout.TextArea(manifestContent);
	}

	private void ExtractManifest() {
		if (File.Exists(apkPath)) {
			using (ZipArchive archive = ZipFile.OpenRead(apkPath)) {
				ZipArchiveEntry manifestEntry = archive.GetEntry("AndroidManifest.xml");

				if (manifestEntry != null) {
					Debug.Log($"Found manifest: {manifestEntry.FullName}");
					using (StreamReader reader = new StreamReader(manifestEntry.Open())) {
						manifestContent = reader.ReadToEnd();
					}
				} else {
					manifestContent = "Manifest not found in APK.";
				}
			}
		} else {
			manifestContent = "APK not found.";
		}
	}
}

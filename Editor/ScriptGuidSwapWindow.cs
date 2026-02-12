using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	public class ScriptGuidSwapWindow : EditorWindow {
		private MonoScript legacyScript;
		private MonoScript newScript;
		private string statusMessage = "";

		[MenuItem(MenuItems.MenuPathPrefix + "Script GUID Swap...", priority = 8)]
		public static void ShowWindow() => GetWindow<ScriptGuidSwapWindow>("Script GUID Swap");

		private void OnGUI() {
			EditorGUILayout.LabelField("Script GUID Swap", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Swaps GUIDs in .meta files. All references to legacy script will point to new script.", MessageType.Info);
			EditorGUILayout.Space();

			legacyScript = (MonoScript)EditorGUILayout.ObjectField("Legacy Script", legacyScript, typeof(MonoScript), false);
			newScript = (MonoScript)EditorGUILayout.ObjectField("New Script", newScript, typeof(MonoScript), false);

			EditorGUILayout.Space();

			GUI.enabled = legacyScript != null && newScript != null;
			if (GUILayout.Button("Swap GUIDs", GUILayout.Height(30)))
				swapGuids();
			GUI.enabled = true;

			EditorGUILayout.Space();
			EditorGUILayout.TextArea(statusMessage, GUILayout.ExpandHeight(true));
		}

		private void swapGuids() {
			if (!EditorUtility.DisplayDialog("Swap Script GUIDs",
				$"Swap GUIDs between '{legacyScript.name}' and '{newScript.name}'?\n\nAll references to legacy script will point to new script.",
				"Yes", "Cancel"))
				return;

			var legacyPath = AssetDatabase.GetAssetPath(legacyScript);
			var newPath = AssetDatabase.GetAssetPath(newScript);
			var legacyMetaPath = legacyPath + ".meta";
			var newMetaPath = newPath + ".meta";

			if (!File.Exists(legacyMetaPath) || !File.Exists(newMetaPath)) {
				statusMessage = "Error: Meta files not found.";
				return;
			}

			var legacyGuid = AssetDatabase.AssetPathToGUID(legacyPath);
			var newGuid = AssetDatabase.AssetPathToGUID(newPath);
			var randomGuid = Guid.NewGuid().ToString("N");

			var legacyMeta = File.ReadAllText(legacyMetaPath);
			var newMeta = File.ReadAllText(newMetaPath);

			legacyMeta = legacyMeta.Replace($"guid: {legacyGuid}", $"guid: {randomGuid}");
			newMeta = newMeta.Replace($"guid: {newGuid}", $"guid: {legacyGuid}");

			File.WriteAllText(legacyMetaPath, legacyMeta);
			File.WriteAllText(newMetaPath, newMeta);

			AssetDatabase.ImportAsset(legacyPath, ImportAssetOptions.ForceUpdate);
			AssetDatabase.ImportAsset(newPath, ImportAssetOptions.ForceUpdate);
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

			statusMessage = $"Success!\n\nGUID changes:\n{legacyScript.name}: {legacyGuid} → {randomGuid}\n{newScript.name}: {newGuid} → {legacyGuid}\n\nAll references now point to new script.";
		}
	}
}

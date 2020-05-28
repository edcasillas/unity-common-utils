using System;
using System.IO;
using CommonUtils.UI.PromoBadges;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	[CustomEditor(typeof(AppDataCollection))]
	public class PromoBadgeAppDataCollectionEditor : UnityEditor.Editor {
		private AppDataCollection appDataCollection;

		private void OnEnable() => appDataCollection = (AppDataCollection)target;

		public override void OnInspectorGUI() {
			if (GUILayout.Button("Save JSON")) {
				var path = EditorUtility.SaveFilePanel("Save PromoBadge app data collection", "", "promoBadgeData.json", "json");
				if (!string.IsNullOrWhiteSpace(path)) {
					var jsonContents = JsonUtility.ToJson(appDataCollection);
					try {
						File.WriteAllText(path, jsonContents);
						Debug.Log($"PromoBadge app data collection file was saved at {path}");
					} catch (Exception ex) {
						Debug.LogError($"Could not save the PromoBadge app data collection file: {ex.Message}");
					}
				} else {
					Debug.Log("Save cancelled");
				}
			}

			DrawDefaultInspector();
		}
	}
}
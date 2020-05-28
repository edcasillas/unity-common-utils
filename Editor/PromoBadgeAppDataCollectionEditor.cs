using System;
using System.IO;
using CommonUtils.UI.PromoBadges;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	[CustomEditor(typeof(AppDataCollectionConfigurator))]
	public class PromoBadgeAppDataCollectionEditor : UnityEditor.Editor {
		private SerializedProperty appList;

		private void OnEnable() => appList = serializedObject.FindProperty(nameof(AppDataCollectionConfigurator.AppData)).FindPropertyRelative(nameof(AppDataCollection.Apps));

		public override void OnInspectorGUI() {
			if (GUILayout.Button("Save JSON")) {
				saveJson();
			}

			EditorGUILayout.PropertyField(appList);
			serializedObject.ApplyModifiedProperties();
		}

		private void saveJson() {
			var path = EditorUtility.SaveFilePanel("Save PromoBadge app data collection", "", "promoBadgeData.json", "json");
			if (!string.IsNullOrWhiteSpace(path)) {
				var appDataCollection = (AppDataCollectionConfigurator)target;
				var jsonContents = JsonUtility.ToJson(appDataCollection.AppData);
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
	}
}
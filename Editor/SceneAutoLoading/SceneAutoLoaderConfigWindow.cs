using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.SceneAutoLoading {
	public class SceneAutoLoaderConfigWindow : EditorWindow {
		private static SceneAutoLoaderConfigWindow instance = null;
		private SceneAsset selectedScene;
		private bool loadMasterOnPlay;
		private bool autoSaveOnPlay;

		[MenuItem("Tools/Configure Scene Autoloader...")]
		private static void OpenActiveWindow() {
			if (!instance) {
				instance              = GetWindow<SceneAutoLoaderConfigWindow>();
				instance.titleContent = new GUIContent("SceneAutoLoader");
				instance.maxSize      = new Vector2(325f, 200f);
			}

			if (!string.IsNullOrWhiteSpace(SceneAutoLoader.MasterScene)) {
				instance.selectedScene = BuildUtils.GetSceneAssetFromPath(SceneAutoLoader.MasterScene);
				instance.loadMasterOnPlay = SceneAutoLoader.LoadMasterOnPlay;
				instance.autoSaveOnPlay = SceneAutoLoader.AutoSaveOnPlay;
			} else {
				instance.loadMasterOnPlay = false;
				instance.autoSaveOnPlay = false;
			}

			instance.Show();
		}

		private void OnGUI() {
			EditorGUILayout.HelpBox("The scene you select below will be automatically played when you hit the Play button on the Editor.", MessageType.Info);

			selectedScene = (SceneAsset) EditorGUILayout.ObjectField("Master scene", selectedScene, typeof(SceneAsset), false);
			if (GUILayout.Button("Select first active scene in build.")) {
				if (EditorBuildSettings.scenes.Any(s=>s.enabled)) {
					var buildSettingsScene = EditorBuildSettings.scenes.First(s => s.enabled);
					selectedScene = BuildUtils.GetSceneAssetFromPath(buildSettingsScene.path);
				} else {
					if(EditorUtility.DisplayDialog("Scene Autoloader", "There are no scenes in the build settings.", "Open Build Settings", "Cancel")) {
						BuildUtils.OpenBuildSettings();
					}
				}
			}

			loadMasterOnPlay = selectedScene && EditorGUILayout.Toggle("Load this scene on Play", loadMasterOnPlay);

			if (selectedScene && !loadMasterOnPlay) {
				EditorGUILayout.HelpBox("Your master scene won't be loaded on Play. Check the box above to fix this.", MessageType.Warning);
			}

			autoSaveOnPlay = loadMasterOnPlay && EditorGUILayout.Toggle("Auto Save on Play", autoSaveOnPlay);

			string selectedScenePath = null;
			if (selectedScene) {
				selectedScenePath = BuildUtils.GetScenePath(selectedScene);
			}

			GUI.enabled =
				selectedScenePath != SceneAutoLoader.MasterScene      ||
				loadMasterOnPlay  != SceneAutoLoader.LoadMasterOnPlay ||
				autoSaveOnPlay    != SceneAutoLoader.AutoSaveOnPlay;

			if (GUILayout.Button("Save changes")) {
				SceneAutoLoader.MasterScene = selectedScenePath;
				SceneAutoLoader.LoadMasterOnPlay = loadMasterOnPlay;
				SceneAutoLoader.AutoSaveOnPlay = autoSaveOnPlay;
				instance.Close();
				return;
			}

			GUI.enabled = true;
		}
	}
}

using CommonUtils.Editor.Inspector.SceneRefs;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.SceneAutoLoading {
	public class InitialSceneSelectorWindow : EditorWindow {
		private static InitialSceneSelectorWindow instance = null;
		private         SceneAsset                     selectedScene;
		private bool loadMasterOnPlay;

		[MenuItem("File/Scene Autoload/Configure...")]
		private static void OpenActiveWindow() {
			if (!instance) {
				instance              = GetWindow<InitialSceneSelectorWindow>();
				instance.titleContent = new GUIContent("SceneAutoLoader");
				instance.maxSize      = new Vector2(325f, 120f);
			}

			if (!string.IsNullOrWhiteSpace(SceneAutoLoader.MasterScene)) {
				instance.selectedScene = BuildUtils.GetSceneAssetFromPath(SceneAutoLoader.MasterScene);
				instance.loadMasterOnPlay = SceneAutoLoader.LoadMasterOnPlay;
			} else {
				instance.loadMasterOnPlay = false;
			}

			instance.Show();
		}

		private void OnGUI() {
			selectedScene = (SceneAsset) EditorGUILayout.ObjectField("Master scene", selectedScene, typeof(SceneAsset), false);
			loadMasterOnPlay = selectedScene && EditorGUILayout.Toggle("Load this scene on Play", loadMasterOnPlay);

			string selectedScenePath = null;
			if (selectedScene) {
				selectedScenePath = BuildUtils.GetScenePath(selectedScene);
			}
			
			GUI.enabled = selectedScenePath != SceneAutoLoader.MasterScene || loadMasterOnPlay != SceneAutoLoader.LoadMasterOnPlay;
			if (GUILayout.Button("Save changes")) {
				SceneAutoLoader.MasterScene = selectedScenePath;
				SceneAutoLoader.LoadMasterOnPlay = loadMasterOnPlay;
				instance.Close();
				return;
			}

			GUI.enabled = true;
		}
	}
}

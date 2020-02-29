using CommonUtils.Editor.Inspector.SceneRefs;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CommonUtils.Editor.SceneAutoLoading {
	public class InitialSceneSelectorWindow : EditorWindow {
		#region Constants
		private const string cEditorPrefLoadMasterOnPlay = "SceneAutoLoader.LoadMasterOnPlay";
		private const string cEditorPrefMasterScene      = "SceneAutoLoader.MasterScene";
		private const string cEditorPrefPreviousScene    = "SceneAutoLoader.PreviousScene";
		#endregion

		private static InitialSceneSelectorWindow instance = null;
		private         SceneAsset                     selectedScene;
		private bool loadMasterOnPlay;

		#region Properties (connected to EditorPrefs)
		private static bool LoadMasterOnPlay {
			get => EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false);
			set => EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value);
		}

		private static string masterScenePath {
			get => EditorPrefs.GetString(cEditorPrefMasterScene, null);
			set => EditorPrefs.SetString(cEditorPrefMasterScene, value);
		}

		private static string PreviousScene {
			get => EditorPrefs.GetString(cEditorPrefPreviousScene, EditorSceneManager.GetActiveScene().path);
			set => EditorPrefs.SetString(cEditorPrefPreviousScene, value);
		}
		#endregion

		[MenuItem("File/Scene Autoload/test Window")]
		private static void OpenActiveWindow() {
			if (!instance) {
				instance              = GetWindow<InitialSceneSelectorWindow>();
				instance.titleContent = new GUIContent("Select master scene");
				instance.maxSize      = new Vector2(325f, 120f);
			}

			if (!string.IsNullOrWhiteSpace(masterScenePath)) {
				instance.selectedScene = BuildUtils.GetSceneAssetFromPath(masterScenePath);
				instance.loadMasterOnPlay = LoadMasterOnPlay;
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
			
			GUI.enabled = selectedScenePath != masterScenePath || loadMasterOnPlay != LoadMasterOnPlay;
			if (GUILayout.Button("Save changes")) {
				masterScenePath = selectedScenePath;
				LoadMasterOnPlay = loadMasterOnPlay;
				instance.Close();
				return;
			}

			GUI.enabled = true;
		}
	}
}

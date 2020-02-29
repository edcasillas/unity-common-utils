using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CommonUtils.Editor {
	/// <summary>
	/// Scene auto loader.
	/// </summary>
	/// <description>
	/// This class adds a File > Scene Autoload menu containing options to select
	/// a "master scene" enable it to be auto-loaded when the user presses play
	/// in the editor. When enabled, the selected scene will be loaded on play,
	/// then the original scene will be reloaded on stop.
	///
	/// Based on an idea on this thread:
	/// http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
	///
	/// Original credit: http://wiki.unity3d.com/index.php/SceneAutoLoader?_ga=2.138639863.517959549.1582943548-1919341696.1564424378
	/// Modified by: Ed Casillas
	/// </description>
	[InitializeOnLoad] // Makes sure this gets executed.
	public static class SceneAutoLoader {
		#region Constants
		private const string cEditorPrefLoadMasterOnPlay = "SceneAutoLoader.LoadMasterOnPlay";
		private const string cEditorPrefMasterScene      = "SceneAutoLoader.MasterScene";
		private const string cEditorPrefPreviousScene    = "SceneAutoLoader.PreviousScene";
		#endregion
	
		/// <summary>
		/// Static constructor binds a playmode-changed callback.
		/// </summary>
		static SceneAutoLoader() => EditorApplication.playModeStateChanged += OnPlayModeChanged;

		#region Properties (connected to EditorPrefs)
		public static bool LoadMasterOnPlay {
			get => EditorPrefs.GetBool(cEditorPrefLoadMasterOnPlay, false);
			set => EditorPrefs.SetBool(cEditorPrefLoadMasterOnPlay, value);
		}

		public static string MasterScene {
			get => EditorPrefs.GetString(cEditorPrefMasterScene);
			set => EditorPrefs.SetString(cEditorPrefMasterScene, value);
		}

		private static string PreviousScene {
			get => EditorPrefs.GetString(cEditorPrefPreviousScene, EditorSceneManager.GetActiveScene().path);
			set => EditorPrefs.SetString(cEditorPrefPreviousScene, value);
		}
		#endregion

		#region Menu items to select the "master" scene and control whether or not to load it.
		[MenuItem("File/Scene Autoload/Select Master Scene...")]
		private static void SelectMasterScene() {
			var masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
			masterScene = masterScene.Replace(Application.dataPath, "Assets"); //project relative instead of absolute path
			if (!string.IsNullOrEmpty(masterScene)) {
				MasterScene      = masterScene;
				LoadMasterOnPlay = true;
			}
		}

		[MenuItem("File/Scene Autoload/Load Master On Play", true)]
		private static bool ShowLoadMasterOnPlay() { return !LoadMasterOnPlay; }

		[MenuItem("File/Scene Autoload/Load Master On Play")]
		private static void EnableLoadMasterOnPlay() { LoadMasterOnPlay = true; }

		[MenuItem("File/Scene Autoload/Don't Load Master On Play", true)]
		private static bool ShowDontLoadMasterOnPlay() { return LoadMasterOnPlay; }

		[MenuItem("File/Scene Autoload/Don't Load Master On Play")]
		private static void DisableLoadMasterOnPlay() { LoadMasterOnPlay = false; }
		#endregion

		/// <summary>
		/// Play mode change callback handles the scene load/reload.
		/// </summary>
		private static void OnPlayModeChanged(PlayModeStateChange state) {
			if (!LoadMasterOnPlay) {
				return;
			}

			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
				// User pressed play -- autoload master scene.
				PreviousScene = EditorSceneManager.GetActiveScene().path;
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
					try {
						EditorSceneManager.OpenScene(MasterScene);
					} catch {
						Debug.LogError($"error: scene not found: {MasterScene}");
						EditorApplication.isPlaying = false;

					}
				} else {
					// User cancelled the save operation -- cancel play as well.
					EditorApplication.isPlaying = false;
				}
			}

			// isPlaying check required because cannot OpenScene while playing
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) {
				// User pressed stop -- reload previous scene.
				try {
					EditorSceneManager.OpenScene(PreviousScene);
				} catch {
					Debug.LogError($"error: scene not found: {PreviousScene}");
				}
			}
		}
	}
}
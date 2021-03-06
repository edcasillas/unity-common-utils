using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonUtils.Editor.SceneAutoLoading {
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
		private const string cEditorPrefAutoSaveOnPlay = "SceneAutoLoader.AutoSaveOnPlay";
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

		public static bool AutoSaveOnPlay {
			get => EditorPrefs.GetBool(cEditorPrefAutoSaveOnPlay, false);
			set => EditorPrefs.SetBool(cEditorPrefAutoSaveOnPlay, value);
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

		/// <summary>
		/// Play mode change callback handles the scene load/reload.
		/// </summary>
		private static void OnPlayModeChanged(PlayModeStateChange state) {
			if (!LoadMasterOnPlay) {
				return;
			}

			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
				// User pressed play -- autoload master scene.
				PreviousScene = SceneManager.GetActiveScene().path;

				if (string.IsNullOrWhiteSpace(PreviousScene)) {
					var userSelection = EditorUtility.DisplayDialogComplex("Scene Autoloader",
																		   "You must save the current scene to be able to load the master scene.",
																		   "Save",
																		   "Cancel",
																		   "Play current scene only");
					switch (userSelection) {
						case 0:
							if (EditorSceneManager.SaveOpenScenes()) {
								PreviousScene = EditorSceneManager.GetActiveScene().path;
							} else {
								EditorUtility.DisplayDialog("Scene Autoloader",
															"Could not play the game because the current scene is not saved.",
															"Ok");
								EditorApplication.isPlaying = false;
								return;
							}
							break;
						case 1:
							EditorApplication.isPlaying = false;
							return;
						case 2:
							return;
					}
				}

				if (PreviousScene == MasterScene)
					return;

				if ((AutoSaveOnPlay && EditorSceneManager.SaveOpenScenes()) || EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
					try {
						EditorSceneManager.OpenScene(MasterScene);
					} catch {
						Debug.LogError($"error: scene not found: {MasterScene}");
						MasterScene = null;
						AutoSaveOnPlay = false;
						LoadMasterOnPlay = false;
						PreviousScene = null;
						//EditorApplication.isPlaying = false;

					}
				} else {
					// User cancelled the save operation -- cancel play as well.
					EditorApplication.isPlaying = false;
				}
			}

			// isPlaying check required because cannot OpenScene while playing
			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode && !string.IsNullOrWhiteSpace(PreviousScene)) {
				if(SceneManager.GetActiveScene().path == PreviousScene) return;
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
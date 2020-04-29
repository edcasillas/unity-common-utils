using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using CommonUtils.Input;
using CommonUtils.Input.ButtonExternalControllers;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CommonUtils.Editor {
	public class ButtonsFromKeyboardWindow : EditorWindow {
		private static ButtonsFromKeyboardWindow instance = null;
		private static ButtonFromKeyboard[] buttonsFromKeyboard;
		private static IEnumerable<Button> unmappedButtons;
		private static object context;
		private Vector2 scroll;

		/*
		 * TODO Subscribe to scene and prefab stage change events to reload automatically.
		 */

		[MenuItem("Window/Buttons from Keyboard... #&%k")]
		private static void OpenActiveWindow() {
			if (!instance) {
				instance = GetWindow<ButtonsFromKeyboardWindow>();
				instance.titleContent = new GUIContent("Buttons from Keyboard");
				// TODO instance.maxSize
			}

			refresh();
		}

		private void OnGUI() {
			if(hasContextChanged() || GUILayout.Button("Refresh")) refresh();

			try {
				scroll = EditorGUILayout.BeginScrollView(scroll);
				EditorExtensions.RichLabelField("<b>Mappings</b>");
				if (buttonsFromKeyboard.IsNullOrEmpty()) {
					EditorGUILayout.HelpBox("No button from keyboard mappings have been found in the current scene.", MessageType.Info);
				} else {
					foreach (var buttonFromKeyboard in buttonsFromKeyboard) {
						Undo.RecordObject(buttonFromKeyboard,"change button key mapping.");
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField(buttonFromKeyboard, typeof(ButtonFromKeyboard));
						buttonFromKeyboard.KeyCode = (KeyCode)EditorGUILayout.EnumPopup(buttonFromKeyboard.KeyCode);
						EditorGUILayout.EndHorizontal();
					}
				}

				EditorExtensions.RichLabelField("<b>Unmapped Buttons</b>");
				if (unmappedButtons.IsNullOrEmpty()) {
					EditorGUILayout.HelpBox("No unmapped buttons have been found in the current scene.", MessageType.Info);
				} else {
					foreach (var button in unmappedButtons) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField(button, typeof(Button));
						if (GUILayout.Button("Add mapping")) {
							Undo.AddComponent<ButtonFromKeyboard>(button.gameObject);
							refresh();
						}

						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndScrollView();
			} catch { }
		}

		private static void refresh() {
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null) {
				//Debug.Log("In prefab");
				context = prefabStage;
				buttonsFromKeyboard = prefabStage.stageHandle.FindComponentsOfType<ButtonFromKeyboard>();
				unmappedButtons = prefabStage.stageHandle.FindComponentsOfType<Button>().Where(b => !b.GetComponent<ButtonFromKeyboard>());
			} else {
				//Debug.Log("In scene");
				context = SceneManager.GetActiveScene();
				buttonsFromKeyboard = FindObjectsOfType<ButtonFromKeyboard>();
				unmappedButtons = FindObjectsOfType<Button>().Where(b => !b.GetComponent<ButtonFromKeyboard>());
			}
		}

		private static bool hasContextChanged() {
			if (context == null || buttonsFromKeyboard == null || unmappedButtons == null) return true;
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

			if (prefabStage != null) {
				return context != prefabStage;
			}

			if (context is Scene contextScene) {
				return contextScene != SceneManager.GetActiveScene();
			}

			return true;
		}
	}
}

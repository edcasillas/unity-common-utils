using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using CommonUtils.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CommonUtils.Editor {
	public class ButtonsFromKeyboardWindow : EditorWindow {
		private static ButtonsFromKeyboardWindow instance = null;
		private static ButtonFromKeyboard[] buttonsFromKeyboard;
		private static IEnumerable<Button> unmappedButtons;
		private static Scene currentScene;

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
			if(SceneManager.GetActiveScene() != currentScene || buttonsFromKeyboard == null || GUILayout.Button("Refresh")) refresh();

			try {
				EditorExtensions.RichLabelField("<b>Mappings</b>");
				if (buttonsFromKeyboard.IsNullOrEmpty()) {
					EditorGUILayout.HelpBox("No button from keyboard mappings have been found in the current scene.", MessageType.Info);
				} else {
					foreach (var buttonFromKeyboard in buttonsFromKeyboard) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField(buttonFromKeyboard, typeof(ButtonFromKeyboard));
						buttonFromKeyboard.KeyCode = (KeyCode)EditorGUILayout.EnumPopup(buttonFromKeyboard.KeyCode);
						EditorGUILayout.EndHorizontal();
					}
				}

				EditorExtensions.RichLabelField("<b>Unmapped Buttons</b>");
				foreach (var button in unmappedButtons) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.ObjectField(button, typeof(Button));
					if (GUILayout.Button("Add mapping")) {
						button.gameObject.AddComponent<ButtonFromKeyboard>();
						refresh();
					}

					EditorGUILayout.EndHorizontal();
				}
			} catch { }
		}

		private static void refresh() {
			currentScene = SceneManager.GetActiveScene();
			buttonsFromKeyboard = FindObjectsOfType<ButtonFromKeyboard>();
			unmappedButtons = FindObjectsOfType<Button>().Where(b => !b.GetComponent<ButtonFromKeyboard>());
		}
	}
}

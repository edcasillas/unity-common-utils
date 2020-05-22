using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.Extensions;
using CommonUtils.Input.ButtonExternalControllers;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CommonUtils.Editor {
	public class UIKeyboardBindingsWindow : EditorWindow {
		private const string EditorPrefKey_ShowInScene = "CommonUtils.UIKeyboardBindingsWindow.ShowInScene";

		private static UIKeyboardBindingsWindow instance = null;
		private static ButtonFromKeyboard[] bindings;
		private static IEnumerable<Selectable> unmappedButtons;
		private static object context;
		private Vector2 scroll;

		private static bool showInScene {
			get => EditorPrefs.GetBool(EditorPrefKey_ShowInScene, false);
			set => EditorPrefs.SetBool(EditorPrefKey_ShowInScene, value);
		}

		/*
		 * TODO Subscribe to scene and prefab stage change events to reload automatically.
		 */

		[MenuItem("Window/UI Keyboard Bindings... #&%k")]
		private static void OpenActiveWindow() {
			if (!instance) {
				instance = GetWindow<UIKeyboardBindingsWindow>();
				instance.titleContent = new GUIContent("UI Keyboard Bindings");
				// TODO instance.maxSize
			}

			refresh();
		}

		// Window has been selected
		private void OnFocus() {
			// Remove delegate listener if it has previously
			// been assigned.
			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
			// Add (or re-add) the delegate.
			SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		}

		private void OnDestroy() {
			// When the window is destroyed, remove the delegate
			// so that it will no longer do any drawing.
			SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		}

		private void OnGUI() {
			var newShowInSceneValue  = EditorGUILayout.Toggle("Show in Scene View", showInScene);
			EditorGUILayout.HelpBox("After enabling or disabling Show in Scene View, it takes a few seconds to refresh the Scene View. Please move your mouse around in the Scene View to refresh it.", MessageType.Info);
			if (newShowInSceneValue != showInScene) {
				showInScene = newShowInSceneValue;
				OnSceneGUI(null); // Force to repaint when the value changes.
			}

			if(hasContextChanged() || GUILayout.Button("Refresh")) refresh();

			try {
				scroll = EditorGUILayout.BeginScrollView(scroll);

				EditorExtensions.RichLabelField("<b>Key Mappings</b>");
				if (bindings.IsNullOrEmpty()) {
					EditorGUILayout.HelpBox("No button from keyboard mappings have been found in the current scene.", MessageType.Info);
				} else {
					foreach (var buttonFromKeyboard in bindings) {
						if(!buttonFromKeyboard) continue;
						Undo.RecordObject(buttonFromKeyboard,"change button key mapping.");
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField(buttonFromKeyboard.Button, typeof(Button), true);
						buttonFromKeyboard.KeyCode = (KeyCode)EditorGUILayout.EnumPopup(buttonFromKeyboard.KeyCode);
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.Space();

				EditorExtensions.RichLabelField("<b>Unmapped Buttons</b>");
				if (unmappedButtons.IsNullOrEmpty()) {
					EditorGUILayout.HelpBox("No unmapped buttons have been found in the current scene.", MessageType.Info);
				} else {
					foreach (var selectable in unmappedButtons) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField(selectable, typeof(Selectable), true);
						if (GUILayout.Button("Add binding")) {
							Undo.AddComponent<ButtonFromKeyboard>(selectable.gameObject);
							refresh();
						}

						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndScrollView();
			} catch { }
		}

		private void OnSceneGUI(SceneView scnView) {
			if (!showInScene) return;
			if(hasContextChanged() || GUILayout.Button("Refresh")) refresh();
			if (bindings.IsNullOrEmpty()) return;

			foreach (var binding in bindings) {
				if(binding.gameObject.activeInHierarchy && binding.IsInteractable()) Handles.Label(binding.transform.position, binding.KeyCode.ToString());
			}
		}

		private static void refresh() {
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null) {
				//Debug.Log("In prefab");
				context = prefabStage;
				bindings = prefabStage.stageHandle.FindComponentsOfType<ButtonFromKeyboard>();
				unmappedButtons = prefabStage.stageHandle.FindComponentsOfType<Selectable>().Where(b => !b.GetComponent<AbstractButtonExternalController>());
			} else {
				//Debug.Log("In scene");
				context = SceneManager.GetActiveScene();
				bindings = getAllObjectsOnlyInScene<ButtonFromKeyboard>();
				unmappedButtons = getAllObjectsOnlyInScene<Selectable>(b => !b.GetComponent<AbstractButtonExternalController>());
			}
		}

		private static bool hasContextChanged() {
			if (context == null || bindings == null || unmappedButtons == null) return true;
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

			if (prefabStage != null) {
				return context != prefabStage;
			}

			if (context is Scene contextScene) {
				return contextScene != SceneManager.GetActiveScene();
			}

			return true;
		}

		private static T[] getAllObjectsOnlyInScene<T>(Func<T,bool> additionalConstraints = null) where T:MonoBehaviour
			=> (Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
			   .Where(obj => !EditorUtility.IsPersistent(obj.transform.root.gameObject) && !(obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave) && (additionalConstraints == null || additionalConstraints(obj)))
			   .ToArray();
	}
}

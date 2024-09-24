using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Editor {
	/// <summary>
	/// A collection of methods to help showing useful data in CustomEditors.
	/// </summary>
	public static partial class EditorExtensions {
		private static readonly GUIStyle textureFieldLabelStyle = new GUIStyle(GUI.skin.label)
			{ alignment = TextAnchor.UpperCenter, fixedWidth = 70 };

		private static readonly GUIStyle richLabelStyle = new GUIStyle(GUI.skin.label) { richText = true };

		public static void ReadOnlyLabelField(string label, object value) => RichLabelField($"{label}: <b>{value}</b>");

		public static void RichLabelField(string label) => EditorGUILayout.LabelField(label, richLabelStyle);

		public static void ObjectField(string label, Object obj)
			=> EditorGUILayout.ObjectField($"{label}:", obj, typeof(Object), false);

		public static bool ReadonlyDictionary<TKey, TValue>(bool fold, IReadOnlyDictionary<TKey, TValue> dictionary,
			string displayName) where TValue : Object {
			if (dictionary == null) {
				EditorGUILayout.LabelField($"{displayName} is null.");
			} else if (!dictionary.Any()) {
				EditorGUILayout.LabelField($"{displayName} is empty.");
			} else {
				fold = EditorGUILayout.Foldout(fold, new GUIContent($"{displayName} ({dictionary.Count})"), true);
				if (fold) {
					EditorGUI.indentLevel++;
					EditorGUI.BeginDisabledGroup(true);
					foreach (var kvp in dictionary) {
						ObjectField(kvp.Key.ToString(), kvp.Value);
					}

					EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				}
			}

			return fold;
		}

		public static bool ReadonlyDictionaryLabels<TKey, TValue>(bool fold,
			IReadOnlyDictionary<TKey, TValue> dictionary, string displayName) {
			if (dictionary == null) {
				EditorGUILayout.LabelField($"{displayName} is null.");
			} else if (!dictionary.Any()) {
				EditorGUILayout.LabelField($"{displayName} is empty.");
			} else {
				fold = EditorGUILayout.Foldout(fold, new GUIContent($"{displayName} ({dictionary.Count})"), true);
				if (fold) {
					EditorGUI.indentLevel++;
					foreach (var kvp in dictionary) {
						ReadOnlyLabelField(kvp.Key.ToString(), kvp.Value?.ToString() ?? "<null>");
					}

					EditorGUI.indentLevel--;
				}
			}

			return fold;
		}

		public static bool ReadonlyEnumerable(bool fold, IEnumerable enumerable, string displayName) {
			var count = enumerable.Cast<object>().Count();

			if (count == 0) {
				EditorGUILayout.LabelField($"{displayName} is empty.");
			} else {
				fold = EditorGUILayout.Foldout(fold, new GUIContent($"{displayName}"), true);
				if (!fold) return false;

				EditorGUI.indentLevel++;

				var i = 0;
				foreach (var item in enumerable) {
					if (item == null) {
						EditorGUILayout.LabelField($"[{i}]", "<null>");
					} else {
						RenderField(item.GetType(), $"[{i}]", item);
					}
					i++;
				}

				EditorGUI.indentLevel--;
			}

			return fold;
		}

		/// <summary>
		/// Creates a collapsible area.
		/// </summary>
		/// <param name="show">Indicates whether the collapsible area is expanded.</param>
		/// <param name="title">Title for this group.</param>
		/// <param name="contentsDelegate">Delegate containing the contents of this collapsible area.</param>
		/// <param name="indentContents">Indicates whether the contents should be indented by one level.</param>
		/// <param name="showBox">Indicates whether the contents should be surrounded with a box.</param>
		/// <returns>New value for <paramref name="show"/>.</returns>
		public static bool Collapse(bool show, string title, Action contentsDelegate, bool indentContents = true,
			bool showBox = false) {
			if (GUILayout.Button($"{(show ? "\u25BC" : "\u25B6")} {title}", EditorStyles.toolbarButton))
				show = !show;

			if (indentContents) EditorGUI.indentLevel++;
			if (show) {
				if (showBox) BoxGroup(contentsDelegate);
				else contentsDelegate();
			}

			if (indentContents) EditorGUI.indentLevel--;

			return show;
		}

		public static void BoxGroup(Action contentsDelegate, string title = null) {
			GUILayout.BeginVertical(title, "box");
			if (!string.IsNullOrWhiteSpace(title)) {
				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}

			contentsDelegate();
			GUILayout.EndVertical();
		}

		public static void Disabled(Action contentsDelegate, bool isDisabled = true) {
			EditorGUI.BeginDisabledGroup(isDisabled);
			contentsDelegate.Invoke();
			EditorGUI.EndDisabledGroup();
		}

		public static Texture2D TextureField(string name, Texture2D texture) {
			GUILayout.BeginVertical();
			GUILayout.Label(name, textureFieldLabelStyle);
			var result = (Texture2D)EditorGUILayout.ObjectField(texture,
				typeof(Texture2D),
				false,
				GUILayout.Width(70),
				GUILayout.Height(70));
			GUILayout.EndVertical();
			return result;
		}

		public static void ShowScriptField<T>(this T target, string label = "Script") where T : MonoBehaviour {
			GUI.enabled = false;
			EditorGUILayout.ObjectField(label, MonoScript.FromMonoBehaviour(target), target.GetType(), false);
			GUI.enabled = true;
		}

		public static void ShowLoadingSpinner(this EditorWindow editorWindow, string label, float? value = null) {
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
			var spinnerRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
			EditorGUI.ProgressBar(spinnerRect, value ?? -1f, string.Empty);
			editorWindow.Repaint();
		}
	}
}
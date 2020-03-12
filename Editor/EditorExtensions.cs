using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	/// <summary>
	/// A collection of methods to help showing useful data in CustomEditors.
	/// </summary>
	public static class EditorExtensions {
		private static GUIStyle _richTextLabelStyle;

		private static GUIStyle richTextLabelStyle {
			get {
				if (_richTextLabelStyle == null) {
					_richTextLabelStyle = new GUIStyle(GUI.skin.label) {
						richText = true
					};
				}

				return _richTextLabelStyle;
			}
		}

		public static void ReadOnlyLabelField(string label, object value) => RichLabelField($"{label}: <b>{value}</b>");

		public static void RichLabelField(string label) => EditorGUILayout.LabelField(label, richTextLabelStyle);

		public static void ObjectField(string label, Object obj)
			=> EditorGUILayout.ObjectField($"{label}:", obj, typeof(Object), false);

		public static bool ReadonlyDictionary<TKey, TValue>(bool   fold, IDictionary<TKey, TValue> dictionary,
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

		public static bool ReadonlyDictionaryLabels<TKey, TValue>(bool fold, IDictionary<TKey, TValue> dictionary, string displayName) {
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
						ReadOnlyLabelField(kvp.Key.ToString(), kvp.Value.ToString());
					}

					EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				}
			}

			return fold;
		}

		public static bool ReadonlyEnumerable<T>(bool fold, IEnumerable<T> enumerable, string displayName) where T : Object {
			if (enumerable == null) {
				EditorGUILayout.LabelField($"{displayName} is null.");
			} else if (!enumerable.Any()) {
				EditorGUILayout.LabelField($"{displayName} is empty.");
			} else {
				fold = EditorGUILayout.Foldout(fold, new GUIContent($"{displayName} ({enumerable.Count()})"), true);
				if (fold) {
					EditorGUI.indentLevel++;
					EditorGUI.BeginDisabledGroup(true);

					var i = 0;
					foreach (var item in enumerable) {
						ObjectField(i.ToString(), item);
						i++;
					}

					EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				}
			}

			return fold;
		}

		public static bool ReadonlyEnumerable(bool fold, IEnumerable<string> enumerable, string displayName) {
			if (enumerable == null) {
				EditorGUILayout.LabelField($"{displayName} is null.");
			} else if (!enumerable.Any()) {
				EditorGUILayout.LabelField($"{displayName} is empty.");
			} else {
				fold = EditorGUILayout.Foldout(fold, new GUIContent($"{displayName} ({enumerable.Count()})"), true);
				if (fold) {
					EditorGUI.indentLevel++;
					EditorGUI.BeginDisabledGroup(true);

					var i = 0;
					foreach (var item in enumerable) {
						ReadOnlyLabelField(i.ToString(), item);
						i++;
					}

					EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				}
			}

			return fold;
		}
	}
}
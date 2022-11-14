using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Editor {
	/// <summary>
	/// A collection of methods to help showing useful data in CustomEditors.
	/// </summary>
	public static class EditorExtensions {
		private static readonly GUIStyle _textureFieldLabelStyle = new GUIStyle(GUI.skin.label)
			{ alignment = TextAnchor.UpperCenter, fixedWidth = 70 };

		public static void ReadOnlyLabelField(string label, object value) => RichLabelField($"{label}: <b>{value}</b>");

		public static void RichLabelField(string label) => EditorGUILayout.LabelField(label, GUI.skin.label);

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

		public static bool ReadonlyEnumerable<T>(bool fold, IEnumerable<T> enumerable, string displayName)
			where T : Object {
			if (enumerable == null) {
				EditorGUILayout.LabelField($"{displayName} is null.");
			} else if (!enumerable.Any()) {
				EditorGUILayout.LabelField($"{displayName} is empty.");
			} else {
				fold = EditorGUILayout.Foldout(fold, new GUIContent($"{displayName} ({enumerable.Count()})"), true);
				if (fold) {
					EditorGUI.indentLevel++;

					var i = 0;
					foreach (var item in enumerable) {
						ObjectField(i.ToString(), item);
						i++;
					}

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
			GUILayout.Label(name, _textureFieldLabelStyle);
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

		/// <summary>
		/// Renders an editor field of the proper specified <paramref name="type"/>, labeled with
		/// <paramref name="displayName"/> and containing the specified <paramref name="value"/>.
		/// </summary>
		/// <remarks>
		/// Instead of just using the type of the specified <paramref name="value"/>, this method receives a mandatory
		/// <paramref name="type"/> because when the value is null, its type is still object, instead of the actual
		/// type we probably want to render; so in case it's null, we will render the proper field and use the default
		/// value for the type.
		///
		/// Example: Calling as RenderField(typeof(int), "some name", null) will render an Int field containing a zero
		/// (default value for int). If we only used the value, the method would fallback to just print a label and
		/// not allowing to set any value.
		/// </remarks>
		public static object RenderField(Type type, string displayName, object value) {
			if (type == typeof(bool)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.Toggle((bool)(value ?? default(bool))) :
					EditorGUILayout.Toggle(displayName, (bool)(value ?? default(bool)));
			}

			if (type == typeof(string)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.TextField((string)value) :
					EditorGUILayout.TextField(displayName, (string)value);
			}

			if (type == typeof(int)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.IntField((int)(value ?? default(int))) :
					EditorGUILayout.IntField(displayName, (int)(value ?? default(int)));
			}

			if (type == typeof(float)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.FloatField((float)(value ?? default(float))) :
					EditorGUILayout.FloatField(displayName, (float)(value ?? default(float)));
			}

			if (type == typeof(double)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.DoubleField((double)(value ?? default(double))) :
					EditorGUILayout.DoubleField(displayName, (double)(value ?? default(double)));
			}

			if (type == typeof(long)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.DoubleField((long)(value ?? default(long))) :
					EditorGUILayout.DoubleField(displayName, (long)(value ?? default(long)));
			}

			if (type == typeof(ulong)) {
				var prev = (ulong)(value ?? default(ulong));
				var result = Math.Abs(Math.Round(string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.DoubleField(prev) :
					EditorGUILayout.DoubleField(displayName, prev)));
				return (ulong)result;
			}

			if (type.IsSubclassOf(typeof(Object))) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.ObjectField((Object)value, type, true) :
					EditorGUILayout.ObjectField(displayName, (Object)value, type, true);
			}

			if (type == typeof(Bounds)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.BoundsField((Bounds)(value ?? default(Bounds))) :
					EditorGUILayout.BoundsField(displayName, (Bounds)(value ?? default(Bounds)));
			}

			if (type == typeof(BoundsInt)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.BoundsIntField((BoundsInt)(value ?? default(BoundsInt))) :
					EditorGUILayout.BoundsIntField(displayName, (BoundsInt)(value ?? default(BoundsInt)));
			}

			if (type == typeof(Color)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.ColorField((Color)(value ?? default(Color))) :
					EditorGUILayout.ColorField(displayName, (Color)(value ?? default(Color)));
			}

			if (type == typeof(AnimationCurve)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.CurveField((AnimationCurve)value) :
					EditorGUILayout.CurveField(displayName, (AnimationCurve)value);
			}

			if (type.IsEnum) {
				if (value == null) {
					if (Enum.IsDefined(type, 0)) {
						value = Enum.ToObject(type, 0);
					} else {
						EditorGUILayout.LabelField(displayName, "<null>");
						return null;
					}
				}

				return type.GetCustomAttributes(typeof(FlagsAttribute), true).Any() ?
					(string.IsNullOrEmpty(displayName) ?
						EditorGUILayout.EnumFlagsField((Enum)value) :
						EditorGUILayout.EnumFlagsField(displayName, (Enum)value)) :
					(string.IsNullOrEmpty(displayName) ?
						EditorGUILayout.EnumPopup((Enum)value) :
						EditorGUILayout.EnumPopup(displayName, (Enum)value));
			}

			if (type == typeof(Gradient)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.GradientField((Gradient)value) :
					EditorGUILayout.GradientField(displayName, (Gradient)value);
			}

			if (type == typeof(Rect)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.RectField((Rect)(value ?? default(Rect))) :
					EditorGUILayout.RectField(displayName, (Rect)(value ?? default(Rect)));
			}

			if (type == typeof(RectInt)) {
				return string.IsNullOrEmpty(displayName) ?
					EditorGUILayout.RectIntField((RectInt)(value ?? default(RectInt))) :
					EditorGUILayout.RectIntField(displayName, (RectInt)(value ?? default(RectInt)));
			}

			if (type == typeof(Vector2)) {
				// Vector2Field doesn't have a labelless override
				return EditorGUILayout.Vector2Field(displayName, (Vector2)(value ?? default(Vector2)));
			}

			if (type == typeof(Vector3)) {
				// Vector3Field doesn't have a labelless override
				return EditorGUILayout.Vector3Field(displayName, (Vector3)(value ?? default(Vector3)));
			}

			if (type == typeof(Quaternion)) {
				// Vector3Field doesn't have a labelless override
				return Quaternion.Euler(EditorGUILayout.Vector3Field(displayName,
					((Quaternion)(value ?? default(Quaternion))).eulerAngles));
			}

			if (type == typeof(Vector4)) {
				// Vector4Field doesn't have a labelless override
				return EditorGUILayout.Vector4Field(displayName, (Vector4)(value ?? default(Vector4)));
			}

			if (type == typeof(Vector2Int)) {
				// Vector2IntField doesn't have a labelless override
				return EditorGUILayout.Vector2IntField(displayName, (Vector2Int)(value ?? default(Vector2Int)));
			}

			if (type == typeof(Vector3Int)) {
				// Vector3IntField doesn't have a labelless override
				return EditorGUILayout.Vector3IntField(displayName, (Vector3Int)(value ?? default(Vector3Int)));
			}

			if (type.IsGenericType) {
				// Checking for dictionaries (where each item is a KeyValuePair). Here we completely ignore the display name.
				var baseType = type.GetGenericTypeDefinition();
				if (baseType == typeof(KeyValuePair<,>)) {
					Type[] argTypes = type.GetGenericArguments();
					EditorGUILayout.BeginHorizontal();
					RenderField(argTypes[0], null, type.GetProperty("Key")!.GetValue(value, null));
					RenderField(argTypes[1], null, type.GetProperty("Value")!.GetValue(value, null));
					EditorGUILayout.EndHorizontal();
					return value;
				}
			}

			// fallback
			if (string.IsNullOrEmpty(displayName)) {
				EditorGUILayout.LabelField(value?.ToString() ?? "<null>");
			} else {
				EditorGUILayout.LabelField(displayName, value?.ToString() ?? "<null>");
			}

			return value;
		}
	}
}
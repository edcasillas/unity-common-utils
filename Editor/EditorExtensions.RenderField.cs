using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.Editor {
	public static partial class EditorExtensions {
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
		public static object RenderField(Type type, string displayName, object value, string tooltip = null) {
			var guiContent = displayName != null ? new GUIContent(displayName, tooltip) : null;

			// Check if the type is marked as [Serializable]
			if (type.IsSerializable && !type.IsPrimitive && !type.IsEnum && type != typeof(string)) {
				if (value == null) {
					var contents = new GUIContent("<null>");
					EditorGUILayout.LabelField(guiContent, contents);
					return value;
				}

				// Render the display name
				if (guiContent != null) {
					EditorGUILayout.LabelField(guiContent);
				}

				// Indent the fields
				EditorGUI.indentLevel++;
				foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
					// Skip non-serializable fields
					if (!field.IsPublic && !field.GetCustomAttributes(typeof(SerializeField), false).Any()) continue;

					// Render each field recursively
					object fieldValue;
					try {
						fieldValue = field.GetValue(value);
					} catch (Exception ex) {
						Debug.LogException(ex);
						fieldValue = null;
					}

					var fieldType = field.FieldType;
					var fieldDisplayName = ObjectNames.NicifyVariableName(field.Name);
					var newValue = RenderField(fieldType, fieldDisplayName, fieldValue);

					// Set the new value back to the field
					if (value != null && !Equals(newValue, fieldValue)) {
						field.SetValue(value, newValue);
					}
				}
				EditorGUI.indentLevel--;

				return value;
			}

			if (type == typeof(bool)) {
				return guiContent == null ?
					EditorGUILayout.Toggle((bool)(value ?? default(bool))) :
					EditorGUILayout.Toggle(guiContent, (bool)(value ?? default(bool)));
			}

			if (type == typeof(string)) {
				return guiContent == null ?
					EditorGUILayout.TextField((string)value) :
					EditorGUILayout.TextField(guiContent, (string)value);
			}

			if (type == typeof(int)) {
				return guiContent == null ?
					EditorGUILayout.IntField((int)(value ?? default(int))) :
					EditorGUILayout.IntField(guiContent, (int)(value ?? default(int)));
			}

			if (type == typeof(float)) {
				return guiContent == null ?
					EditorGUILayout.FloatField((float)(value ?? default(float))) :
					EditorGUILayout.FloatField(guiContent, (float)(value ?? default(float)));
			}

			if (type == typeof(double)) {
				return guiContent == null ?
					EditorGUILayout.DoubleField((double)(value ?? default(double))) :
					EditorGUILayout.DoubleField(guiContent, (double)(value ?? default(double)));
			}

			if (type == typeof(long)) {
				return guiContent == null ?
					EditorGUILayout.LongField((long)(value ?? default(long))) :
					EditorGUILayout.LongField(guiContent, (long)(value ?? default(long)));
			}

			if (type == typeof(ulong)) {
				var prev = (ulong)(value ?? default(ulong));
				var result = Math.Abs(Math.Round(guiContent == null ?
					EditorGUILayout.DoubleField(prev) :
					EditorGUILayout.DoubleField(guiContent, prev)));
				return (ulong)result;
			}

			if (type.IsSubclassOf(typeof(Object))) {
				return guiContent == null ?
					EditorGUILayout.ObjectField((Object)value, type, true) :
					EditorGUILayout.ObjectField(guiContent, (Object)value, type, true);
			}

			if (type == typeof(Bounds)) {
				return guiContent == null ?
					EditorGUILayout.BoundsField((Bounds)(value ?? default(Bounds))) :
					EditorGUILayout.BoundsField(guiContent, (Bounds)(value ?? default(Bounds)));
			}

			if (type == typeof(BoundsInt)) {
				return guiContent == null ?
					EditorGUILayout.BoundsIntField((BoundsInt)(value ?? default(BoundsInt))) :
					EditorGUILayout.BoundsIntField(guiContent, (BoundsInt)(value ?? default(BoundsInt)));
			}

			if (type == typeof(Color)) {
				return guiContent == null ?
					EditorGUILayout.ColorField((Color)(value ?? default(Color))) :
					EditorGUILayout.ColorField(guiContent, (Color)(value ?? default(Color)));
			}

			if (type == typeof(AnimationCurve)) {
				return guiContent == null ?
					EditorGUILayout.CurveField((AnimationCurve)value) :
					EditorGUILayout.CurveField(guiContent, (AnimationCurve)value);
			}

			if (type.IsEnum) {
				if (value == null) {
					if (Enum.IsDefined(type, 0)) {
						value = Enum.ToObject(type, 0);
					} else {
						if (guiContent == null) {
							EditorGUILayout.LabelField("<null>");
						} else {
							EditorGUILayout.LabelField(guiContent, new GUIContent("<null>"));
						}
						return null;
					}
				}

				return type.GetCustomAttributes(typeof(FlagsAttribute), true).Any() ?
					(guiContent == null ?
						EditorGUILayout.EnumFlagsField((Enum)value) :
						EditorGUILayout.EnumFlagsField(guiContent, (Enum)value)) :
					(guiContent == null ?
						EditorGUILayout.EnumPopup((Enum)value) :
						EditorGUILayout.EnumPopup(guiContent, (Enum)value));
			}

			if (type == typeof(Gradient)) {
				return guiContent == null ?
					EditorGUILayout.GradientField((Gradient)value) :
					EditorGUILayout.GradientField(guiContent, (Gradient)value);
			}

			if (type == typeof(Rect)) {
				return guiContent == null ?
					EditorGUILayout.RectField((Rect)(value ?? default(Rect))) :
					EditorGUILayout.RectField(guiContent, (Rect)(value ?? default(Rect)));
			}

			if (type == typeof(RectInt)) {
				return guiContent == null ?
					EditorGUILayout.RectIntField((RectInt)(value ?? default(RectInt))) :
					EditorGUILayout.RectIntField(guiContent, (RectInt)(value ?? default(RectInt)));
			}

			if (type == typeof(Vector2)) {
				// Vector2Field doesn't have a labelless override
				return EditorGUILayout.Vector2Field(guiContent, (Vector2)(value ?? default(Vector2)));
			}

			if (type == typeof(Vector3)) {
				// Vector3Field doesn't have a labelless override
				return EditorGUILayout.Vector3Field(guiContent, (Vector3)(value ?? default(Vector3)));
			}

			if (type == typeof(Quaternion)) {
				// Vector3Field doesn't have a labelless override
				return Quaternion.Euler(EditorGUILayout.Vector3Field(guiContent,
					((Quaternion)(value ?? default(Quaternion))).eulerAngles));
			}

			if (type == typeof(Vector4)) {
				// Vector4Field doesn't have a labelless override
				return EditorGUILayout.Vector4Field(guiContent, (Vector4)(value ?? default(Vector4)));
			}

			if (type == typeof(Vector2Int)) {
				// Vector2IntField doesn't have a labelless override
				return EditorGUILayout.Vector2IntField(guiContent, (Vector2Int)(value ?? default(Vector2Int)));
			}

			if (type == typeof(Vector3Int)) {
				// Vector3IntField doesn't have a labelless override
				return EditorGUILayout.Vector3IntField(guiContent, (Vector3Int)(value ?? default(Vector3Int)));
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
			if (guiContent == null) {
				EditorGUILayout.LabelField(value?.ToString() ?? "<null>");
			} else {
				var contents = new GUIContent(value?.ToString() ?? "<null>");
				EditorGUILayout.LabelField(guiContent, contents);
			}

			return value;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
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
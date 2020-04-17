using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CommonUtils.Serializables;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
	public abstract class AbstractSerializableDictionaryDrawer<TDictionary, TKey,TValue> : AbstractBoxedPropertyDrawer // Based on this answer: https://forum.unity.com/threads/finally-a-serializable-dictionary-for-unity-extracted-from-system-collections-generic.335797/#post-2173836
		where TDictionary : AbstractSerializableDictionary<TKey, TValue>, new() {

		private struct LabelInfo {
			public GUIContent Label;
			public float Width => EditorStyles.label.CalcSize(Label).x;
		}

		private static readonly Dictionary<string, LabelInfo> prefixLabel = new Dictionary<string, LabelInfo> {
			{ "Key", new LabelInfo{Label = new GUIContent(" Key ")} },
			{ "Value", new LabelInfo{Label = new GUIContent(" Value ")} },
		};

		private TDictionary dictionary;
		private bool foldout;
		private const float kButtonWidth = 18f;

		protected override float FooterHeight => EditorGUIUtility.standardVerticalSpacing;

		protected override int GetLineCount(SerializedProperty property, GUIContent label) {
			CheckInitialize(property, label);
			if (foldout && dictionary.Count > 0) return (dictionary.Count + 1);
			return 1;
		}

		protected override void DrawBoxContents(Rect position, SerializedProperty property, GUIContent label) {
			CheckInitialize(property, label);

			var foldoutRect = position;
			foldoutRect.width -= 2 * kButtonWidth;
			foldoutRect.x += 10f;
			EditorGUI.BeginChangeCheck();

			label.text += $" ({dictionary.Count} items)";
			if (dictionary.Count > 0) {
				foldout = EditorGUI.Foldout(foldoutRect, foldout, label, true);
			} else {
				EditorGUI.LabelField(foldoutRect, label);
			}
			if (EditorGUI.EndChangeCheck())
				EditorPrefs.SetBool(label.text, foldout);

			var buttonRect = position;
			buttonRect.x = position.width - kButtonWidth + position.x;
			buttonRect.width = kButtonWidth + 2;

			if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton)) {
				AddNewItem();
			}

			buttonRect.x -= kButtonWidth;

			if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight)) {
				ClearDictionary();
			}

			if (!foldout || dictionary.Count == 0)
				return;

			foreach (var item in dictionary) {
				var key = item.Key;
				var value = item.Value;

				position.y += PaddedLine;

				var keyRect = position;
				keyRect.width /= 2;
				keyRect.width -= 4;
				EditorGUI.BeginChangeCheck();

				var newKey = DoField(keyRect, typeof(TKey), key, "Key");
				if (EditorGUI.EndChangeCheck()) {
					try {
						dictionary.Remove(key);
						dictionary.Add(newKey, value);
					} catch (Exception e) {
						Debug.Log(e.Message);
					}

					break;
				}

				var valueRect = position;
				valueRect.x = position.width / 2 + 15;
				valueRect.width = keyRect.width - kButtonWidth;

				EditorGUI.BeginChangeCheck();
				value = DoField(valueRect, typeof(TValue), value, "Value");
				if (EditorGUI.EndChangeCheck()) {
					dictionary[key] = value;
					break;
				}

				var removeRect = valueRect;
				removeRect.x = valueRect.xMax + 2;
				removeRect.width = kButtonWidth;
				if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight)) {
					RemoveItem(key);
					break;
				}
			}
		}

		private void RemoveItem(TKey key) => dictionary.Remove(key);

		private void CheckInitialize(SerializedProperty property, GUIContent label) {
			if (dictionary != null) return;
			var target = SerializeableCollectionsPropertyHelper.GetParent(property);
			dictionary = fieldInfo.GetValue(target) as TDictionary;

			if (dictionary == null)
			{
				dictionary = new TDictionary();
				fieldInfo.SetValue(target, dictionary);
			}

			foldout = EditorPrefs.GetBool(label.text);
		}

		private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
			new Dictionary<Type,Func<Rect,object,object>>()
			{
				{ typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
				{ typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
				{ typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
				{ typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
				{ typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
				{ typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
				{ typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
				{ typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
			};

		private static T DoField<T>(Rect rect, Type type, T value, string prefix) {
			EditorGUI.PrefixLabel(rect, prefixLabel[prefix].Label);
			rect.x += prefixLabel[prefix].Width;
			rect.width -= prefixLabel[prefix].Width;

			if (_Fields.TryGetValue(type, out var field))
				return (T)field(rect, value);

			if (type.IsEnum)
				return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

			if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
				return (T)(object)EditorGUI.ObjectField(rect, (UnityEngine.Object)(object)value, type, true);
			}

			Debug.Log("Type is not supported: " + type);
			return value;
		}

		private void ClearDictionary() => dictionary.Clear();

		private void AddNewItem() {
			TKey key;
			if (typeof(TKey) == typeof(string))
				key = (TKey)(object)"";
			else key = default;

			if (key == null) {
				Debug.LogError($"Sorry, {typeof(TKey)} is not supported as a dictionary key at the moment.");
				return;
			}

			var value = default(TValue);
			try {
				dictionary.Add(key, value);
				foldout = true;
			} catch (Exception e) {
				Debug.Log(e.Message);
			}
		}
	}
}

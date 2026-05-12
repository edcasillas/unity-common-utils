using System;
using System.Collections.Generic;
using CommonUtils.Serializables.SerializableDictionaries;
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

		private static readonly Dictionary<Type, Func<Rect, object, object>> fields =
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

		private TDictionary dictionary;
		private bool foldout;
		private const float kButtonWidth = 18f;

		protected override float FooterHeight => EditorGUIUtility.standardVerticalSpacing;

		protected override int GetLineCount(SerializedProperty property, GUIContent label) {
			CheckInitialize(property, label);
			var lines = 1;
			if (foldout) lines += 1 + dictionary.Count;
			return lines;
		}

		protected override float GetContentsHeight(SerializedProperty property, GUIContent label) {
			CheckInitialize(property, label);

			var height = LineHeight;
			if (!foldout) return height;

			height += PaddedLine;

			var valuesProperty = property.FindPropertyRelative("values");
			for (var i = 0; i < dictionary.Count; i++) {
				height += PadSize + GetItemPropertyHeight(valuesProperty, i);
			}

			return height;
		}

		protected override void DrawBoxContents(Rect position, SerializedProperty property, GUIContent label) {
			CheckInitialize(property, label);

			#region Header foldout and buttons
			var foldoutRect = position;
			foldoutRect.width -= 2 * kButtonWidth;
			foldoutRect.x += 10f;
			EditorGUI.BeginChangeCheck();

			label.text += $" ({dictionary.Count} items)";
			foldout = EditorGUI.Foldout(foldoutRect, foldout, label, true);
			if (EditorGUI.EndChangeCheck())
				EditorPrefs.SetBool($"{property.propertyPath}-foldout", foldout);

			var buttonRect = position;
			buttonRect.x = position.width - kButtonWidth + position.x;
			buttonRect.width = kButtonWidth + 2;

			if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight)) {
				ClearDictionary();
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
			#endregion

			if (!foldout)
				return;

			position.y += PaddedLine;

			#region Add
			if (drawAddForm(position)) {
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
			#endregion

			if (dictionary.Count == 0)
				return;

			var valuesProperty = property.FindPropertyRelative("values");
			var index = 0;
			foreach (var item in dictionary) {
				var key = item.Key;
				var value = item.Value;

				position.y += PaddedLine;
				var isSerializedValue = CanDrawSerializedValue(valuesProperty, index);
				position.height = GetValuePropertyHeight(valuesProperty, index);

				var valueRect = position;
				if (isSerializedValue) {
					valueRect.x += 14f;
					valueRect.width -= 14f;
					valueRect.width -= kButtonWidth + 2;
					if (DrawSerializedValueInputField(valueRect, valuesProperty, index, key)) {
						property.serializedObject.ApplyModifiedProperties();
						EditorUtility.SetDirty(property.serializedObject.targetObject);
						break;
					}
				}
				else {
					var keyRect = position;
					keyRect.height = LineHeight;
					keyRect.width = (position.width / 2) - 4;

					EditorGUI.BeginDisabledGroup(true);
					keyRect = drawPrefixLabel(keyRect, "Key");
					DrawKeyInputField(keyRect, key);
					EditorGUI.EndDisabledGroup();

					valueRect.x = position.x + (position.width / 2) + 15;
					valueRect.width = keyRect.width - kButtonWidth + prefixLabel["Key"].Width;
					valueRect = drawPrefixLabel(valueRect, "Value");

					EditorGUI.BeginChangeCheck();
					value = DrawValueInputField(valueRect, value);
					if (EditorGUI.EndChangeCheck()) {
						dictionary[key] = value;
						EditorUtility.SetDirty(property.serializedObject.targetObject);
						break;
					}
				}

				var removeRect = valueRect;
				removeRect.x = position.xMax - kButtonWidth;
				removeRect.y = position.y;
				removeRect.height = LineHeight;
				removeRect.width = kButtonWidth;
				if (GUI.Button(removeRect, new GUIContent("-", "Remove item"), EditorStyles.miniButtonRight)) {
					RemoveItem(key);
					EditorUtility.SetDirty(property.serializedObject.targetObject);
					break;
				}

				position.y += GetItemPropertyHeight(valuesProperty, index) - LineHeight;
				position.height = LineHeight;
				index++;
			}
		}

		private TKey newKeyToAdd;
		private TValue newValueToAdd;

		private bool drawAddForm(Rect position) {
			var keyRect = position;
			keyRect.width /= 2;
			keyRect.width -= 4;
			keyRect = drawPrefixLabel(keyRect, "Key");
			newKeyToAdd = DrawKeyInputField(keyRect, newKeyToAdd);

			var valueRect = position;
			valueRect.x = position.x + (position.width / 2) + 15;
			valueRect.width = keyRect.width - kButtonWidth + prefixLabel["Key"].Width;
			valueRect = drawPrefixLabel(valueRect, "Value");
			newValueToAdd = DrawValueInputField(valueRect, newValueToAdd);

			var removeRect = valueRect;
			removeRect.x = valueRect.xMax + 2;
			removeRect.width = kButtonWidth;
			if (GUI.Button(removeRect, new GUIContent("+", "Add item"), EditorStyles.miniButtonRight)) {
				if (newKeyToAdd == null) {
					EditorUtility.DisplayDialog("Serializable dictionary", "Key cannot be null.", "Ok");
					return false;
				}
				if (dictionary.ContainsKey(newKeyToAdd)) {
					EditorUtility.DisplayDialog("Serializable dictionary", "Cannot add because the key already exists in the dictionary.", "Ok");
					return false;
				}
				try {
					dictionary.Add(newKeyToAdd, newValueToAdd);
					foldout = true;
					return true;
				} catch (Exception e) {
					Debug.Log(e.Message);
				}
			}

			return false;
		}

		private void RemoveItem(TKey key) => dictionary.Remove(key);

		private void CheckInitialize(SerializedProperty property, GUIContent label) {
			/*if (dictionary != null) {
				Debug.Log($"NOT Initializing {property.propertyPath}");
				return;
			}*/

			var target = SerializeableCollectionsPropertyHelper.GetParent(property);
			dictionary = fieldInfo.GetValue(target) as TDictionary;

			if (dictionary == null) {
				dictionary = new TDictionary();
				fieldInfo.SetValue(target, dictionary);
			}

			foldout = EditorPrefs.GetBool($"{property.propertyPath}-foldout");
		}

		protected virtual TKey DrawKeyInputField(Rect rect, TKey value) => drawInputField(rect, value);
		protected virtual TValue DrawValueInputField(Rect rect, TValue value) => drawInputField(rect, value);

		private float GetValuePropertyHeight(SerializedProperty valuesProperty, int index) {
			if (CanDrawSerializedValue(valuesProperty, index)) {
				return EditorGUI.GetPropertyHeight(valuesProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
			}

			return LineHeight;
		}

		private float GetItemPropertyHeight(SerializedProperty valuesProperty, int index) {
			return GetValuePropertyHeight(valuesProperty, index);
		}

		private bool DrawSerializedValueInputField(Rect rect, SerializedProperty valuesProperty, int index, TKey key) {
			if (!CanDrawSerializedValue(valuesProperty, index)) return false;

			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(rect, valuesProperty.GetArrayElementAtIndex(index), new GUIContent(key?.ToString() ?? $"Element {index}"), true);
			return EditorGUI.EndChangeCheck();
		}

		private static bool CanDrawSerializedValue(SerializedProperty valuesProperty, int index) {
			if (valuesProperty == null || index >= valuesProperty.arraySize) return false;

			var valueProperty = valuesProperty.GetArrayElementAtIndex(index);
			return valueProperty.propertyType == SerializedPropertyType.Generic;
		}

		private static Rect drawPrefixLabel(Rect rect, string prefix) {
			EditorGUI.PrefixLabel(rect, prefixLabel[prefix].Label);
			rect.x += prefixLabel[prefix].Width;
			rect.width -= prefixLabel[prefix].Width;
			return rect;
		}

		private static T drawInputField<T>(Rect rect, T value) {
			var type = typeof(T);
			if (fields.TryGetValue(type, out var field))
				return (T)field(rect, value);

			if (type.IsEnum)
				return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

			if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
				return (T)(object)EditorGUI.ObjectField(rect, (UnityEngine.Object)(object)value, type, true);
			}

			if (type.IsClass && Attribute.IsDefined(type, typeof(SerializableAttribute)) && type.GetConstructor(Type.EmptyTypes) != null) {
				if (value == null) value = Activator.CreateInstance<T>();
				EditorGUI.LabelField(rect, ObjectNames.NicifyVariableName(type.Name));
				return value;
			}

			EditorGUI.HelpBox(rect, $"{type} is not supported", MessageType.Error);
			return value;
		}

		private void ClearDictionary() => dictionary.Clear();
	}
}

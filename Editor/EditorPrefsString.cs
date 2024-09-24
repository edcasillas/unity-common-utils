using System;
using UnityEditor;

namespace CommonUtils.Editor {
	public class EditorPrefsString {
		private readonly string editorPrefsKey;
		private readonly string defaultValue;
		private readonly Func<string> defaultValueDelegate;
		private readonly bool isProjectSpecific;

		private string actualKey;

		private string ActualKey => actualKey ??= $"{(isProjectSpecific ? $"{PlayerSettings.productGUID}." : string.Empty)}{editorPrefsKey}";

		public EditorPrefsString(string editorPrefsKey, string defaultValue = null, bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.defaultValue = defaultValue;
			this.isProjectSpecific = isProjectSpecific;
		}

		public EditorPrefsString(string editorPrefsKey, Func<string> defaultValueDelegate, bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.defaultValueDelegate = defaultValueDelegate;
			this.isProjectSpecific = isProjectSpecific;
		}

		public string Value {
			get => EditorPrefs.GetString(ActualKey, defaultValue ?? defaultValueDelegate?.Invoke());
			set => EditorPrefs.SetString(ActualKey, value);
		}

		public void Clear() => EditorPrefs.DeleteKey(editorPrefsKey);

		public static implicit operator string(EditorPrefsString editorPrefsString) => editorPrefsString.Value;

		public override string ToString() => Value;
	}

	public class EditorPrefsInt {
		private readonly string editorPrefsKey;
		private readonly int defaultValue;
		private readonly bool isProjectSpecific;

		private string actualKey;

		private string ActualKey => actualKey ??= $"{(isProjectSpecific ? $"{PlayerSettings.productGUID}." : string.Empty)}{editorPrefsKey}";

		public EditorPrefsInt(string editorPrefsKey, int defaultValue = 0, bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.defaultValue = defaultValue;
			this.isProjectSpecific = isProjectSpecific;
		}

		public int Value {
			get => EditorPrefs.GetInt(ActualKey, defaultValue);
			set => EditorPrefs.SetInt(ActualKey, value);
		}

		public void Clear() => EditorPrefs.DeleteKey(editorPrefsKey);

		public static implicit operator int(EditorPrefsInt editorPrefsString) => editorPrefsString.Value;

		public override string ToString() => Value.ToString();
	}
}
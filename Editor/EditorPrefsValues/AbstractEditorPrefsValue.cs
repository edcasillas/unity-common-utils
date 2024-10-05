using System;
using UnityEditor;

namespace CommonUtils.Editor.EditorPrefsValues {
	public abstract class AbstractEditorPrefsValue<T> {
		private enum DefaultValueType {
			None,
			Concrete,
			Delegate
		}

		private readonly string editorPrefsKey;
		private readonly T defaultValue;
		private readonly Func<T> defaultValueDelegate;
		private readonly bool isProjectSpecific;
		private readonly DefaultValueType defaultValueType;

		private string actualKey;
		protected string ActualKey => actualKey ??= $"{(isProjectSpecific ? $"{PlayerSettings.productGUID}." : string.Empty)}{editorPrefsKey}";

		protected AbstractEditorPrefsValue(string editorPrefsKey, bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.isProjectSpecific = isProjectSpecific;
			defaultValueType = DefaultValueType.None;
		}

		protected AbstractEditorPrefsValue(string editorPrefsKey, T defaultValue, bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.defaultValue = defaultValue;
			this.isProjectSpecific = isProjectSpecific;
			defaultValueType = DefaultValueType.Concrete;
		}

		protected AbstractEditorPrefsValue(string editorPrefsKey, Func<T> defaultValueDelegate, bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.defaultValueDelegate = defaultValueDelegate;
			this.isProjectSpecific = isProjectSpecific;
			defaultValueType = DefaultValueType.Delegate;
		}

		public abstract T Value { get; set; }

		protected T GetDefaultValue() => defaultValueType switch {
			DefaultValueType.None => default(T),
			DefaultValueType.Concrete => defaultValue,
			DefaultValueType.Delegate => defaultValueDelegate.Invoke(),
			_ => throw new ArgumentOutOfRangeException()
		};

		public void Clear() => EditorPrefs.DeleteKey(ActualKey);

		public static implicit operator T(AbstractEditorPrefsValue<T> instance) => instance.Value;

		public override string ToString() => Value.ToString();
	}
}
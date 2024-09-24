using System;
using UnityEditor;

namespace CommonUtils.Editor {
	public abstract class AbstractEditorPrefsValue<T> {
		private readonly string editorPrefsKey;
		protected readonly T DefaultValue;
		private readonly bool isProjectSpecific;

		private string actualKey;

		protected string ActualKey => actualKey ??= $"{(isProjectSpecific ? $"{PlayerSettings.productGUID}." : string.Empty)}{editorPrefsKey}";

		protected AbstractEditorPrefsValue(string editorPrefsKey, T defaultValue = default(T), bool isProjectSpecific = false) {
			this.editorPrefsKey = editorPrefsKey;
			this.DefaultValue = defaultValue;
			this.isProjectSpecific = isProjectSpecific;
		}

		public abstract T Value { get; set; }

		public void Clear() => EditorPrefs.DeleteKey(ActualKey);

		public static implicit operator T(AbstractEditorPrefsValue<T> instance) => instance.Value;

		public override string ToString() => Value.ToString();
	}

	public class EditorPrefsString : AbstractEditorPrefsValue<string> {
		private readonly Func<string> defaultValueDelegate;

		public EditorPrefsString(string editorPrefsKey, string defaultValue = null, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue, isProjectSpecific) { }
		public EditorPrefsString(string editorPrefsKey, Func<string> defaultValueDelegate, bool isProjectSpecific = false) : base(editorPrefsKey, isProjectSpecific: isProjectSpecific) => this.defaultValueDelegate = defaultValueDelegate;

		public override string Value {
			get => EditorPrefs.GetString(ActualKey, DefaultValue ?? defaultValueDelegate?.Invoke());
			set => EditorPrefs.SetString(ActualKey, value);
		}
	}

	public class EditorPrefsInt : AbstractEditorPrefsValue<int> {
		public EditorPrefsInt(string editorPrefsKey, int defaultValue = 0, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue) { }

		public override int Value {
			get => EditorPrefs.GetInt(ActualKey, DefaultValue);
			set => EditorPrefs.SetInt(ActualKey, value);
		}
	}

	public class EditorPrefsEnum<TEnum> : AbstractEditorPrefsValue<TEnum> where TEnum : struct, Enum {
		public EditorPrefsEnum(string editorPrefsKey, TEnum defaultValue = default(TEnum), bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue, isProjectSpecific) { }

		public override TEnum Value {
			get {
				var intValue = EditorPrefs.GetInt(ActualKey, Convert.ToInt32(DefaultValue));
				return (TEnum)Enum.ToObject(typeof(TEnum), intValue);
			}
			set => EditorPrefs.SetInt(ActualKey, Convert.ToInt32(value));
		}
	}
}
using System;
using UnityEditor;

namespace CommonUtils.Editor {
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

	public class EditorPrefsString : AbstractEditorPrefsValue<string> {
		public EditorPrefsString(string editorPrefsKey, bool isProjectSpecific = false) : base(editorPrefsKey, isProjectSpecific) { }
		public EditorPrefsString(string editorPrefsKey, string defaultValue, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue, isProjectSpecific) { }
		public EditorPrefsString(string editorPrefsKey, Func<string> defaultValueDelegate, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValueDelegate, isProjectSpecific) { }

		public override string Value {
			get => EditorPrefs.GetString(ActualKey, GetDefaultValue());
			set => EditorPrefs.SetString(ActualKey, value);
		}
	}

	public class EditorPrefsInt : AbstractEditorPrefsValue<int> {
		public EditorPrefsInt(string editorPrefsKey, bool isProjectSpecific = false) : base(editorPrefsKey, isProjectSpecific) { }
		public EditorPrefsInt(string editorPrefsKey, int defaultValue, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue, isProjectSpecific) { }
		public EditorPrefsInt(string editorPrefsKey, Func<int> defaultValueDelegate, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValueDelegate, isProjectSpecific) { }

		public override int Value {
			get => EditorPrefs.GetInt(ActualKey, GetDefaultValue());
			set => EditorPrefs.SetInt(ActualKey, value);
		}
	}

	public class EditorPrefsEnum<TEnum> : AbstractEditorPrefsValue<TEnum> where TEnum : struct, Enum {
		public EditorPrefsEnum(string editorPrefsKey, bool isProjectSpecific = false) : base(editorPrefsKey, isProjectSpecific) { }
		public EditorPrefsEnum(string editorPrefsKey, TEnum defaultValue, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue, isProjectSpecific) { }
		public EditorPrefsEnum(string editorPrefsKey, Func<TEnum> defaultValueDelegate, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValueDelegate, isProjectSpecific) { }

		public override TEnum Value {
			get {
				var intValue = EditorPrefs.GetInt(ActualKey, Convert.ToInt32(GetDefaultValue()));
				return (TEnum)Enum.ToObject(typeof(TEnum), intValue);
			}
			set => EditorPrefs.SetInt(ActualKey, Convert.ToInt32(value));
		}
	}
}
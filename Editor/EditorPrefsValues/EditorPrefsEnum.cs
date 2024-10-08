using System;
using UnityEditor;

namespace CommonUtils.Editor.EditorPrefsValues {
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
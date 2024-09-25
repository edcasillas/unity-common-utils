using System;
using UnityEditor;

namespace CommonUtils.Editor.EditorPrefsValues {
	public class EditorPrefsString : AbstractEditorPrefsValue<string> {
		public EditorPrefsString(string editorPrefsKey, bool isProjectSpecific = false) : base(editorPrefsKey, isProjectSpecific) { }
		public EditorPrefsString(string editorPrefsKey, string defaultValue, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue, isProjectSpecific) { }
		public EditorPrefsString(string editorPrefsKey, Func<string> defaultValueDelegate, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValueDelegate, isProjectSpecific) { }

		public override string Value {
			get => EditorPrefs.GetString(ActualKey, GetDefaultValue());
			set => EditorPrefs.SetString(ActualKey, value);
		}
	}
}
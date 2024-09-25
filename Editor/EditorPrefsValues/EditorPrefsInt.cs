using System;
using UnityEditor;

namespace CommonUtils.Editor.EditorPrefsValues
{
	public class EditorPrefsInt : AbstractEditorPrefsValue<int> {
		public EditorPrefsInt(string editorPrefsKey, bool isProjectSpecific = false) : base(editorPrefsKey, isProjectSpecific) { }
		public EditorPrefsInt(string editorPrefsKey, int defaultValue, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValue, isProjectSpecific) { }
		public EditorPrefsInt(string editorPrefsKey, Func<int> defaultValueDelegate, bool isProjectSpecific = false) : base(editorPrefsKey, defaultValueDelegate, isProjectSpecific) { }

		public override int Value {
			get => EditorPrefs.GetInt(ActualKey, GetDefaultValue());
			set => EditorPrefs.SetInt(ActualKey, value);
		}
	}
}
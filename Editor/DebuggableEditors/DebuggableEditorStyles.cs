using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.DebuggableEditors
{
	public static class DebuggableEditorStyles {
		private static GUIStyle textAreaStyle;
		public static GUIStyle TextAreaStyle => textAreaStyle ??= new GUIStyle(EditorStyles.textArea) { wordWrap = true };
	}
}
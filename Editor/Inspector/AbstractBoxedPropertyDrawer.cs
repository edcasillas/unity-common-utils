using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector {
	public abstract class AbstractBoxedPropertyDrawer : PropertyDrawer {
		protected static readonly RectOffset BoxPadding = EditorStyles.helpBox.padding;

		protected virtual float PadSize { get; } = 2f;
		protected virtual float LineHeight { get; } = EditorGUIUtility.singleLineHeight;
		protected virtual float PaddedLine => PadSize + LineHeight;
		protected virtual float FooterHeight { get; } = 10f;

		protected abstract int GetLineCount(SerializedProperty property, GUIContent label);

		/// <summary>
		/// Ensure that what we draw in OnGUI always has the room it needs
		/// </summary>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			var lineCount = GetLineCount(property, label);
			return BoxPadding.vertical + LineHeight * lineCount + PadSize * (lineCount - 1) + FooterHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// Draw the Box Background
			position.height -= FooterHeight;
			GUI.Box(EditorGUI.IndentedRect(position), GUIContent.none, EditorStyles.helpBox);
			position        = BoxPadding.Remove(position);
			position.height = LineHeight;

			DrawBoxContents(position, property, label);
		}

		protected abstract void DrawBoxContents(Rect position, SerializedProperty property, GUIContent label);
	}
}

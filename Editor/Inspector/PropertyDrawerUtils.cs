using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.Inspector.SceneRefs {
	public static class PropertyDrawerUtils {
		/// <summary>
		/// Draw a GUI button, choosing between a short and a long button text based on if it fits
		/// </summary>
		public static bool ButtonHelper(Rect   position, string msgShort, string msgLong, GUIStyle style, string tooltip = null) {
			var content = new GUIContent(msgLong);
			content.tooltip = tooltip;

			var longWidth = style.CalcSize(content).x;
			if (longWidth > position.width)
				content.text = msgShort;

			return GUI.Button(position, content, style);
		}

		/// <summary>
		/// Given a position rect, get its field portion
		/// </summary>
		public static Rect GetFieldRect(Rect position) {
			position.width -= EditorGUIUtility.labelWidth;
			position.x     += EditorGUIUtility.labelWidth;
			return position;
		}

		/// <summary>
		/// Given a position rect, get its label portion
		/// </summary>
		public static Rect GetLabelRect(Rect position, float padSize) {
			position.width = EditorGUIUtility.labelWidth - padSize;
			return position;
		}
	}
}
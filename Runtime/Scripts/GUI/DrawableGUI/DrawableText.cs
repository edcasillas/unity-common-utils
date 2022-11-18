using UnityEngine;
using System.Collections;

namespace DrawableGUI {
	/// <summary>
	/// Text to be drawn in GUI.
	/// </summary>
	[System.Serializable]
	public class DrawableText {
		public string text;
		public bool usePreview = false;
		public string preview;
		public Vector2 Position;
		public bool Gizmo;
		public bool Autosize = true;
		public Vector2 Size;

		/// <summary>
		/// Draws the text in a label.
		/// </summary>
		/// <param name="origin">Origin of parent window.</param>
		/// <param name="style">Style.</param>
		public void Draw(Vector2 origin, GUIStyle style) {
			string realTextToDraw = text;// Localization.Get (text);
			Vector2 size;
			if(Autosize) {
				size = style.CalcSize(new GUIContent(realTextToDraw));			// Calculates the size in pixels of the label.
			} else {
				size = Size;
			}
			Rect rect = new Rect(
				            origin.x + Position.x,
				            origin.y + Position.y, 
				            size.x, 
				            size.y);
			if(!Application.isPlaying && Gizmo)
				GUI.Box(rect, "");
			GUI.Label(rect, realTextToDraw, style);
		}
	}
}
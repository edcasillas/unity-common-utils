﻿using UnityEngine;
using System.Collections;

namespace GUIMaquetter {
	/// <summary>
	/// Base class for GUIItems.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public abstract class GUIItem{
		/// <summary>
		/// The name of the item.
		/// </summary>
		public string Name;
		/// <summary>
		/// The rect that defines position and size of the item.
		/// </summary>
		public Rect Rect;

		[SerializeField] protected bool AutoSize;

		public abstract void Draw();

		protected virtual void ExecuteAutoSize(){}
	}

	/// <summary>
	/// Base class for GUIItems that contain text.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public abstract class GUIItemText : GUIItem {
		/// <summary>
		/// The text of the item.
		/// </summary>
		public string Text;
	}

	/// <summary>
	/// Base class for GUIItems that contain editable text.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public abstract class GUIItemEditableText : GUIItemText {
		/// <summary>
		/// When true, enables the user to edit the text when the application is playing.
		/// </summary>
		public bool EnableUserEdit;
	}

	/// <summary>
	/// Base class for GUIItems that may contain text, images and/or tooltips.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public abstract class GUIItemContent : GUIItem {
		/// <summary>
		/// The content which will fill the item.
		/// </summary>
		public GUIContent Content;
	}

	/// <summary>
	/// Box maquette.
	/// </summary>
	[System.Serializable]
	public class Box : GUIItemContent {
		public override void Draw() {
			if (AutoSize) {
				var sizeOfContent = GUI.skin.box.CalcSize(Content);
				Rect.width = sizeOfContent.x;
				Rect.height = sizeOfContent.y;
			}
			GUI.Box(Rect, Content);
		}
	}

	/// <summary>
	/// Label maquette.
	/// </summary>
	[System.Serializable]
	public class Label : GUIItemText {
		public override void Draw() {
			if (AutoSize) {
				var sizeOfContent = GUI.skin.label.CalcSize(new GUIContent(Text));
				Rect.width = sizeOfContent.x;
				Rect.height = sizeOfContent.y;
			}
			GUI.Label(Rect, Text);
		}
	}

	/// <summary>
	/// Text field maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class TextField : GUIItemEditableText {
		public override void Draw() {
			if (AutoSize) {
				var sizeOfContent = GUI.skin.textField.CalcSize(new GUIContent(Text));
				Rect.width = sizeOfContent.x;
				Rect.height = sizeOfContent.y;
			}

			var editedText = GUI.TextField (Rect, Text);
			if(EnableUserEdit) Text = editedText;
		}
	}

	/// <summary>
	/// Text area maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class TextArea : GUIItemEditableText {
		public override void Draw() {
			if (AutoSize) {
				var sizeOfContent = GUI.skin.textArea.CalcSize(new GUIContent(Text));
				Rect.width = sizeOfContent.x;
				Rect.height = sizeOfContent.y;
			}

			var editedText = GUI.TextArea(Rect,Text);
			if(EnableUserEdit) Text = editedText;
		}
	}

	/// <summary>
	/// Button maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class Button : GUIItemContent {
		/// <summary>
		/// Specifies whether the button was pressed.
		/// </summary>
		public bool IsPressed { get; set; }

		public override void Draw() {
			if (AutoSize) {
				var sizeOfContent = GUI.skin.button.CalcSize(Content);
				Rect.width = sizeOfContent.x;
				Rect.height = sizeOfContent.y;
			}
			IsPressed = GUI.Button(Rect, Content);
		}
	}
}
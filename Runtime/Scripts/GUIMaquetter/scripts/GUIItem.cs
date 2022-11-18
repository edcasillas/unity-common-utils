using UnityEngine;
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
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class Box : GUIItemContent {}

	/// <summary>
	/// Label maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class Label : GUIItemText {}

	/// <summary>
	/// Text field maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class TextField : GUIItemEditableText {}

	/// <summary>
	/// Text area maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class TextArea : GUIItemEditableText {}

	/// <summary>
	/// Button maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class Button : GUIItemContent {
		/// <summary>
		/// Specifies whether the button was pressed.
		/// </summary>
		[HideInInspector]
		public bool IsPressed = false;
	}
}
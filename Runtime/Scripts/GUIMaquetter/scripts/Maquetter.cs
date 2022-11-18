using UnityEngine;
using System.Collections.Generic;

namespace GUIMaquetter{
	/// <summary>
	/// The class that will make your GUI prototyping easy!
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class Maquetter {
		/// <summary>
		/// Size to stretch the GUI.
		/// </summary>
		public Vector2 SizeOfGUI = new Vector2(800f, 600f);
		/// <summary>
		/// List of boxes to be drawn.
		/// </summary>
		public List<Box> Boxes;
		/// <summary>
		/// List of labels to be drawn.
		/// </summary>
		public List<Label> Labels;
		/// <summary>
		/// List of buttons to be drawn.
		/// </summary>
		public List<Button> Buttons;
		/// <summary>
		/// List of combo boxes to be drawn.
		/// </summary>
		public List<ComboBox> ComboBoxes;
		/// <summary>
		/// List of text fields to be drawn.
		/// </summary>
		public List<TextField> TextFields;
		/// <summary>
		/// List of text areas to be drawn.
		/// </summary>
		public List<TextArea> TextAreas;
		/// <summary>
		/// List of images to be drawn.
		/// </summary>
		public List<Image> Images;

		/// <summary>
		/// Draws all the GUI items.
		/// Call this method at the start of the OnGUI method in the GUIMaquetter handler.
		/// </summary>
		public void Draw(){
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
			                           new Vector3(Screen.width/SizeOfGUI.x,Screen.height/SizeOfGUI.y, 1));

			GUI.BeginGroup (new Rect(0f,0f,SizeOfGUI.x, SizeOfGUI.y));

			if(Boxes != null) foreach(Box box in Boxes){ box.Draw(); }
			if(Labels != null) foreach(Label label in Labels){ label.Draw(); }
			if(Buttons != null) foreach(Button button in Buttons){ button.Draw(); }
			if(ComboBoxes != null) foreach(ComboBox combo in ComboBoxes){ combo.Draw(); }
			if(TextFields != null) foreach(TextField textField in TextFields) { textField.Draw(); }
			if(TextAreas != null) foreach(TextArea textArea in TextAreas){ textArea.Draw(); }
			if(Images != null) foreach(Image image in Images){ image.Draw (); }

			GUI.EndGroup ();
		}

		/// <summary>
		/// Gets the box with the given name.
		/// </summary>
		/// <returns>The box.</returns>
		/// <param name="name">Name.</param>
		public Box GetBox(string name){ return Boxes.Find (o => o.Name == name); }
		/// <summary>
		/// Gets the label with the given name.
		/// </summary>
		/// <returns>The label.</returns>
		/// <param name="name">Name.</param>
		public Label GetLabel(string name) {return Labels.Find (o => o.Name == name); }
		/// <summary>
		/// Gets the button with the given name.
		/// </summary>
		/// <returns>The button.</returns>
		/// <param name="name">Name.</param>
		public Button GetButton(string name) {return Buttons.Find (o => o.Name == name); }
		/// <summary>
		/// Gets the combo box with the given name.
		/// </summary>
		/// <returns>The combo box.</returns>
		/// <param name="name">Name.</param>
		public ComboBox GetComboBox(string name) {return ComboBoxes.Find (o => o.Name == name); }
		/// <summary>
		/// Gets the text field with the given name.
		/// </summary>
		/// <returns>The text field.</returns>
		/// <param name="name">Name.</param>
		public TextField GetTextField(string name) {return TextFields.Find (o => o.Name == name); }
		/// <summary>
		/// Gets the text area with the given name.
		/// </summary>
		/// <returns>The text area.</returns>
		/// <param name="name">Name.</param>
		public TextArea GetTextArea(string name) {return TextAreas.Find (o => o.Name == name); }
		/// <summary>
		/// Gets the image with the given name.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="name">Name.</param>
		public Image GetImage(string name) {return Images.Find (o => o.Name == name); }
	}
}
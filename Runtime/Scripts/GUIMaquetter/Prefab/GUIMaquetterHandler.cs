using UnityEngine;
using GUIMaquetter;

[ExecuteInEditMode]
public class GUIMaquetterHandler : MonoBehaviour {
	/// <summary>
	/// This is the main maquetter object, which will draw all of your GUI Items.
	/// </summary>
	public Maquetter maquetter;

	// Declare here the GUIItems you'll use in your code. Here's a little example:
	// private TextField aTextField;

	void Start () {
		// Always initialize your GUI items when starting.
		initializeGUIMaquetterItems();
	}

	void OnGUI(){
		// Call maquetter.Draw() always at the beginning of OnGUI.
		maquetter.Draw (Event.current);

		// Now you can play with your maquette. Here's a little example:
		// aTextField.Text = "Hello world!";
	}

	private void initializeGUIMaquetterItems(){
		// Initialize the GUIItems you'll be using in your code.
		// Here's a little example:
		// aTextField = maquetter.GetTextField ("nameOfTheTextField");
	}
}
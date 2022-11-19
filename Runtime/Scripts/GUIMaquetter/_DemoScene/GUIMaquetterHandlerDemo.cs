using UnityEngine;
using GUIMaquetter;

/// <summary>
/// GUI maquetter handler demo.
/// </summary>
[ExecuteInEditMode]
public class GUIMaquetterHandlerDemo : MonoBehaviour {
	/// <summary>
	/// This is the main maquetter object, which will draw all of your GUI Items.
	/// </summary>
	public Maquetter maquetter;

	// The following declarations are used to access the GUIItems from code.
	private TextField txtEdit;
	private Button btnPress;
	private Label lblChange;

	private void Start() {
		initializeGUIMaquetterItems();

		// Always initialize your GUI items when starting.
	}

	private void OnGUI(){
		// Call maquetter.Draw() always at the beginning of OnGUI.
		maquetter.Draw(Event.current);

		// Now you can play with your maquette. Here's a little example:
		if(btnPress.IsPressed){
			lblChange.Text = txtEdit.Text;
		}
	}

	private void initializeGUIMaquetterItems(){
		// Initialize the GUIItems you'll be using in your code.
		// Here's a little example:
		txtEdit = maquetter.GetTextField ("txtEdit");
		btnPress = maquetter.GetButton ("btnPress");
		lblChange = maquetter.GetLabel ("lblChange");
	}
}
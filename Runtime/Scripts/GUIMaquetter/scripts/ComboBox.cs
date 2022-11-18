using UnityEngine;
using System.Collections;

namespace GUIMaquetter {
	/// <summary>
	/// Combo Box maquette.
	/// </summary>
	/// <remarks>Edited from http://wiki.unity3d.com/index.php/PopupList</remarks>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class ComboBox : GUIItem {
		public GUIContent[] ListContent;

		private static bool forceToUnShow = false; 
		private static int useControlID = -1;
		private bool isClickedComboButton = false;
		private int selectedItemIndex = 0;
	
		private string buttonStyle = "button";
		private string boxStyle = "box";
		
		public int Show() {
			if( forceToUnShow ) {
				forceToUnShow = false;
				isClickedComboButton = false;
			}
			
			bool done = false;
			int controlID = GUIUtility.GetControlID( FocusType.Passive );       
			
			switch( Event.current.GetTypeForControl(controlID) ) {
			case EventType.MouseUp: {
					if( isClickedComboButton ) {
						done = true;
					}
				}
				break;
			}       
			
			if( GUI.Button(Rect, (ListContent!=null && ListContent.Length>0) ? ListContent[selectedItemIndex] : new GUIContent(Name), buttonStyle ) ) {
				if( useControlID == -1 ) {
					useControlID = controlID;
					isClickedComboButton = false;
				}
				
				if( useControlID != controlID ) {
					forceToUnShow = true;
					useControlID = controlID;
				}
				isClickedComboButton = true;
			}

			if( isClickedComboButton ) {
				if(ListContent!=null && ListContent.Length>0){
					Rect listRect = new Rect( Rect.x, Rect.y + Rect.height,
					                         Rect.width, Rect.height * ListContent.Length );
					
					GUI.Box( listRect, "", boxStyle );
					int newSelectedItemIndex = GUI.SelectionGrid( listRect, selectedItemIndex, ListContent, 1, GUI.skin.label );
					if( newSelectedItemIndex != selectedItemIndex )
						selectedItemIndex = newSelectedItemIndex;
				}
			}
			
			if( done )
				isClickedComboButton = false;
			
			return selectedItemIndex;
		}
		
		public int SelectedItemIndex{
			get{ return selectedItemIndex; }
			set{ selectedItemIndex = value; }
		}
	}
}
using UnityEngine;
using System.Collections;

namespace GUIMaquetter {
	/// <summary>
	/// Image maquette.
	/// </summary>
	/// <author>Eduardo Casillas</author>
	[System.Serializable]
	public class Image : GUIItem {
		/// <summary>
		/// The texture to be drawn.
		/// </summary>
		public Texture2D Texture;
		
		/// <summary>
		/// Draws the image calculating it's size.
		/// </summary>
		public void Draw(){
			try {
				if(Texture!=null){
					Rect = new Rect(
						Rect.x, 
						Rect.y,
						(Texture != null ? Texture.width : 0), 
						(Texture != null ? Texture.height : 0));
					GUI.DrawTexture(Rect,Texture);
				}
			} catch (System.Exception) {
				string errorDetails = 
					(Texture == null ? "Texture is null; " : "Texture exists; ") + 
						"Rect = " + Rect.ToString() +".";
				Debug.Log ("Texture: Could not draw the texture. Details: " + errorDetails);
			}
		}
	}
}
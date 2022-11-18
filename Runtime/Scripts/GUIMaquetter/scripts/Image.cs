using UnityEngine;
using System.Collections;

namespace GUIMaquetter {
	/// <summary>
	/// Image maquette.
	/// </summary>
	[System.Serializable]
	public class Image : GUIItem {
		/// <summary>
		/// The texture to be drawn.
		/// </summary>
		public Texture2D Texture;

		/// <summary>
		/// Draws the image calculating it's size.
		/// </summary>
		public override void Draw(){
			try {
				if(Texture){
					Rect = new Rect(
						Rect.x,
						Rect.y,
						(Texture && AutoSize ? Texture.width : Rect.width),
						(Texture && AutoSize ? Texture.height : Rect.height));
					GUI.DrawTexture(Rect,Texture);
				}
			} catch (System.Exception) {
				var errorDetails =
					(!Texture ? "Texture is null; " : "Texture exists; ") +
						"Rect = " + Rect +".";
				Debug.Log ("Texture: Could not draw the texture. Details: " + errorDetails);
			}
		}
	}
}
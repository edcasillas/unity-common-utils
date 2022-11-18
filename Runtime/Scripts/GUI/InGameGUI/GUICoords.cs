using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InGameGUI {
	public enum AspectRatio {
		PORTRAIT_16_9=0,  	//1280×720
		PORTRAIT_16_10,   	//1280x800
		PORTRAIT_4_3,		//1280x960
		PORTRAIT_3_2,		//1152x768
		LANDSCAPE_16_9,		//1280×720
		LANDSCAPE_16_10,	//1280x800
		LANDSCAPE_4_3,		//1280x960
		LANDSCAPE_3_2,		//1152x768
		SQUARE_1_1			//1024x1024	
	}
	
	// TODO: Make static / Reuse
	public static class GUICoords{
		private static AspectRatio currentAspectRatio;
		private static Vector2 guiSize;
		private static Vector2 lastScreenSize;
		
		static GUICoords(){
			lastScreenSize = new Vector2(Screen.width,Screen.height);
			setCurrentAspecRatio ();
			setWidthAndHeight (currentAspectRatio);
		}
		
		private static void setCurrentAspecRatio() {
			float realAspectFactor = ((float) Screen.width /(float) Screen.height);
			List <float> list = new List <float> { (16f/9f),(16f/10f),(4f/3f),(1f/1f),(3f/4f),(10f/16f),(9f/16f),(3/2),(2/3) };
			float nearAspectFactor = list.Aggregate((x, y) => Mathf.Abs(x - realAspectFactor) < Mathf.Abs(y - realAspectFactor) ? x : y);
			
			if (nearAspectFactor == (16f / 9f)) currentAspectRatio = AspectRatio.LANDSCAPE_16_9;
			else if (nearAspectFactor == (16f / 10f)) currentAspectRatio = AspectRatio.LANDSCAPE_16_10;
			else if (nearAspectFactor == (4f / 3f)) currentAspectRatio = AspectRatio.LANDSCAPE_4_3;
			else if (nearAspectFactor == (1f / 1f)) currentAspectRatio = AspectRatio.SQUARE_1_1;
			else if (nearAspectFactor == (10f / 16f)) currentAspectRatio = AspectRatio.PORTRAIT_16_10;
			else if (nearAspectFactor == (9f / 16f)) currentAspectRatio = AspectRatio.PORTRAIT_16_9;
			else if (nearAspectFactor == (3f / 4f)) currentAspectRatio = AspectRatio.PORTRAIT_4_3;
			else if (nearAspectFactor == (3f / 2f)) currentAspectRatio = AspectRatio.LANDSCAPE_3_2;
			else if (nearAspectFactor == (2f / 3f)) currentAspectRatio = AspectRatio.PORTRAIT_3_2;
		}
		
		private static void setWidthAndHeight (AspectRatio aspectRatio) {
			switch (aspectRatio) {
			case AspectRatio.PORTRAIT_16_9: guiSize = new Vector2(720f, 1280f); break;
			case AspectRatio.PORTRAIT_16_10: guiSize = new Vector2(800f, 1280f); break;
			case AspectRatio.PORTRAIT_4_3: guiSize = new Vector2(960f, 1280f); break;
			case AspectRatio.PORTRAIT_3_2: guiSize = new Vector2(768f, 1152f); break;
			case AspectRatio.LANDSCAPE_16_9: guiSize = new Vector2(1280f, 720f); break;
			case AspectRatio.LANDSCAPE_16_10: guiSize = new Vector2(1280f, 800f); break;
			case AspectRatio.LANDSCAPE_4_3: guiSize = new Vector2(1280f, 960f); break;
			case AspectRatio.LANDSCAPE_3_2: guiSize = new Vector2(1152f, 768f); break;
			case AspectRatio.SQUARE_1_1: guiSize = new Vector2(1024f, 1024f); break;
			}
		}
		
		public static void AdjustGUIMatrix(){
			Vector2 screenSize = new Vector2(Screen.width,Screen.height);
			if(Vector2.Distance(screenSize,lastScreenSize)!=0){
				setCurrentAspecRatio ();
				setWidthAndHeight (currentAspectRatio);
				lastScreenSize = screenSize;
				Debug.Log ("GUICoords: Screen resized: [Aspect: " + currentAspectRatio.ToString () + 
				           "; Screen: " + screenSize.ToString () + "; GUI: " + guiSize.ToString() + "]");
			}
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width/guiSize.x,Screen.height/guiSize.y, 1));
		}
		
		public static float GetCoordX(float percentaje){ return guiSize.x * percentaje; }
		public static float GetCoordY(float percentaje){ return guiSize.y * percentaje; }
	}
}
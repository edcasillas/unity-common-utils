using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CommonUtils.LegacyGUI {
	public enum AspectRatio {
		PORTRAIT_16_9=0,  	//eg 1280×720
		PORTRAIT_16_10,   	//eg 1280x800
		PORTRAIT_4_3,		//eg 1280x960
		PORTRAIT_3_2,		//eg 1152x768
		LANDSCAPE_16_9,		//eg 1280×720
		LANDSCAPE_16_10,	//eg 1280x800
		LANDSCAPE_4_3,		//eg 1280x960
		LANDSCAPE_3_2,		//eg 1152x768
		SQUARE_1_1			//eg 1024x1024
	}

	public interface IGUICoords {
		Vector2 LastScreenSize { get; }
		AspectRatio CurrentAspectRatio { get; }
		Vector2 GUISize { get; }
		void AdjustGUIMatrix();
		float GetCoordX(float percentage);
		float GetCoordY(float percentage);
		Vector2 GetCoords(Vector2 percentage);
	}

	public class GUICoords : IGUICoords {
		public static GUICoords Instance { get; }

		static GUICoords() => Instance = new GUICoords();

		public Vector2 LastScreenSize { get; private set; }
		public AspectRatio CurrentAspectRatio { get; private set; }
		public Vector2 GUISize { get; private set; }

		private GUICoords() {
			LastScreenSize = new Vector2(Screen.width,Screen.height);
			CurrentAspectRatio = GetCurrentAspectRatio();
			GUISize = GetWidthAndHeightForAspectRatio(CurrentAspectRatio);
		}

		private AspectRatio GetCurrentAspectRatio() {
			var realAspectFactor = ((float) Screen.width /(float) Screen.height);
			var list = new List <float> { (16f/9f),(16f/10f),(4f/3f),(1f/1f),(3f/4f),(10f/16f),(9f/16f),(3/2),(2/3) };
			var nearAspectFactor = list.Aggregate((x, y) => Mathf.Abs(x - realAspectFactor) < Mathf.Abs(y - realAspectFactor) ? x : y);

			return nearAspectFactor switch {
				16f / 9f => AspectRatio.LANDSCAPE_16_9,
				16f / 10f => AspectRatio.LANDSCAPE_16_10,
				4f / 3f => AspectRatio.LANDSCAPE_4_3,
				1f / 1f => AspectRatio.SQUARE_1_1,
				10f / 16f => AspectRatio.PORTRAIT_16_10,
				9f / 16f => AspectRatio.PORTRAIT_16_9,
				3f / 4f => AspectRatio.PORTRAIT_4_3,
				3f / 2f => AspectRatio.LANDSCAPE_3_2,
				/* 2f / 3f */ _ => AspectRatio.PORTRAIT_3_2
			};
		}

		private Vector2 GetWidthAndHeightForAspectRatio(AspectRatio aspectRatio) => aspectRatio switch {
			AspectRatio.PORTRAIT_16_9 => new Vector2(720f, 1280f),
			AspectRatio.PORTRAIT_16_10 => new Vector2(800f, 1280f),
			AspectRatio.PORTRAIT_4_3 => new Vector2(960f, 1280f),
			AspectRatio.PORTRAIT_3_2 => new Vector2(768f, 1152f),
			AspectRatio.LANDSCAPE_16_9 => new Vector2(1280f, 720f),
			AspectRatio.LANDSCAPE_16_10 => new Vector2(1280f, 800f),
			AspectRatio.LANDSCAPE_4_3 => new Vector2(1280f, 960f),
			AspectRatio.LANDSCAPE_3_2 => new Vector2(1152f, 768f),
			AspectRatio.SQUARE_1_1 => new Vector2(1024f, 1024f),
			_ => throw new ArgumentOutOfRangeException($"{aspectRatio} is not a valid value")
		};

		public void AdjustGUIMatrix() {
			var screenSize = new Vector2(Screen.width,Screen.height);
			if(Vector2.Distance(screenSize,LastScreenSize)!=0){
				CurrentAspectRatio = GetCurrentAspectRatio();
				GUISize = GetWidthAndHeightForAspectRatio(CurrentAspectRatio);
				LastScreenSize = screenSize;
				Debug.Log ($"GUICoords: Screen resized: [Aspect: {CurrentAspectRatio}; Screen: {screenSize}; GUI: {GUISize}]");
			}
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width/GUISize.x,Screen.height/GUISize.y, 1));
		}

		public float GetCoordX(float percentage) => GUISize.x * percentage;
		public float GetCoordY(float percentage) => GUISize.y * percentage;
		public Vector2 GetCoords(Vector2 percentage) => new Vector2(GetCoordX(percentage.x), GetCoordY(percentage.y));
	}
}
using System;

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

	public enum AspectRatioOrientation {
		PORTRAIT, LANDSCAPE, SQUARE
	}

	public static class AspectRatioExtensions {
		public static AspectRatioOrientation GetOrientation(this AspectRatio ratio) {
			switch (ratio) {
				case AspectRatio.PORTRAIT_16_9:
				case AspectRatio.PORTRAIT_16_10:
				case AspectRatio.PORTRAIT_4_3:
				case AspectRatio.PORTRAIT_3_2:
					return AspectRatioOrientation.PORTRAIT;
				case AspectRatio.LANDSCAPE_16_9:
				case AspectRatio.LANDSCAPE_16_10:
				case AspectRatio.LANDSCAPE_4_3:
				case AspectRatio.LANDSCAPE_3_2:
					return AspectRatioOrientation.LANDSCAPE;
				case AspectRatio.SQUARE_1_1:
					return AspectRatioOrientation.SQUARE;
				default:
					throw new ArgumentOutOfRangeException(nameof(ratio), ratio, null);
			}
		}
	}
}
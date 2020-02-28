using UnityEngine;

namespace CommonUtils.Extensions {
	public static class CanvasExtensions {
		/// <summary>
		/// Calculates the inverse size (1/size) of the specified <paramref name="rectTransform"/> in the current <paramref name="canvas"/>.
		/// </summary>
		/// <param name="canvas">Canvas for which to calculate the size.</param>
		/// <param name="rectTransform">Rect transform to calculate its size.</param>
		/// <returns>Inverse size of the <paramref name="rectTransform"/></returns>
		public static Vector2 GetInverseSize(this Canvas canvas, RectTransform rectTransform) {
			var canvasScaleFactor = canvas.scaleFactor;
			var sizeDelta         = rectTransform.sizeDelta;
			var localScale        = rectTransform.localScale;
			return new Vector2(1.0f / (sizeDelta.x * canvasScaleFactor * localScale.x),
							   1.0f / (sizeDelta.y * canvasScaleFactor * localScale.y));
		}
	}
}
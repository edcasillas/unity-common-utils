using UnityEngine;

namespace CommonUtils.Extensions {
	public static class RectTransformExtensions {
		/// <summary>
		///   <para>Returns true if the <paramref name="other"/> RectTransform overlaps this one.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		public static bool Overlaps(this RectTransform rect, RectTransform other) => rect.WorldRect().Overlaps(other.WorldRect());

		/// <summary>
		///   <para>Returns true if the <paramref name="other"/> RectTransform overlaps this one. If <paramref name="allowInverse"/> is true, the widths and heights of the Rects are allowed to take negative values (ie, the min value is greater than the max), and the test will still work.</para>
		/// </summary>
		/// <param name="other">Other rectangle to test overlapping with.</param>
		/// <param name="allowInverse">Does the test allow the widths and heights of the Rects to be negative?</param>
		public static bool Overlaps(this RectTransform rect, RectTransform other, bool allowInverse) => rect.WorldRect().Overlaps(other.WorldRect(), allowInverse);

		public static Rect WorldRect(this RectTransform rectTransform) {
			var sizeDelta           = rectTransform.sizeDelta;
			var rectTransformWidth  = sizeDelta.x * rectTransform.lossyScale.x;
			var rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

			var position = rectTransform.position;
			return new Rect(position.x - rectTransformWidth  / 2f,
							position.y - rectTransformHeight / 2f,
							rectTransformWidth,
							rectTransformHeight);
		}
		
		public static Rect ToScreenSpace(this RectTransform transform) // TODO This might be a duplicate of RectTransformExtensions.WorldRect
		{
			Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
			Rect    rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
			rect.x -= (transform.pivot.x          * size.x);
			rect.y -= ((1.0f - transform.pivot.y) * size.y);
			return rect;
		}
	}
}
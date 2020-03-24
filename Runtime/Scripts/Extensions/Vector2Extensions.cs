using UnityEngine;

namespace CommonUtils.Extensions {
	public static class Vector2Extensions {
		public static Vector2 RotateAround(this Vector2 point, Vector2 origin, float theta) {
			var s = Mathf.Sin(theta);
			var c = Mathf.Cos(theta);

			// translate point back to origin:
			point.x -= origin.x;
			point.y -= origin.y;

			// rotate point
			var xnew = point.x * c - point.y * s;
			var ynew = point.x * s + point.y * c;

			// translate point back:
			point.x = xnew + origin.x;
			point.y = ynew + origin.y;
			return point;
		}
	}
}
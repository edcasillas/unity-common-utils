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

		/// <summary>
		/// Returns a <see cref="Vector2"/> from a well-formatted string representing a vector2: ({0}, {1}).
		/// In the case of a null, empty or malformed input, the <paramref name="defaultValue"/> is returned.
		/// </summary>
		public static Vector2 ToVector2(this string inputString, Vector2 defaultValue = default) {
			if (string.IsNullOrEmpty(inputString)) return defaultValue;
			var trimmed = inputString
				.Replace("(", string.Empty)
				.Replace(")", string.Empty)
				.Replace(" ", string.Empty);
			var components = trimmed.Split(',');
			if (components.Length != 2) return defaultValue;

			if (!float.TryParse(components[0], out var x)) return defaultValue;
			if (!float.TryParse(components[1], out var y)) return defaultValue;
			return new Vector2(x, y);
		}

		public static Vector3 ToVector3(this Vector2 vector2, float zValue = 0) => new Vector3(vector2.x, vector2.y, zValue);
	}
}
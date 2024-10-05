using UnityEngine;

namespace CommonUtils.LegacyGUI
{
	/// <summary>
	/// Provides methods to draw lines in the GUI.
	/// </summary>
	public class GUILines {
		private static Texture2D _antiAliasedTexture;
		/// <summary> Texture used for anti-aliased lines. </summary>
		private static Texture2D antiAliasedTexture {
			get {
				if (!_antiAliasedTexture) _antiAliasedTexture = initLineTexture(true);
				return _antiAliasedTexture;
			}
		}

		private static Texture2D _simpleTexture;
		/// <summary> Texture used for simple lines. </summary>
		private static Texture2D simpleTexture {
			get {
				if (!_simpleTexture) _simpleTexture = initLineTexture(false);
				return _simpleTexture;
			}
		}

		/// <summary>
		/// Draws a bezier line.
		/// </summary>
		/// <param name="start">The start point of the bezier curve.</param>
		/// <param name="startTangent">The tangent at the start point.</param>
		/// <param name="end">The end point of the bezier curve.</param>
		/// <param name="endTangent">The tangent at the end point.</param>
		/// <param name="color">The color of the line.</param>
		/// <param name="width">The width of the line.</param>
		/// <param name="antiAlias">If set to true, the line will be anti-aliased.</param>
		/// <param name="segments">The number of segments to divide the bezier curve into.</param>
		public static void DrawBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments) {
			var lastV = cubeBezier(start, startTangent, end, endTangent, 0);
			for (var i = 1; i <= segments; ++i) {
				var v = cubeBezier(start, startTangent, end, endTangent, i/(float)segments);

				DrawLine(
					lastV,
					v,
					color, width, antiAlias);
				lastV = v;
			}
		}

		/// <summary>
		/// Draws a line between two points.
		/// </summary>
		/// <param name="pointA">The start point of the line.</param>
		/// <param name="pointB">The end point of the line.</param>
		/// <param name="color">The color of the line.</param>
		/// <param name="width">The width of the line.</param>
		/// <param name="antiAlias">If set to true, the line will be anti-aliased.</param>
		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
		{
			Color savedColor = GUI.color;
			Matrix4x4 savedMatrix = GUI.matrix;

			if (antiAlias) width *= 3;
			float angle = Vector3.Angle(pointB - pointA, Vector2.right) * (pointA.y <= pointB.y?1:-1);
			float m = (pointB - pointA).magnitude;
			if (m > 0.01f)
			{
				Vector3 dz = new Vector3(pointA.x, pointA.y, 0);

				GUI.color = color;
				GUI.matrix = translationMatrix(dz) * GUI.matrix;
				GUIUtility.ScaleAroundPivot(new Vector2(m, width), new Vector3(-0.5f, 0, 0));
				GUI.matrix = translationMatrix(-dz) * GUI.matrix;
				GUIUtility.RotateAroundPivot(angle, Vector2.zero);
				GUI.matrix = translationMatrix(dz + new Vector3(width / 2, -m / 2) * Mathf.Sin(angle * Mathf.Deg2Rad)) * GUI.matrix;

				GUI.DrawTexture(new Rect(0, 0, 1, 1), antiAlias ? antiAliasedTexture : simpleTexture);
			}
			GUI.matrix = savedMatrix;
			GUI.color = savedColor;
		}

		/// <summary>
		/// Calculates the position on a cubic bezier curve.
		/// </summary>
		/// <param name="start">The start point of the bezier curve.</param>
		/// <param name="startTangent">The tangent at the start point.</param>
		/// <param name="end">The end point of the bezier curve.</param>
		/// <param name="endTangent">The tangent at the end point.</param>
		/// <param name="t">The interpolation factor (0 to 1).</param>
		/// <returns>The interpolated point on the bezier curve.</returns>
		private static Vector2 cubeBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, float t){
			float rt = 1-t;
			float rtt = rt * t;
			return rt*rt*rt * start + 3 * rt * rtt * startTangent + 3 * rtt * t * endTangent + t*t*t* end;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="v">The translation vector.</param>
		/// <returns>The translation matrix.</returns>
		private static Matrix4x4 translationMatrix(Vector3 v) => Matrix4x4.TRS(v,Quaternion.identity,Vector3.one);

		private static Texture2D initLineTexture(bool antiAlias) {
			var result = new Texture2D(1, antiAlias ? 3 : 1, TextureFormat.ARGB32, true);
			if(antiAlias) result.SetPixel(0, 0, new Color(1, 1, 1, 0));
			result.SetPixel(0, 1, Color.white);
			if(antiAlias) result.SetPixel(0, 2, new Color(1, 1, 1, 0));
			result.Apply();
			return result;
		}
	}
}

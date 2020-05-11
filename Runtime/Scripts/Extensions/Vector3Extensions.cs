using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CommonUtils.Extensions {
	public static class Vector3Extensions {
		public static Vector2 GetAnglesTo(this Vector3 referenceVector, Vector3 compareVector)
			=> new Vector2(-Mathf.Asin(Vector3.Cross(compareVector, referenceVector).y) * Mathf.Rad2Deg,
						   -Mathf.Asin(Vector3.Cross(compareVector, referenceVector).x) * Mathf.Rad2Deg);

		public static Vector3 RandomRange(Vector3 min, Vector3 max) => new Vector3(Random.Range(min.x, max.x),
																				   Random.Range(min.y, max.y),
																				   Random.Range(min.z, max.z));

		public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion rotation) => rotation * (point - pivot) + pivot;

		/// <summary>
		/// Translates, rotates and scales the <paramref name="vector"/> by the position, rotation and scale of the transform.
		/// </summary>
		/// <param name="vector">Vector to transform.</param>
		/// <param name="transform">Transform to be applied.</param>
		/// <returns>Transformed vector.</returns>
		public static Vector3 ApplyTransform(this Vector3 vector, Transform transform) => vector.Transform(transform.position, transform.rotation, transform.lossyScale);

		public static Vector3 Transform(this Vector3 vector, Vector3 position, Quaternion rotation, Vector3 scale) {
			vector = Vector3.Scale(vector, new Vector3(scale.x, scale.y, scale.z));
			vector = vector.RotateAround(Vector3.zero, rotation);
			vector += position;
			return vector;
		}

		public static Vector3 InverseApplyTransform(this Vector3 vector, Transform transform) => vector.InverseTransform(transform.position, transform.rotation, transform.lossyScale);

		public static Vector3 InverseTransform(this Vector3 vector, Vector3 position, Quaternion rotation, Vector3 scale) {
			vector -= position;
			vector = vector.RotateAround(Vector3.zero, Quaternion.Inverse(rotation));
			vector = Vector3.Scale(vector, new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z));
			return vector;
		}

		public static bool NearlyEquals(this Vector3 lhs, Vector3 rhs, double inaccuracy = 9.99999943962493E-11) => Vector3.SqrMagnitude(lhs - rhs) < inaccuracy;

		public static Vector3 MidPointTo(this Vector3 origin, Vector3 destination) => new Vector3(
																								  (origin.x + destination.x) /2,
																								  (origin.y + destination.y) /2,
																								  (origin.z + destination.z) /2
																								 );

		public static bool IsInside(this Vector3 vector, Collider collider) => vector == collider.ClosestPoint(vector);

		/// <summary>
		/// Transforms a <paramref name="worldPoint"/> seen by <paramref name="worldCamera"/> to a screen point within the specified <paramref name="canvas"/>.
		/// Supports only <see cref="RenderMode.ScreenSpaceCamera"/> and <see cref="RenderMode.ScreenSpaceOverlay"/> render modes for the <paramref name="canvas"/>.
		/// Returns <see cref="Vector3.negativeInfinity"/> if the <paramref name="worldPoint"/> is not in front of the <paramref name="worldCamera"/>.
		/// </summary>
		/// <param name="worldPoint">Vector3 to be transformed to a screen point.</param>
		/// <param name="worldCamera">Camera looking at the <paramref name="worldPoint"/>.</param>
		/// <param name="canvas">Target canvas of the screen point.</param>
		/// <returns>A Vector3 within the specified <paramref name="canvas"/> that is in the same screen position as the <paramref name="worldPoint"/>.</returns>
		/// <exception cref="NotImplementedException"><paramref name="canvas"/> has an unsupported RenderMode.</exception>
		/// <example>
		/// This sample shows how to call the <see cref="WorldToScreenPointInCanvas"/> method to set the position of a UI image.
		/// <code>
		/// var screenPoint = targetObject.transform.position.WorldToScreenPointInCanvas(Camera.main, canvas);
		/// if(screenPoint != Vector3.negativeInfinity) uiImage.RectTransform.position = screenPoint;
		/// </code>
		/// </example>
		public static Vector3 WorldToScreenPointInCanvas(this Vector3 worldPoint, Camera worldCamera, Canvas canvas) {
			var direction = worldPoint - worldCamera.transform.position;
			if (!(Vector3.Dot(worldCamera.transform.forward, direction) > 0.0f)) {
				return Vector3.negativeInfinity;
			}

			var screenPoint = worldCamera.WorldToScreenPoint(worldPoint);
			switch (canvas.renderMode) {
				case RenderMode.ScreenSpaceCamera:
					return MathUtils.ScreenPointToLocalPointInRectangle(canvas, position: screenPoint);
				case RenderMode.ScreenSpaceOverlay:
					return screenPoint;
				default:
					throw new NotImplementedException("RenderMode not Supported.");
			}
		}

		public static string ToStringVerbose(this Vector3 v) => $"({v.x}, {v.y}, {v.z})";
	}
}
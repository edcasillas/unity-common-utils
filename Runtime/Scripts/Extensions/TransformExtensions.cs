using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class TransformExtensions {
		/// <summary>
		/// Gets a value indicating whether the position of the <paramref name="transform"/> is inside the specified <paramref name="collider"/>.
		/// </summary>
		/// <param name="transform">Transform to check.</param>
		/// <param name="collider">Collider to check.</param>
		/// <returns><c>true</c> when the position of the <paramref name="transform"/> is inside the <paramref name="collider"/>; otherwise <c>false</c></returns>
		public static bool IsInside(this Transform transform, Collider collider) => transform.position.IsInside(collider);

		public static Transform[] GetChildren(this Transform transform) {
			var result = new Transform[transform.childCount];
			var i = 0;
			foreach (Transform child in transform) {
				if(child == transform) continue;
				result[i++] = child;
			}

			return result;
		}

		/// <summary>
		/// Removes all children from the specified <paramref name="transform"/>.
		/// </summary>
		/// <param name="transform">Parent transform from which to remove children.</param>
		/// <param name="where">Condition to filter children to be removed.</param>
		/// <returns>Count of removed children.</returns>
		/// <remarks>Please remember that in play mode, objects will be actually destroyed the next frame.</remarks>
		public static int RemoveChildren(this Transform transform, Func<Transform, bool> where = null) {
			if (!transform) return 0;
			if (where == null) where = o => true;

			var count = 0;

			foreach (Transform child in transform) {
				if (where(child)) {
#if UNITY_EDITOR
					if (!Application.isPlaying) {
						UnityEditor.Undo.DestroyObjectImmediate(child.gameObject);
					} else {
						GameObject.Destroy(child.gameObject);
					}
#else
					GameObject.Destroy(child.gameObject);
#endif
					count++;
				}
			}

			return count;
		}

		public static int Depth(this Transform transform) => TransformDepthCalculator.Instance.GetDepth(transform);

		/// <summary>
		/// Returns a value indicating whether the specified <paramref name="transform"/> is nearly looking at the direction where the specified <paramref name="point"/> is.
		/// </summary>
		/// <param name="transform">Transform to perform the check with.</param>
		/// <param name="point">Point to be tested.</param>
		/// <param name="angleCos">
		/// A value between 0 and 1 indicating the cosine of the angle between the transform's forward vector and the direction to the point.
		/// Higher values mean the forward transform should be closer to looking at the exact direction of the point.
		/// Negative values expect the point to be behind the transform.
		/// </param>
		/// <param name="projectOnSamePlane">Indicates whether this check should be done in 2D, ignoring the Y axis.</param>
		public static bool IsNearlyFacingTowards(this Transform transform, Vector3 point, float angleCos = 0.95f, bool projectOnSamePlane = false) {
			var transformPosition = transform.position;
			if (projectOnSamePlane) point = new Vector3(point.x, transformPosition.y, point.z);
			var directionToPoint = point - transformPosition;
			var dotProduct = Vector3.Dot(transform.forward, directionToPoint.normalized);
			return dotProduct >= angleCos;
		}

		#region Temporary reset transforms
		private class transformData {
			public Vector3 Position;
			public Quaternion Rotation;
		}

		private static readonly Dictionary<Transform, transformData> temporaryStoredTransformData = new Dictionary<Transform, transformData>();

		public static void TemporaryResetTransform(this Transform transform) {
			if (!temporaryStoredTransformData.ContainsKey(transform)) {
				temporaryStoredTransformData.Add(transform, new transformData());
			}
			temporaryStoredTransformData[transform].Position = transform.localPosition;
			temporaryStoredTransformData[transform].Rotation = transform.localRotation;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}

		public static void TemporaryUnResetTransform(this Transform transform) {
			if (!temporaryStoredTransformData.ContainsKey(transform)) {
				Debug.LogError($"There's no data stored for transform in Game Object '{transform.name}' to be reset.", transform);
				return;
			}

			transform.localPosition = temporaryStoredTransformData[transform].Position;
			transform.localRotation = temporaryStoredTransformData[transform].Rotation;

			temporaryStoredTransformData.Remove(transform);
		}
		#endregion

		public static Transform FindChildWithTag(this Transform parent, string tag) => parent.Cast<Transform>().FirstOrDefault(child => child.CompareTag(tag));
	}
}
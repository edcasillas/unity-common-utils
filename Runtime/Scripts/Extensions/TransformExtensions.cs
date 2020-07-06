using System;
using System.Collections.Generic;
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
	}
}
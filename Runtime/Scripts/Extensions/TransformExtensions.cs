using System;
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
	}
}
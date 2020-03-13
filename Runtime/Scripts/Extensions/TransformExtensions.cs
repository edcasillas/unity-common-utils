using System;
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class TransformExtensions {
		public static bool IsInside(this Transform transform, BoxCollider collider) {
			var point    = collider.transform.InverseTransformPoint(transform.position) - collider.center;
			var halfSize = collider.size * 0.5f;
			return point.x < halfSize.x && point.x > -halfSize.x &&
				   point.y < halfSize.y && point.y > -halfSize.y &&
				   point.z < halfSize.z && point.z > -halfSize.z;
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
	}
}
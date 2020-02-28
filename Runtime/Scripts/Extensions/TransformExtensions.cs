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
	}
}
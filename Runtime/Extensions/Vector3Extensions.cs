using UnityEngine;

namespace CommonUtils.Extensions {
	public static class Vector3Extensions {
		public static Vector2 GetAnglesTo(this Vector3 referenceVector, Vector3 compareVector) => new Vector2(
			-Mathf.Asin(Vector3.Cross(compareVector, referenceVector).y) * Mathf.Rad2Deg,
			-Mathf.Asin(Vector3.Cross(compareVector, referenceVector).x) * Mathf.Rad2Deg);
	}
}
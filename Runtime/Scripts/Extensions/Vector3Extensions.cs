using UnityEngine;

namespace CommonUtils.Extensions {
	public static class Vector3Extensions {
		public static Vector2 GetAnglesTo(this Vector3 referenceVector, Vector3 compareVector)
			=> new Vector2(-Mathf.Asin(Vector3.Cross(compareVector, referenceVector).y) * Mathf.Rad2Deg,
						   -Mathf.Asin(Vector3.Cross(compareVector, referenceVector).x) * Mathf.Rad2Deg);

		public static Vector3 RandomRange(Vector3 min, Vector3 max) => new Vector3(Random.Range(min.x, max.x),
																				   Random.Range(min.y, max.y),
																				   Random.Range(min.z, max.z));
	}
}
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class Vector3Extensions {
		public static Vector2 GetAnglesTo(this Vector3 referenceVector, Vector3 compareVector)
			=> new Vector2(-Mathf.Asin(Vector3.Cross(compareVector, referenceVector).y) * Mathf.Rad2Deg,
						   -Mathf.Asin(Vector3.Cross(compareVector, referenceVector).x) * Mathf.Rad2Deg);

		public static Vector3 RandomRange(Vector3 min, Vector3 max) => new Vector3(Random.Range(min.x, max.x),
																				   Random.Range(min.y, max.y),
																				   Random.Range(min.z, max.z));
		
		public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion rotation) => rotation * (point - pivot) + pivot;
		
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
	}
}
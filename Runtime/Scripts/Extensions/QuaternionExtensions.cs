using UnityEngine;

namespace CommonUtils.Extensions {
	public static class QuaternionExtensions {
		public static Quaternion Clamp(this Quaternion q, Vector3 min, Vector3 max)
			=> Quaternion.Euler(Mathf.Clamp(MathUtils.NormalizeAngle(q.eulerAngles.x), min.x, max.x),
								Mathf.Clamp(MathUtils.NormalizeAngle(q.eulerAngles.y), min.y, max.y),
								Mathf.Clamp(MathUtils.NormalizeAngle(q.eulerAngles.z), min.z, max.z));
	}
}
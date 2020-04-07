using UnityEngine;

namespace CommonUtils.Extensions {
	public static class BoundsExtensions {
		public static BoundingSphere ToBoundingSphere(this Bounds bounds) => new BoundingSphere {position = bounds.center, radius = bounds.extents.magnitude};
	}
}

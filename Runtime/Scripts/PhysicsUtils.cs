using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils {
	public static class PhysicsUtils {
		public static IEnumerable<Collider> GetCapsuleOverlappingColliders(Vector3 capsulePoint1, Vector3 capsulePoint2, float radius, int layerMask = ~0) {
			var result = new Collider[10];
			if (Physics.OverlapCapsuleNonAlloc(capsulePoint1,
											   capsulePoint2,
											   radius,
											   result,
											   layerMask,
											   QueryTriggerInteraction.Ignore) > 0) {
				return result.Where(c => c);
			}

			return Enumerable.Empty<Collider>();
		}

		public static Tuple<Vector3, Vector3> GetCapsuleCentersOfSpheres(Vector3 position, float height, float radius) {
			var distanceToSpheres = Vector3.up * ((height - (2 * radius)) / 2);
			// The center of the sphere at the top of the capsule.
			var capsulePoint1 = position + distanceToSpheres;
			// The center of the sphere at the bottom of the capsule.
			var capsulePoint2 = position - distanceToSpheres;
			return new Tuple<Vector3, Vector3>(capsulePoint1, capsulePoint2);
		}
	}
}
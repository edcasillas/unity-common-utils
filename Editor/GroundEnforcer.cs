using CommonUtils.Extensions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	public static class GroundEnforcer {
		[MenuItem("GameObject/Move selected to ground position", priority = 40)]
		private static void LogSelectedTransformName() {
			foreach (var transform in Selection.transforms) {
				Debug.Log($"Moving {transform.name} to ground position.", transform);
				if(Physics.Raycast(transform.position,Vector3.down, out var hitInfo,float.MaxValue, ~0, QueryTriggerInteraction.Ignore)) {
					transform.position = hitInfo.point;
				} else {
					Debug.LogError($"Couldn't move {transform.name} to ground position because no collider was detected below.", transform);
				}
			}
		}
		
		[MenuItem("GameObject/Move selected to ground position", true)]
		private static bool ValidateLogSelectedTransformName() => !Selection.transforms.IsNullOrEmpty();
	}
}
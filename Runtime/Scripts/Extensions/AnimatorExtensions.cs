using UnityEngine;

namespace CommonUtils.Extensions {
	public static class AnimatorExtensions {
		public static bool HasParameter(this Animator animator, string name) {
			var allParameters = animator.parameters;
			foreach (var param in allParameters) {
				if (param.name == name) return true;
			}

			return false;
		}

		public static bool HasParameter(this Animator animator, int nameHash) {
			var allParameters = animator.parameters;
			foreach (var param in allParameters) {
				if (param.nameHash == nameHash) return true;
			}

			return false;
		}
	}
}

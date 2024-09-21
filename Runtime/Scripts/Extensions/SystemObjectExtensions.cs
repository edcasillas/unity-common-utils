using UnityEngine;

namespace CommonUtils.Extensions {
	public static class SystemObjectExtensions {
		public static bool IsNullOrDestroyed(this object obj) {
			if (obj is Object o) return !o;
			return obj == null;
		}
	}
}
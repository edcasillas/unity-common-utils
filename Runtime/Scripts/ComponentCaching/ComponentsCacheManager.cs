using UnityEngine;

namespace CommonUtils.ComponentCaching {
	public static class ComponentsCacheManager {
		private static ComponentsCache cacheInstance;

		private static ComponentsCache Cache {
			get {
				if (!cacheInstance) {
					cacheInstance = new GameObject(nameof(ComponentsCache)).AddComponent<ComponentsCache>();
				}

				return cacheInstance;
			}
		}

		public static T GetCachedComponent<T>(this GameObject gameObject) where T : Component =>
			Cache.GetComponentFrom<T>(gameObject);
	}
}
using UnityEngine;

namespace CommonUtils.ObjectPooling {
	public partial class PrefabPoolManager {
		public static PrefabPoolManager Instance { get; private set; }

		/// <summary>
		/// Retrieves an instance of <typeparamref name="TPoolable"/> from the pool using the position and rotation defined in the prefab.
		/// </summary>
		public static TPoolable InstantiateFromPool<TPoolable>(TPoolable prefab, bool? active = true) where TPoolable : IObjectFromPool {
			var t = prefab.transform;
			return InstantiateFromPool(prefab, t.position, t.rotation, active);
		}

		public static TPoolable InstantiateFromPool<TPoolable>(TPoolable prefab, Vector3 position, Quaternion rotation, bool? active = true) where TPoolable : IObjectFromPool {
			if (Instance) {
				return Instance.instantiate(prefab, position, rotation, active);
			}

			var result = GameObject.Instantiate(prefab.gameObject, position, rotation);
			if (active.HasValue) result.gameObject.SetActive(active.Value);
			return result.GetComponent<TPoolable>();
		}

		public static void DestroyAndReturnToPool(GameObject gameObject) {
			if (Instance) {
				Instance.Destroy(gameObject);
				return;
			}
			GameObject.Destroy(gameObject);
		}

		public static void RecycleAll() {
			if (!Instance) {
				Debug.LogError($"Cannot recycle objects because there is no instance of {nameof(PrefabPoolManager)}");
				return;
			}
			Instance.recycleAll();
		}
	}
}
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonUtils.ObjectPooling {
	[Serializable]
	public class PrefabPool {
		[SerializeField] private Transform container;
		[SerializeField] private GameObject prefab;
		private readonly UniqueItemsQueue<IObjectFromPool> pool = new UniqueItemsQueue<IObjectFromPool>();

		public GameObject Prefab => prefab;
		public Transform Container => container;
		public int InitialCount { get; private set; }
		public int CurrentCount { get; private set; }

		public PrefabPool() { } // Ctor required for serialization.

		public PrefabPool(GameObject prefab, Transform parent) {
			this.prefab = prefab;
			container = new GameObject(prefab.name).transform;
			container.SetParent(parent);
		}

		/// <summary>
		/// Initializes the prefab pool:
		/// 1. Assigns the PoolId property of each preconfigured object (children).
		/// 2. Adds each preconfigured object to the pool queue.
		/// 3. Initializes the count of initial objects.
		/// </summary>
		public void Setup() {
			foreach (Transform childTransform in Container) {
				if(childTransform == Container) continue;
				var childPoolable = childTransform.GetComponent<IObjectFromPool>(); // NOTE GetComponentsInChildren<IPoolable> returns an empty array, that's why we must iterate one by one getting the component.
				if(childPoolable == null) continue;
				childPoolable.PoolId = prefab.name;
				pool.Enqueue(childPoolable);
				InitialCount = ++CurrentCount;
			}
		}

		public GameObject Get(Vector3 position, Quaternion rotation, bool? active = true) {
			GameObject result;
			IObjectFromPool poolable;
			if (pool.Any()) {
				poolable = pool.Dequeue();
				poolable.transform.position = position;
				poolable.transform.rotation = rotation;
				result = poolable.gameObject;
			} else {
				result = Object.Instantiate(prefab, position, rotation, container);
				poolable = result.GetComponent<IObjectFromPool>();
				if (poolable != null) {
					poolable.PoolId = prefab.name;
					CurrentCount++;
				}
			}

			poolable?.OnInstantiatedFromPool();

			if(active.HasValue) result.SetActive(active.Value);
			return result;
		}

		public void Store(IObjectFromPool obj) {
			obj.gameObject.SetActive(false);
			obj.gameObject.transform.SetParent(Container);
			if(pool.Enqueue(obj)) obj.OnReturnedToPool();
		}

		public bool Contains(IObjectFromPool obj) => pool.Contains(obj);
	}
}

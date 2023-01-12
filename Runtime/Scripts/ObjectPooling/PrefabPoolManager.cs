using CommonUtils.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CommonUtils.ObjectPooling {
	public partial class PrefabPoolManager : EnhancedMonoBehaviour {
		private readonly Dictionary<string, PrefabPool> pools = new Dictionary<string, PrefabPool>();

		[SerializeField] private PrefabPool[] preconfiguredPools;
		[SerializeField] private List<GameObject> prefabs;

		public Dictionary<string, string> PoolsStatus
			=> pools.ToDictionary(p => p.Key, p => $"Initial count: {p.Value.InitialCount}; Current count: {p.Value.CurrentCount}");

		private Dictionary<string, GameObject> prefabCache;

		#region Unity Lifecycle
		protected virtual void Awake() {
			this.DebugLog("Initializing pool manager.");
			prefabCache = new Dictionary<string, GameObject>();

			foreach (var preconfiguredPool in preconfiguredPools) {
				if (preconfiguredPool.Prefab.GetComponent<IObjectFromPool>() != null) {
					preconfiguredPool.Setup();
					pools.Add(preconfiguredPool.Prefab.name, preconfiguredPool);
					this.DebugLog($"Preconfigured pool of {preconfiguredPool.Prefab.name} has been registered.");
				} else {
					Debug.LogError($"Prefab {preconfiguredPool.Prefab.name} does not implement interface {nameof(IObjectFromPool)}. Can't create a pool for it.", this);
					Destroy(preconfiguredPool.Container.gameObject);
				}
			}

			foreach (var prefab in prefabs) {
				if(!prefab) continue;
				#if UNITY_EDITOR // We only want to perform this check in the editor, so builds don't waste time on this.
				if (prefab.GetComponent<IObjectFromPool>() == null) {
					Debug.LogWarning($"Can't add {prefab.name} to the pool because the game object doesn't have a component implementing {nameof(IObjectFromPool)}.", this);
					continue;
				}
				#endif
				prefabCache.Add(prefab.name, prefab);
			}

			Instance = this;
		}

		private protected void OnDestroy() => Instance = null;
		#endregion

		private TPoolable instantiate<TPoolable>(TPoolable prefab, Vector3 position, Quaternion rotation, bool? active = true) where TPoolable : IObjectFromPool {
			this.DebugLog($"Instantiating {prefab.name}");
			if (!pools.ContainsKey(prefab.name)) {
				registerPoolAtRuntime(prefab.gameObject);
			}

			return pools[prefab.name].Get(position, rotation, active).GetComponent<TPoolable>();
		}

		public void Destroy<TPoolable>(TPoolable component, float t) where TPoolable : IObjectFromPool => StartCoroutine(destroyDelayed(component, t));

		private IEnumerator destroyDelayed<TPoolable>(TPoolable component, float t) where TPoolable : IObjectFromPool {
			yield return new WaitForSeconds(t);
			Destroy(component.gameObject);
		}

		public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation) {
			this.DebugLog($"Instantiating {prefabId}");

			// If a pool already exists for the specified prefab, retrieve an instance from it.
			if (pools.ContainsKey(prefabId)) return pools[prefabId].Get(position, rotation);

			// Otherwise instantiate the prefab.
			if (!prefabCache.ContainsKey(prefabId)) { // Load from resources if the prefab doesn't exist in cache.
				Debug.LogWarning($"Prefab {prefabId} will be loaded from the Resources folder. For better performance, assign the prefab to {name}.", this);
				var prefabFromResources = Resources.Load<GameObject>(prefabId);
				if (!prefabFromResources) {
					Debug.LogError($"Prefab {prefabId} couldn't be loaded from the Resources folder.", this);
					return null;
				}

				prefabCache.Add(prefabId, prefabFromResources);
			}

			var prefab = prefabCache[prefabId];

			// Try creating a pool for it.
			if (prefab.GetComponent<IObjectFromPool>() != null) {
				registerPoolAtRuntime(prefab);
				return pools[prefabId].Get(position, rotation);
			}

			// If everything else fails, just instantiate the object the usual way
			Debug.LogWarning($"Prefab {prefab} could not be added to the pool because the game object doesn't have a component implementing {nameof(IObjectFromPool)}.", this);
			return GameObject.Instantiate(prefab, position, rotation);
		}

		public void Destroy(GameObject gameObject) {
			if(IsVerbose) Debug.Log($"Destroying {gameObject.name}", gameObject);
			var poolable = gameObject.GetComponent<IObjectFromPool>();
			if (poolable == null || string.IsNullOrEmpty(poolable.PoolId)) {
				GameObject.Destroy(gameObject);
				return;
			}
			if (pools.ContainsKey(poolable.PoolId))
				pools[poolable.PoolId].Store(poolable);
		}

		private void registerPoolAtRuntime(GameObject prefab) {
			if (!prefab) return;
			pools.Add(prefab.name, new PrefabPool(prefab, transform));
			Debug.LogWarning($"Pool of {prefab.name} has been registered at runtime. For better performance add a preconfigured pool for this prefab.");
		}
	}
}
using CommonUtils.Extensions;
using CommonUtils.Inspector.HelpBox;
using CommonUtils.Verbosables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.ObjectPooling {
	/// <summary>
	/// This is the main component of the Object pooling functionality.
	/// </summary>
	/// <remarks>
	/// This manager can be used in any game, and it's also ready to be used with Photon by implementing IPunPrefabPool.
	/// In order to use this manager in a game connected with Photon, create and use a derived class like this:
	///
	/// <code>
	/// public class PunPrefabPoolManager : PrefabPoolManager, IPunPrefabPool { }
	/// </code>
	///
	/// No other code should be necessary since the interface is already covered here.
	/// </remarks>
	public partial class PrefabPoolManager : EnhancedMonoBehaviour {
		private readonly Dictionary<string, PrefabPool> pools = new Dictionary<string, PrefabPool>();

		#region Inspector fields
		[HelpBox("To configure a pool for some prefab, create an empty child of this manager, then under it add a " +
				 "few instances of the prefab, making sure they're deactivated. Finally, add an item to this list " +
				 "referencing the container and its prefab. If this configuration is omitted, pools will be created " +
				 "at runtime when an IObjectFromPool is instantiated.", HelpBoxMessageType.Info)]
		[SerializeField] private PrefabPool[] preconfiguredPools;

		[Space]
		[HelpBox("When an additional object needs to be instantiated, the PrefabPoolManager will look in the " +
				 "Resources folder for its prefab. To avoid the performance impact of loading from Resources, add" +
				 " those prefabs to this list.", HelpBoxMessageType.Info)]
		[SerializeField] private List<GameObject> prefabs;
		#endregion

		/// <summary>
		/// Maps the name of the prefab to the prefab itself to get constant time access to prefabs by their name.
		/// </summary>
		private Dictionary<string, GameObject> prefabCache;

		[ShowInInspector] public IEnumerable<GameObject> CachedPrefabs => prefabCache.Values;

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

		private void recycleAll() {
			foreach (var objectFromPool in GetComponentsInChildren<IObjectFromPool>()) {
				Destroy(objectFromPool.gameObject);
			}
		}

		private void registerPoolAtRuntime(GameObject prefab) {
			if (!prefab) return;
			pools.Add(prefab.name, new PrefabPool(prefab, transform));
			Debug.LogWarning($"Pool of {prefab.name} has been registered at runtime. For better performance add a preconfigured pool for this prefab.");
		}

		private bool contains(IObjectFromPool objectFromPool) {
			if (string.IsNullOrEmpty(objectFromPool.PoolId))
				throw new ArgumentException("The PoolId of the object cannot be null or empty.");

			if (!pools.ContainsKey(objectFromPool.PoolId)) {
				return false;
			}

			return pools[objectFromPool.PoolId].Contains(objectFromPool);
		}
	}
}
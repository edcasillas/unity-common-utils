using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.ComponentCaching {
	[HelpURL("https://github.com/edcasillas/unity-common-utils/wiki/Components-Cache")]
	public class ComponentsCache : MonoBehaviour {
		private static readonly Dictionary<GameObject, Dictionary<Type, object>> cachedObjects = new Dictionary<GameObject, Dictionary<Type, object>>();
		public IEnumerable<GameObject> CachedObjects => cachedObjects.Keys;

		public T GetComponentFrom<T>(GameObject go) {
			if (!go) return default;

			var t = typeof(T);

			if (!cachedObjects.ContainsKey(go)) cachedObjects.Add(go, new Dictionary<Type, object>());
			if (!cachedObjects[go].ContainsKey(t)) {
				cachedObjects[go].Add(t, go.GetComponent<T>());
			}

			return (T)cachedObjects[go][t];
		}

		private void OnDestroy() => cachedObjects.Clear();
	}
}
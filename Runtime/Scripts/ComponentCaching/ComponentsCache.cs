using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.ComponentCaching {
	[HelpURL("https://github.com/edcasillas/unity-common-utils/wiki/Components-Cache")]
	public class ComponentsCache : MonoBehaviour {
		private static readonly Dictionary<GameObject, Dictionary<Type, object>> cachedObjects =
			new Dictionary<GameObject, Dictionary<Type, object>>();

		public IEnumerable<GameObject> CachedObjects => cachedObjects.Keys;

		public T GetComponentFrom<T>(GameObject go, bool addIfNotFound = false) where T : Component {
			if (!go) return default;

			var t = typeof(T);

			if (!cachedObjects.ContainsKey(go)) cachedObjects.Add(go, new Dictionary<Type, object>());
			if (!cachedObjects[go].ContainsKey(t)) {
				var component = go.GetComponent<T>();
				if (!component && addIfNotFound) component = go.AddComponent<T>();
				if (component) cachedObjects[go].Add(t, go.GetComponent<T>());
				else return null;
			}

			return (T) cachedObjects[go][t];
		}

		private void OnDestroy() => cachedObjects.Clear();
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonUtils {
	[HelpURL("https://github.com/edcasillas/unity-common-utils/wiki/Components-Cache")]
	public static class ComponentsCache {
		private static readonly Dictionary<GameObject, Dictionary<Type, object>> cachedObjects =
			new Dictionary<GameObject, Dictionary<Type, object>>();

		static ComponentsCache() => SceneManager.sceneUnloaded += onSceneUnloaded;

		private static void onSceneUnloaded(Scene arg0) => cachedObjects.Clear();

		public static T GetCachedComponent<T>(this GameObject gameObject) where T : Component =>
			getComponentFrom<T>(gameObject);

		public static T GetCachedComponent<T>(this MonoBehaviour monoBehaviour) where T : Component
			=> getComponentFrom<T>(monoBehaviour.gameObject);

		private static T getComponentFrom<T>(GameObject go, bool addIfNotFound = false) where T : Component {
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
	}
}
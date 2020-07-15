using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace CommonUtils {
	public static class ObjectFinder {
		/// <summary>
		/// Works the same as <see cref="UnityEngine.Object.FindObjectsOfType{T}"/> but supports searching by interface type.
		/// </summary>
		/// <param name="includeInactive">Should Components on inactive GameObjects be included in the found set?</param>
		/// <typeparam name="T">The type of Component to retrieve.</typeparam>
		/// <returns>Collection of components of type <typeparamref name="T"/></returns>
		public static IEnumerable<T> FindObjectsOfType<T>(bool includeInactive = false) {
			var result = Enumerable.Empty<T>();
			for (int index = 0; index < SceneManager.sceneCount; ++index) {
				result = result.Concat(SceneManager.GetSceneAt(index)
					.GetRootGameObjects()
					.SelectMany(go => go.GetComponentsInChildren<T>(includeInactive)));
			}

			return result;
		}
	}
}
using UnityEngine;

namespace CommonUtils.Extensions {
	public static class GameObjectExtensions {
		public static GameObject[] GetChildren(this GameObject gameObject) {
			var result = new GameObject[gameObject.transform.childCount];
			var i = 0;
			foreach (Transform child in gameObject.transform) {
				if (child == gameObject.transform) continue;
				result[i++] = child.gameObject;
			}

			return result;
		}
	}
}
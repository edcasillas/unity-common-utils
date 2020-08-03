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

		/// <summary>
		/// Changes the layer of the specified <paramref name="gameObject"/> and all of its children.
		/// </summary>
		public static void SetLayerRecursively(this GameObject gameObject, int layer) {
			foreach (var child in gameObject.GetComponentsInChildren<Transform>()) {
				child.gameObject.layer = layer;
			}
		}
	}
}
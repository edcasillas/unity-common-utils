using UnityEngine;

namespace CommonUtils.UnityComponents {
	public static class UnityComponentExtensions {
		/// <summary>
		/// Returns true when this <paramref name="unityComponent"/> is a valid GameObject in the scene.
		/// </summary>
		/// <param name="unityComponent">Component to check.</param>
		/// <returns>True if the component is valid, otherwise false.</returns>
		public static bool IsValid(this IUnityComponent unityComponent) {
			if (unityComponent == null) return false;
			if (unityComponent is Component nativeComponent) {
				return nativeComponent;
			}
			return unityComponent.gameObject;
		}

		/// <summary>
		/// Adds an empty child to the specified <paramref name="unityComponent"/>, making sure its local position,
		/// rotation and scale have default values.
		/// </summary>
		/// <param name="unityComponent">Component for which a child game object will be added.</param>
		/// <param name="name">Optional name for the game object</param>
		public static GameObject AddChild(this IUnityComponent unityComponent, string name = null) {
			var result = new GameObject().transform;
			result.SetParent(unityComponent.transform);
			result.localPosition = Vector3.zero;
			result.localRotation = Quaternion.identity;
			result.localScale = Vector3.one;
			if (name != null) result.name = name;
			return result.gameObject;
		}
	}
}
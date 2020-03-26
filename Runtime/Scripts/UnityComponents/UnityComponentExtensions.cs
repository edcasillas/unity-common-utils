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
	}
}
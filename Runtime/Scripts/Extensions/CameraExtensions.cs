using UnityEngine;

namespace CommonUtils.Extensions {
	public static class CameraExtensions {
		public static void LayerCullingShow(this Camera cam, int layerMask) => cam.cullingMask |= layerMask;

		public static void LayerCullingShow(this Camera cam, string layer) => LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));

		public static void LayerCullingHide(this Camera cam, int layerMask) => cam.cullingMask &= ~layerMask;

		public static void LayerCullingHide(this Camera cam, string layer) => LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));

		public static void LayerCullingToggle(this Camera cam, int layerMask) => cam.cullingMask ^= layerMask;

		public static void LayerCullingToggle(this Camera cam, string layer) => LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer));

		public static bool LayerCullingIncludes(this Camera cam, int layerMask) => (cam.cullingMask & layerMask) > 0;

		public static bool LayerCullingIncludes(this Camera cam, string layer) => LayerCullingIncludes(cam, 1 << LayerMask.NameToLayer(layer));

		public static void LayerCullingToggle(this Camera cam, int layerMask, bool isOn) {
			var included = LayerCullingIncludes(cam, layerMask);
			if (isOn && !included) {
				LayerCullingShow(cam, layerMask);
			} else if (!isOn && included) {
				LayerCullingHide(cam, layerMask);
			}
		}

		public static void LayerCullingToggle(this Camera cam, string layer, bool isOn) => LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer), isOn);

		/// <summary>
		/// Gets a value indicating whether the specified <paramref name="worldPosition"/> is inside the frustum of the <paramref name="camera"/>.
		/// </summary>
		public static bool CanSee(this Camera camera, Vector3 worldPosition) {
			var viewportPosition = camera.WorldToViewportPoint(worldPosition);
			return viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
				   viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
				   viewportPosition.z > 0;
		}
	}
}
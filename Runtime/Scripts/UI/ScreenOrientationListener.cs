using CommonUtils.Extensions;
using CommonUtils.LegacyGUI;
using UnityEngine;

namespace CommonUtils.UI {
	public class ScreenOrientationListener : EnhancedMonoBehaviour {
		[SerializeField] private AspectRatioOrientationEvent onOrientationChanged;

		private AspectRatioOrientation previousOrientation;

		private void Start() => previousOrientation = GUICoords.Instance.CurrentAspectRatio.GetOrientation();

		private void Update() {
			GUICoords.Instance.RefreshCurrentAspectRatio();
			var orientation = GUICoords.Instance.CurrentAspectRatio.GetOrientation();
			if (orientation == previousOrientation) return;
			this.DebugLog($"Orientation has changed to {orientation}");
			onOrientationChanged?.Invoke(orientation);
			previousOrientation = orientation;
		}
	}
}
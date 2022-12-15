using UnityEngine;

namespace CommonUtils.UI.Tween {
	[RequireComponent(typeof(CanvasGroup))]
	public class AlphaTween : AbstractTweener<float> {
		private CanvasGroup _canvasGroup;
		private CanvasGroup canvasGroup {
			get {
				if (!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
				return _canvasGroup;
			}
		}

		protected override float InitializeStartValue() => canvasGroup.alpha;
		protected override void OnAnimationUpdated(float updatedValue) => canvasGroup.alpha = updatedValue;
	}
}
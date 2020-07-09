using CommonUtils.UnityComponents;
using UnityEngine;

namespace CommonUtils.UI.BlinkerUIElements {
	public interface IBlinkerUIElement : IUnityComponent {
		float BlinkingRate { get; }
		CanvasGroup CanvasGroup { get; }
	}
}
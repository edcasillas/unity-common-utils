using CommonUtils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.BlinkerUIElements {
	[AddComponentMenu("UI/Blinker UI Element")]
    [RequireComponent(typeof(Image), typeof(CanvasGroup))]
    public class BlinkerUIElement : MonoBehaviour, IBlinkerUIElement {
		#pragma warning disable 649
        [SerializeField] private float blinkingRate;
        [SerializeField] private bool autoStart;
		#pragma warning restore 649

        public float BlinkingRate { get => blinkingRate; set => blinkingRate = value; }

		private CanvasGroup canvasGroup;

		public CanvasGroup CanvasGroup {
			get {
				if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
				return canvasGroup;
			}
		}

		private void OnEnable() {
			if(autoStart) this.StartBlinking();
		}

		private void OnDisable() {
			if(autoStart) this.StopBlinking();
		}
	}
}
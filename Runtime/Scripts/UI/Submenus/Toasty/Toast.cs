using CommonUtils.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.Submenus.Toasty {
	[AddComponentMenu("UI/Animated Submenu/Toast")]
	public class Toast : AbstractSubmenu {
		[SerializeField] private Image image;
		[SerializeField] private TMP_Text text;
		[SerializeField] private float addedSecondsOnChange = 0.5f;

		[ShowInInspector] public float FontSize { get; private set; }
		[ShowInInspector] public bool ShouldEnableFontAutoSize { get; private set; }
		[ShowInInspector] public float WaitTimeToHide { get; private set; }

		protected override void OnInit() {
			// For now we are only concerned about the height of the toast, assuming the width will be controlled by the manager.
			HiddenValue = Vector2.zero;
			ShownValue = RectTransform.sizeDelta;
			FontSize = text.fontSize;
			ShouldEnableFontAutoSize = text.enableAutoSizing;
		}

		public void ChangeDisplayedValues(string displayText, Sprite sprite) {
			if (sprite) image.sprite = sprite;
			else image.gameObject.SetActive(false);

			text.text = displayText;
			WaitTimeToHide += addedSecondsOnChange;
			if (WaitTimeToHide > AutoHide) WaitTimeToHide = AutoHide;
		}

		public override void Show() {
			base.Show();
			text.enableAutoSizing = false;
		}

		public override void Hide() {
			base.Hide();
			FontSize = text.fontSize;
			text.enableAutoSizing = false;
		}

		protected override void OnShown() {
			base.OnShown();
			text.enableAutoSizing = ShouldEnableFontAutoSize;
		}

		protected override void OnHidden() {
			text.enableAutoSizing = ShouldEnableFontAutoSize;
			base.OnHidden();
		}

		public override void OnAnimationUpdated(Vector2 updatedValue) {
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, updatedValue.y);
			text.fontSize = (updatedValue.y * FontSize) / ShownValue.y; // rule of three
		}

		protected override IEnumerator WaitAndHide() {
			WaitTimeToHide = AutoHide;
			while (WaitTimeToHide > 0) {
				WaitTimeToHide -= Time.unscaledDeltaTime;
				yield return null;
			}
			this.DebugLog("Hiding");
			Hide();
			HideCoroutine = null;
		}
	}
}
using UnityEngine;

namespace CommonUtils.UI.Submenus.Toasty {
	[AddComponentMenu("UI/Animated Submenu/Toast")]
	public class Toast : AbstractSubmenu {
		[ShowInInspector] public Vector2 position { get; set; }

		protected override void OnInit() {
			float rectWidth = (RectTransform.anchorMax.x - RectTransform.anchorMin.x)*Screen.width;
			float rectHeight =(RectTransform.anchorMax.y - RectTransform.anchorMin.y)*Screen.height;
			 position = new Vector2(RectTransform.anchorMin.x*Screen.width, RectTransform.anchorMin.y * Screen.height);

			// For now we are only concerned about the height of the toast, assuming the width will be controlled by the manager.
			HiddenValue = Vector2.zero;
			ShownValue = RectTransform.sizeDelta;
		}

		public override void OnAnimationUpdated(Vector2 updatedValue)
			=> RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, updatedValue.y);
	}
}
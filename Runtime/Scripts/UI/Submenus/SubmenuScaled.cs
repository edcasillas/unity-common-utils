using UnityEngine;

namespace CommonUtils.UI.Submenus {
	[AddComponentMenu("UI/Animated Submenu/Scaled Submenu")]
	public class SubmenuScaled : AbstractSubmenu {
		public override void OnAnimationUpdated(Vector2 updatedValue) => RectTransform.localScale = new Vector3(updatedValue.x, updatedValue.y, 1f);

		protected override void OnInit() {
			HiddenValue = Vector2.zero;
			ShownValue = Vector2.one;
			RectTransform.localScale = Vector2.zero;
		}
	}
}
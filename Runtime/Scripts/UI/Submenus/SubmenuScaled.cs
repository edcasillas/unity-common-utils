using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils.UI.Submenus {
	[AddComponentMenu("UI/Animated Submenu/Scaled Submenu")]
	public class SubmenuScaled : AbstractSubmenu {
		public override void OnAnimationUpdated(Vector2 updatedValue) {
			this.DebugLog(updatedValue.ToString);
			RectTransform.localScale = new Vector3(updatedValue.x, updatedValue.y, 1f);
		}

		protected override void OnInit() {
			hiddenValue = Vector2.zero;
			shownValue = Vector2.one;
			RectTransform.localScale = Vector2.zero;
		}
	}
}
using UnityEngine;

namespace CommonUtils.UI.Submenus.Toasty
{
    public class Toast : AbstractSubmenu
    {
		protected override void OnInit() {
			// For now we are only concerned about the height of the toast, assuming the width will be controlled by the manager.
			HiddenValue = Vector2.zero;
			ShownValue = new Vector2(0, RectTransform.rect.height);
		}

		public override void OnAnimationUpdated(Vector2 updatedValue) => throw new System.NotImplementedException();
	}
}

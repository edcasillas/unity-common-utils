using UnityEngine;

namespace CommonUtils.UI.Submenus {
	[AddComponentMenu("UI/Animated Submenu/Slide-in Submenu")]
	public class SubmenuSlideIn : AbstractSubmenu {
		public enum SlideInDirections {
			LeftToRight,
			RightToLeft,
			BottomUp,
			TopDown
		}

		public SlideInDirections Direction;

		protected override void OnInit() {
			var anchoredPosition = RectTransform.anchoredPosition;
			switch(Direction) {
			case SlideInDirections.LeftToRight:
				HiddenValue = new Vector2(-RectTransform.rect.width, RectTransform.anchoredPosition.y);
				break;
			case SlideInDirections.RightToLeft:
				HiddenValue = new Vector2(RectTransform.rect.width, RectTransform.anchoredPosition.y);
				break;
			case SlideInDirections.BottomUp:
				HiddenValue = new Vector2(RectTransform.anchoredPosition.x, -RectTransform.rect.height);
				break;
			case SlideInDirections.TopDown:
				HiddenValue = new Vector2(RectTransform.anchoredPosition.x, RectTransform.rect.height);
				break;
			}
			ShownValue = new Vector2(anchoredPosition.x, anchoredPosition.y);
		}

		public override void OnAnimationUpdated(Vector2 updatedValue) => RectTransform.anchoredPosition = updatedValue;
	}
}
using UnityEngine;

namespace CommonUtils.UI.Submenus {
	[AddComponentMenu("UI/Animated Submenu/Slide-in Submenu")]
	public class SubmenuSlideIn : AbstractSubmenu {
		public enum SlideInDirections {
			LeftToRight,
			RightToLeft,
			BottomUp
		}

		public SlideInDirections Direction;

		protected override void OnInit() {
			var anchoredPosition = RectTransform.anchoredPosition;
			switch(Direction) {
			case SlideInDirections.LeftToRight:
				hiddenValue = new Vector2(-RectTransform.rect.width, RectTransform.anchoredPosition.y);
				break;
			case SlideInDirections.RightToLeft:
				hiddenValue = new Vector2(RectTransform.rect.width, RectTransform.anchoredPosition.y);
				break;
			case SlideInDirections.BottomUp:
				hiddenValue = new Vector2(RectTransform.anchoredPosition.x, -RectTransform.rect.height);
				break;
			}
			shownValue = new Vector2(anchoredPosition.x, anchoredPosition.y);
			OnAnimationUpdated(hiddenValue);
		}

		public override void OnAnimationUpdated(Vector2 updatedValue) => RectTransform.anchoredPosition = updatedValue;
	}
}
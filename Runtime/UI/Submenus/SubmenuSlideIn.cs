using UnityEngine;

namespace CommonUtils.UI.Submenus {
	public class SubmenuSlideIn : AbstractSubmenu {
		public enum SlideInDirections {
			LeftToRight,
			RightToLeft
		}

		public SlideInDirections Direction;

		protected override void OnInit() {
			var anchoredPosition = RectTransform.anchoredPosition;
			switch(Direction) {
			case SlideInDirections.LeftToRight:
				hiddenValue = new Vector2(-RectTransform.rect.width, RectTransform.anchoredPosition.y);
				shownValue = new Vector2(anchoredPosition.x, anchoredPosition.y);
				break;
			case SlideInDirections.RightToLeft:
				hiddenValue = new Vector2(RectTransform.rect.width, RectTransform.anchoredPosition.y);
				shownValue = new Vector2(anchoredPosition.x, anchoredPosition.y);
				break;
			}
			OnAnimationUpdated(hiddenValue);
		}

		public override void OnAnimationUpdated(Vector2 updatedValue) {
			RectTransform.anchoredPosition = updatedValue;
		}
	}
}
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.ProgressDisplay
{
	[RequireComponent(typeof(Image))]
	public class ImageFillProgressDisplay : AbstractProgressDisplay {
		private Image image;

		private void Awake() {
			image = GetComponent<Image>();
			image.fillAmount = 0;
		}

		protected override void DisplayProgress(float value) => image.fillAmount = Mathf.Clamp01(value);
	}
}
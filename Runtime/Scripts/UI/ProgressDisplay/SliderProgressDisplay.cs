using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.ProgressDisplay {
	[RequireComponent(typeof(Slider))]
	public class SliderProgressDisplay : AbstractProgressDisplay {
		private Slider slider;
		private Slider Slider => slider ??= GetComponent<Slider>();
		protected override void DisplayProgress(float value) => Slider.value = value;
	}
}
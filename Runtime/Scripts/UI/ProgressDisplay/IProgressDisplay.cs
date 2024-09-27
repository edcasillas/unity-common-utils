using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.ProgressDisplay {
	public interface IProgressDisplay {
		float Progress { set; }
	}

	public abstract class AbstractProgressDisplay : EnhancedMonoBehaviour, IProgressDisplay {
		[SerializeField] private Text text;

		private float progress;

		[ShowInInspector]
		public float Progress {
			get => progress;
			set {
				progress = value;
				DisplayProgress(value);
				if (text) text.text = $"{progress * 100:P1}";
			}
		}

		protected abstract void DisplayProgress(float value);
	}

	[RequireComponent(typeof(Slider))]
	public class SliderProgressDisplay : AbstractProgressDisplay {
		private Slider slider;
		private Slider Slider => slider ??= GetComponent<Slider>();
		protected override void DisplayProgress(float value) => Slider.value = value;
	}

	[RequireComponent(typeof(Image))]
	public class ImageFillProgressDisplay : AbstractProgressDisplay {
		private Image image;

		private void Awake() {
			image = GetComponent<Image>();
			image.fillAmount = 0;
		}

		protected override void DisplayProgress(float value) => image.fillAmount = value;
	}
}

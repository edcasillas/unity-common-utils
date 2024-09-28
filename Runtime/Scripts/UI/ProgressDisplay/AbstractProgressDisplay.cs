using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI.ProgressDisplay
{
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
}
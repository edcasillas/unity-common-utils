using TMPro;
using UnityEngine;

namespace CommonUtils.UI.ProgressDisplay
{
	public abstract class AbstractProgressDisplay : EnhancedMonoBehaviour, IProgressDisplay {
		[SerializeField] private TMP_Text text;

		private float progress;

		[ShowInInspector]
		public float Progress {
			set {
				progress = value;
				DisplayProgress(value);
				if (text) text.text = $"{progress:P1}";
			}
		}

		protected abstract void DisplayProgress(float value);
	}
}
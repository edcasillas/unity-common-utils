using CommonUtils;
using CommonUtils.UI.ProgressDisplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Demos.UI.ProgressDisplay.Scripts {
	public class ProgressDisplayDemo : EnhancedMonoBehaviour {
		[SerializeField] private List<GameObject> displayGameObjects;

		private IList<IProgressDisplay> progressDisplays;

		[ShowInInspector] public int DisplaysCount => displayGameObjects.Count;

		private float progress;
		[ShowInInspector]
		public float Progress {
			get => progress;
			set {
				this.progress = Mathf.Clamp01(value);
				foreach (var display in progressDisplays) {
					display.Progress = progress;
				}
			}
		}

		private void Awake() => progressDisplays = displayGameObjects.Select(go => go.GetComponent<IProgressDisplay>()).ToList();
	}
}

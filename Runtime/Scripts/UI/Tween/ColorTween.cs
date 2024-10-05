using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CommonUtils.UI.Tween {
	/*
	 * TODO This component has a similar effect as using BlinkerUIElements. Maybe need to reconsider it.
	 */
	[RequireComponent(typeof(Graphic))]
	public class ColorTween : AbstractTweener<Color> {
		#region Public inspector properties
		[FormerlySerializedAs("IncludeChildren")]
		[SerializeField] private bool includeChildren = false;
		#endregion

		private ICollection<Graphic> graphicComponents;

		#region Unity Lifecycle
		protected override void Awake() {
			base.Awake();
			if (includeChildren) { graphicComponents = GetComponentsInChildren<Graphic>(); }
			else { graphicComponents = new List<Graphic>() { GetComponent<Graphic>() }; }
		}
		#endregion

		protected override Color InitializeStartValue() => GetComponent<Graphic>().color;

		protected override void OnAnimationUpdated(Color updatedColor) {
			foreach (var graphic in graphicComponents) {
				graphic.color = updatedColor;
			}
		}


	}
}
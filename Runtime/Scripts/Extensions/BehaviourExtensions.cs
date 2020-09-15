using System.Collections.Generic;
using UnityEngine;

namespace CommonUtils.Extensions {
	/// <summary>
	/// Extensions for components that can be enabled or disabled.
	/// </summary>
	public static class BehaviourExtensions {
		/// <summary>
		/// Enables or disables all <paramref name="behaviours"/>.
		/// </summary>
		public static void SetEnabled(this IEnumerable<Behaviour> behaviours, bool enable) {
			if(behaviours.IsNullOrEmpty()) return;
			foreach (var behaviour in behaviours) {
				behaviour.enabled = enable;
			}
		}
	}
}
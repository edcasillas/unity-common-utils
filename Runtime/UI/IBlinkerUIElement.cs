using System.Collections.Generic;
using UnityEngine.UI;

namespace UI {
	public interface IBlinkerUIElement : IUnityComponent {
		float BlinkingRate { get; }
		IEnumerable<Graphic> BlinkerGraphics { get; }
		IReadOnlyDictionary<Graphic, float> OriginalAlphaValues { get; }
	}
}
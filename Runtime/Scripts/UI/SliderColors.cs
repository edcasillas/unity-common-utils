using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script automatically changes the color of the specified sprite based on the value of the slider.
///
/// Based off an old NGUI example.
/// </summary>
[RequireComponent(typeof(Slider))]
public class SliderColors : MonoBehaviour {
	[SerializeField]
	private Color[] colors = new Color[] { Color.red, Color.yellow, Color.green };

	private Image image;
	private Slider slider;

	private void Start() {
		slider = GetComponent<Slider>();
		image = slider.fillRect.GetComponent<Image>();
		Update();
	}

	private void Update() {
		var val = slider.value;
		val *= (colors.Length - 1);
		var startIndex = Mathf.FloorToInt(val);

		var c = colors[0];

		if (startIndex >= 0) {
			if (startIndex + 1 < colors.Length) {
				var factor = (val - startIndex);
				c = Color.Lerp(colors[startIndex], colors[startIndex + 1], factor);
			} else if (startIndex < colors.Length) {
				c = colors[startIndex];
			} else c = colors[colors.Length - 1];
		}

		c.a = image.color.a;
		image.color = c;
	}
}
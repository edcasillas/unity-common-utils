using UnityEngine;

namespace CommonUtils.UI {
	public class SimpleColorPicker : MonoBehaviour {
		[SerializeField] private RectTransform texture;
		[SerializeField] private Texture2D refSprite;
		[SerializeField] private ColorEvent onColorPicked;

		[ShowInInspector] public Color Color { get; private set; }

		public void PickColor() {
			var imagePos = texture.position;
			var globalPosX = UnityEngine.Input.mousePosition.x - imagePos.x;
			var globalPosY = UnityEngine.Input.mousePosition.y - imagePos.y;

			var localPosX = (int)(globalPosX * (refSprite.width / texture.rect.width));
			var localPosY = (int)(globalPosY * (refSprite.height / texture.rect.height));

			Color = refSprite.GetPixel(localPosX, localPosY);
			onColorPicked.Invoke(Color);
		}
	}
}
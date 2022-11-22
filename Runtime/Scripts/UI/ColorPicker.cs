using UnityEngine;

namespace CommonUtils.UI {
	public class ColorPicker : MonoBehaviour {
		[SerializeField] private RectTransform texture;
		[SerializeField] private GameObject target;
		[SerializeField] private Texture2D refSprite;

		public void OnClickPickerColor() {
			setColor();
		}

		private void setColor() {
			var imagePos = texture.position;
			var globalPosX = UnityEngine.Input.mousePosition.x - imagePos.x;
			var globalPosY = UnityEngine.Input.mousePosition.y - imagePos.y;

			var localPosX = (int)(globalPosX * (refSprite.width / texture.rect.width));
			var localPosY = (int)(globalPosY * (refSprite.height / texture.rect.height));

			var c = refSprite.GetPixel(localPosX, localPosY);

			SetActualColor(c);
		}

		private void SetActualColor(Color c) {
			target.GetComponent<MeshRenderer>().material.color = c;
		}
	}
}
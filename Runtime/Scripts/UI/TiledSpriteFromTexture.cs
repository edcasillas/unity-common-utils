using System;
using CommonUtils.Inspector.HelpBox;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUtils.UI {
	/// <summary>
	/// This component takes a raw <see cref="Texture2D"/> and creates a tiled sprite of it on the fly.
	/// The texture can be set at design time in the inspector, or at runtime when calling <see cref="SetTextureTiled"/>.
	/// Specially useful when retrieving textures from a remote source.
	/// </summary>
	public class TiledSpriteFromTexture : MonoBehaviour {
		[HelpBox("All fields are optional. Values can be set at runtime by calling the SetTextureTiled method.", HelpBoxMessageType.Info)]
		public Texture2D Texture;
		public float Border;
		public float PixelsPerUnit = 100f;

		private Image _image;
		private Image image {
			get {
				if (!_image) _image = GetComponent<Image>();
				if (!_image) _image = gameObject.AddComponent<Image>();
				return _image;
			}
		}

		private void Awake() {
			image.type = Image.Type.Tiled;
			if (Texture != null) SetTextureTiled(Texture, Border, PixelsPerUnit);
		}

		public void SetTextureTiled(Texture2D texture, float border = 0f, float pixelsPerUnit = 100f) {
			if (!texture) {
				Debug.LogError($"Cannot set the texture of {name} because the parameter is null.", this);
				return;
			}

			texture.wrapMode = TextureWrapMode.Repeat;
			try {
				image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero,
					pixelsPerUnit, 0, SpriteMeshType.Tight, new Vector4(border, border, border, border));
			}
			catch (Exception ex) {
				Debug.LogError($"Could not set a tiled texture in {name}: {ex.Message}", this);
			}
		}
	}
}
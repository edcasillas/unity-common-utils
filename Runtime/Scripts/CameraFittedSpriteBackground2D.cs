using UnityEngine;

namespace CommonUtils {
	/// <summary>
	/// Fits a <see cref="SpriteRenderer"/> to fully cover a camera viewport on a fixed world-space Z plane.
	/// </summary>
	/// <remarks>
	/// This is intended for camera-rendered 2D backgrounds that must sit behind world-space gameplay objects
	/// while screen-space UI remains free to render on top. The sprite is uniformly scaled until it covers the
	/// complete viewport, preserving the sprite aspect ratio and cropping any excess on the longer axis.
	/// </remarks>
	[RequireComponent(typeof(SpriteRenderer))]
	public class CameraFittedSpriteBackground2D : EnhancedMonoBehaviour {
		/// <summary>
		/// Camera whose viewport should be covered by the sprite. When left empty, <see cref="Camera.main"/> is used.
		/// </summary>
		[Tooltip("Camera whose viewport should be covered by the sprite. When left empty, Camera.main is used.")]
		[SerializeField] private Camera gameCamera;

		/// <summary>
		/// World-space Z position where the background sprite will be placed.
		/// </summary>
		[Tooltip("World-space Z position where the background sprite will be placed.")]
		[SerializeField] private float planeZ = 1f;

		/// <summary>
		/// Sprite sorting order applied to the background renderer.
		/// Use a low value so gameplay sprites can render in front of the background.
		/// </summary>
		[Tooltip("Sprite sorting order applied to the background renderer. Use a low value so gameplay sprites can render in front of the background.")]
		[SerializeField] private int sortingOrder = -100;

		private SpriteRenderer spriteRenderer;

		private void Awake() {
			spriteRenderer = GetComponent<SpriteRenderer>();
			if (gameCamera) return;
			Debug.LogWarning("No game camera was provided. Using main camera.");
			gameCamera = Camera.main;
		}

		private void Start() => fitToCamera();

		private void OnValidate() {
			if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer) spriteRenderer.sortingOrder = sortingOrder;
		}

		/// <summary>
		/// Positions and uniformly scales the sprite so it covers the configured camera viewport.
		/// </summary>
		private void fitToCamera() {
			if (!gameCamera || !spriteRenderer || !spriteRenderer.sprite) return;

			spriteRenderer.sortingOrder = sortingOrder;

			var distanceFromCamera = planeZ - gameCamera.transform.position.z;
			var bottomLeft = gameCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceFromCamera));
			var topRight = gameCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceFromCamera));
			var cameraSize = topRight - bottomLeft;
			var spriteSize = spriteRenderer.sprite.bounds.size;
			var scale = Mathf.Max(cameraSize.x / spriteSize.x, cameraSize.y / spriteSize.y);

			transform.position = new Vector3(bottomLeft.x + cameraSize.x * 0.5f, bottomLeft.y + cameraSize.y * 0.5f, planeZ);
			transform.localScale = Vector3.one * scale;
		}
	}
}

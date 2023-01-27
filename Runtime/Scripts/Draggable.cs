using UnityEngine;

namespace CommonUtils {
	public class Draggable : EnhancedMonoBehaviour {
		[SerializeField] private Camera defaultCamera;

		private Camera camRef;
		[ShowInInspector]
		public Camera Camera {
			get {
				if (!camRef) {
					camRef = defaultCamera;
					if (!camRef) camRef = Camera.main;
				}

				return camRef;
			}
			set => camRef = value;
		}

		[ShowInInspector] public Vector3 MousePositionOffset { get; private set; }

		private void OnMouseDown() => MousePositionOffset = transform.position - getMouseWorldPosition();

		private void OnMouseDrag() => transform.position = getMouseWorldPosition() + MousePositionOffset;

		private Vector3 getMouseWorldPosition() => Camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
	}
}
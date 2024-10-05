using UnityEngine;

namespace CommonUtils.Draggables {
	[RequireComponent(typeof(Collider2D))]
	public class Draggable2D : AbstractDraggable {
		[ShowInInspector] public Vector3 MousePositionOffset { get; private set; }

		private void OnMouseDown() => MousePositionOffset = transform.position - getMouseWorldPosition();

		private void OnMouseDrag() => transform.position = getMouseWorldPosition() + MousePositionOffset;

		private Vector3 getMouseWorldPosition() => Camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
	}
}
using UnityEngine;

namespace CommonUtils.Draggables {
	[RequireComponent(typeof(Collider))]
	public class Draggable3D : AbstractDraggable {
		private void OnMouseDrag() {
			var distanceToScreen = Camera.WorldToScreenPoint(transform.position).z;
			transform.position = Camera.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x,
				UnityEngine.Input.mousePosition.y,
				distanceToScreen));
		}
	}
}
using CommonUtils.Extensions;
using CommonUtils.Verbosables;
using System;
using UnityEngine;

namespace CommonUtils.Draggables {
	[RequireComponent(typeof(Collider))]
	public class Draggable3D : AbstractDraggable {
		[SerializeField] private bool freezeX;
		[SerializeField] private bool freezeY;
		[SerializeField] private bool freezeZ;

		private void OnMouseDown() => this.DebugLog(() => $"Drag start on {name}.");

		private void OnMouseUp() => this.DebugLog(() => $"Drag end on {name}.");

		private void OnMouseDrag() {
			var distanceToScreen = Camera.WorldToScreenPoint(transform.position).z;
			var wantedPosition = Camera.ScreenToWorldPoint(new Vector3(
				UnityEngine.Input.mousePosition.x,
				UnityEngine.Input.mousePosition.y,
				distanceToScreen));

			if (freezeX) wantedPosition.x = transform.position.x;
			if (freezeY) wantedPosition.y = transform.position.y;
			if (freezeZ) wantedPosition.z = transform.position.z;

			transform.position = wantedPosition;
		}
	}
}
using UnityEngine;

namespace CommonUtils.Draggables {
	public abstract class AbstractDraggable : EnhancedMonoBehaviour {
		[SerializeField]
		private Camera defaultCamera;

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

		private void Reset() => defaultCamera = Camera.main;
	}
}
using UnityEngine;

namespace CommonUtils {
	public static class ExtendedGizmos {
		public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color   _color = default) {
			#if UNITY_EDITOR
			if (_color != default)
				UnityEditor.Handles.color = _color;
			var angleMatrix = Matrix4x4.TRS(_pos, _rot, UnityEditor.Handles.matrix.lossyScale);
			using (new UnityEditor.Handles.DrawingScope(angleMatrix)) {
				var pointOffset = (_height - (_radius * 2)) / 2;

				//draw sideways
				UnityEditor.Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
				UnityEditor.Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
				UnityEditor.Handles.DrawLine(new Vector3(0, pointOffset, _radius),  new Vector3(0, -pointOffset, _radius));
				UnityEditor.Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);

				//draw frontways
				UnityEditor.Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
				UnityEditor.Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
				UnityEditor.Handles.DrawLine(new Vector3(_radius,  pointOffset, 0), new Vector3(_radius,  -pointOffset, 0));
				UnityEditor.Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);

				//draw center
				UnityEditor.Handles.DrawWireDisc(Vector3.up   * pointOffset, Vector3.up, _radius);
				UnityEditor.Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

			}
			#endif
		}
	}
}
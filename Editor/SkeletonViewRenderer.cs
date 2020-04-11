using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	public static class SkeletonViewRenderer {
		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		static void RenderCustomGizmo(SkeletonView skeleton, GizmoType gizmoType) {
			var childNodes = skeleton.RootNode.GetComponentsInChildren<Transform>();
			foreach (Transform child in childNodes) {
				if (child == skeleton.RootNode) {
					Handles.color = skeleton.RootColor;
					Handles.FreeMoveHandle(child.position,
						child.rotation,
						skeleton.RootSize,
						Vector3.zero,
						Handles.SphereHandleCap);
				} else {
					Handles.color = skeleton.BoneColor;
					Handles.DrawLine(child.position, child.parent.position);
					Handles.FreeMoveHandle(child.position,
						child.rotation,
						skeleton.JointSize,
						Vector3.zero,
						Handles.SphereHandleCap);
				}
			}
		}
	}
}
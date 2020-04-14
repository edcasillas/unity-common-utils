using System.Collections.Generic;
using CommonUtils.Extensions;
using CommonUtils.SkeletonViewer;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor {
	public static class SkeletonViewRenderer {
		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		private static void renderCustomGizmo(SkeletonView skeleton, GizmoType gizmoType) {
			var root = skeleton.RootNode ? skeleton.RootNode : skeleton.transform;
			var childNodes = root.GetComponentsInChildren<Transform>();

			var toBeIgnored = new HashSet<Transform>();
			foreach (var child in childNodes) {
				if(toBeIgnored.Contains(child)) continue;

				var options = child.GetComponent<SkeletonViewOptions>();

				if (options) {
					if (options.IgnoreChildren) {
						toBeIgnored.SafeAdd(child.GetComponentsInChildren<Transform>());
					}

					if(options.IgnoreThis) continue;
				}

				if (child == root) {
					Handles.color = skeleton.RootColor;
					Handles.FreeMoveHandle(child.position, child.rotation, skeleton.RootSize, Vector3.zero, Handles.SphereHandleCap);
				} else {
					Handles.color = skeleton.BoneColor;
					Handles.DrawLine(child.position, child.parent.position);
					Handles.FreeMoveHandle(child.position, child.rotation, skeleton.JointSize, Vector3.zero, Handles.SphereHandleCap);
				}

				if(options && options.ShowName) Handles.Label(child.position, child.name);
			}
		}
	}
}
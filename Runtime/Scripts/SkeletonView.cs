using CommonUtils.Extensions;
using UnityEngine;

namespace CommonUtils {
	/// <summary>
	/// Put this component in a SkinnedMeshRenderer to draw its skeleton as gizmos.
	/// </summary>
	[AddComponentMenu("Mesh/Skeleton View")]
	public class SkeletonView : MonoBehaviour { // based on http://answers.unity.com/comments/714888/view.html
		#if UNITY_EDITOR
#pragma warning disable 649
		[SerializeField] private bool        alwaysShow;
		[SerializeField] private Color       rootColor = Color.green;
		[SerializeField] private Color       boneColor = Color.blue;
		[SerializeField] private Transform   rootNode;
		[SerializeField] [HideInInspector] private Transform[] childNodes;
#pragma warning restore 649

		private void OnDrawGizmos()         { if(alwaysShow) drawJoints(); }
		private void OnDrawGizmosSelected() { if(!alwaysShow) drawJoints(); }

		private void drawJoints() {
			if (!rootNode && GetComponent<SkinnedMeshRenderer>()) rootNode = GetComponent<SkinnedMeshRenderer>().rootBone;
			if (!rootNode) return;

			if (childNodes.IsNullOrEmpty()) {
				PopulateChildren();
			}

			foreach (Transform child in childNodes) {
				if (child == rootNode) {
					Gizmos.color = rootColor;
					Gizmos.DrawCube(child.position, new Vector3(.1f, .1f, .1f));
				} else {
					Gizmos.color = boneColor;
					Gizmos.DrawLine(child.position, child.parent.position);
					Gizmos.DrawCube(child.position, new Vector3(.01f, .01f, .01f));
				}
			}
		}

		public void PopulateChildren() => childNodes = rootNode.GetComponentsInChildren<Transform>();
		#endif
	}
}
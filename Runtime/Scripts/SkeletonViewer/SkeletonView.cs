using UnityEngine;

namespace CommonUtils.SkeletonViewer {
	/// <summary>
	/// Put this component in a SkinnedMeshRenderer to draw its skeleton as gizmos.
	/// </summary>
	[AddComponentMenu("Mesh/Skeleton View")]
	public class SkeletonView : MonoBehaviour { // based on http://answers.unity.com/comments/714888/view.html
#if UNITY_EDITOR
#pragma warning disable 649
		[Tooltip("Leave root empty to use the current transform as root.")]
		[SerializeField] private Transform   rootNode;

		[Tooltip("Uncheck to hide the skeleton.")]
		[SerializeField] private bool viewSkeleton = true;
		[SerializeField] private Color       rootColor = Color.green;
		[SerializeField] [Range(0.001f, 0.1f)] private float rootSize = 0.05f;
		[SerializeField] private Color       boneColor = Color.blue;
		[SerializeField] [Range(0.001f, 0.1f)] private float jointSize = 0.01f;
#pragma warning restore 649

		public Transform RootNode => rootNode;
		public bool IsEnabled => viewSkeleton;
		public Color RootColor => rootColor;
		public Color BoneColor => boneColor;
		public float RootSize => rootSize;
		public float JointSize => jointSize;
#endif
	}
}
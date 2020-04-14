using UnityEngine;

namespace CommonUtils.SkeletonViewer {
	public class SkeletonViewOptions : MonoBehaviour {
#if UNITY_EDITOR
#pragma warning disable 649
		[SerializeField] private bool showName;
		[SerializeField] private bool ignoreThis;
		[SerializeField] private bool ignoreChildren;
#pragma warning restore 649

		public bool ShowName => showName;
		public bool IgnoreThis => ignoreThis;
		public bool IgnoreChildren => ignoreChildren;
#endif
	}
}

using CommonUtils.UnityComponents;

namespace CommonUtils.UI.ProgressDisplay {
	public interface IProgressDisplay : IUnityComponent {
		float Progress { set; }
	}
}

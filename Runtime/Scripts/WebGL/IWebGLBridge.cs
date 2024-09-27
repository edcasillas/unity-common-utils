using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;

namespace CommonUtils.WebGL {
	public interface IWebGLBridge : IUnityComponent, IVerbosable {
		bool IsMobileBrowser { get; }
		WebBrowserType BrowserType { get; }
	}
}
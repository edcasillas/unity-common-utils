using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;
using System;

namespace CommonUtils.WebGL {
	public interface IWebGLBridge : IUnityComponent, IVerbosable {
		bool IsMobileBrowser { get; }
		WebBrowserType BrowserType { get; }

		void SubscribeToOnPointerLockChanged(Action<bool> s);
		void UnsubscribeFromOnPointerLockChanged(Action<bool> s);
	}
}
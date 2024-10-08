using CommonUtils.UnityComponents;
using CommonUtils.Verbosables;
using System;

namespace CommonUtils.WebGL {
	/// <summary>
	/// Provides a bridge for interacting with WebGL-specific functionality such as pointer lock events, full-screen
	/// toggling, and key event management.
	/// </summary>
	public interface IWebGLBridge : IUnityComponent, IVerbosable {
		bool IsMobileBrowser { get; }
		WebBrowserType BrowserType { get; }

		void SubscribeToOnPointerLockChanged(Action<bool> s);
		void UnsubscribeFromOnPointerLockChanged(Action<bool> s);
	}
}
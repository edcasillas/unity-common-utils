using System.Runtime.InteropServices;
using UnityEngine;

namespace CommonUtils.iOS {
	public static class iPhoneSpeaker {
#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void _forceToSpeaker();
#endif

		public static void ForceToSpeaker() {
#if UNITY_IPHONE
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				_forceToSpeaker();
			}
#endif
		}
	}
}
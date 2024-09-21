using System;
using System.Threading.Tasks;

namespace CommonUtils.RestSdk {
	public static class InternetTester {
		/// <summary>
		/// Test the connection against the specified URL and executes the callback when finished.
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="url">URL to test. If null, this method will check against Google.</param>
		public static void Test(Action<string> callback, string url = null) {
			#if UNITY_WEBGL && !UNITY_EDITOR
			callback(null);
			#else
			if(string.IsNullOrEmpty(url)) {
				url = "https://www.google.com";
			}
			new RestClient(url).Ping(null,
				response => { callback(response.IsSuccess ? null : response.ErrorMessage); });
			#endif
		}

		public static async Task<string> TestAsync(string url = null, bool verbose = false) {
			#if UNITY_WEBGL && !UNITY_EDITOR
			return null;
			#else
			if(string.IsNullOrWhiteSpace(url)) url = "https://www.google.com";
			var restClient = new RestClient(url);
			var response = await restClient.PingAsync(null);
			return response.IsSuccess ? null : response.ErrorMessage;
			#endif
		}
	}
}
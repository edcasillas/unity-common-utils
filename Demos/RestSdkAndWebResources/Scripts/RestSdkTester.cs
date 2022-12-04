using CommonUtils;
using CommonUtils.RestSdk;
using UnityEngine;

namespace Demos.RestSdkAndWebResources {
	public class RestSdkTester : MonoBehaviour {
		public enum CallResult {
			None,
			Busy,
			Success,
			Error
		}

		[SerializeField] private string apiUrl = "https://api.weather.gov";

		private IRestClient restClient;

		[ShowInInspector] public CallResult Status { get; private set; }
		[ShowInInspector] public TestDto ObtainedData { get; private set; }

		private void Awake() => restClient = new RestClient(apiUrl);

		[ShowInInspector]
		public void MakeAGetRequest(string relativePath = "gridpoints/MTR/85,105/forecast") {
			Status = CallResult.Busy;
			restClient.Get<TestDto>(relativePath,
				response => {
					Status = response.IsSuccess ? CallResult.Success : CallResult.Error;
					ObtainedData = response.IsSuccess ? response.Data : null;
				});
		}
	}
}
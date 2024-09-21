using UnityEngine.Networking;

namespace CommonUtils.Extensions
{
    public static class UnityWebRequestExtensions
    {
		/// <summary>
		/// Gets a value indicating whether the <paramref name="request"/> is
		/// finished and has an error. Returns false if the request is not
		/// yet done.
		/// </summary>
		public static bool FinishedWithError(this UnityWebRequest request) {
			if (!request.isDone) return false;
#if UNITY_2020_2_OR_NEWER
			return request.result != UnityWebRequest.Result.Success;
#else
			return request.isNetworkError || request.isHttpError;
#endif
		}
    }
}

using UnityEngine;

namespace CommonUtils.WebResources {
	public struct DownloadableAudioClip {
		public AudioClip AudioClip;
		public DownloadStatus DownloadStatus;
		public string ErrorMessage;

		public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
		public bool FinishedLoading => DownloadStatus == DownloadStatus.Loaded || DownloadStatus == DownloadStatus.Error;

		/// <summary>
		/// Deletes the audioclip contained in this object and restarts the download status.
		/// </summary>
		public void Reset() {
			AudioClip = null;
			DownloadStatus = DownloadStatus.NotInited;
			ErrorMessage = null;
		}
	}
}
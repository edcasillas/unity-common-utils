using CommonUtils.Extensions;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace CommonUtils {
	/// <summary>
	/// A component to load a video from the Streaming Assets folder, based on Unity's documentation.
	/// </summary>
	[RequireComponent(typeof(VideoPlayer))]
	public class VideoPlayerFromStreamingAssets : MonoBehaviour, IVerbosable {
		[SerializeField] private string fileName; // eg myFile.mp4
		[SerializeField] private bool verbose;

		public bool IsVerbose => verbose;

		private VideoPlayer videoPlayer;
		private string status;

		private void Start() {
			videoPlayer = GetComponent<VideoPlayer>();

			// Obtain the location of the video clip.
			videoPlayer.url = Path.Combine(Application.streamingAssetsPath, fileName);

			this.DebugLog($"Trying to run video from {videoPlayer.url}");

			// Restart from beginning when done.
			videoPlayer.isLooping = true;

			videoPlayer.Play();
		}
	}
}
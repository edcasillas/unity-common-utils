using CommonUtils.Extensions;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace CommonUtils {
	/// <summary>
	/// A component to load a video from the Streaming Assets folder, based on Unity's documentation.
	/// </summary>
	[RequireComponent(typeof(VideoPlayer))]
	public class VideoPlayerFromStreamingAssets : EnhancedMonoBehaviour {
		[Tooltip("Example: myFile.mp4")]
		[SerializeField] private string fileName;
		[SerializeField] private bool autoPlay = true;

		private VideoPlayer videoPlayer;

		[ShowInInspector] public bool IsPlaying => videoPlayer.isPlaying;
		[ShowInInspector] public bool IsPaused => videoPlayer.isPaused;

		#region Unity Lifecycle
		private void Awake() {
			videoPlayer = GetComponent<VideoPlayer>();
			// Obtain the location of the video clip.
			videoPlayer.url = Path.Combine(Application.streamingAssetsPath, fileName);
		}

		private void Start() {
			if(!autoPlay) return;
			Play();
		}
		#endregion

		#region Play/Pause
		[ShowInInspector]
		public void Play() {
			if(videoPlayer.isPlaying) return;
			this.DebugLog(() => $"Trying to run video from {videoPlayer.url}");
			// Restart from beginning when done.
			videoPlayer.isLooping = true;
			videoPlayer.Play();
		}

		[ShowInInspector]
		public void Pause() {
			if(videoPlayer.isPaused) return;
			this.DebugLog("Pausing video.");
			videoPlayer.Pause();
		}
		#endregion
	}
}
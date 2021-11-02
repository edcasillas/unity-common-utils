using System;
using System.Collections;
using System.Collections.Generic;
using CommonUtils.RestSdk;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonUtils.WebResources {
	/// <summary>
	/// Contains utility methods to load web resources.
	/// </summary>
	public static class WebLoader {
		private const string contentTypeHeaderKey = "CONTENT-TYPE";

		private static readonly Dictionary<string, AudioType> audioContentTypesDict =
			new Dictionary<string, AudioType>() {
				{"audio/ogg", AudioType.OGGVORBIS},
				{"audio/wav", AudioType.WAV},

				/*
				 * TODO MP3 doesn't seem to be supported by Unity (at least on desktop and editor, need to test other platforms).
				 * It might be possible to convert MP3 files to WAV using this SO answer: https://gamedev.stackexchange.com/a/114886
				 * but I need to figure out whether adding this dll will be supported by all platforms.
				 */
				//{"audio/mpeg", AudioType.MPEG}
			};

		/// <summary>
		/// Loads a <see cref="Texture"/> from the specified <paramref name="url"/> and returns the result at <paramref name="onFinish"/>.
		/// </summary>
		/// <param name="url">URL to retrieve the <see cref="Texture"/> from.</param>
		/// <param name="onFinish">Callback to receive the response.</param>
		public static void LoadWebTexture(string url, Action<RestResponse<Texture>> onFinish) {
			if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
			if (onFinish == null) throw new ArgumentNullException(nameof(onFinish));
			Coroutiner.StartCoroutine(requestImage(url, onFinish), "WebImageLoader");
		}

		/*
		/// <summary>
		/// Loads an <see cref="AudioClip"/> from the specified <paramref name="url"/> into <paramref name="downloadableAudioClip"/>.
		/// </summary>
		/// <param name="url">URL to retrieve the <see cref="AudioClip"/> from.</param>
		/// <param name="downloadableAudioClip">Object to receive the resulting <see cref="AudioClip"/>.</param>
		public static void LoadWebAudioClip(string url, DownloadableAudioClip downloadableAudioClip) {
			if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
			Coroutiner.StartCoroutine(requestAudioClip(url, downloadableAudioClip), "WebAudioClipLoader");
		}*/

		/// <summary>
		/// Loads an <see cref="AudioClip"/> from the specified <paramref name="url"/> and returns the result at <paramref name="onFinish"/>.
		/// </summary>
		/// <param name="url">URL to retrieve the <see cref="AudioClip"/> from.</param>
		/// <param name="onFinish">Callback to receive the response.</param>
		public static void LoadWebAudioClip(string url, Action<RestResponse<AudioClip>> onFinish) {
			if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
			if (onFinish == null) throw new ArgumentNullException(nameof(onFinish));
			Coroutiner.StartCoroutine(requestAudioClip(url, onFinish), "WebAudioClipLoader");
		}

		/*public static void LoadWebAudioClipMp3(string url, Action<RestResponse<AudioClip>> onFinish) {
			if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
			if (onFinish == null) throw new ArgumentNullException(nameof(onFinish));
			Coroutiner.StartCoroutine(requestMp3AudioClip(url, onFinish), "WebAudioClipLoader");
		}*/

		#region Private methods
		/// <summary>
		/// Private coroutine to retrieve a <see cref="Texture"/> from the specified <paramref name="url"/>
		/// </summary>
		/// <param name="url">URL to retrieve the <see cref="Texture"/> from.</param>
		/// <param name="onFinish">Callback to receive the response.</param>
		private static IEnumerator requestImage(string url, Action<RestResponse<Texture>> onFinish) {
			var response = new RestResponse<Texture>();

			using (var uwr = UnityWebRequestTexture.GetTexture(url)) {
				yield return uwr.SendWebRequest();

				#if UNITY_2020_2_OR_NEWER
				if(uwr.result != UnityWebRequest.Result.Success) {
				#else
				if (uwr.isNetworkError || uwr.isHttpError) {
				#endif

					Debug.LogError($"Couldn't retrieve texture from '{url}': {uwr.error}");
					response.StatusCode = GetStatusCodeFromMessage(uwr.error, out var errMsg);
					response.ErrorMessage = errMsg;
					Debug.Log(uwr.error);
				} else {
					var texture = DownloadHandlerTexture.GetContent(uwr);
					if (!texture) {
						Debug.LogError($"Couldn't receive a texture from '{url}'");
						response.StatusCode = 500;
					} else {
						response.StatusCode = 200;
						response.Data = texture;
					}
				}
			}

			onFinish(response);
		}

		/// <summary>
		/// Private coroutine to retrieve an audio clip from the specified <paramref name="url"/>.
		/// </summary>
		/// <param name="url">URL to retrieve the <see cref="AudioClip"/> from.</param>
		/// <param name="onFinish">Callback to receive the response.</param>
		private static IEnumerator requestAudioClip(string url, Action<RestResponse<AudioClip>> onFinish, bool is3dSound = false, bool stream = false) {
			Debug.Log($"Attempting download of an audio clip from '{url}'");

			#pragma warning disable 618 // TODO For now, ignore the deprecation warning. We want to still use WWW instead of UnityWebRequestMultimedia because the latter requires to know the AudioType upfront.
			var www = new WWW(url);
			#pragma warning restore 618

			yield return www;

			var response = new RestResponse<AudioClip> {
				ErrorMessage = www.error
			};

			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError($"Couldn't retrieve audio clip from '{url}': {www.error}");
				response.StatusCode = GetStatusCodeFromMessage(www.error, out var errMsg);
				response.ErrorMessage = errMsg;
			} else {
				if (!www.responseHeaders.ContainsKey(contentTypeHeaderKey)) {
					response.ErrorMessage = "Content type is unknown.";
					response.StatusCode = 500;
					Debug.LogError($"Couldn't retrieve audio clip from '{url}': {response.ErrorMessage}");
				} else {
					var receivedContentType = www.responseHeaders["CONTENT-TYPE"];

					if (!audioContentTypesDict.ContainsKey(receivedContentType)) {
						response.ErrorMessage = $"'{receivedContentType}' is not a supported content type.";
						response.StatusCode = 500;
						Debug.LogError($"Couldn't retrieve audio clip from '{url}': {response.ErrorMessage}");
					} else {
						AudioClip clip = null;

						try {
							clip = www.GetAudioClip(is3dSound, stream, audioContentTypesDict[receivedContentType]);
						} catch (Exception ex) {
							Debug.LogErrorFormat($"An error occured decoding the downloaded audio clip bytes: {ex.Message}");
						}

						if (clip == null) {
							response.ErrorMessage = "Unable to retrieve an audioclip from response.";
							response.StatusCode = 500;
							Debug.LogErrorFormat($"Couldn't retrieve audio clip from '{url}': {response.ErrorMessage}");
						} else {
							while (clip.loadState == AudioDataLoadState.Loading) { yield return null; }

							if (clip.loadState == AudioDataLoadState.Loaded) {
								response.StatusCode = 200;
								response.Data = clip;
							} else {
								response.StatusCode = 418; // I'm a Teapot :)
							}
						}
					}
				}
			}

			onFinish(response);
		}

		/*
		/// <summary>
		/// Private coroutine to retrieve an audio clip from the specified <paramref name="url"/>.
		/// </summary>
		/// <param name="url">URL to retrieve the <see cref="AudioClip"/> from.</param>
		/// <param name="onFinish">Callback to receive the response.</param>
		private static IEnumerator requestMp3AudioClip(string url, Action<RestResponse<AudioClip>> onFinish) {
			Debug.Log($"Attempting download of an audio clip from '{url}'");

			var response = new RestResponse<AudioClip> ();

			using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG)) {
				yield return uwr.SendWebRequest();

				if (uwr.isNetworkError || uwr.isHttpError) {
					Debug.LogError($"Couldn't retrieve MP3 audio clip from '{url}': {uwr.error}");
					response.StatusCode = getStatusCodeFromMessage(uwr.error, out var errMsg);
					response.ErrorMessage = errMsg;
					Debug.Log(uwr.error);
				} else {
					var clip = DownloadHandlerAudioClip.GetContent(uwr);
					if (!clip) {
						Debug.LogError($"Couldn't receive an MP3 audio clip from '{url}'");
						response.StatusCode = 500;
					} else {
						response.StatusCode = 200;
						response.Data = clip;
					}
				}
			}

			onFinish(response);
		}*/

		// FIXME No funciona :(
		/*private static IEnumerator requestAudioClip(string url, DownloadableAudioClip downloadableAudioClip) {
			Debug.Log($"Requesting AudioClip to: {url}");
			downloadableAudioClip.DownloadStatus = DownloadStatus.Loading;

			var www = new WWW(url);
			yield return www;

			Debug.Log($"Finished requesting AudioClip to: {url}");

			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogErrorFormat($"Couldn't retrieve audio clip from '{url}': {www.error}");
				downloadableAudioClip.ErrorMessage = www.error;
				downloadableAudioClip.DownloadStatus = DownloadStatus.Error;
			} else {
				var clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
				if (clip == null) {
					downloadableAudioClip.ErrorMessage = $"Couldn't retrieve audio clip from '{url}'";
					downloadableAudioClip.DownloadStatus = DownloadStatus.Error;
					Debug.LogError(downloadableAudioClip.ErrorMessage);
				} else {
					downloadableAudioClip.DownloadStatus = DownloadStatus.Loading;
					while (clip.loadState == AudioDataLoadState.Loading) { yield return null; }

					if (clip.loadState == AudioDataLoadState.Loaded) {
						downloadableAudioClip.DownloadStatus = DownloadStatus.Loaded;
						downloadableAudioClip.AudioClip = clip;
					} else {
						downloadableAudioClip.ErrorMessage = $"Couldn't retrieve audio clip from '{url}'";
						downloadableAudioClip.DownloadStatus = DownloadStatus.Error;
					}
				}
			}
		}*/

		/// <summary>
		/// Tries parsing the first 3 characters of an Http response message to find the status code.
		/// </summary>
		/// <param name="errorMessage">Error message from the HttpResponse.</param>
		/// <param name="outputMessage">Remainder of the error message.</param>
		/// <returns>Error code.</returns>
		internal static int GetStatusCodeFromMessage(string errorMessage, out string outputMessage) {
			outputMessage = errorMessage;
			if (errorMessage.Length < 3) { return 0; }

			var errorCodeStr = errorMessage.Substring(0, 3);
			int errorCodeInt = 500;
			if (int.TryParse(errorCodeStr, out var errCode)) {
				errorCodeInt = errCode;
				if (errorMessage.Length > 3) {
					outputMessage = errorMessage.Substring(3);
				}
			}

			return errorCodeInt;
		}
		#endregion
	}
}